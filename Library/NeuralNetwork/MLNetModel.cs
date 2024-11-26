using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using System.Text.Json;
using LotoLibrary.Infrastructure.Logging;
using LotoLibrary.Models;
using System.Globalization;
using LotoLibrary.Interfaces;
using LotoLibrary.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System;

namespace LotoLibrary.NeuralNetwork
{
    public class MLNetModel
    {
        private readonly MLContext _mlContext;
        private readonly IMLLogger _logger;
        private readonly string _modelPath;
        private readonly ModelConfigurationService _configService;
        private ITransformer _trainedModel;
        private readonly FileService _fileService;

        public MLNetModel(IMLLogger logger, string modelPath = "LotoModel.zip")
        {
            _mlContext = new MLContext(seed: 42);
            _logger = logger;
            _modelPath = modelPath;
            _fileService = new FileService();
            _configService = new ModelConfigurationService(_logger);

            // Garantir cultura invariant para operações numéricas
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            _logger.LogInformation($"MLNetModel inicializado com modelPath: {modelPath}");
        }

        public void Train(string caminhoPercentuais, bool usarCrossValidation = true)
        {
            try
            {
                _logger.LogInformation($"Iniciando treinamento com arquivo: {caminhoPercentuais}");

                // Carregar configuração
                var config = _configService.LoadConfiguration();

                // Preparar dados
                var percentuais = _fileService.CarregarDados<Dictionary<int, Dictionary<int, double>>>(caminhoPercentuais);
                var dados = PrepararDadosParaCrossValidation(percentuais);

                if (usarCrossValidation)
                {
                    ExecutarCrossValidation(dados, config.CrossValidationFolds);
                }

                // Criar e treinar modelo
                _trainedModel = CriarEtreinarModelo(dados, config);

                // Avaliar modelo
                AvaliarModelo(dados);

                // Salvar modelo
                SalvarModelo(dados.Schema);

                _logger.LogInformation("Treinamento concluído com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro durante treinamento: {ex.Message}", ex);
                throw;
            }
        }

        private IDataView PrepararDadosParaCrossValidation(Dictionary<int, Dictionary<int, double>> percentuais)
        {
            try
            {
                var dadosTreino = new List<SubgrupoMLFeatures>();

                foreach (var subgrupo in percentuais)
                {
                    var features = subgrupo.Value.Values.Select(v => (float)v).ToArray();
                    var label = features.Max(); // Usando o maior percentual como label

                    dadosTreino.Add(new SubgrupoMLFeatures
                    {
                        Features = features,
                        Label = label
                    });
                }

                return _mlContext.Data.LoadFromEnumerable(dadosTreino);
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
                var options = _configService.GetTrainerOptions(config);

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

        private ITransformer CriarEtreinarModelo(IDataView dados, ModelConfigurationService.ModelConfiguration config)
        {
            try
            {
                var options = _configService.GetTrainerOptions(config);

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

                _logger.LogInformation("Iniciando treinamento do modelo");
                var modelo = pipeline.Fit(dados);
                _logger.LogInformation("Modelo treinado com sucesso");

                return modelo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar/treinar modelo: {ex.Message}", ex);
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

        public void CarregarModelo()
        {
            try
            {
                if (!File.Exists(_modelPath))
                {
                    throw new FileNotFoundException($"Arquivo do modelo não encontrado: {_modelPath}");
                }

                _trainedModel = _mlContext.Model.Load(_modelPath, out _);
                _logger.LogInformation($"Modelo carregado de: {_modelPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar modelo: {ex.Message}", ex);
                throw;
            }
        }

        public float Predict(float[] features)
        {
            try
            {
                if (_trainedModel == null)
                {
                    throw new InvalidOperationException("Modelo não está treinado");
                }

                var inputData = new SubgrupoMLFeatures { Features = features };
                var predictionEngine = _mlContext.Model.CreatePredictionEngine<SubgrupoMLFeatures, SubgrupoMLOutput>(_trainedModel);
                var prediction = predictionEngine.Predict(inputData);

                _logger.LogInformation($"Predição realizada com sucesso: {prediction.Score}");
                return prediction.Score;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao realizar predição: {ex.Message}", ex);
                throw;
            }
        }

        public void AtualizarConfiguracao(ModelConfigurationService.ModelConfiguration novaConfig)
        {
            try
            {
                _configService.SaveConfiguration(novaConfig);
                _logger.LogInformation("Configuração atualizada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar configuração: {ex.Message}", ex);
                throw;
            }
        }
    }

    public class SubgrupoMLFeatures
    {
        [VectorType(7)]
        [ColumnName("Features")]
        public float[] Features { get; set; }

        [ColumnName("Label")]
        public float Label { get; set; }
    }

    public class SubgrupoMLOutput
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}