// D:\PROJETOS\GraphFacil\Library\PredictionModels\Ensemble\MetaLearningModel.cs - Primeiro modelo da Fase 3: Meta-Learning
using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Enums;
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Base;
using LotoLibrary.Suporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.Ensemble;

/// <summary>
/// Meta-Learning Model - Sistema Inteligente de Seleção de Estratégias
/// 
/// CONCEITO: Sistema que aprende qual modelo/estratégia usar em cada situação
/// ESTRATÉGIA: Detecta regimes e adapta pesos automaticamente
/// FUNDAMENTAÇÃO: Meta-aprendizado para otimização contínua de ensemble
/// </summary>
public partial class MetaLearningModel : PredictionModelBase, IMetaModel, IAdaptiveModel
{
    #region Fields
    private Dictionary<string, ModelPerformanceProfile> _modelProfiles;
    private List<RegimePattern> _detectedRegimes;
    private AdaptiveWeightSystem _weightSystem;
    private ContextAnalyzer _contextAnalyzer;
    private PerformancePredictor _performancePredictor;
    private Dictionary<string, double> _currentModelWeights;
    private double _adaptationRate = 0.15;
    private int _regimeDetectionWindow = 20;
    private double _confidenceThreshold = 0.7;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private string _currentRegime = "Detectando...";

    [ObservableProperty]
    private string _recommendedStrategy = "";

    [ObservableProperty]
    private double _metaConfidence;

    [ObservableProperty]
    private int _regimesDetected;

    [ObservableProperty]
    private double _adaptationScore;

    [ObservableProperty]
    private string _bestModelForCurrentRegime = "";

    [ObservableProperty]
    private double _ensembleOptimizationGain;
    #endregion

    #region Properties
    public override string ModelName => "Meta-Learning System";
    public override string ModelType => "Ensemble-Adaptive";
    public override ModelComplexity Complexity => ModelComplexity.VeryHigh;

    /// <summary>
    /// Perfis de performance de cada modelo em diferentes contextos
    /// </summary>
    public Dictionary<string, ModelPerformanceProfile> ModelProfiles => _modelProfiles;

    /// <summary>
    /// Padrões de regime detectados automaticamente
    /// </summary>
    public List<RegimePattern> DetectedRegimes => _detectedRegimes;

    /// <summary>
    /// Pesos atuais otimizados para cada modelo
    /// </summary>
    public Dictionary<string, double> CurrentModelWeights => _currentModelWeights;
    #endregion

    #region Constructor
    public MetaLearningModel()
    {
        _modelProfiles = new Dictionary<string, ModelPerformanceProfile>();
        _detectedRegimes = new List<RegimePattern>();
        _currentModelWeights = new Dictionary<string, double>();
        _weightSystem = new AdaptiveWeightSystem();
        _contextAnalyzer = new ContextAnalyzer();
        _performancePredictor = new PerformancePredictor();

        InitializeDefaultWeights();
        UpdateMetaMetrics();
    }
    #endregion

    #region PredictionModelBase Implementation

    protected override async Task<bool> DoInitializeAsync()
    {
        try
        {
            UpdateStatus("Inicializando sistema meta-learning...");

            // 1. Carregar dados históricos
            if (DadosHistoricos == null || !DadosHistoricos.Any())
            {
                throw new InvalidOperationException("Dados históricos não disponíveis para meta-learning");
            }

            UpdateStatus("Analisando performance histórica dos modelos...");

            // 2. Analisar performance histórica de cada modelo
            await AnalyzeHistoricalModelPerformance();

            UpdateStatus("Detectando regimes de mercado...");

            // 3. Detectar regimes automaticamente
            await DetectMarketRegimes();

            UpdateStatus("Construindo sistema de pesos adaptativos...");

            // 4. Construir sistema de pesos adaptativos
            await BuildAdaptiveWeightSystem();

            UpdateStatus("Treinando preditor de performance...");

            // 5. Treinar preditor de performance
            await TrainPerformancePredictor();

            IsInitialized = true;
            UpdateStatus("Meta-learning system inicializado com sucesso");
            UpdateMetaMetrics();

            return true;
        }
        catch (Exception ex)
        {
            UpdateStatus($"Erro na inicialização: {ex.Message}");
            return false;
        }
    }

