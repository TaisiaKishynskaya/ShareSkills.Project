using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace App.Services.RecommendationSystem.Test
{
    public class CourseTrainData
    {
        [LoadColumn(0)] public float SkillsMatchCount;
        [LoadColumn(1)] public float FavTeacherCount;
        [LoadColumn(2), ColumnName("Label")] public float Rating;
    }

    public class CoursePredictData
    {
        public Guid CourseId;
        public Guid StudentId;
        public float SkillsMatchCount;
        public float FavTeacherCount;
    }

    public class CourseScorePrediction
    {
        [ColumnName("Score")] public float Score;
    }

    public class CourseCfData
    {
        [LoadColumn(0)] public uint StudentId;
        [LoadColumn(1)] public uint CourseId;
        [LoadColumn(2), ColumnName("Label")] public float Grade;
    }

    public class CourseRecommendationService
    {
        private ITransformer _model;
        private DataViewSchema _schema;
        private ITransformer _cfCourseModel;
        private PredictionEngine<CourseCfData, CourseScorePrediction> _cfCourseEngine;
        private readonly AppDbContext _ctx;
        private readonly MLContext _ml;
        private const string _path = "courses.zip";
        private readonly ICosineSimilarityService _cosine;

        public CourseRecommendationService(AppDbContext ctx, ICosineSimilarityService cosine)
        {
            _ctx = ctx;
            _ml = new MLContext(seed: 0);
            _cosine = cosine;
            TrainModel();
        }

        public void TrainModel()
        {
            var data = new List<CourseTrainData>();
            foreach (var gr in _ctx.Grades)
            {
                var course = gr.Courses.First();
                data.Add(new CourseTrainData
                {
                    SkillsMatchCount = 0,
                    FavTeacherCount = 0,
                    Rating = gr.Grade
                });
            }

            var dv = _ml.Data.LoadFromEnumerable(data);

            var cfCourseData = _ctx.Grades
                .SelectMany(g => g.Students.Select(s => new CourseCfData
                {
                    StudentId = CourseRecommendationService.GuidToUIntMapper.Map(s.Id),
                    CourseId = CourseRecommendationService.GuidToUIntMapper.Map(g.Courses.First().Id),
                    Grade = g.Grade
                }))
                .ToList();
            var cfCdDv = _ml.Data.LoadFromEnumerable(cfCourseData);

            var cfCoursePipe = _ml.Transforms.Conversion.MapValueToKey("UserEncoded", nameof(CourseCfData.StudentId))
                                 .Append(_ml.Transforms.Conversion.MapValueToKey("ItemEncoded", nameof(CourseCfData.CourseId)))
                                 .Append(_ml.Recommendation().Trainers.MatrixFactorization(
                                     new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
                                     {
                                         MatrixColumnIndexColumnName = "UserEncoded",
                                         MatrixRowIndexColumnName = "ItemEncoded",
                                         LabelColumnName = "Label",
                                         NumberOfIterations = 20,
                                         ApproximationRank = 64
                                     }));
            _cfCourseModel = cfCoursePipe.Fit(cfCdDv);
            _cfCourseEngine = _ml.Model.CreatePredictionEngine<CourseCfData, CourseScorePrediction>(_cfCourseModel);

            var pipe = _ml.Transforms.Concatenate("Features",
                            nameof(CourseTrainData.SkillsMatchCount),
                            nameof(CourseTrainData.FavTeacherCount))
                       .Append(_ml.Regression.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));
            _model = pipe.Fit(dv);

            using (var fs = new FileStream(_path, FileMode.Create, FileAccess.Write))
                _ml.Model.Save(_model, dv.Schema, fs);

            _schema = dv.Schema;
        }

        public IList<CourseEntity> GetRecommendedCourses(Guid studentId, int topN = 5)
        {
            if (_model == null) TrainModel();

            var seen = _ctx.Grades
                .Where(g => g.Students.Any(s => s.Id == studentId))
                .SelectMany(g => g.Courses.Select(c => c.Id))
                .ToHashSet();

            var prefSkills = _ctx.Grades
                .Where(g => g.Grade >= 3 && g.Students.Any(s => s.Id == studentId))
                .SelectMany(g => g.Courses)
                .SelectMany(c => c.Teachers.Select(t => t.SkillId))
                .ToHashSet();

            var favT = _ctx.TeacherRatings
                .Where(r => r.StudentId == studentId && r.Rating >= 4)
                .Select(r => r.TeacherId).ToHashSet();

            var cands = _ctx.Courses
                .Where(c => !seen.Contains(c.Id) && c.Teachers.Any(t => prefSkills.Contains(t.SkillId)))
                .ToList();

            var pd = new List<CoursePredictData>();
            foreach (var c in cands)
            {
                pd.Add(new CoursePredictData
                {
                    CourseId = c.Id,
                    StudentId = studentId,
                    SkillsMatchCount = c.Teachers.Count(t => prefSkills.Contains(t.SkillId)),
                    FavTeacherCount = c.Teachers.Count(t => favT.Contains(t.Id))
                });
            }

            var fd = pd.Select(x => new CourseTrainData
            {
                SkillsMatchCount = x.SkillsMatchCount,
                FavTeacherCount = x.FavTeacherCount,
                Rating = 0
            });
            var dv2 = _ml.Data.LoadFromEnumerable(fd);
            var ftPreds = _model.Transform(dv2);

            var cfPredData = pd.Select(x => new CourseCfData
            {
                StudentId = GuidToUIntMapper.Map(x.StudentId),
                CourseId = GuidToUIntMapper.Map(x.CourseId),
                Grade = 0
            }).ToList();
            var cfDv2 = _ml.Data.LoadFromEnumerable(cfPredData);
            var cfPreds = _cfCourseModel.Transform(cfDv2);

            var ftScores = _ml.Data.CreateEnumerable<CourseScorePrediction>(ftPreds, reuseRowObject: false).ToArray();
            var cfScores = _ml.Data.CreateEnumerable<CourseScorePrediction>(cfPreds, reuseRowObject: false).ToArray();

            var results = ftScores
                .Select((p, i) =>
                {
                    var baseScore = p.Score;
                    var cfScore = cfScores[i].Score;
                    var cosineBonus = _cosine.Compute(
                        new float[] { pd[i].SkillsMatchCount },
                        new float[] { pd[i].SkillsMatchCount });
                    var finalScore = 0.5f * cfScore + 0.4f * baseScore + 0.1f * cosineBonus;
                    return new { pd[i].CourseId, Score = finalScore };
                })
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .Select(x => _ctx.Courses.First(c => c.Id == x.CourseId))
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
