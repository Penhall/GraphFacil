using LotoLibrary.Infrastructure;
using LotoLibrary.Interfaces;
using LotoLibrary.NeuralNetwork.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace LotoLibrary.NeuralNetwork
{
    public class MLNetModel
    {
        private readonly MLContext _mlContext;
        private readonly IMLLogger _logger;
        private readonly string _modelPath;
        private readonly ModelConfigurationService _configService;
        private readonly string _tipo;
        private ITransformer _trainedModel;
        private readonly FileService _fileService;

        public MLNetModel(IMLLogger logger, string modelPath, string tipo)
        {
            _mlContext = new MLContext(seed: 42);
            _logger = logger;
            _modelPath = modelPath;
            _tipo = tipo.ToUpper();
            _fileService = new FileService();
            _configService = new ModelConfigurationService(_logger);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            _logger.LogInformation($"MLNetModel inicializado com modelPath: {modelPath}");
        }

        private IEstimator<ITransformer> CriarPipeline(ModelConfigurationService.ModelConfiguration config)
        {
            var options = _configService.GetTrainerOptions(config);

            // Dimensão correta baseada no tipo
            int dimensao = _tipo == "SS" ? 7 : 5;

            var pipeline = _mlContext.Transforms
                .NormalizeMinMax("NormalizedFeatures", "Features")
                .Append(_mlContext.Transforms.CopyColumns("Features", "NormalizedFeatures"))
                .Append(_mlContext.Regression.Trainers.FastForest(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    numberOfLeaves: options.NumberOfLeaves,
                    numberOfTrees: options.NumberOfTrees,
                    minimumExampleCountPerLeaf: options.MinimumExampleCountPerLeaf
                ));

            return pipeline;
        }

        // Adicionar este método à classe MLNetModel
        public void CarregarModelo()
        {
            try
            {
                if (!File.Exists(_modelPath))
                {
                    _logger.LogWarning($"Arquivo do modelo não encontrado: {_modelPath}");
                    throw new FileNotFoundException($"Arquivo do modelo não encontrado: {_modelPath}");
                }

                DataViewSchema schema;
                _trainedModel = _mlContext.Model.Load(_modelPath, out schema);
                _logger.LogInformation($"Modelo carregado com sucesso de: {_modelPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar modelo: {ex.Message}", ex);
                throw;
            }
        }

        public void Train(string caminhoPercentuais, bool usarCrossValidation = true)
        {
            try
            {
                _logger.LogInformation($"Iniciando treinamento com arquivo: {caminhoPercentuais}");

                var config = _configService.LoadConfiguration();
                var percentuais = _fileService.CarregarDados<Dictionary<int, Dictionary<int, double>>>(caminhoPercentuais);

                ValidarDimensoesDados(percentuais);
                var dados = PrepararDadosParaCrossValidation(percentuais);

                if (usarCrossValidation)
                {
                    ExecutarCrossValidation(dados, config.CrossValidationFolds);
                }

                var pipeline = CriarPipeline(config);
                _trainedModel = pipeline.Fit(dados);

                AvaliarModelo(dados);
                SalvarModelo(dados.Schema);

                _logger.LogInformation("Treinamento concluído com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro durante treinamento: {ex.Message}", ex);
                throw;
            }
        }

        private void ValidarDimensoesDados(Dictionary<int, Dictionary<int, double>> percentuais)
        {
            var dimensaoEsperada = _tipo == "SS" ? 7 : 5;
            foreach (var item in percentuais)
            {
                if (item.Value.Count != dimensaoEsperada)
                {
                    throw new ArgumentException($"Dimensão incorreta nos dados. Esperado {dimensaoEsperada}, encontrado {item.Value.Count}");
                }
            }
        }

        private IDataView PrepararDadosParaCrossValidation(Dictionary<int, Dictionary<int, double>> percentuais)
        {
            try
            {
                if (_tipo == "SS")
                {
                    var dadosTreino = percentuais.Select(p => new SubgrupoSSFeatures
                    {
                        Features = p.Value.Values.Select(v => (float)v).ToArray(),
                        Label = p.Value.Values.Max(v => (float)v)
                    }).ToList();

                    return _mlContext.Data.LoadFromEnumerable(dadosTreino);
                }
                else
                {
                    var dadosTreino = percentuais.Select(p => new SubgrupoNSFeatures
                    {
                        Features = p.Value.Values.Select(v => (float)v).ToArray(),
                        Label = p.Value.Values.Max(v => (float)v)
                    }).ToList();

                    return _mlContext.Data.LoadFromEnumerable(dadosTreino);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao preparar dados para cross validation: {ex.Message}", ex);
                throw;
            }
        }

        private void ExecutarCrossValidation(IDataView dados, int numFolds)
        {
            try
            {
                _logger.LogInformation($"Iniciando validação cruzada com {numFolds} folds");

                var config = _configService.LoadConfiguration();
                var pipeline = CriarPipeline(config);

                var results = _mlContext.Regression.CrossValidate(
                    data: dados,
                    estimator: pipeline,
                    numberOfFolds: numFolds);

                var metricsAverage = new Dictionary<string, double>
                {
                    {"R² Médio", results.Average(r => r.Metrics.RSquared)},
                    {"RMSE Médio", results.Average(r => r.Metrics.RootMeanSquaredError)},
                    {"MAE Médio", results.Average(r => r.Metrics.MeanAbsoluteError)}
                };

                _logger.LogMetrics("Cross Validation Results", metricsAverage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na validação cruzada: {ex.Message}", ex);
                throw;
            }
        }

        private void AvaliarModelo(IDataView dados)
        {
            try
            {
                var predictions = _trainedModel.Transform(dados);
                var metrics = _mlContext.Regression.Evaluate(predictions);

                var metricsDict = new Dictionary<string, double>
                {
                    {"R²", metrics.RSquared},
                    {"RMSE", metrics.RootMeanSquaredError},
                    {"MAE", metrics.MeanAbsoluteError},
                    {"MSE", metrics.MeanSquaredError}
                };

                _logger.LogMetrics("Model Evaluation", metricsDict);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao avaliar modelo: {ex.Message}", ex);
                throw;
            }
        }

        private void SalvarModelo(DataViewSchema schema)
        {
            try
            {
                _mlContext.Model.Save(_trainedModel, schema, _modelPath);
                _logger.LogInformation($"Modelo salvo em: {_modelPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao salvar modelo: {ex.Message}", ex);
                throw;
            }
        }

        //public float Predict(float[] features)
        //{
        //    try
        //    {
        //        if (_trainedModel == null)
        //        {
        //            throw new InvalidOperationException("Modelo não está treinado");
        //        }

        //        var inputData = _tipo == "SS" ?
        //            (BaseMLFeatures)new SubgrupoSSFeatures { Features = features } :
        //            new SubgrupoNSFeatures { Features = features };

        //        var predictionEngine = _mlContext.Model.CreatePredictionEngine<BaseMLFeatures, SubgrupoMLOutput>(_trainedModel);
        //        var prediction = predictionEngine.Predict(inputData);

        //        _logger.LogInformation($"Predição realizada com sucesso: {prediction.Score}");
        //        return prediction.Score;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Erro ao realizar predição: {ex.Message}", ex);
        //        throw;
        //    }
        //}


        public float Predict(float[] features)
        {
            try
            {
                if (_trainedModel == null)
                    throw new InvalidOperationException("Modelo não está treinado");

                var input = new MLFeatures
                {
                    Features = features,
                    Label = 0
                };

                var predictionEngine = _mlContext.Model.CreatePredictionEngine<MLFeatures, MLOutput>(_trainedModel);
                return predictionEngine.Predict(input).Score;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao realizar predição: {ex.Message}", ex);
                throw;
            }
        }


    }
}