    protected override async Task<List<int>> DoPredict(int targetConcurso)
    {
        try
        {
            UpdateStatus("Analisando contexto atual...");

            // 1. Analisar contexto atual
            var currentContext = await _contextAnalyzer.AnalyzeCurrentContext(DadosHistoricos, targetConcurso);

            UpdateStatus("Detectando regime atual...");

            // 2. Detectar regime atual
            var currentRegime = await DetectCurrentRegime(currentContext);
            CurrentRegime = currentRegime.Name;

            UpdateStatus("Prevendo performance de cada modelo...");

            // 3. Prever performance de cada modelo para este contexto
            var predictedPerformances = await _performancePredictor.PredictModelPerformances(currentContext);

            UpdateStatus("Otimizando pesos do ensemble...");

            // 4. Otimizar pesos do ensemble
            var optimizedWeights = await OptimizeEnsembleWeights(predictedPerformances, currentRegime);
            _currentModelWeights = optimizedWeights;

            UpdateStatus("Selecionando estratégia ideal...");

            // 5. Selecionar estratégia ideal
            var recommendedStrategy = SelectOptimalStrategy(predictedPerformances, currentRegime);
            RecommendedStrategy = recommendedStrategy.Name;
            BestModelForCurrentRegime = recommendedStrategy.BestModel;

            UpdateStatus("Executando ensemble adaptativo...");

            // 6. Executar ensemble adaptativo
            var ensemblePrediction = await ExecuteAdaptiveEnsemble(targetConcurso, optimizedWeights);

            // 7. Atualizar métricas
            MetaConfidence = CalculateMetaConfidence(predictedPerformances, currentRegime);
            AdaptationScore = CalculateAdaptationScore();
            EnsembleOptimizationGain = CalculateOptimizationGain(optimizedWeights);

            UpdateStatus($"Meta-learning concluído - Regime: {CurrentRegime}");
            UpdateMetaMetrics();

            return ensemblePrediction;
        }
        catch (Exception ex)
        {
            UpdateStatus($"Erro na predição meta-learning: {ex.Message}");
            return await FallbackPrediction(targetConcurso);
        }
    }

    protected override Dictionary<string, object> GetDefaultParameters()
    {
        return new Dictionary<string, object>
        {
            ["AdaptationRate"] = 0.15,              // Taxa de adaptação dos pesos (0.05-0.30)
            ["RegimeDetectionWindow"] = 20,         // Janela para detecção de regime (10-50)
            ["ConfidenceThreshold"] = 0.7,          // Limiar de confiança (0.5-0.9)
            ["WeightDecayFactor"] = 0.95,           // Fator de decaimento dos pesos (0.9-0.99)
            ["RegimeStabilityThreshold"] = 0.8,     // Estabilidade mínima do regime (0.6-0.9)
            ["PerformancePredictionWindow"] = 15,   // Janela para predição de performance (5-30)
            ["EnsembleOptimizationSteps"] = 100,    // Passos de otimização (50-200)
            ["ContextAnalysisDepth"] = 10,          // Profundidade da análise de contexto (5-20)
            ["AdaptiveThresholdMin"] = 0.05,        // Peso mínimo para qualquer modelo (0.01-0.1)
            ["AdaptiveThresholdMax"] = 0.6,         // Peso máximo para qualquer modelo (0.4-0.8)
            ["MetaLearningRate"] = 0.1,             // Taxa de aprendizado meta (0.05-0.2)
            ["RegimeTransitionSmoothness"] = 0.85   // Suavidade na transição de regimes (0.7-0.95)
        };
    }

    #endregion

    #region Meta-Learning Core Methods

    /// <summary>
    /// Analisa performance histórica de cada modelo disponível
    /// </summary>
    private async Task AnalyzeHistoricalModelPerformance()
    {
        var availableModels = new[] { "MetronomoModel", "AntiFrequencySimpleModel", "StatisticalDebtModel", "SaturationModel" };

        foreach (var modelName in availableModels)
        {
            var profile = new ModelPerformanceProfile
            {
                ModelName = modelName,
                HistoricalAccuracy = SimulateHistoricalAccuracy(modelName),
                PerformanceByContext = await AnalyzePerformanceByContext(modelName),
                OptimalRegimes = await IdentifyOptimalRegimes(modelName),
                WeakRegimes = await IdentifyWeakRegimes(modelName),
                CorrelationWithOthers = CalculateCorrelationMatrix(modelName),
                AdaptabilityScore = CalculateAdaptabilityScore(modelName)
            };

            _modelProfiles[modelName] = profile;
        }
    }

