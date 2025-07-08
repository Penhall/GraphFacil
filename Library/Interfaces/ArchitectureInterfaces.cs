// ============================================================================
// INTERFACES PRINCIPAIS - LOTOFÁCIL PREDICTION SYSTEM
// ============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Models;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface base para todos os modelos de predição
    /// </summary>
    public interface IPredictionModel
    {
        string ModelName { get; }
        string ModelType { get; }
        bool IsInitialized { get; }
        double Confidence { get; }
        
        Task<bool> InitializeAsync(Lances historicalData);
        Task<bool> TrainAsync(Lances trainingData);
        Task<PredictionResult> PredictAsync(int targetConcurso);
        Task<ValidationResult> ValidateAsync(Lances validationData);
        void Reset();
    }

    /// <summary>
    /// Interface para modelos que suportam parâmetros configuráveis
    /// </summary>
    public interface IConfigurableModel : IPredictionModel
    {
        Dictionary<string, object> Parameters { get; set; }
        void UpdateParameters(Dictionary<string, object> newParameters);
        Dictionary<string, object> GetOptimalParameters(Lances validationData);
    }

    /// <summary>
    /// Interface para modelos que explicam suas predições
    /// </summary>
    public interface IExplainableModel : IPredictionModel
    {
        ModelExplanation ExplainPrediction(PredictionResult prediction);
        List<FeatureImportance> GetFeatureImportances();
    }

    /// <summary>
    /// Interface para ensemble de modelos
    /// </summary>
    public interface IEnsembleModel : IPredictionModel
    {
        List<IPredictionModel> SubModels { get; }
        Dictionary<string, double> ModelWeights { get; set; }
        
        void AddModel(IPredictionModel model, double weight = 1.0);
        void RemoveModel(string modelName);
        void UpdateWeights(Dictionary<string, double> weights);
        EnsembleMetrics GetEnsembleMetrics();
    }

    /// <summary>
    /// Interface para meta-modelo que decide estratégias dinamicamente
    /// </summary>
    public interface IMetaModel : IPredictionModel
    {
        void RegisterModel(IPredictionModel model);
        Task<MetaDecision> DecideStrategyAsync(MarketConditions conditions);
        void UpdatePerformanceHistory(string modelName, double performance);
    }

    /// <summary>
    /// Interface para factory de modelos
    /// </summary>
    public interface IModelFactory
    {
        IPredictionModel CreateModel(ModelType type, Dictionary<string, object> parameters = null);
        IEnsembleModel CreateEnsemble(List<ModelType> modelTypes);
        IMetaModel CreateMetaModel();
    }

    /// <summary>
    /// Interface para análise de performance
    /// </summary>
    public interface IPerformanceAnalyzer
    {
        Task<PerformanceReport> AnalyzeAsync(IPredictionModel model, Lances testData);
        Task<ComparisonReport> CompareModelsAsync(List<IPredictionModel> models, Lances testData);
        Task<OptimizationResult> OptimizeParametersAsync(IConfigurableModel model, Lances data);
    }
}

// ============================================================================
// MODELOS DE DADOS
// ============================================================================

namespace LotoLibrary.Models.Prediction
{
    public class PredictionResult
    {
        public List<int> PredictedNumbers { get; set; }
        public Dictionary<int, double> NumberProbabilities { get; set; }
        public double OverallConfidence { get; set; }
        public string ModelUsed { get; set; }
        public DateTime PredictionTime { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class ValidationResult
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public List<int> CorrectPredictions { get; set; }
        public TimeSpan ValidationTime { get; set; }
    }

    public class ModelExplanation
    {
        public string Reasoning { get; set; }
        public List<DecisionFactor> Factors { get; set; }
        public Dictionary<int, string> NumberReasons { get; set; }
    }

    public class FeatureImportance
    {
        public string FeatureName { get; set; }
        public double Importance { get; set; }
        public string Description { get; set; }
    }

    public class EnsembleMetrics
    {
        public Dictionary<string, double> ModelContributions { get; set; }
        public double DiversityScore { get; set; }
        public double WeightOptimality { get; set; }
    }

    public class MetaDecision
    {
        public string SelectedStrategy { get; set; }
        public Dictionary<string, double> StrategyWeights { get; set; }
        public string Reasoning { get; set; }
        public double ConfidenceLevel { get; set; }
    }

    public class MarketConditions
    {
        public int RecentConcursos { get; set; }
        public double Volatility { get; set; }
        public Dictionary<string, double> ModelPerformances { get; set; }
        public List<string> DetectedPatterns { get; set; }
    }

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
        MetaLearning
    }
}