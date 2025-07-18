// D:\PROJETOS\GraphFacil\Library\Engines\ModelFactory.cs - Implementação corrigida da interface
using System.Collections.Generic;
using System.Linq;
using System;
using LotoLibrary.Enums;
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Prediction;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using LotoLibrary.PredictionModels.AntiFrequency;
using LotoLibrary.PredictionModels.AntiFrequency.Statistical;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.PredictionModels.Ensemble;

namespace LotoLibrary.Engines
{
    /// <summary>
    /// Factory corrigida para criação de modelos de predição
    /// </summary>
    public partial class ModelFactory : IModelFactory
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

            RegisterModel(
                ModelType.Saturation,
                () => new SaturationModel(),
                new ModelInfo
                {
                    Type = ModelType.Saturation,
                    Name = "Saturation Model",
                    Description = "Modelo baseado em análise de saturação estatística de dezenas",
                    Category = ModelCategory.AntiFrequency,
                    DefaultParameters = new Dictionary<string, object>
                    {
                        { "JanelaAnalise", 50 },
                        { "LimiarSaturacao", 0.7 },
                        { "FatorPonderacao", 0.6 }
                    },
                    RequiredDataSize = 30,
                    EstimatedAccuracy = 0.65,
                    IsConfigurable = true
                }
            );

            RegisterModel(
                ModelType.Statistical,
                () => new StatisticalDebtModel(),
                new ModelInfo
                {
                    Type = ModelType.Statistical,
                    Name = "Statistical Debt Model",
                    Description = "Modelo baseado em análise de débito estatístico de dezenas",
                    Category = ModelCategory.AntiFrequency,
                    DefaultParameters = new Dictionary<string, object>
                    {
                        { "PeriodoAnalise", 100 },
                        { "PesoDebito", 0.8 },
                        { "FatorCorrecao", 0.15 }
                    },
                    RequiredDataSize = 40,
                    EstimatedAccuracy = 0.68,
                    IsConfigurable = true
                }
            );

            RegisterModel(
                ModelType.DeepLearning,
                () => new DeepLearningModel(),
                new ModelInfo
                {
                    Type = ModelType.DeepLearning,
                    Name = "Deep Learning Model",
                    Description = "Modelo de aprendizado profundo para predição avançada",
                    Category = ModelCategory.Individual,
                    DefaultParameters = new Dictionary<string, object>
                    {
                        { "Epochs", 100 },
                        { "LearningRate", 0.001 },
                        { "BatchSize", 32 },
                        { "HiddenLayers", 3 }
                    },
                    RequiredDataSize = 200,
                    EstimatedAccuracy = 0.72,
                    IsConfigurable = true
                }
            );

            RegisterModel(
                ModelType.MetaLearning,
                () => new MetaLearningModel(),
                new ModelInfo
                {
                    Type = ModelType.MetaLearning,
                    Name = "Meta Learning Model",
                    Description = "Modelo ensemble que combina múltiplos algoritmos de predição",
                    Category = ModelCategory.Ensemble,
                    DefaultParameters = new Dictionary<string, object>
                    {
                        { "NumModelos", 5 },
                        { "StrategyBlending", "Weighted" },
                        { "ConfiancaMinima", 0.6 }
                    },
                    RequiredDataSize = 100,
                    EstimatedAccuracy = 0.75,
                    IsConfigurable = true
                }
            );
        }

        private void RegisterModel(ModelType type, Func<IPredictionModel> creator, ModelInfo info)
        {
            _modelCreators[type] = creator;
            _modelInfos[type] = info;
        }

        #region Utility Methods
        public List<ModelInfo> GetModelsByCategory(ModelCategory category)
        {
            return _modelInfos.Values.Where(info => info.Category == category).ToList();
        }

        public List<ModelInfo> GetAvailableModels()
        {
            return _modelInfos.Values.Where(info => info.Status == ModelStatus.Available).ToList();
        }

        public bool IsModelTypeSupported(ModelType type)
        {
            return _modelCreators.ContainsKey(type);
        }
        #endregion
        #endregion
    }
}
