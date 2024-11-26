using LotoLibrary.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.NeuralNetwork.Services;

public class CrossValidationService
{
    private readonly MLContext _mlContext;
    private readonly IMLLogger _logger;

    public CrossValidationService(IMLLogger logger)
    {
        _mlContext = new MLContext(seed: 42);
        _logger = logger;
    }

    public void PerformCrossValidation(IDataView data, int numberOfFolds = 5)
    {
        try
        {
            _logger.LogInformation($"Iniciando validação cruzada com {numberOfFolds} folds...");

            var results = _mlContext.Regression.CrossValidate(
                data,
                new EstimatorChain<RegressionPredictionTransformer<FastForestRegressionModelParameters>>()
                    .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"))
                    .Append(_mlContext.Transforms.CopyColumns("Features", "NormalizedFeatures"))
                    .Append(_mlContext.Regression.Trainers.FastForest()),
                numberOfFolds);

            // Calcular métricas médias
            var avgMetrics = new Dictionary<string, double>
                {
                    {"R² Médio", results.Average(r => r.Metrics.RSquared)},
                    {"RMSE Médio", results.Average(r => r.Metrics.RootMeanSquaredError)},
                    {"MAE Médio", results.Average(r => r.Metrics.MeanAbsoluteError)}
                };

            _logger.LogMetrics("Cross Validation", avgMetrics);
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro durante validação cruzada", ex);
            throw;
        }
    }

}