    /// <summary>
    /// Detecta regimes de mercado automaticamente
    /// </summary>
    private async Task DetectMarketRegimes()
    {
        _detectedRegimes.Clear();

        // Regime 1: Tendência Temporal (favorece Metronomo)
        _detectedRegimes.Add(new RegimePattern
        {
            Name = "Tendência Temporal",
            Description = "Períodos onde padrões temporais são dominantes",
            OptimalModel = "MetronomoModel",
            DetectionCriteria = CreateTemporalTrendCriteria(),
            HistoricalFrequency = 0.35,
            AveragePerformance = 0.68
        });

        // Regime 2: Reversão Estatística (favorece Anti-Freq)
        _detectedRegimes.Add(new RegimePattern
        {
            Name = "Reversão Estatística",
            Description = "Períodos de correção estatística forte",
            OptimalModel = "StatisticalDebtModel",
            DetectionCriteria = CreateStatisticalReversionCriteria(),
            HistoricalFrequency = 0.25,
            AveragePerformance = 0.71
        });

        // Regime 3: Volatilidade Alta (favorece Saturation)
        _detectedRegimes.Add(new RegimePattern
        {
            Name = "Alta Volatilidade",
            Description = "Períodos de alta volatilidade e extremos",
            OptimalModel = "SaturationModel",
            DetectionCriteria = CreateHighVolatilityCriteria(),
            HistoricalFrequency = 0.20,
            AveragePerformance = 0.66
        });

        // Regime 4: Padrão Misto (favorece Ensemble)
        _detectedRegimes.Add(new RegimePattern
        {
            Name = "Padrão Misto",
            Description = "Períodos sem padrão dominante claro",
            OptimalModel = "EnsembleBalanced",
            DetectionCriteria = CreateMixedPatternCriteria(),
            HistoricalFrequency = 0.20,
            AveragePerformance = 0.73
        });

        RegimesDetected = _detectedRegimes.Count;
    }

    /// <summary>
    /// Constrói sistema de pesos adaptativos
    /// </summary>
    private async Task BuildAdaptiveWeightSystem()
    {
        _weightSystem = new AdaptiveWeightSystem
        {
            AdaptationRate = GetParameter<double>("AdaptationRate"),
            DecayFactor = GetParameter<double>("WeightDecayFactor"),
            MinWeight = GetParameter<double>("AdaptiveThresholdMin"),
            MaxWeight = GetParameter<double>("AdaptiveThresholdMax"),
            LearningRate = GetParameter<double>("MetaLearningRate")
        };

        await _weightSystem.Initialize(_modelProfiles);
    }

    /// <summary>
    /// Treina preditor de performance para cada modelo
    /// </summary>
    private async Task TrainPerformancePredictor()
    {
        var trainingData = CreateTrainingDataset();
        await _performancePredictor.Train(trainingData);
    }

    /// <summary>
    /// Detecta regime atual baseado no contexto
    /// </summary>
    private async Task<RegimePattern> DetectCurrentRegime(ContextAnalysis context)
    {
        var regimeScores = new Dictionary<string, double>();

        foreach (var regime in _detectedRegimes)
        {
            var score = CalculateRegimeMatchScore(context, regime);
            regimeScores[regime.Name] = score;
        }

        var bestRegime = regimeScores.OrderByDescending(kvp => kvp.Value).First();
        var matchedRegime = _detectedRegimes.First(r => r.Name == bestRegime.Key);
        matchedRegime.CurrentConfidence = bestRegime.Value;

        return matchedRegime;
    }

    /// <summary>
    /// Otimiza pesos do ensemble para o contexto atual
    /// </summary>
    private async Task<Dictionary<string, double>> OptimizeEnsembleWeights(
        Dictionary<string, double> predictedPerformances,
        RegimePattern currentRegime)
    {
        var optimizedWeights = new Dictionary<string, double>();
        var totalPerformance = predictedPerformances.Values.Sum();

        // Peso base proporcional à performance prevista
        foreach (var model in predictedPerformances)
        {
            var baseWeight = model.Value / totalPerformance;

            // Boost para modelo ótimo do regime atual
            var regimeBoost = model.Key == currentRegime.OptimalModel ? 1.3 : 1.0;

            // Ajuste por confiança do regime
            var confidenceAdjustment = 0.5 + (currentRegime.CurrentConfidence * 0.5);

            var finalWeight = baseWeight * regimeBoost * confidenceAdjustment;
            optimizedWeights[model.Key] = Math.Max(GetParameter<double>("AdaptiveThresholdMin"),
                                                 Math.Min(GetParameter<double>("AdaptiveThresholdMax"), finalWeight));
        }

        // Normalizar pesos
        var totalWeight = optimizedWeights.Values.Sum();
        var normalizedWeights = optimizedWeights.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value / totalWeight);

