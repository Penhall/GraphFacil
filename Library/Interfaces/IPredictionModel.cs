// D:\PROJETOS\GraphFacil\Library\Interfaces\IPredictionModel.cs - Interface principal
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface base para todos os modelos de predição
    /// </summary>
    public interface IPredictionModel
    {
        #region Properties
        string ModelName { get; }
        string ModelType { get; }
        bool IsInitialized { get; }
        double Confidence { get; }
        DateTime LastTrainingTime { get; }
        int TrainingDataSize { get; }
        #endregion

        #region Core Methods
        Task<bool> InitializeAsync(Lances historicalData);
        Task<bool> TrainAsync(Lances trainingData);
        Task<PredictionResult> PredictAsync(int targetConcurso);
        Task<ValidationResult> ValidateAsync(Lances validationData);
        void Reset();
        #endregion

        #region Events
        event EventHandler<string> OnStatusChanged;
        event EventHandler<double> OnConfidenceChanged;
        #endregion
    }

    /// <summary>
    /// Interface para modelos que suportam configuração de parâmetros
    /// </summary>
    public interface IConfigurableModel : IPredictionModel
    {
        Dictionary<string, object> Parameters { get; set; }
        void UpdateParameters(Dictionary<string, object> newParameters);
        Dictionary<string, object> GetDefaultParameters();
        bool ValidateParameters(Dictionary<string, object> parameters);
    }

    /// <summary>
    /// Interface para modelos que explicam suas predições
    /// </summary>
    public interface IExplainableModel : IPredictionModel
    {
        ModelExplanation ExplainPrediction(PredictionResult prediction);
        List<FeatureImportance> GetFeatureImportances();
        string GetModelDescription();
    }

    /// <summary>
    /// Interface para ensemble de modelos
    /// </summary>
    public interface IEnsembleModel : IPredictionModel
    {
        List<IPredictionModel> SubModels { get; }
        Dictionary<string, double> ModelWeights { get; set; }
        
        void AddModel(IPredictionModel model, double weight = 1.0);
        bool RemoveModel(string modelName);
        void UpdateWeights(Dictionary<string, double> weights);
        EnsembleMetrics GetEnsembleMetrics();
        Task OptimizeWeightsAsync(Lances validationData);
    }

    /// <summary>
    /// Interface para análise de performance
    /// </summary>
    public interface IPerformanceAnalyzer
    {
        Task<PerformanceReport> AnalyzeAsync(IPredictionModel model, Lances testData);
        Task<ComparisonReport> CompareModelsAsync(List<IPredictionModel> models, Lances testData);
        Task<List<PerformanceMetric>> GetDetailedMetricsAsync(IPredictionModel model, Lances testData);
    }

    /// <summary>
    /// Interface para factory de modelos
    /// </summary>
    public interface IModelFactory
    {
        IPredictionModel CreateModel(ModelType type, Dictionary<string, object> parameters = null);
        IEnsembleModel CreateEnsemble(List<ModelType> modelTypes, Dictionary<string, double> weights = null);
        List<ModelType> GetAvailableModelTypes();
        ModelInfo GetModelInfo(ModelType type);
    }
}

// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class PredictionResult
    {
        public List<int> PredictedNumbers { get; set; } = new List<int>();
        public Dictionary<int, double> NumberProbabilities { get; set; } = new Dictionary<int, double>();
        public double OverallConfidence { get; set; }
        public string ModelUsed { get; set; }
        public DateTime PredictionTime { get; set; }
        public int TargetConcurso { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        
        // Métricas adicionais
        public TimeSpan ProcessingTime { get; set; }
        public string Version { get; set; } = "1.0";
    }

    public class ValidationResult
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public List<int> CorrectPredictions { get; set; } = new List<int>();
        public List<int> IncorrectPredictions { get; set; } = new List<int>();
        public TimeSpan ValidationTime { get; set; }
        public int TotalTests { get; set; }
        public Dictionary<string, double> DetailedMetrics { get; set; } = new Dictionary<string, double>();
    }

    public class ModelExplanation
    {
        public string Reasoning { get; set; }
        public List<DecisionFactor> Factors { get; set; } = new List<DecisionFactor>();
        public Dictionary<int, string> NumberReasons { get; set; } = new Dictionary<int, string>();
        public double ConfidenceLevel { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }

    public class DecisionFactor
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
    }

    public class FeatureImportance
    {
        public string FeatureName { get; set; }
        public double Importance { get; set; }
        public string Description { get; set; }
        public FeatureType Type { get; set; }
    }

    public class EnsembleMetrics
    {
        public Dictionary<string, double> ModelContributions { get; set; } = new Dictionary<string, double>();
        public double DiversityScore { get; set; }
        public double WeightOptimality { get; set; }
        public double OverallStability { get; set; }
        public DateTime LastOptimization { get; set; }
    }

    public class PerformanceReport
    {
        public string ModelName { get; set; }
        public DateTime ReportTime { get; set; }
        public ValidationResult ValidationResults { get; set; }
        public List<PerformanceMetric> Metrics { get; set; } = new List<PerformanceMetric>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public PerformanceGrade Grade { get; set; }
    }

    public class ComparisonReport
    {
        public DateTime ComparisonTime { get; set; }
        public List<ModelComparison> ModelComparisons { get; set; } = new List<ModelComparison>();
        public string BestModelName { get; set; }
        public string RecommendedStrategy { get; set; }
        public Dictionary<string, object> Statistics { get; set; } = new Dictionary<string, object>();
    }

    public class ModelComparison
    {
        public string ModelName { get; set; }
        public double Accuracy { get; set; }
        public double Confidence { get; set; }
        public TimeSpan AverageProcessingTime { get; set; }
        public PerformanceGrade Grade { get; set; }
        public int Rank { get; set; }
    }

    public class PerformanceMetric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public MetricType Type { get; set; }
        public bool IsGoodHigh { get; set; } // True se valor alto for bom
    }

    public class ModelInfo
    {
        public ModelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> RequiredParameters { get; set; } = new List<string>();
        public Dictionary<string, object> DefaultParameters { get; set; } = new Dictionary<string, object>();
        public bool SupportsExplanation { get; set; }
        public bool IsConfigurable { get; set; }
        public TimeSpan TypicalTrainingTime { get; set; }
    }

    // Enums
    public enum ModelType
    {
        // Modelos Existentes
        Metronomo,
        Oscillator,
        MLNet,
        
        // Modelos Anti-Frequencistas
        AntiFrequency,
        StatisticalDebt,
        Saturation,
        PendularOscillator,
        
        // Modelos Avançados
        GraphNeuralNetwork,
        Autoencoder,
        ReinforcementLearning,
        
        // Ensemble e Meta
        Ensemble,
        MetaLearning,
        
        // Especiais
        Random,
        Frequency
    }

    public enum FeatureType
    {
        Statistical,
        Temporal,
        Pattern,
        Frequency,
        Technical,
        Meta
    }

    public enum PerformanceGrade
    {
        F = 0,  // < 50%
        D = 1,  // 50-55%
        C = 2,  // 55-60%
        B = 3,  // 60-65%
        A = 4,  // 65-70%
        S = 5   // > 70%
    }

    public enum MetricType
    {
        Accuracy,
        Precision,
        Recall,
        Performance,
        Stability,
        Speed
    }
}

