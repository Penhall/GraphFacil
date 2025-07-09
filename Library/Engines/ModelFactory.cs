// D:\PROJETOS\GraphFacil\LotoLibrary\Engines\ModelFactory.cs - Expansão para suportar novos modelos
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Prediction;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.PredictionModels.AntiFrequency;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Engines
{
    /// <summary>
    /// Factory expandida para criação de todos os modelos de predição
    /// </summary>
    public class ModelFactory : IModelFactory
    {
        #region Private Fields
        private readonly Dictionary<ModelType, Func<IPredictionModel>> _modelCreators;
        private readonly Dictionary<ModelType, ModelInfo> _modelInfos;
        #endregion

        #region Constructor
        public ModelFactory()
        {
            _modelCreators = new Dictionary<ModelType, Func<IPredictionModel>>();
            _modelInfos = new Dictionary<ModelType, ModelInfo>();
            
            RegisterBuiltInModels();
        }
        #endregion

        #region IModelFactory Implementation
        public IPredictionModel CreateModel(ModelType type, Dictionary<string, object> parameters = null)
        {
            try
            {
                if (!_modelCreators.ContainsKey(type))
                {
                    throw new NotSupportedException($"Tipo de modelo '{type}' não está registrado");
                }

                var model = _modelCreators[type]();
                
                // Configurar parâmetros se o modelo suportar configuração
                if (model is IConfigurableModel configurableModel && parameters != null)
                {
                    if (configurableModel.ValidateParameters(parameters))
                    {
                        configurableModel.UpdateParameters(parameters);
                    }
                    else
                    {
                        throw new ArgumentException($"Parâmetros inválidos para o modelo '{type}'");
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar modelo {type}: {ex.Message}", ex);
            }
        }

        public IEnsembleModel CreateEnsemble(List<ModelType> modelTypes, Dictionary<string, double> weights = null)
        {
            try
            {
                var models = new List<IPredictionModel>();
                
                foreach (var modelType in modelTypes)
                {
                    models.Add(CreateModel(modelType));
                }

                // TODO: Implementar EnsembleModel na Fase 3
                throw new NotImplementedException("EnsembleModel será implementado na Fase 3");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar ensemble: {ex.Message}", ex);
            }
        }

        public List<ModelType> GetAvailableModelTypes()
        {
            return _modelCreators.Keys.ToList();
        }

        public ModelInfo GetModelInfo(ModelType type)
        {
            return _modelInfos.ContainsKey(type) ? _modelInfos[type] : null;
        }
        #endregion

        #region Model Registration
        private void RegisterBuiltInModels()
        {
            // Modelo Original - Metronomo
            RegisterModel(
                ModelType.Metronomo,
                () => new MetronomoModel(),
                new ModelInfo
                {
                    Type = ModelType.Metronomo,
                    Name = "Metronomo Model",
                    Description = "Modelo original baseado em oscilação e sincronização de dezenas",
                    Category = ModelCategory.Individual,
                    DefaultParameters = new Dictionary<string, object>(),
                    RequiredDataSize = 50,
                    EstimatedAccuracy = 0.605,
                    IsConfigurable = false
                }
            );

            // Novo Modelo - Anti-Frequency Simple
            RegisterModel(
                ModelType.AntiFrequencySimple,
                () => new AntiFrequencySimpleModel(),
                new ModelInfo
                {
                    Type = ModelType.AntiFrequencySimple,
                    Name = "Anti-Frequency Simple",
                    Description = "Modelo que prioriza dezenas com menor frequência histórica",
                    Category = ModelCategory.AntiFrequency,
                    DefaultParameters = new Dictionary<string, object>
                    {
                        { "JanelaHistorica", 100 },
                        { "FatorDecaimento", 0.1 },
                        { "Epsilon", 0.001 },
                        { "PesoTemporal", 0.8 }
                    },
                    RequiredDataSize = 20,
                    EstimatedAccuracy = 0.63,
                    IsConfigurable = true
                }
            );

            // Placeholders para modelos futuros
            RegisterFutureModels();
        }

        private void RegisterFutureModels()
        {
            // Placeholder para StatisticalDebtModel
            _modelInfos[ModelType.StatisticalDebt] = new ModelInfo
            {
                Type = ModelType.StatisticalDebt,
                Name = "Statistical Debt Model",
                Description = "Modelo baseado em dívida estatística de cada dezena",
                Category = ModelCategory.AntiFrequency,
                DefaultParameters = new Dictionary<string, object>
                {
                    { "JanelaAnalise", 150 },
                    { "FatorDecaimento", 0.05 },
                    { "ThresholdDivida", 2.0 },
                    { "NormalizarVolatilidade", true }
                },
                RequiredDataSize = 100,
                EstimatedAccuracy = 0.64,
                IsConfigurable = true,
                Status = ModelStatus.PlannedForPhase2
            };

            // Placeholder para SaturationModel
            _modelInfos[ModelType.Saturation] = new ModelInfo
            {
                Type = ModelType.Saturation,
                Name = "Saturation Model",
                Description = "Modelo que detecta saturação usando indicadores técnicos",
                Category = ModelCategory.AntiFrequency,
                DefaultParameters = new Dictionary<string, object>
                {
                    { "PeriodoRSI", 14 },
                    { "LimitesSuperior", 70 },
                    { "LimiteInferior", 30 },
                    { "BandsBollinger", true }
                },
                RequiredDataSize = 50,
                EstimatedAccuracy = 0.65,
                IsConfigurable = true,
                Status = ModelStatus.PlannedForPhase2
            };

            // Placeholder para PendularOscillatorModel
            _modelInfos[ModelType.PendularOscillator] = new ModelInfo
            {
                Type = ModelType.PendularOscillator,
                Name = "Pendular Oscillator Model",
                Description = "Modelo baseado em análise de ciclos e acoplamento",
                Category = ModelCategory.AntiFrequency,
                DefaultParameters = new Dictionary<string, object>
                {
                    { "JanelaFFT", 200 },
                    { "ThresholdCorrelacao", 0.7 },
                    { "DetectarInflexoes", true },
                    { "UsarAcoplamento", true }
                },
                RequiredDataSize = 200,
                EstimatedAccuracy = 0.66,
                IsConfigurable = true,
                Status = ModelStatus.PlannedForPhase2
            };
        }

        private void RegisterModel(ModelType type, Func<IPredictionModel> creator, ModelInfo info)
        {
            _modelCreators[type] = creator;
            _modelInfos[type] = info;
        }
        #endregion

        #region Utility Methods
        public List<ModelInfo> GetModelsByCategory(ModelCategory category)
        {
            return _modelInfos.Values.Where(info => info.Category == category).ToList();
        }

        public List<ModelInfo> GetAvailableModels()
        {
            return _modelInfos.Values.Where(info => info.Status == ModelStatus.Available).ToList();
        }

        public List<ModelInfo> GetPlannedModels()
        {
            return _modelInfos.Values.Where(info => info.Status == ModelStatus.PlannedForPhase2).ToList();
        }

        public bool IsModelTypeSupported(ModelType type)
        {
            return _modelCreators.ContainsKey(type);
        }

        public ModelInfo GetModelConfiguration(ModelType type)
        {
            return _modelInfos.ContainsKey(type) ? _modelInfos[type] : null;
        }
        #endregion
    }

    #region Supporting Enums and Classes
    public enum ModelType
    {
        // Modelos Individuais
        Metronomo,
        MLNet,
        
        // Modelos Anti-Frequencistas
        AntiFrequencySimple,
        StatisticalDebt,
        Saturation,
        PendularOscillator,
        
        // Modelos Avançados (Fase 4)
        GraphNeuralNetwork,
        Autoencoder,
        ReinforcementLearning,
        
        // Modelos Ensemble (Fase 3)
        BasicEnsemble,
        WeightedEnsemble,
        StackedEnsemble,
        
        // Meta-Modelos (Fase 5)
        MetaLearning,
        AdaptiveWeights,
        RegimeDetection
    }

    public enum ModelCategory
    {
        Individual,
        AntiFrequency,
        Advanced,
        Ensemble,
        Meta
    }

    public enum ModelStatus
    {
        Available,
        PlannedForPhase2,
        PlannedForPhase3,
        PlannedForPhase4,
        PlannedForPhase5,
        Deprecated
    }

    public class ModelInfo
    {
        public ModelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ModelCategory Category { get; set; }
        public Dictionary<string, object> DefaultParameters { get; set; }
        public int RequiredDataSize { get; set; }
        public double EstimatedAccuracy { get; set; }
        public bool IsConfigurable { get; set; }
        public ModelStatus Status { get; set; } = ModelStatus.Available;
        public string Version { get; set; } = "1.0";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<string> Dependencies { get; set; } = new List<string>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
    #endregion
}
