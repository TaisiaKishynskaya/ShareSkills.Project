using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace App.Services.RecommendationSystem.Test
{
    // DTO для regression
    public class TeacherTrainData
    {
        [LoadColumn(0)] public float SkillsMatchCount;
        [LoadColumn(1)] public float TimeDiff;
        [LoadColumn(2)] public float RelevantCoursesCount;
        [LoadColumn(3), ColumnName("Label")] public float Rating;
    }

    // DTO для prediction regression
    public class TeacherPredictData
    {
        public Guid TeacherId;
        public Guid StudentId;
        public float SkillsMatchCount;
        public float TimeDiff;
        public float RelevantCoursesCount;
    }

    // DTO для CF
    public class TeacherCfData
    {
        [LoadColumn(0)] public uint StudentId;
        [LoadColumn(1)] public uint TeacherId;
        [LoadColumn(2), ColumnName("Label")] public float Rating;
    }

    // Prediction output
    public class TeacherScorePrediction
    {
        [ColumnName("Score")] public float Score;
    }

    public class TeacherRatingService
    {
        private readonly AppDbContext _context;
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private ITransformer _cfModel;
        private DataViewSchema _schema;
        private PredictionEngine<TeacherCfData, TeacherScorePrediction> _cfEngine;
        private readonly ICosineSimilarityService _cosine;

        public TeacherRatingService(AppDbContext context, ICosineSimilarityService cosine)
        {
            _context    = context;
            _cosine     = cosine;
            _mlContext  = new MLContext(seed: 0);
            TrainModel();
        }

        public void TrainModel()
        {
            var teacherRatings = _context.TeacherRatings;
            var classTimeMap = _context.ClassTimes
                .Select((ct, idx) => new { ct.Id, Index = idx })
                .ToDictionary(x => x.Id, x => x.Index);

            // student stats
            var studentStats = teacherRatings
                .GroupBy(tr => tr.StudentId)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        avgTime: g.Select(tr => classTimeMap[
                                    _context.Teachers.First(t => t.Id == tr.TeacherId).ClassTimeId])
                                 .DefaultIfEmpty(0).Average(),
                        skills: g.Select(tr => _context.Teachers
                                    .First(t => t.Id == tr.TeacherId).SkillId)
                                .ToHashSet()
                    )
                );

            // regression training data
            var trainingData = teacherRatings.ToList().Select(tr =>
            {
                var stats   = studentStats[tr.StudentId];
                var teacher = _context.Teachers.First(t => t.Id == tr.TeacherId);
                float skillsMatch = stats.skills.Contains(teacher.SkillId) ? 1f : 0f;
                float timeDiff    = Math.Abs(classTimeMap[teacher.ClassTimeId] - (float)stats.avgTime);
                float relCourses  = teacher.Courses.Count(c => c.Teachers
                                            .Any(tch => stats.skills.Contains(tch.SkillId)));
                return new TeacherTrainData
                {
                    SkillsMatchCount     = skillsMatch,
                    TimeDiff             = timeDiff,
                    RelevantCoursesCount = relCourses,
                    Rating               = tr.Rating
                };
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // ___ CF training ___
            var cfData = teacherRatings.Select(r => new TeacherCfData {
                StudentId = GuidToUIntMapper.Map(r.StudentId),
                TeacherId = GuidToUIntMapper.Map(r.TeacherId),
                Rating    = r.Rating
            }).ToList();
            var cfDv = _mlContext.Data.LoadFromEnumerable(cfData);
            var cfPipeline = _mlContext.Transforms.Conversion.MapValueToKey("UserEncoded", nameof(TeacherCfData.StudentId))
                              .Append(_mlContext.Transforms.Conversion.MapValueToKey("ItemEncoded", nameof(TeacherCfData.TeacherId)))
                              .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                                  new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options {
                                      MatrixColumnIndexColumnName = "UserEncoded",
                                      MatrixRowIndexColumnName    = "ItemEncoded",
                                      LabelColumnName             = "Label",
                                      NumberOfIterations          = 20,
                                      ApproximationRank           = 64
                                  }));
            _cfModel  = cfPipeline.Fit(cfDv);
            _cfEngine = _mlContext.Model.CreatePredictionEngine<TeacherCfData, TeacherScorePrediction>(_cfModel);
            // ___ end CF ___

            // regression pipeline
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                                nameof(TeacherTrainData.SkillsMatchCount),
                                nameof(TeacherTrainData.TimeDiff),
                                nameof(TeacherTrainData.RelevantCoursesCount))
                           .Append(_mlContext.Regression.Trainers.FastTree(
                                labelColumnName: "Label",
                                featureColumnName: "Features"));

            _model = pipeline.Fit(dataView);
            _mlContext.Model.Save(_model, dataView.Schema, "teachers.zip");
            _schema = dataView.Schema;
        }

        public IList<TeacherEntity> GetRecommendedTeachers(Guid studentId, int count)
        {
            if (_model == null) TrainModel();

            var classTimeMap = _context.ClassTimes
                .Select((ct, idx) => new { ct.Id, Index = idx })
                .ToDictionary(x => x.Id, x => x.Index);

            var past = _context.TeacherRatings.Where(tr => tr.StudentId == studentId).ToList();
            var ratedIds = past.Select(tr => tr.TeacherId).ToHashSet();
            float avgTime = (float)past.Select(tr => classTimeMap[
                    _context.Teachers.First(t => t.Id == tr.TeacherId).ClassTimeId])
                .DefaultIfEmpty(0).Average();
            var skillsRated = past.Select(tr => _context.Teachers
                                    .First(t => t.Id == tr.TeacherId).SkillId)
                                  .ToHashSet();

            var predictionData = _context.Teachers
                .Where(t => !ratedIds.Contains(t.Id))
                .Select(t => new TeacherPredictData
                {
                    TeacherId              = t.Id,
                    StudentId              = studentId,
                    SkillsMatchCount       = skillsRated.Contains(t.SkillId) ? 1f : 0f,
                    TimeDiff               = Math.Abs(classTimeMap[t.ClassTimeId] - avgTime),
                    RelevantCoursesCount   = t.Courses.Count(c => c.Teachers
                                                .Any(tc => skillsRated.Contains(tc.SkillId)))
                })
                .ToList();

            var featureData = predictionData.Select(pd => new TeacherTrainData
            {
                SkillsMatchCount       = pd.SkillsMatchCount,
                TimeDiff               = pd.TimeDiff,
                RelevantCoursesCount   = pd.RelevantCoursesCount,
                Rating                 = 0
            });
            var predDv = _mlContext.Data.LoadFromEnumerable(featureData);
            var regPreds = _model.Transform(predDv);
            var regScores = _mlContext.Data.CreateEnumerable<TeacherScorePrediction>(
                                regPreds, reuseRowObject: false).ToArray();

            // ___ CF predictions ___
            var cfPredData = predictionData.Select(pd => new TeacherCfData {
                StudentId = GuidToUIntMapper.Map(pd.StudentId),
                TeacherId = GuidToUIntMapper.Map(pd.TeacherId),
                Rating    = 0
            }).ToList();
            var cfDv2 = _mlContext.Data.LoadFromEnumerable(cfPredData);
            var cfPreds = _cfModel.Transform(cfDv2);
            var cfScores = _mlContext.Data.CreateEnumerable<TeacherScorePrediction>(
                                cfPreds, reuseRowObject: false).ToArray();
            // ___ end CF ___

            var results = regScores
                .Select((p, i) => {
                    var reg   = p.Score;
                    var cf    = cfScores[i].Score;
                    var cos   = _cosine.Compute(
                                   new float[]{ predictionData[i].SkillsMatchCount },
                                   new float[]{ 1f });
                    var final = 0.5f * cf + 0.4f * reg + 0.1f * cos;
                    return new { predictionData[i].TeacherId, Score = final };
                })
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => _context.Teachers.First(t => t.Id == x.TeacherId))
                .ToList();

            return results;
        }

        public static class GuidToUIntMapper
        {
            public static uint Map(Guid guid) =>
                BitConverter.ToUInt32(guid.ToByteArray(), 0);
        }
    }
}
