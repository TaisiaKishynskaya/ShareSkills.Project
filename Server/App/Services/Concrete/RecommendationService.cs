using Libraries.Contracts;
using Microsoft.ML;
using Libraries.Entities.Concrete;

namespace App.Services.Concrete;

public class RecommendationService
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model;

    public RecommendationService()
    {
        _mlContext = new MLContext();
        _model = _mlContext.Model.Load("MLModels/TeacherRecommendationModel.zip", out _);
    }

    public float PredictRating(uint userId, uint teacherId)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<TeacherRatingData, TeacherPrediction>(_model);
        var input = new TeacherRatingData { UserId = userId, TeacherId = teacherId };
        var prediction = predictionEngine.Predict(input);
        return prediction.Score;
    }

    public List<(TeacherEntity Teacher, float Score)> RecommendTopTeachers(uint userId, List<TeacherEntity> teachers)
    {
        var top = teachers
            .Select(t => new
            {
                Teacher = t,
                Score = PredictRating(userId, (uint)t.Id.GetHashCode()) // або збережене числове ID
            })
            .OrderByDescending(t => t.Score)
            .Take(5)
            .Select(t => (t.Teacher, t.Score))
            .ToList();

        return top;
    }
}