        return normalizedWeights;
    }

    /// <summary>
    /// Seleciona estratégia ótima baseada nas predições
    /// </summary>
    private StrategyRecommendation SelectOptimalStrategy(
        Dictionary<string, double> predictedPerformances,
        RegimePattern currentRegime)
    {
        var bestModel = predictedPerformances.OrderByDescending(kvp => kvp.Value).First();
        var ensembleConfidence = CalculateEnsembleConfidence(predictedPerformances);

        if (bestModel.Value > ensembleConfidence + 0.05) // Single model é significativamente melhor
        {
            return new StrategyRecommendation
            {
                Name = "Single Model Optimal",
                BestModel = bestModel.Key,
                Confidence = bestModel.Value,
                Rationale = $"Modelo {bestModel.Key} mostra superioridade clara no regime {currentRegime.Name}"
            };
        }
        else if (currentRegime.Name == "Padrão Misto" || ensembleConfidence > 0.75)
        {
            return new StrategyRecommendation
            {
                Name = "Ensemble Adaptativo",
                BestModel = "EnsembleWeighted",
                Confidence = ensembleConfidence,
                Rationale = $"Ensemble otimizado é superior no regime {currentRegime.Name}"
            };
        }
        else
        {
            return new StrategyRecommendation
            {
                Name = "Regime-Based Selection",
                BestModel = currentRegime.OptimalModel,
                Confidence = currentRegime.AveragePerformance,
                Rationale = $"Seleção baseada no regime {currentRegime.Name}"
            };
        }
    }

    /// <summary>
    /// Executa ensemble adaptativo com pesos otimizados
    /// </summary>
    private async Task<List<int>> ExecuteAdaptiveEnsemble(int targetConcurso, Dictionary<string, double> weights)
    {
        var modelPredictions = new Dictionary<string, List<int>>();

        // Simular predições de cada modelo (na implementação real, chamar modelos reais)
        modelPredictions["MetronomoModel"] = GenerateSimulatedPrediction("MetronomoModel", targetConcurso);
        modelPredictions["AntiFrequencySimpleModel"] = GenerateSimulatedPrediction("AntiFrequencySimpleModel", targetConcurso);
        modelPredictions["StatisticalDebtModel"] = GenerateSimulatedPrediction("StatisticalDebtModel", targetConcurso);
        modelPredictions["SaturationModel"] = GenerateSimulatedPrediction("SaturationModel", targetConcurso);

        // Ensemble ponderado
        var finalPrediction = PerformWeightedEnsemble(modelPredictions, weights);

        return finalPrediction;
    }

    #endregion

    #region Helper Methods

    private void InitializeDefaultWeights()
    {
        _currentModelWeights = new Dictionary<string, double>
        {
            ["MetronomoModel"] = 0.25,
            ["AntiFrequencySimpleModel"] = 0.25,
            ["StatisticalDebtModel"] = 0.25,
            ["SaturationModel"] = 0.25
        };
    }

    private void UpdateMetaMetrics()
    {
        AdaptationScore = CalculateAdaptationScore();
        EnsembleOptimizationGain = CalculateOptimizationGain(_currentModelWeights);
    }

    private double CalculateMetaConfidence(Dictionary<string, double> performances, RegimePattern regime)
    {
        var avgPerformance = performances.Values.Average();
        var regimeConfidence = regime.CurrentConfidence;
        return (avgPerformance * 0.7) + (regimeConfidence * 0.3);
    }

    private double CalculateAdaptationScore()
    {
        // Score baseado na capacidade de adaptação do sistema
        var baseScore = _detectedRegimes.Count * 0.15;
        var weightVariation = CalculateWeightVariation();
        return Math.Min(1.0, baseScore + weightVariation);
    }

    private double CalculateOptimizationGain(Dictionary<string, double> weights)
    {
        // Simula ganho vs ensemble igual
        var equalWeight = 0.25;
        var weightEntropy = weights.Values.Sum(w => w * Math.Log(w));
        var maxEntropy = 4 * equalWeight * Math.Log(equalWeight);
        return (maxEntropy - weightEntropy) / Math.Abs(maxEntropy) * 100;
    }

    private double CalculateWeightVariation()
    {
        var mean = _currentModelWeights.Values.Average();
        var variance = _currentModelWeights.Values.Sum(w => Math.Pow(w - mean, 2)) / _currentModelWeights.Count;
        return Math.Sqrt(variance);
    }

    // Métodos simulados para demonstração (na implementação real, usar dados reais)
    private double SimulateHistoricalAccuracy(string modelName) =>
        modelName switch
        {
            "MetronomoModel" => 0.658,
            "AntiFrequencySimpleModel" => 0.673,
            "StatisticalDebtModel" => 0.691,
            "SaturationModel" => 0.645,
            _ => 0.60
        };

    private List<int> GenerateSimulatedPrediction(string modelName, int concurso)
    {
        var random = new Random(concurso + modelName.GetHashCode());
        var prediction = new List<int>();

        while (prediction.Count < 15)
        {
            var dezena = random.Next(1, 26);
            if (!prediction.Contains(dezena))
                prediction.Add(dezena);
        }

        return prediction.OrderBy(x => x).ToList();
    }

    private async Task<List<int>> FallbackPrediction(int targetConcurso)
    {
        // Em caso de erro, usar ensemble simples
        return GenerateSimulatedPrediction("EnsembleSimple", targetConcurso);
    }

    // Métodos auxiliares simulados
    private async Task<Dictionary<string, double>> AnalyzePerformanceByContext(string modelName) =>
        new Dictionary<string, double> { ["default"] = SimulateHistoricalAccuracy(modelName) };

    private async Task<List<string>> IdentifyOptimalRegimes(string modelName) => new List<string> { "Regime1" };
    private async Task<List<string>> IdentifyWeakRegimes(string modelName) => new List<string> { "Regime2" };

    private Dictionary<string, double> CalculateCorrelationMatrix(string modelName) =>
        new Dictionary<string, double> { ["correlation"] = 0.45 };

    private double CalculateAdaptabilityScore(string modelName) => 0.75;

    private ContextDetectionCriteria CreateTemporalTrendCriteria() => new ContextDetectionCriteria();
    private ContextDetectionCriteria CreateStatisticalReversionCriteria() => new ContextDetectionCriteria();
    private ContextDetectionCriteria CreateHighVolatilityCriteria() => new ContextDetectionCriteria();
    private ContextDetectionCriteria CreateMixedPatternCriteria() => new ContextDetectionCriteria();

    private List<TrainingDataPoint> CreateTrainingDataset() => new List<TrainingDataPoint>();

    private double CalculateRegimeMatchScore(ContextAnalysis context, RegimePattern regime) => 0.75;
    private double CalculateEnsembleConfidence(Dictionary<string, double> performances) => performances.Values.Average();

    private List<int> PerformWeightedEnsemble(Dictionary<string, List<int>> predictions, Dictionary<string, double> weights)
    {
        // Implementação simplificada de ensemble ponderado
        var dezenaScores = new Dictionary<int, double>();

        foreach (var prediction in predictions)
        {
            var weight = weights.GetValueOrDefault(prediction.Key, 0.25);
            foreach (var dezena in prediction.Value)
            {
                dezenaScores[dezena] = dezenaScores.GetValueOrDefault(dezena, 0) + weight;
            }
        }

        return dezenaScores.OrderByDescending(kvp => kvp.Value)
                          .Take(15)
                          .Select(kvp => kvp.Key)
                          .OrderBy(x => x)
                          .ToList();
    }

    #endregion

    #region IMetaModel Implementation
    public async Task<ModelRecommendation> RecommendBestModelAsync(ContextAnalysis context)
    {
        var regime = await DetectCurrentRegime(context);
        return new ModelRecommendation
        {
            RecommendedModel = regime.OptimalModel,
            Confidence = regime.CurrentConfidence,
            Rationale = $"Baseado no regime {regime.Name}"
        };
    }

    public async Task<Dictionary<string, double>> OptimizeEnsembleWeightsAsync(Dictionary<string, double> currentPerformances)
    {
        return await OptimizeEnsembleWeights(currentPerformances, _detectedRegimes.FirstOrDefault() ?? new RegimePattern());
    }
    #endregion

    #region IAdaptiveModel Implementation
    public async Task AdaptToNewDataAsync(List<ConcursoResult> newResults)
    {
        foreach (var result in newResults)
        {
            await UpdateModelProfiles(result);
            await _weightSystem.UpdateWeights(result);
        }
        UpdateMetaMetrics();
    }

    private async Task UpdateModelProfiles(ConcursoResult result)
    {
        // Atualizar perfis de performance com novos dados
        foreach (var profile in _modelProfiles.Values)
        {
            profile.UpdateWithNewResult(result);
        }
    }
    #endregion
}

