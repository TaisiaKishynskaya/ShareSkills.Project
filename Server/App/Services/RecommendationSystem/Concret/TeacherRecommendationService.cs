using App.Services.RecommendationSystem.Abstract;
using App.Services.RecommendationSystem.Models;
using Libraries.Data;
using Libraries.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;

namespace App.Services.RecommendationSystem.Concret;

public class TeacherRecommendationService : ITeacherRecommendationService
{
    private readonly AppDbContext _dbContext;
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private PredictionEngine<TeacherRatingData, TeacherRatingPrediction> _predictionEngine;
    private const string _modelPath = "teacherModel.zip";

    public TeacherRecommendationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _mlContext = new MLContext();
        Train();
        if (File.Exists(_modelPath))
        {
            using var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read);
            _model = _mlContext.Model.Load(stream, out _);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<TeacherRatingData, TeacherRatingPrediction>(_model);
        }
    }
    
    public void Train()
    {
        /*var data = _dbContext.TeacherRatings
            .Select(r => new TeacherRatingData
            {
                StudentId = GuidToUIntMapper.Map(r.StudentId),
                TeacherId = GuidToUIntMapper.Map(r.TeacherId),
                Label = r.Rating
            })
            .ToList();*/

        var data = new TeacherRatingData[]
        {
            new() { StudentId = 1, TeacherId = 1, Label = 4 },
            new() { StudentId = 1, TeacherId = 2, Label = 3 },
            new() { StudentId = 2, TeacherId = 1, Label = 5 },
            new() { StudentId = 2, TeacherId = 2, Label = 2 }
        };
        
        var trainingData = _mlContext.Data.LoadFromEnumerable(data);

        /*var pipeline = _mlContext.Recommendation().Trainers.MatrixFactorization(options)
            .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "StudentId", outputColumnName:"StudentIdFeaturized"))
            .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "TeacherId", outputColumnName:"TeacherIdFeaturized"))
            .Append(_mlContext.Transforms.Concatenate("Features", "StudentId", "TeacherId"))
            .AppendCacheCheckpoint(_mlContext)
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));*/
        
        var pipelien = _mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: "StudentIdEncoded",
                inputColumnName: nameof(TeacherRatingData.StudentId))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: "TeacherIdEncoded",
                inputColumnName: nameof(TeacherRatingData.TeacherId)))
            .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "StudentIdEncoded",
                MatrixRowIndexColumnName = "TeacherIdEncoded",
                LabelColumnName = nameof(TeacherRatingData.Label),
                NumberOfIterations = 20,
                ApproximationRank = 100
            }));

        _model = pipelien.Fit(trainingData);

        using var stream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write);
        _mlContext.Model.Save(_model, trainingData.Schema, stream);

        _predictionEngine = _mlContext.Model.CreatePredictionEngine<TeacherRatingData, TeacherRatingPrediction>(_model);
    }

    public List<(TeacherEntity Teacher, float Score)> GetRecommendations(Guid studentId, int topN = 5)
    {
        if (_predictionEngine == null)
            throw new InvalidOperationException("Model is not trained yet.");
        
        // Получить ID уже оцененных преподавателей
        var ratedTeacherIds = _dbContext.TeacherRatings
            .Where(r => r.StudentId == studentId)
            .Select(r => r.TeacherId)
            .ToHashSet();

        // Получить ID преподавателей с положительными оценками
        var positivelyRatedTeacherIds = _dbContext.TeacherRatings
            .Where(r => r.StudentId == studentId && r.Rating >= 3)
            .Select(r => r.TeacherId)
            .ToList();

        // Предпочитаемые предметы на основе учителей с положительными оценками
        var preferredSubjects = _dbContext.Teachers
            .Where(t => positivelyRatedTeacherIds.Contains(t.Id))
            .SelectMany(t => t.Courses.Select(s => s.Name))
            .Distinct()
            .ToList();

        // Преподаватели, которых ещё не оценивали
        var unratedTeachers = _dbContext.Teachers
            .Where(t => !ratedTeacherIds.Contains(t.Id)).Include(teacherEntity => teacherEntity.Courses)
            .ToList();

        // Фильтрация по предметам, если есть предпочтения
        if (preferredSubjects.Any())
        {
            unratedTeachers = unratedTeachers
                .Where(t => t.Courses.Any(s => preferredSubjects.Contains(s.Name)))
                .ToList();
        }

        var results = new List<(TeacherEntity, float)>();

        foreach (var teacher in unratedTeachers)
        {
            var prediction = _predictionEngine.Predict(new TeacherRatingData
            {
                StudentId = GuidToUIntMapper.Map(studentId),
                TeacherId = GuidToUIntMapper.Map(teacher.Id)
            });

            // Повышаем рейтинг, если предметы совпадают
            float similarityBonus = CosineSimilarity(
                teacher.Courses.Select(s => s.Name).ToList(),
                preferredSubjects
            );

            results.Add((teacher, prediction.Score + similarityBonus)); // то есть тут мы даем преподавателю "вес" (оценку) на основании его предикшен результата и доп значения высчитанного на основе косинусного сходства, которое служит как дополнительной мерой к всему том, что было посчитано ранее
        }

        return results
            .OrderByDescending(x => x.Item2)
            .Take(topN)
            .ToList();
    }

    private float CosineSimilarity(List<string> listA, List<string> listB)
    {
        var union = listA.Union(listB).Distinct().ToList();
        var vecA = union.Select(x => listA.Contains(x) ? 1 : 0).ToArray();
        var vecB = union.Select(x => listB.Contains(x) ? 1 : 0).ToArray();

        float dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < vecA.Length; i++)
        {
            dot += vecA[i] * vecB[i];
            magA += vecA[i] * vecA[i];
            magB += vecB[i] * vecB[i];
        }

        return (float)(dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-6));
    }
    
    public static class GuidToUIntMapper
    {
        private static readonly Dictionary<Guid, uint> _map = new();
        private static uint _nextId = 0;

        public static uint Map(Guid guid)
        {
            /*if (_map.TryGetValue(guid, out var id))
                return id;

            id = _nextId + 1;
            _map[guid] = id;
            return id;*/
            
            return BitConverter.ToUInt32(guid.ToByteArray(), 0);
        }
    }
}