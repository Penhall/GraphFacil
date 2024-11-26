using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.NeuralNetwork.Services;

public class ModelTrainingService
{
    private readonly MLContext _mlContext;
    private readonly IMLLogger _logger;
    private readonly string _modelPath;
    private ITransformer _trainedModel;

    public ModelTrainingService(IMLLogger logger, string modelPath = "LotoModel.zip")
    {
        _mlContext = new MLContext(seed: 42);
        _logger = logger;
        _modelPath = modelPath;
    }

    public void TrainModel(IDataView trainingData, FastForestRegressionTrainer.Options options = null)
    {
        try
        {
            _logger.LogInformation("Iniciando treinamento do modelo...");

            // Configurar pipeline
            var pipeline = CreateTrainingPipeline(options);

            // Treinar modelo
            _trainedModel = pipeline.Fit(trainingData);

            // Salvar modelo
            SaveModel(trainingData.Schema);

            _logger.LogInformation("Treinamento concluído com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro durante o treinamento do modelo", ex);
            throw;
        }
    }

    private IEstimator<ITransformer> CreateTrainingPipeline(FastForestRegressionTrainer.Options options = null)
    {
        // Configurar opções padrão se não fornecidas
        options ??= new FastForestRegressionTrainer.Options
        {
            NumberOfTrees = 100,
            NumberOfLeaves = 20,
            MinimumExampleCountPerLeaf = 10
        };

        // Criar pipeline
        var pipeline = _mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features")
            .Append(_mlContext.Transforms.CopyColumns("Features", "NormalizedFeatures"))
            .Append(_mlContext.Regression.Trainers.FastForest(options));

        _logger.LogInformation($"Pipeline configurado com: Trees={options.NumberOfTrees}, Leaves={options.NumberOfLeaves}");

        return pipeline;
    }

    public void EvaluateModel(IDataView testData)
    {
        try
        {
            _logger.LogInformation("Iniciando avaliação do modelo...");

            var predictions = _trainedModel.Transform(testData);
            var metrics = _mlContext.Regression.Evaluate(predictions);

            var metricsDict = new Dictionary<string, double>
                {
                    {"R²", metrics.RSquared},
                    {"RMSE", metrics.RootMeanSquaredError},
                    {"MAE", metrics.MeanAbsoluteError},
                    {"MSE", metrics.MeanSquaredError}
                };

            _logger.LogMetrics("FastForest Regression", metricsDict);
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro durante a avaliação do modelo", ex);
            throw;
        }
    }

    public void SaveModel(DataViewSchema trainingSchema)
    {
        try
        {
            _logger.LogInformation($"Salvando modelo em: {_modelPath}");
            _mlContext.Model.Save(_trainedModel, trainingSchema, _modelPath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao salvar modelo: {_modelPath}", ex);
            throw;
        }
    }

    public void LoadModel()
    {
        try
        {
            _logger.LogInformation($"Carregando modelo de: {_modelPath}");
            _trainedModel = _mlContext.Model.Load(_modelPath, out var modelSchema);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar modelo: {_modelPath}", ex);
            throw;
        }
    }

    public float Predict(PercentualInput input)
    {
        try
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<PercentualInput, PercentualPrediction>(_trainedModel);
            var prediction = predictionEngine.Predict(input);

            _logger.LogInformation($"Predição realizada: {prediction.Resultado}");
            return prediction.Resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao realizar predição", ex);
            throw;
        }
    }
}
