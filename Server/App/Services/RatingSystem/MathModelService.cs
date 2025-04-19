using App.Services.RatingSystem.Models;
using Libraries.Data;
using Microsoft.ML;

namespace App.Services.RatingSystem;

public class Recommender
{
    protected readonly MLContext _mlContext;
    protected readonly AppDbContext _dbContext;
    protected ITransformer _model = null!;
    protected readonly string _modelPath;
    protected string _columnUser = nameof(RatingInput.UserId);
    protected string _columnItem = nameof(RatingInput.ItemId);
    protected string _columnLabel = nameof(RatingInput.Label);

    public Recommender(AppDbContext dbContext, string modelFileName)
    {
        _modelPath = modelFileName;
        _dbContext = dbContext;
        _mlContext = new MLContext();
    }

    //TODO: этот метод использовать в случае, если вы хотите каждый раз пересчитывать модель. Это будет очень долго, поэтому я б рекомендовала этого избегать, если к данным очень частое обращение
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
    
    //TODO: этот метод использовать в случае, если вы хотите 1 раз посчитать модель, а потом использовать уже готовую, сохраненную в файл, модель
    // Этот метод можно поместить в бекграунд процесс, который будет фоном рестартовать и пересчитывать модель раз в час\день что б пользователи имели данные как можно актуальнее
    // Если учителя и курсы не будут меняться со временем, то можно тогда просто сгенерировать модели и использовать их
    protected void TrainAndSave(IEnumerable<RatingInput> inputs)
    {
        var dataView = _mlContext.Data.LoadFromEnumerable(inputs);
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
        _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
    }

    protected float PredictScore(RatingInput input)
    {
        if (_model == null)
            throw new InvalidOperationException("Model is not trained or loaded.");
        var engine = _mlContext.Model.CreatePredictionEngine<RatingInput, PredictionResult>(_model);
        return engine.Predict(input).Score;
    }

    private void LoadModel()
    {
        if (File.Exists(_modelPath))
        {
            DataViewSchema schema;
            _model = _mlContext.Model.Load(_modelPath, out schema);
        }
    }
}