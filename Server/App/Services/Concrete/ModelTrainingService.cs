using Libraries.Contracts;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace App.Services.Concrete;

public class ModelTrainingService
{
    private const string ModelPath = "MLModels/TeacherRecommendationModel.zip";

    public void TrainModel(List<TeacherRatingData> data)
    {
        var mlContext = new MLContext();

        var trainingData = mlContext.Data.LoadFromEnumerable(data);

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = nameof(TeacherRatingData.UserId),
            MatrixRowIndexColumnName = nameof(TeacherRatingData.TeacherId),
            LabelColumnName = nameof(TeacherRatingData.Rating),
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var pipeline = mlContext.Recommendation().Trainers.MatrixFactorization(options);

        var model = pipeline.Fit(trainingData);
        mlContext.Model.Save(model, trainingData.Schema, ModelPath);
    }
}
