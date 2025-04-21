using Microsoft.ML.Data;

namespace App.Services.RecommendationSystem.Models;

public class TeacherRatingPrediction
{
    [ColumnName("PredictedLabel")]
    public float Score { get; set; }
}