// D:\PROJETOS\GraphFacil\Library\Models\Base\PredictionModelBase.cs - Classe base
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LotoLibrary.Models.Base
{
    /// <summary>
    /// Classe base que implementa funcionalidades comuns a todos os modelos de predição
    /// </summary>
    public abstract class PredictionModelBase : IPredictionModel
    {
        #region Fields
        protected Lances _historicalData;
        protected Lances _trainingData;
        protected DateTime _lastTrainingTime;
        protected double _confidence;
        protected bool _isInitialized;
        #endregion

        #region Properties
        public abstract string ModelName { get; }
        public abstract string ModelType { get; }
        public bool IsInitialized => _isInitialized;
        public double Confidence => _confidence;
        public DateTime LastTrainingTime => _lastTrainingTime;
        public int TrainingDataSize => _trainingData?.Count ?? 0;
        #endregion

        #region Events
        public event EventHandler<string> OnStatusChanged;
        public event EventHandler<double> OnConfidenceChanged;
        #endregion

        #region Public Methods
        public virtual async Task<bool> InitializeAsync(Lances historicalData)
        {
            try
            {
                OnStatusChanged?.Invoke(this, "Inicializando modelo...");
                
                if (historicalData == null || !historicalData.Any())
                {
                    OnStatusChanged?.Invoke(this, "Erro: Dados históricos inválidos");
                    return false;
                }

                _historicalData = historicalData;
                
                var initResult = await DoInitializeAsync(historicalData);
                
                _isInitialized = initResult;
                OnStatusChanged?.Invoke(this, initResult ? "Modelo inicializado com sucesso" : "Falha na inicialização");
                
                return initResult;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> TrainAsync(Lances trainingData)
        {
            try
            {
                if (!_isInitialized)
                {
                    OnStatusChanged?.Invoke(this, "Erro: Modelo não inicializado");
                    return false;
                }

                OnStatusChanged?.Invoke(this, "Iniciando treinamento...");
                
                _trainingData = trainingData;
                
                var trainResult = await DoTrainAsync(trainingData);
                
                if (trainResult)
                {
                    _lastTrainingTime = DateTime.Now;
                    OnStatusChanged?.Invoke(this, "Treinamento concluído com sucesso");
                }
                else
                {
                    OnStatusChanged?.Invoke(this, "Falha no treinamento");
                }
                
                return trainResult;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"Erro no treinamento: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<PredictionResult> PredictAsync(int targetConcurso)
        {
            try
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException("Modelo não inicializado");
                }

                OnStatusChanged?.Invoke(this, $"Gerando predição para concurso {targetConcurso}...");
                
                var stopwatch = Stopwatch.StartNew();
                var result = await DoPredictAsync(targetConcurso);
                stopwatch.Stop();

                if (result != null)
                {
                    result.ModelUsed = ModelName;
                    result.PredictionTime = DateTime.Now;
                    result.ProcessingTime = stopwatch.Elapsed;
                    result.TargetConcurso = targetConcurso;
                    
                    _confidence = result.OverallConfidence;
                    OnConfidenceChanged?.Invoke(this, _confidence);
                    OnStatusChanged?.Invoke(this, "Predição gerada com sucesso");
                }

                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"Erro na predição: {ex.Message}");
                throw;
            }
        }

        public virtual async Task<ValidationResult> ValidateAsync(Lances validationData)
        {
            try
            {
                OnStatusChanged?.Invoke(this, "Iniciando validação...");
                
                var result = await DoValidateAsync(validationData);
                
                OnStatusChanged?.Invoke(this, $"Validação concluída - Accuracy: {result.Accuracy:P2}");
                
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"Erro na validação: {ex.Message}");
                throw;
            }
        }

        public virtual void Reset()
        {
            OnStatusChanged?.Invoke(this, "Resetando modelo...");
            
            _isInitialized = false;
            _confidence = 0;
            _historicalData = null;
            _trainingData = null;
            _lastTrainingTime = default;
            
            DoReset();
            
            OnStatusChanged?.Invoke(this, "Modelo resetado");
        }
        #endregion

        #region Abstract Methods
        protected abstract Task<bool> DoInitializeAsync(Lances historicalData);
        protected abstract Task<bool> DoTrainAsync(Lances trainingData);
        protected abstract Task<PredictionResult> DoPredictAsync(int targetConcurso);
        protected abstract Task<ValidationResult> DoValidateAsync(Lances validationData);
        protected abstract void DoReset();
        #endregion

        #region Protected Helpers
        protected void UpdateStatus(string status)
        {
            OnStatusChanged?.Invoke(this, status);
        }

        protected void UpdateConfidence(double confidence)
        {
            _confidence = Math.Max(0, Math.Min(1, confidence));
            OnConfidenceChanged?.Invoke(this, _confidence);
        }

        protected ValidationResult CreateValidationResult(List<Lance> predictions, List<Lance> actual)
        {
            var correct = 0;
            var total = Math.Min(predictions.Count, actual.Count);

            for (int i = 0; i < total; i++)
            {
                var intersection = predictions[i].Lista.Intersect(actual[i].Lista).Count();
                if (intersection >= 11) // Critério mínimo de acerto
                {
                    correct++;
                }
            }

            return new ValidationResult
            {
                Accuracy = total > 0 ? (double)correct / total : 0,
                TotalTests = total,
                CorrectPredictions = new List<int> { correct },
                ValidationTime = TimeSpan.Zero
            };
        }
        #endregion
    }
}