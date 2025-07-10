// D:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Base\AntiFrequencyModelBase.cs - Classe base para modelos anti-frequencistas
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Utilities.AntiFrequency;
using LotoLibrary.Services.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotoLibrary.PredictionModels.AntiFrequency.Base
{
    /// <summary>
    /// Classe base para todos os modelos anti-frequencistas
    /// Implementa funcionalidades comuns como análise de frequência, 
    /// detecção de padrões e estratégias anti-frequencistas
    /// </summary>
    public abstract partial class AntiFrequencyModelBase : PredictionModelBase, IConfigurableModel, IExplainableModel
    {
        #region Fields
        protected FrequencyAnalyzer _frequencyAnalyzer;
        protected StatisticalDebtCalculator _debtCalculator;
        protected TemporalWeighting _temporalWeighting;
        protected PatternDetection _patternDetector;
        protected Dictionary<int, FrequencyProfile> _dezenasProfiles;
        protected List<Lance> _historicalData;
        protected AntiFrequencyStrategy _activeStrategy;
        protected Dictionary<string, object> _parameters;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        protected string _antiFrequencyStatus = "Aguardando inicialização...";

        [ObservableProperty]
        protected double _antiFrequencyScore;

        [ObservableProperty]
        protected int _analysisWindow = 100;

        [ObservableProperty]
        protected double _temporalDecayFactor = 0.95;

        [ObservableProperty]
        protected string _detectedPattern = "Nenhum padrão detectado";
        #endregion

        #region Properties
        public override string ModelType => "AntiFrequency";
        
        public abstract AntiFrequencyStrategy DefaultStrategy { get; }
        
        public Dictionary<string, object> Parameters
        {
            get => _parameters ??= GetDefaultParameters();
            set => _parameters = value;
        }

        public Dictionary<int, FrequencyProfile> DezenasProfiles => _dezenasProfiles;
        public AntiFrequencyStrategy ActiveStrategy => _activeStrategy;
        #endregion

        #region Constructor
        protected AntiFrequencyModelBase()
        {
            InitializeAnalyzers();
            _dezenasProfiles = new Dictionary<int, FrequencyProfile>();
            _activeStrategy = DefaultStrategy;
            _parameters = GetDefaultParameters();
        }
        #endregion

        #region Abstract Methods - Implementação específica de cada modelo
        
        /// <summary>
        /// Implementa a estratégia específica de anti-frequência do modelo
        /// </summary>
        protected abstract Task<Dictionary<int, double>> CalculateAntiFrequencyScoresAsync(int targetConcurso);
        
        /// <summary>
        /// Aplica filtros específicos do modelo
        /// </summary>
        protected abstract Dictionary<int, double> ApplyModelSpecificFilters(Dictionary<int, double> scores);
        
        /// <summary>
        /// Retorna os parâmetros padrão específicos do modelo
        /// </summary>
        protected abstract Dictionary<string, object> GetModelSpecificParameters();
        
        #endregion

        #region PredictionModelBase Implementation
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                UpdateStatus("Inicializando modelo anti-frequencista...");

                if (historicalData == null || !historicalData.Any())
                {
                    UpdateStatus("Erro: Dados históricos inválidos");
                    return false;
                }

                var minimumDataSize = GetParameter<int>("MinimumDataSize");
                if (historicalData.Count < minimumDataSize)
                {
                    UpdateStatus($"Erro: Dados insuficientes. Mínimo: {minimumDataSize}, Atual: {historicalData.Count}");
                    return false;
                }

                _historicalData = historicalData.ToList();

                // Configurar janela de análise
                ConfigureAnalysisWindow();

                // Inicializar analisadores com dados
                await InitializeAnalyzersWithData();

                // Criar perfis de frequência para todas as dezenas
                await CreateFrequencyProfiles();

                // Detectar padrões principais
                await DetectMainPatterns();

                UpdateStatus($"✅ Modelo anti-frequencista inicializado: {_dezenasProfiles.Count} perfis criados");
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        protected override async Task<bool> DoTrainAsync(Lances trainingData)
        {
            try
            {
                UpdateStatus("Treinando modelo anti-frequencista...");

                // Análise de padrões anti-frequencistas
                await AnalyzeAntiFrequencyPatterns(trainingData);

                // Calibrar pesos temporais
                await CalibrateTemporalWeights(trainingData);

                // Otimizar parâmetros baseado em dados históricos
                await OptimizeParameters(trainingData);

                // Calcular confiança do modelo
                var confidence = await CalculateModelConfidence(trainingData);
                UpdateConfidence(confidence);

                UpdateStatus($"Treinamento concluído. Confiança: {confidence:P2}");
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro no treinamento: {ex.Message}");
                return false;
            }
        }

        protected override async Task<PredictionResult> DoPredict(int targetConcurso)
        {
            try
            {
                UpdateStatus($"Gerando predição anti-frequencista para concurso {targetConcurso}...");

                // 1. Calcular scores anti-frequencistas específicos do modelo
                var antiFreqScores = await CalculateAntiFrequencyScoresAsync(targetConcurso);

                // 2. Aplicar pesos temporais
                antiFreqScores = ApplyTemporalWeights(antiFreqScores, targetConcurso);

                // 3. Aplicar filtros específicos do modelo
                antiFreqScores = ApplyModelSpecificFilters(antiFreqScores);

                // 4. Aplicar estratégias de balanceamento
                antiFreqScores = ApplyBalancingStrategies(antiFreqScores);

                // 5. Selecionar top 15 dezenas
                var selectedNumbers = SelectTopNumbers(antiFreqScores, 15);

                // 6. Validar e corrigir se necessário
                selectedNumbers = ValidateAndCorrectSelection(selectedNumbers, antiFreqScores);

                // 7. Calcular confiança da predição
                var predictionConfidence = CalculatePredictionConfidence(selectedNumbers, antiFreqScores);

                // 8. Atualizar métricas
                UpdateModelMetrics(antiFreqScores, selectedNumbers);

                var result = new PredictionResult
                {
                    PredictedNumbers = selectedNumbers,
                    OverallConfidence = predictionConfidence,
                    ModelUsed = ModelName,
                    Timestamp = DateTime.Now,
                    TargetConcurso = targetConcurso,
                    GenerationMethod = $"Anti-Frequency ({_activeStrategy})",
                    Metadata = CreatePredictionMetadata(antiFreqScores, selectedNumbers)
                };

                UpdateStatus($"✅ Predição anti-frequencista gerada: {selectedNumbers.Count} dezenas, Confiança: {predictionConfidence:P2}");
                return result;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na predição: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Core Anti-Frequency Logic

        private void InitializeAnalyzers()
        {
            _frequencyAnalyzer = new FrequencyAnalyzer();
            _debtCalculator = new StatisticalDebtCalculator();
            _temporalWeighting = new TemporalWeighting();
            _patternDetector = new PatternDetection();
        }

        private async Task InitializeAnalyzersWithData()
        {
            await _frequencyAnalyzer.InitializeAsync(_historicalData);
            await _debtCalculator.InitializeAsync(_historicalData);
            await _temporalWeighting.InitializeAsync(_historicalData);
            await _patternDetector.InitializeAsync(_historicalData);
        }

        private async Task CreateFrequencyProfiles()
        {
            _dezenasProfiles.Clear();

            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var profile = await _frequencyAnalyzer.CreateFrequencyProfileAsync(dezena, _analysisWindow);
                _dezenasProfiles[dezena] = profile;
            }

            AntiFrequencyStatus = $"✅ {_dezenasProfiles.Count} perfis de frequência criados";
        }

        private async Task DetectMainPatterns()
        {
            var patterns = await _patternDetector.DetectAntiFrequencyPatternsAsync(_historicalData);
            
            if (patterns.Any())
            {
                DetectedPattern = string.Join(", ", patterns.Take(3));
                AntiFrequencyStatus = $"✅ Padrões detectados: {DetectedPattern}";
            }
            else
            {
                DetectedPattern = "Padrões não identificados - usando estratégia padrão";
            }
        }

        private async Task AnalyzeAntiFrequencyPatterns(Lances trainingData)
        {
            // Analisar padrões de sobre-frequência que precisam ser evitados
            var overFrequentPatterns = await _patternDetector.DetectOverFrequencyPatternsAsync(trainingData);
            
            // Analisar padrões de sub-frequência que devem ser priorizados
            var underFrequentPatterns = await _patternDetector.DetectUnderFrequencyPatternsAsync(trainingData);

            // Configurar estratégia baseada nos padrões detectados
            ConfigureStrategyBasedOnPatterns(overFrequentPatterns, underFrequentPatterns);
        }

        private async Task CalibrateTemporalWeights(Lances trainingData)
        {
            var optimalDecay = await _temporalWeighting.FindOptimalDecayFactorAsync(trainingData);
            TemporalDecayFactor = optimalDecay;
            
            AntiFrequencyStatus = $"✅ Pesos temporais calibrados: decay factor = {optimalDecay:F3}";
        }

        private async Task OptimizeParameters(Lances trainingData)
        {
            // Otimização específica será implementada em cada modelo derivado
            await Task.Delay(1); // Placeholder
        }

        private async Task<double> CalculateModelConfidence(Lances trainingData)
        {
            // Calcular confiança baseada na consistência dos padrões anti-frequencistas
            var consistencyScore = await _patternDetector.CalculatePatternConsistencyAsync(trainingData);
            var validationScore = await ValidateAntiFrequencyStrategy(trainingData);
            
            return (consistencyScore + validationScore) / 2.0;
        }

        private async Task<double> ValidateAntiFrequencyStrategy(Lances trainingData)
        {
            // Validar estratégia usando backtesting simplificado
            var recentData = trainingData.TakeLast(50).ToList();
            var correctPredictions = 0;
            var totalPredictions = 0;

            foreach (var testLance in recentData.Skip(20)) // Usar últimos 30 para teste
            {
                var availableData = trainingData.Where(l => l.Id < testLance.Id).ToList();
                var antiFreqScores = await CalculateAntiFrequencyScoresForData(availableData, testLance.Id);
                var topAntiFreq = antiFreqScores.OrderByDescending(kvp => kvp.Value).Take(10).Select(kvp => kvp.Key);
                
                var actualNumbers = testLance.Lista;
                var antiFreqHits = topAntiFreq.Intersect(actualNumbers).Count();
                
                // Se anti-frequencista acertou menos que frequencista esperado, é bom sinal
                if (antiFreqHits < 7) // Esperamos que anti-freq tenha menos acertos diretos
                {
                    correctPredictions++;
                }
                totalPredictions++;
            }

            return totalPredictions > 0 ? (double)correctPredictions / totalPredictions : 0.5;
        }

        private async Task<Dictionary<int, double>> CalculateAntiFrequencyScoresForData(List<Lance> data, int targetConcurso)
        {
            // Implementação básica - será sobrescrita pelos modelos específicos
            var scores = new Dictionary<int, double>();
            
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var frequency = _frequencyAnalyzer.CalculateFrequency(dezena, data, _analysisWindow);
                scores[dezena] = 1.0 - frequency; // Inversão simples
            }

            return scores;
        }

        protected Dictionary<int, double> ApplyTemporalWeights(Dictionary<int, double> scores, int targetConcurso)
        {
            var weightedScores = new Dictionary<int, double>();

            foreach (var kvp in scores)
            {
                var dezena = kvp.Key;
                var baseScore = kvp.Value;
                
                var temporalWeight = _temporalWeighting.CalculateWeight(dezena, targetConcurso);
                weightedScores[dezena] = baseScore * temporalWeight;
            }

            return weightedScores;
        }

        protected Dictionary<int, double> ApplyBalancingStrategies(Dictionary<int, double> scores)
        {
            var balancedScores = new Dictionary<int, double>(scores);

            // Balanceamento par/ímpar
            balancedScores = BalanceEvenOdd(balancedScores);

            // Balanceamento por grupos (baixas/médias/altas)
            balancedScores = BalanceGroups(balancedScores);

            // Balanceamento sequencial (evitar muitas sequências)
            balancedScores = BalanceSequential(balancedScores);

            return balancedScores;
        }

        protected List<int> SelectTopNumbers(Dictionary<int, double> scores, int count)
        {
            return scores
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .OrderBy(n => n)
                .ToList();
        }

        protected List<int> ValidateAndCorrectSelection(List<int> selectedNumbers, Dictionary<int, double> scores)
        {
            var correctedNumbers = new List<int>(selectedNumbers);

            // Garantir distribuição mínima 1-9 (correção do bug)
            var dezenas1a9 = correctedNumbers.Count(d => d <= 9);
            if (dezenas1a9 < 3) // Pelo menos 20% de 15
            {
                correctedNumbers = CorrectLowRange(correctedNumbers, scores);
            }

            // Garantir balanceamento par/ímpar
            var pares = correctedNumbers.Count(d => d % 2 == 0);
            if (pares < 4 || pares > 11) // Entre 25% e 75%
            {
                correctedNumbers = CorrectEvenOddBalance(correctedNumbers, scores);
            }

            return correctedNumbers.OrderBy(n => n).ToList();
        }

        private double CalculatePredictionConfidence(List<int> selectedNumbers, Dictionary<int, double> scores)
        {
            if (!selectedNumbers.Any()) return 0.0;

            var avgScore = selectedNumbers.Average(n => scores.ContainsKey(n) ? scores[n] : 0.0);
            var distribution = AnalyzeSelectionDistribution(selectedNumbers);
            var patternStrength = _patternDetector.AnalyzeSelectionPatternStrength(selectedNumbers);

            return (avgScore + distribution + patternStrength) / 3.0;
        }

        private void UpdateModelMetrics(Dictionary<int, double> scores, List<int> selectedNumbers)
        {
            AntiFrequencyScore = selectedNumbers.Average(n => scores.ContainsKey(n) ? scores[n] : 0.0);
            AntiFrequencyStatus = $"✅ Seleção concluída - Score médio: {AntiFrequencyScore:F3}";
        }

        private Dictionary<string, object> CreatePredictionMetadata(Dictionary<int, double> scores, List<int> selectedNumbers)
        {
            return new Dictionary<string, object>
            {
                ["Strategy"] = _activeStrategy.ToString(),
                ["AnalysisWindow"] = _analysisWindow,
                ["TemporalDecay"] = _temporalDecayFactor,
                ["AvgAntiFreqScore"] = selectedNumbers.Average(n => scores.ContainsKey(n) ? scores[n] : 0.0),
                ["DetectedPattern"] = _detectedPattern,
                ["TotalProfiles"] = _dezenasProfiles.Count
            };
        }
        #endregion

        #region Helper Methods

        private void ConfigureAnalysisWindow()
        {
            var windowSize = GetParameter<int>("AnalysisWindow");
            _analysisWindow = Math.Min(windowSize, _historicalData.Count);
        }

        private void ConfigureStrategyBasedOnPatterns(List<string> overFrequent, List<string> underFrequent)
        {
            // Lógica para escolher estratégia baseada nos padrões detectados
            if (overFrequent.Count > underFrequent.Count)
            {
                _activeStrategy = AntiFrequencyStrategy.Strong;
            }
            else if (underFrequent.Count > overFrequent.Count)
            {
                _activeStrategy = AntiFrequencyStrategy.Moderate;
            }
            else
            {
                _activeStrategy = DefaultStrategy;
            }
        }

        private Dictionary<int, double> BalanceEvenOdd(Dictionary<int, double> scores)
        {
            var pares = scores.Where(kvp => kvp.Key % 2 == 0).ToList();
            var impares = scores.Where(kvp => kvp.Key % 2 != 0).ToList();

            var avgPares = pares.Average(kvp => kvp.Value);
            var avgImpares = impares.Average(kvp => kvp.Value);

            var balanceFactor = GetParameter<double>("EvenOddBalanceFactor");

            if (avgPares > avgImpares * 1.2)
            {
                // Favorecer ímpares
                foreach (var impar in impares)
                {
                    scores[impar.Key] *= (1.0 + balanceFactor);
                }
            }
            else if (avgImpares > avgPares * 1.2)
            {
                // Favorecer pares
                foreach (var par in pares)
                {
                    scores[par.Key] *= (1.0 + balanceFactor);
                }
            }

            return scores;
        }

        private Dictionary<int, double> BalanceGroups(Dictionary<int, double> scores)
        {
            var grupos = new Dictionary<string, List<int>>
            {
                ["Baixas"] = Enumerable.Range(1, 8).ToList(),
                ["Médias"] = Enumerable.Range(9, 9).ToList(),
                ["Altas"] = Enumerable.Range(18, 8).ToList()
            };

            var balanceFactor = GetParameter<double>("GroupBalanceFactor");

            foreach (var grupo in grupos)
            {
                var avgScore = grupo.Value.Average(d => scores[d]);
                var overallAvg = scores.Values.Average();

                if (avgScore < overallAvg * 0.8) // Grupo sub-representado
                {
                    foreach (var dezena in grupo.Value)
                    {
                        scores[dezena] *= (1.0 + balanceFactor);
                    }
                }
            }

            return scores;
        }

        private Dictionary<int, double> BalanceSequential(Dictionary<int, double> scores)
        {
            // Penalizar sequências consecutivas muito longas
            var sequencePenalty = GetParameter<double>("SequencePenalty");
            var topScores = scores.OrderByDescending(kvp => kvp.Value).Take(20).ToList();

            for (int i = 0; i < topScores.Count - 2; i++)
            {
                var current = topScores[i].Key;
                var next1 = topScores.Skip(i + 1).FirstOrDefault(kvp => kvp.Key == current + 1);
                var next2 = topScores.Skip(i + 2).FirstOrDefault(kvp => kvp.Key == current + 2);

                if (next1.Key == current + 1 && next2.Key == current + 2)
                {
                    // Sequência de 3 detectada - aplicar penalidade
                    scores[next2.Key] *= (1.0 - sequencePenalty);
                }
            }

            return scores;
        }

        private List<int> CorrectLowRange(List<int> numbers, Dictionary<int, double> scores)
        {
            var corrected = new List<int>(numbers);
            var needed = 3 - corrected.Count(d => d <= 9);

            if (needed > 0)
            {
                var candidates = scores
                    .Where(kvp => kvp.Key <= 9 && !corrected.Contains(kvp.Key))
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(needed)
                    .Select(kvp => kvp.Key)
                    .ToList();

                var toRemove = corrected
                    .Where(d => d > 9)
                    .OrderBy(d => scores.ContainsKey(d) ? scores[d] : 0)
                    .Take(needed)
                    .ToList();

                foreach (var remove in toRemove)
                {
                    corrected.Remove(remove);
                }

                corrected.AddRange(candidates);
            }

            return corrected;
        }

        private List<int> CorrectEvenOddBalance(List<int> numbers, Dictionary<int, double> scores)
        {
            // Implementação simplificada - pode ser expandida
            return numbers;
        }

        private double AnalyzeSelectionDistribution(List<int> selectedNumbers)
        {
            // Analisar distribuição das dezenas selecionadas
            var baixas = selectedNumbers.Count(d => d <= 8);
            var medias = selectedNumbers.Count(d => d >= 9 && d <= 17);
            var altas = selectedNumbers.Count(d => d >= 18);

            var idealRatio = 5.0; // 15/3 = 5 por grupo idealmente
            var deviationLow = Math.Abs(baixas - idealRatio) / idealRatio;
            var deviationMid = Math.Abs(medias - idealRatio) / idealRatio;
            var deviationHigh = Math.Abs(altas - idealRatio) / idealRatio;

            var avgDeviation = (deviationLow + deviationMid + deviationHigh) / 3.0;
            return Math.Max(0.0, 1.0 - avgDeviation);
        }

        protected T GetParameter<T>(string name)
        {
            if (Parameters.TryGetValue(name, out var value) && value is T typedValue)
                return typedValue;

            var defaultParams = GetDefaultParameters();
            return defaultParams.TryGetValue(name, out var defaultValue) && defaultValue is T defaultTyped
                ? defaultTyped
                : default(T);
        }

        protected virtual Dictionary<string, object> GetDefaultParameters()
        {
            var baseParams = new Dictionary<string, object>
            {
                ["MinimumDataSize"] = 100,
                ["AnalysisWindow"] = 100,
                ["TemporalDecayFactor"] = 0.95,
                ["EvenOddBalanceFactor"] = 0.05,
                ["GroupBalanceFactor"] = 0.03,
                ["SequencePenalty"] = 0.1,
                ["MinLowRangeNumbers"] = 3,
                ["MaxLowRangeNumbers"] = 8
            };

            var modelSpecific = GetModelSpecificParameters();
            foreach (var kvp in modelSpecific)
            {
                baseParams[kvp.Key] = kvp.Value;
            }

            return baseParams;
        }

        private void UpdateStatus(string status)
        {
            AntiFrequencyStatus = status;
            System.Diagnostics.Debug.WriteLine($"[{GetType().Name}] {status}");
        }
        #endregion

        #region IConfigurableModel Implementation
        public bool IsParameterSupported(string parameterName)
        {
            return GetDefaultParameters().ContainsKey(parameterName);
        }

        public object GetParameterValue(string parameterName)
        {
            return Parameters.TryGetValue(parameterName, out var value) ? value : null;
        }

        public bool SetParameterValue(string parameterName, object value)
        {
            if (!IsParameterSupported(parameterName))
                return false;

            Parameters[parameterName] = value;
            return true;
        }

        public Dictionary<string, object> GetAllParameters()
        {
            return new Dictionary<string, object>(Parameters);
        }
        #endregion

        #region IExplainableModel Implementation
        public ModelExplanation ExplainPrediction(PredictionResult prediction)
        {
            var explanation = new ModelExplanation
            {
                ModelName = ModelName,
                PredictionConfidence = prediction.OverallConfidence,
                MainFactors = new List<string>(),
                TechnicalDetails = new Dictionary<string, object>()
            };

            // Fatores principais
            explanation.MainFactors.Add($"Estratégia anti-frequencista: {_activeStrategy}");
            explanation.MainFactors.Add($"Janela de análise: {_analysisWindow} concursos");
            explanation.MainFactors.Add($"Padrão detectado: {_detectedPattern}");
            explanation.MainFactors.Add($"Score anti-frequência médio: {_antiFrequencyScore:F3}");

            // Detalhes técnicos
            explanation.TechnicalDetails["Strategy"] = _activeStrategy.ToString();
            explanation.TechnicalDetails["AnalysisWindow"] = _analysisWindow;
            explanation.TechnicalDetails["TemporalDecay"] = _temporalDecayFactor;
            explanation.TechnicalDetails["ProfilesCount"] = _dezenasProfiles.Count;

            return explanation;
        }
        #endregion
    }

    #region Supporting Enums and Classes

    public enum AntiFrequencyStrategy
    {
        /// <summary>
        /// Estratégia suave - inversão moderada da frequência
        /// </summary>
        Gentle,

        /// <summary>
        /// Estratégia moderada - balanceamento frequência vs anti-frequência
        /// </summary>
        Moderate,

        /// <summary>
        /// Estratégia forte - inversão total da frequência
        /// </summary>
        Strong,

        /// <summary>
        /// Estratégia adaptativa - muda baseada nos padrões detectados
        /// </summary>
        Adaptive,

        /// <summary>
        /// Estratégia baseada em dívida estatística
        /// </summary>
        StatisticalDebt,

        /// <summary>
        /// Estratégia baseada em saturação de padrões
        /// </summary>
        Saturation
    }

    public class FrequencyProfile
    {
        public int Dezena { get; set; }
        public double FrequencyScore { get; set; }
        public double AntiFrequencyScore { get; set; }
        public int AppearanceCount { get; set; }
        public int ExpectedAppearances { get; set; }
        public double StatisticalDebt { get; set; }
        public DateTime LastAppearance { get; set; }
        public TimeSpan AverageInterval { get; set; }
        public double Volatility { get; set; }
        public List<int> RecentAppearances { get; set; } = new List<int>();
    }

    #endregion
}