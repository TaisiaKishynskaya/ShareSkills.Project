using App.Services.RatingSystem.Models;
using Libraries.Data;
using Microsoft.ML;

namespace App.Services.RatingSystem;

public class Recommender// : IMathModelService
{/*
    private readonly MLContext _mlContext;
    private readonly AppDbContext _dbContext;
    private ITransformer _model;
    
    public MathModelService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _mlContext = new MLContext();
    }

    public async Task Train()
    {
        // Завантажуємо всі оцінки (GradeEntity)
        var flatRatings = await _dbContext.Grades
            .SelectMany(g => g.Students.Select(u => new RatingInput {
                UserId = u.Id.ToString(),
                CourseId = g.Courses.First().Id.ToString(),
                Label = g.Grade
            }))
            .ToListAsync();

        // Створюємо IDataView
        var data = _mlContext.Data.LoadFromEnumerable(flatRatings);

        // Конвеєр перетворень:
        // 1) MapValueToKey для Guid
        // 2) Matrix Factorization
        var pipeline = _mlContext.Transforms.Conversion
            .MapValueToKey("userKey", nameof(RatingInput.UserId))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("courseKey", nameof(RatingInput.CourseId)))
            .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "userKey",
                    MatrixRowIndexColumnName = "courseKey",
                    LabelColumnName = nameof(RatingInput.Label),
                    NumberOfIterations = 20,
                    ApproximationRank = 50
                }))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Score"));

        _model = pipeline.Fit(data);
    }

    public float Predict(Guid userId, Guid courseId)
    {
        var engine = _mlContext.Model.CreatePredictionEngine<RatingInput, CoursePrediction>(_model);
        var input = new RatingInput { UserId = userId.ToString(), CourseId = courseId.ToString() };
        var pred = engine.Predict(input);
        return pred.Score;
    }

    public async Task<IEnumerable<CourseEntity>> RecommendCourses(Guid userId, int topN = 5)
    {
        var courses = await _dbContext.Courses.ToListAsync();
        var scored = courses.Select(c => new { Course = c, Score = Predict(userId, c.Id) })
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .Select(x => x.Course);
        return scored;
    }*/
    
    
    protected readonly MLContext _mlContext;
    protected readonly AppDbContext _dbContext;
    protected ITransformer _model = null!;
    protected string _columnUser = nameof(RatingInput.UserId);
    protected string _columnItem = nameof(RatingInput.ItemId);
    protected string _columnLabel = nameof(RatingInput.Label);

    public Recommender(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _mlContext = new MLContext();
    }

    protected void TrainModel(IEnumerable<RatingInput> dataInputs)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(dataInputs);
        var pipeline = _mlContext.Transforms.Conversion
            .MapValueToKey("userKey", _columnUser)
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("itemKey", _columnItem))
            .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "userKey",
                    MatrixRowIndexColumnName = "itemKey",
                    LabelColumnName = _columnLabel,
                    NumberOfIterations = 20,
                    ApproximationRank = 50
                }))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Score"));

        _model = pipeline.Fit(dataView);
    }

    protected float PredictScore(RatingInput input)
    {
        var engine = _mlContext.Model.CreatePredictionEngine<RatingInput, PredictionResult>(_model);
        return engine.Predict(input).Score;
    }
}