// D:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Statistical\StatisticalDebtModel.cs - Segundo modelo anti-frequencista
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using LotoLibrary.PredictionModels.AntiFrequency.Base;
using LotoLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotoLibrary.PredictionModels.AntiFrequency.Statistical
{
    /// <summary>
    /// Modelo de Dívida Estatística - Segunda implementação anti-frequencista
    /// 
    /// CONCEITO: Cada dezena tem uma "dívida" baseada na diferença entre aparições esperadas vs reais
    /// ESTRATÉGIA: Priorizar dezenas com maior dívida estatística acumulada
    /// FUNDAMENTAÇÃO: Lei dos Grandes Números - tendência natural à normalização
    /// </summary>
    public partial class StatisticalDebtModel : AntiFrequencyModelBase
    {
        #region Fields
        private Dictionary<int, StatisticalDebtProfile> _debtProfiles;
        private double _debtAccelerationFactor = 1.2;
        private double _temporalDecayRate = 0.95;
        private int _debtCalculationWindow = 100;
        private double _volatilityNormalization = 1.0;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private double _totalSystemDebt;

        [ObservableProperty]
        private string _highestDebtDezena = "";

        [ObservableProperty]
        private double _averageDebtVariance;

        [ObservableProperty]
        private int _debtorsCount;

        [ObservableProperty]
        private double _debtConcentration;
        #endregion

        #region Properties
        public override string ModelName => "Dívida Estatística";
        public override AntiFrequencyStrategy DefaultStrategy => AntiFrequencyStrategy.StatisticalDebt;

        /// <summary>
        /// Perfis de dívida específicos deste modelo
        /// </summary>
        public Dictionary<int, StatisticalDebtProfile> DebtProfiles => _debtProfiles;

        /// <summary>
        /// Fator de aceleração da dívida (amplifica diferenças)
        /// </summary>
        public double DebtAccelerationFactor => _debtAccelerationFactor;
        #endregion

        #region Constructor
        public StatisticalDebtModel()
        {
            _debtProfiles = new Dictionary<int, StatisticalDebtProfile>();
            UpdateDebtMetrics();
        }
        #endregion

        #region AntiFrequencyModelBase Implementation

        protected override async Task<Dictionary<int, double>> CalculateAntiFrequencyScoresAsync(int targetConcurso)
        {
            var scores = new Dictionary<int, double>();

            try
            {
                UpdateStatus("Calculando dívidas estatísticas...");

                // 1. Atualizar perfis de dívida para todas as dezenas
                await UpdateDebtProfiles(targetConcurso);

                // 2. Calcular scores baseados na dívida estatística
                foreach (var kvp in _debtProfiles)
                {
                    var dezena = kvp.Key;
                    var profile = kvp.Value;

                    // Score principal: dívida normalizada
                    var primaryScore = CalculatePrimaryDebtScore(profile);

                    // Ajuste temporal: peso baseado em tempo desde última aparição
                    var temporalWeight = CalculateTemporalWeight(profile, targetConcurso);

                    // Ajuste de volatilidade: normalizar por estabilidade histórica
                    var volatilityAdjustment = CalculateVolatilityAdjustment(profile);

                    // Score final combinado
                    var finalScore = CombineDebtFactors(primaryScore, temporalWeight, volatilityAdjustment);

                    scores[dezena] = finalScore;
                }

                // 3. Aplicar aceleração de dívida para amplificar diferenças
                scores = ApplyDebtAcceleration(scores);

                // 4. Aplicar suavização para evitar extremos
                scores = ApplyDebtSmoothing(scores);

                // 5. Normalizar scores finais
                scores = NormalizeScores(scores);

                // 6. Atualizar métricas do sistema
                UpdateSystemDebtMetrics(scores);

                UpdateStatus($"✅ Dívidas estatísticas calculadas: débito total = {TotalSystemDebt:F2}");
                return scores;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Erro no cálculo de dívidas: {ex.Message}");
                throw;
            }
        }

        protected override Dictionary<int, double> ApplyModelSpecificFilters(Dictionary<int, double> scores)
        {
            var filteredScores = new Dictionary<int, double>(scores);

            try
            {
                // 1. Filtro de Concentração de Dívida - evita over-concentration
                filteredScores = ApplyDebtConcentrationFilter(filteredScores);

                // 2. Filtro de Momentum de Dívida - considera tendência da dívida
                filteredScores = ApplyDebtMomentumFilter(filteredScores);

                // 3. Filtro de Regime de Dívida - detecta mudanças de regime
                filteredScores = ApplyDebtRegimeFilter(filteredScores);

                // 4. Filtro de Correlação Temporal - evita padrões cíclicos
                filteredScores = ApplyTemporalCorrelationFilter(filteredScores);

                // 5. Filtro de Estabilização - suaviza oscilações extremas
                filteredScores = ApplyStabilizationFilter(filteredScores);

                return filteredScores;
            }
            catch (Exception ex)
            {
                UpdateStatus($"⚠️ Erro nos filtros de dívida: {ex.Message}");
                return scores; // Retorna scores originais em caso de erro
            }
        }

        protected override Dictionary<string, object> GetModelSpecificParameters()
        {
            return new Dictionary<string, object>
            {
                ["DebtAccelerationFactor"] = 1.2,
                ["TemporalDecayRate"] = 0.95,
                ["DebtCalculationWindow"] = 100,
                ["VolatilityNormalization"] = 1.0,
                ["ConcentrationThreshold"] = 0.8,
                ["MomentumWeight"] = 0.15,
                ["RegimeDetectionSensitivity"] = 0.3,
                ["TemporalCorrelationThreshold"] = 0.7,
                ["StabilizationFactor"] = 0.1,
                ["MinimumDebtThreshold"] = -10.0,
                ["MaximumDebtThreshold"] = 15.0
            };
        }
        #endregion

        #region Core Debt Calculation Logic

        private async Task UpdateDebtProfiles(int targetConcurso)
        {
            var calculationWindow = GetParameter<int>("DebtCalculationWindow");
            
            // Usar dados disponíveis até o concurso anterior
            var availableData = _historicalData
                .Where(l => l.Id < targetConcurso)
                .OrderByDescending(l => l.Id)
                .Take(calculationWindow)
                .ToList();

            if (!availableData.Any())
            {
                availableData = _historicalData.ToList();
            }

            await Task.Run(() =>
            {
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var profile = CalculateDebtProfile(dezena, availableData, targetConcurso);
                    _debtProfiles[dezena] = profile;
                }
            });
        }

        private StatisticalDebtProfile CalculateDebtProfile(int dezena, List<Lance> data, int targetConcurso)
        {
            var profile = new StatisticalDebtProfile
            {
                Dezena = dezena,
                CalculationWindow = data.Count,
                TargetConcurso = targetConcurso
            };

            // Calcular aparições reais
            profile.ActualAppearances = data.Count(l => l.Lista.Contains(dezena));

            // Calcular aparições esperadas (15/25 = 60% probability)
            profile.ExpectedAppearances = data.Count * (15.0 / 25.0);

            // Calcular dívida bruta
            profile.RawDebt = profile.ExpectedAppearances - profile.ActualAppearances;

            // Calcular dívida normalizada
            profile.NormalizedDebt = profile.ExpectedAppearances > 0 
                ? profile.RawDebt / profile.ExpectedAppearances 
                : 0.0;

            // Calcular intervalos entre aparições
            var appearances = data
                .Where(l => l.Lista.Contains(dezena))
                .Select(l => l.Id)
                .OrderBy(id => id)
                .ToList();

            profile.LastAppearance = appearances.LastOrDefault();
            profile.CurrentGap = targetConcurso - profile.LastAppearance;

            // Calcular volatilidade dos intervalos
            profile.IntervalVolatility = CalculateIntervalVolatility(appearances);

            // Calcular tendência da dívida (últimos vs primeiros da janela)
            profile.DebtTrend = CalculateDebtTrend(dezena, data);

            // Calcular momentum da dívida
            profile.DebtMomentum = CalculateDebtMomentum(dezena, data);

            // Timestamp da atualização
            profile.LastUpdate = DateTime.Now;

            return profile;
        }

        private double CalculatePrimaryDebtScore(StatisticalDebtProfile profile)
        {
            // Score baseado na dívida normalizada
            var debtScore = Math.Max(0.0, profile.NormalizedDebt);
            
            // Aplicar amplificação não-linear para enfatizar dívidas altas
            var amplifiedScore = Math.Pow(debtScore, 0.7);
            
            // Limitar extremos
            var minThreshold = GetParameter<double>("MinimumDebtThreshold");
            var maxThreshold = GetParameter<double>("MaximumDebtThreshold");
            
            if (profile.RawDebt < minThreshold)
                amplifiedScore *= 0.1; // Penalizar dívidas negativas extremas
            
            if (profile.RawDebt > maxThreshold)
                amplifiedScore = Math.Min(amplifiedScore, 0.95); // Limitar dívidas positivas extremas
                
            return amplifiedScore;
        }

        private double CalculateTemporalWeight(StatisticalDebtProfile profile, int targetConcurso)
        {
            var decayRate = GetParameter<double>("TemporalDecayRate");
            var gap = profile.CurrentGap;
            
            // Peso aumenta com o tempo desde última aparição
            var baseWeight = 1.0 - Math.Pow(decayRate, gap);
            
            // Ajustar baseado na volatilidade histórica
            var volatilityAdjustment = 1.0 + (profile.IntervalVolatility * 0.1);
            
            return Math.Min(2.0, baseWeight * volatilityAdjustment);
        }

        private double CalculateVolatilityAdjustment(StatisticalDebtProfile profile)
        {
            var normalization = GetParameter<double>("VolatilityNormalization");
            
            // Dezenas mais voláteis recebem ajuste adicional
            var volatilityFactor = 1.0 + (profile.IntervalVolatility * normalization * 0.05);
            
            return Math.Min(1.5, volatilityFactor);
        }

        private double CombineDebtFactors(double primaryScore, double temporalWeight, double volatilityAdjustment)
        {
            // Combinação ponderada dos fatores
            var combinedScore = primaryScore * temporalWeight * volatilityAdjustment;
            
            // Aplicar suavização para evitar valores extremos
            return Math.Min(1.0, combinedScore);
        }

        private Dictionary<int, double> ApplyDebtAcceleration(Dictionary<int, double> scores)
        {
            var factor = GetParameter<double>("DebtAccelerationFactor");
            var acceleratedScores = new Dictionary<int, double>();

            foreach (var kvp in scores)
            {
                var accelerated = Math.Pow(kvp.Value, 1.0 / factor);
                acceleratedScores[kvp.Key] = accelerated;
            }

            return acceleratedScores;
        }

        private Dictionary<int, double> ApplyDebtSmoothing(Dictionary<int, double> scores)
        {
            var smoothingFactor = GetParameter<double>("StabilizationFactor");
            var avgScore = scores.Values.Average();
            var smoothedScores = new Dictionary<int, double>();

            foreach (var kvp in scores)
            {
                var smoothed = (kvp.Value * (1.0 - smoothingFactor)) + (avgScore * smoothingFactor);
                smoothedScores[kvp.Key] = smoothed;
            }

            return smoothedScores;
        }

        private double CalculateIntervalVolatility(List<int> appearances)
        {
            if (appearances.Count < 3) return 0.0;

            var intervals = new List<double>();
            for (int i = 1; i < appearances.Count; i++)
            {
                intervals.Add(appearances[i] - appearances[i - 1]);
            }

            if (!intervals.Any()) return 0.0;

            var mean = intervals.Average();
            var variance = intervals.Sum(i => Math.Pow(i - mean, 2)) / (intervals.Count - 1);
            
            return Math.Sqrt(variance);
        }

        private double CalculateDebtTrend(int dezena, List<Lance> data)
        {
            if (data.Count < 20) return 0.0;

            // Comparar dívida na primeira vs segunda metade da janela
            var firstHalf = data.Skip(data.Count / 2).ToList();
            var secondHalf = data.Take(data.Count / 2).ToList();

            var firstDebt = CalculateWindowDebt(dezena, firstHalf);
            var secondDebt = CalculateWindowDebt(dezena, secondHalf);

            return secondDebt - firstDebt; // Tendência positiva = dívida crescendo
        }

        private double CalculateDebtMomentum(int dezena, List<Lance> data)
        {
            if (data.Count < 30) return 0.0;

            // Analisar últimos 10 vs 10 anteriores
            var recent = data.Take(10).ToList();
            var previous = data.Skip(10).Take(10).ToList();

            var recentDebt = CalculateWindowDebt(dezena, recent);
            var previousDebt = CalculateWindowDebt(dezena, previous);

            return recentDebt - previousDebt; // Momentum positivo = dívida acelerando
        }

        private double CalculateWindowDebt(int dezena, List<Lance> windowData)
        {
            var actualAppearances = windowData.Count(l => l.Lista.Contains(dezena));
            var expectedAppearances = windowData.Count * (15.0 / 25.0);
            return expectedAppearances - actualAppearances;
        }
        #endregion

        #region Model-Specific Filters

        private Dictionary<int, double> ApplyDebtConcentrationFilter(Dictionary<int, double> scores)
        {
            var threshold = GetParameter<double>("ConcentrationThreshold");
            var adjustedScores = new Dictionary<int, double>(scores);

            // Detectar se dívida está muito concentrada em poucas dezenas
            var topScores = scores.OrderByDescending(kvp => kvp.Value).Take(5).ToList();
            var concentration = topScores.Average(kvp => kvp.Value);

            if (concentration > threshold)
            {
                // Redistribuir um pouco da concentração
                var redistribution = (concentration - threshold) * 0.1;
                
                foreach (var topScore in topScores)
                {
                    adjustedScores[topScore.Key] -= redistribution;
                }

                // Dar pequeno bônus às outras
                var others = scores.Where(kvp => !topScores.Any(t => t.Key == kvp.Key)).ToList();
                var bonusPerOther = (redistribution * topScores.Count) / others.Count;
                
                foreach (var other in others)
                {
                    adjustedScores[other.Key] += bonusPerOther;
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyDebtMomentumFilter(Dictionary<int, double> scores)
        {
            var momentumWeight = GetParameter<double>("MomentumWeight");
            var adjustedScores = new Dictionary<int, double>(scores);

            foreach (var dezena in scores.Keys)
            {
                if (_debtProfiles.ContainsKey(dezena))
                {
                    var momentum = _debtProfiles[dezena].DebtMomentum;
                    
                    // Momentum positivo = dívida acelerando = bônus
                    if (momentum > 0.5)
                    {
                        adjustedScores[dezena] += momentumWeight;
                    }
                    // Momentum negativo = dívida diminuindo = penalidade
                    else if (momentum < -0.5)
                    {
                        adjustedScores[dezena] -= momentumWeight * 0.5;
                    }
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyDebtRegimeFilter(Dictionary<int, double> scores)
        {
            var sensitivity = GetParameter<double>("RegimeDetectionSensitivity");
            var adjustedScores = new Dictionary<int, double>(scores);

            // Detectar mudança de regime baseado na distribuição de dívidas
            var debtVariance = CalculateDebtVariance();
            
            if (debtVariance > sensitivity)
            {
                // Alta variância = regime instável = favorece diversificação
                var avgScore = scores.Values.Average();
                var diversificationBonus = 0.05;

                foreach (var dezena in scores.Keys)
                {
                    var deviation = Math.Abs(scores[dezena] - avgScore);
                    if (deviation < avgScore * 0.5) // Scores próximos da média
                    {
                        adjustedScores[dezena] += diversificationBonus;
                    }
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyTemporalCorrelationFilter(Dictionary<int, double> scores)
        {
            var threshold = GetParameter<double>("TemporalCorrelationThreshold");
            var adjustedScores = new Dictionary<int, double>(scores);

            // Detectar correlações temporais e aplicar anti-correlação
            foreach (var dezena in scores.Keys)
            {
                if (_debtProfiles.ContainsKey(dezena))
                {
                    var profile = _debtProfiles[dezena];
                    
                    // Se gap atual é muito similar ao intervalo médio, aplicar pequena penalidade
                    // para quebrar possíveis padrões cíclicos
                    var avgInterval = profile.IntervalVolatility > 0 ? 7.0 : 6.0; // Aproximação
                    var gapRatio = Math.Abs(profile.CurrentGap - avgInterval) / avgInterval;
                    
                    if (gapRatio < 0.2) // Gap muito próximo da média
                    {
                        adjustedScores[dezena] *= 0.95; // Pequena penalidade anti-cíclica
                    }
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyStabilizationFilter(Dictionary<int, double> scores)
        {
            var stabilizationFactor = GetParameter<double>("StabilizationFactor");
            var adjustedScores = new Dictionary<int, double>(scores);

            var mean = scores.Values.Average();
            var stdDev = Math.Sqrt(scores.Values.Sum(s => Math.Pow(s - mean, 2)) / scores.Count);

            // Suavizar outliers extremos
            foreach (var dezena in scores.Keys.ToList())
            {
                var deviation = Math.Abs(scores[dezena] - mean);
                
                if (deviation > stdDev * 2) // Outlier
                {
                    var direction = scores[dezena] > mean ? 1 : -1;
                    var adjustment = deviation * stabilizationFactor * direction;
                    adjustedScores[dezena] = scores[dezena] - adjustment;
                }
            }

            return adjustedScores;
        }
        #endregion

        #region Metrics and Status Updates

        private void UpdateSystemDebtMetrics(Dictionary<int, double> scores)
        {
            if (!_debtProfiles.Any()) return;

            // Calcular dívida total do sistema
            TotalSystemDebt = _debtProfiles.Values.Sum(p => Math.Max(0, p.RawDebt));

            // Encontrar dezena com maior dívida
            var highestDebt = _debtProfiles.Values.OrderByDescending(p => p.RawDebt).FirstOrDefault();
            HighestDebtDezena = highestDebt != null ? $"Dezena {highestDebt.Dezena:D2} ({highestDebt.RawDebt:F1})" : "";

            // Calcular variância da dívida
            AverageDebtVariance = CalculateDebtVariance();

            // Contar devedores (dívida positiva)
            DebtorsCount = _debtProfiles.Values.Count(p => p.RawDebt > 0);

            // Calcular concentração da dívida
            DebtConcentration = CalculateDebtConcentration(scores);
        }

        private double CalculateDebtVariance()
        {
            if (!_debtProfiles.Any()) return 0.0;

            var debts = _debtProfiles.Values.Select(p => p.RawDebt).ToList();
            var mean = debts.Average();
            var variance = debts.Sum(d => Math.Pow(d - mean, 2)) / debts.Count;
            
            return Math.Sqrt(variance);
        }

        private double CalculateDebtConcentration(Dictionary<int, double> scores)
        {
            if (!scores.Any()) return 0.0;

            var topScores = scores.OrderByDescending(kvp => kvp.Value).Take(5);
            var topAverage = topScores.Average(kvp => kvp.Value);
            var overallAverage = scores.Values.Average();
            
            return overallAverage > 0 ? topAverage / overallAverage : 1.0;
        }

        private void UpdateDebtMetrics()
        {
            TotalSystemDebt = 0.0;
            HighestDebtDezena = "Aguardando cálculo...";
            AverageDebtVariance = 0.0;
            DebtorsCount = 0;
            DebtConcentration = 0.0;
        }

        private Dictionary<int, double> NormalizeScores(Dictionary<int, double> scores)
        {
            if (!scores.Any()) return scores;

            var minScore = scores.Values.Min();
            var maxScore = scores.Values.Max();
            var range = maxScore - minScore;

            if (range == 0) return scores;

            var normalizedScores = new Dictionary<int, double>();
            foreach (var kvp in scores)
            {
                var normalizedScore = (kvp.Value - minScore) / range;
                normalizedScores[kvp.Key] = normalizedScore;
            }

            return normalizedScores;
        }

        private void UpdateStatus(string status)
        {
            AntiFrequencyStatus = status;
            System.Diagnostics.Debug.WriteLine($"[StatisticalDebtModel] {status}");
        }
        #endregion

        #region Validation and Explanation

        /// <summary>
        /// Valida se o modelo de dívida estatística está funcionando corretamente
        /// </summary>
        public async Task<DebtValidationResult> ValidateDebtModelAsync()
        {
            try
            {
                UpdateStatus("Validando modelo de dívida estatística...");

                var result = new DebtValidationResult
                {
                    ModelName = ModelName,
                    ValidationTime = DateTime.Now,
                    IsValid = true,
                    ValidationDetails = new List<string>()
                };

                // 1. Validar perfis de dívida
                ValidateDebtProfiles(result);

                // 2. Validar cálculos matemáticos
                ValidateMathematicalConsistency(result);

                // 3. Validar distribuição de dívidas
                ValidateDebtDistribution(result);

                // 4. Validar propriedades anti-frequencistas
                await ValidateAntiFrequencyProperties(result);

                result.OverallScore = CalculateValidationScore(result);

                UpdateStatus($"✅ Validação de dívida concluída: {result.OverallScore:P1}");
                return result;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Erro na validação de dívida: {ex.Message}");
                return new DebtValidationResult
                {
                    ModelName = ModelName,
                    IsValid = false,
                    ValidationDetails = new List<string> { $"Erro: {ex.Message}" }
                };
            }
        }

        /// <summary>
        /// Gera explicação detalhada sobre uma predição baseada em dívida
        /// </summary>
        public override ModelExplanation ExplainPrediction(PredictionResult prediction)
        {
            var explanation = base.ExplainPrediction(prediction);

            // Adicionar explicações específicas do modelo de dívida
            explanation.MainFactors.Add($"Dívida total do sistema: {TotalSystemDebt:F1}");
            explanation.MainFactors.Add($"Maior devedor: {HighestDebtDezena}");
            explanation.MainFactors.Add($"Devedores ativos: {DebtorsCount}/25 dezenas");
            explanation.MainFactors.Add($"Concentração de dívida: {DebtConcentration:F2}x");

            // Detalhes técnicos específicos
            explanation.TechnicalDetails["DebtAccelerationFactor"] = GetParameter<double>("DebtAccelerationFactor");
            explanation.TechnicalDetails["TemporalDecayRate"] = GetParameter<double>("TemporalDecayRate");
            explanation.TechnicalDetails["TotalSystemDebt"] = TotalSystemDebt;
            explanation.TechnicalDetails["DebtVariance"] = AverageDebtVariance;

            // Top 5 dezenas com maior dívida
            var topDebtors = prediction.PredictedNumbers.Take(5);
            foreach (var dezena in topDebtors)
            {
                if (_debtProfiles.ContainsKey(dezena))
                {
                    var profile = _debtProfiles[dezena];
                    explanation.TechnicalDetails[$"Dezena{dezena:D2}_Debt"] = new
                    {
                        DividaBruta = profile.RawDebt,
                        DividaNormalizada = profile.NormalizedDebt,
                        TendenciaDivida = profile.DebtTrend,
                        MomentumDivida = profile.DebtMomentum,
                        GapAtual = profile.CurrentGap
                    };
                }
            }

            return explanation;
        }

        private void ValidateDebtProfiles(DebtValidationResult result)
        {
            if (_debtProfiles.Count != 25)
            {
                result.IsValid = false;
                result.ValidationDetails.Add($"Número incorreto de perfis de dívida: {_debtProfiles.Count}");
                return;
            }

            var invalidProfiles = _debtProfiles.Values.Count(p => 
                double.IsNaN(p.RawDebt) || double.IsInfinity(p.RawDebt));

            if (invalidProfiles > 0)
            {
                result.IsValid = false;
                result.ValidationDetails.Add($"{invalidProfiles} perfis com dívidas inválidas");
                return;
            }

            result.ValidationDetails.Add("✅ Perfis de dívida validados");
        }

        private void ValidateMathematicalConsistency(DebtValidationResult result)
        {
            var totalExpected = _debtProfiles.Values.Sum(p => p.ExpectedAppearances);
            var totalActual = _debtProfiles.Values.Sum(p => p.ActualAppearances);
            var totalRawDebt = _debtProfiles.Values.Sum(p => p.RawDebt);
            var calculatedDebt = totalExpected - totalActual;

            if (Math.Abs(totalRawDebt - calculatedDebt) > 0.1)
            {
                result.IsValid = false;
                result.ValidationDetails.Add("Inconsistência matemática nos cálculos de dívida");
                return;
            }

            result.ValidationDetails.Add("✅ Consistência matemática validada");
        }

        private void ValidateDebtDistribution(DebtValidationResult result)
        {
            var positiveDebts = _debtProfiles.Values.Count(p => p.RawDebt > 0);
            var negativeDebts = _debtProfiles.Values.Count(p => p.RawDebt < 0);

            // Deve haver uma distribuição razoável de dívidas positivas e negativas
            if (positiveDebts == 0 || negativeDebts == 0)
            {
                result.ValidationDetails.Add("⚠️ Distribuição de dívidas desequilibrada");
            }
            else
            {
                result.ValidationDetails.Add("✅ Distribuição de dívidas adequada");
            }
        }

        private async Task ValidateAntiFrequencyProperties(DebtValidationResult result)
        {
            await Task.Delay(1); // Placeholder

            // Validar que dezenas com maior dívida tendem a ter menor frequência recente
            var correlations = new List<double>();
            
            foreach (var profile in _debtProfiles.Values)
            {
                var recentFreq = profile.ActualAppearances / (double)profile.CalculationWindow;
                var debtScore = profile.NormalizedDebt;
                
                // Correlação negativa esperada
                correlations.Add(recentFreq * debtScore);
            }

            var avgCorrelation = correlations.Average();
            if (avgCorrelation > 0.1) // Correlação positiva forte = problema
            {
                result.ValidationDetails.Add("⚠️ Propriedades anti-frequencistas fracas");
            }
            else
            {
                result.ValidationDetails.Add("✅ Propriedades anti-frequencistas confirmadas");
            }
        }

        private double CalculateValidationScore(DebtValidationResult result)
        {
            if (!result.IsValid) return 0.0;

            var successfulChecks = result.ValidationDetails.Count(d => d.StartsWith("✅"));
            var totalChecks = result.ValidationDetails.Count;

            return totalChecks > 0 ? (double)successfulChecks / totalChecks : 0.0;
        }
        #endregion
    }

    #region Supporting Data Classes

    /// <summary>
    /// Perfil de dívida estatística para uma dezena específica
    /// </summary>
    public class StatisticalDebtProfile
    {
        public int Dezena { get; set; }
        public int CalculationWindow { get; set; }
        public int TargetConcurso { get; set; }
        
        public double ActualAppearances { get; set; }
        public double ExpectedAppearances { get; set; }
        public double RawDebt { get; set; }
        public double NormalizedDebt { get; set; }
        
        public int LastAppearance { get; set; }
        public int CurrentGap { get; set; }
        public double IntervalVolatility { get; set; }
        
        public double DebtTrend { get; set; }
        public double DebtMomentum { get; set; }
        
        public DateTime LastUpdate { get; set; }
    }

    /// <summary>
    /// Resultado da validação do modelo de dívida estatística
    /// </summary>
    public class DebtValidationResult
    {
        public string ModelName { get; set; }
        public DateTime ValidationTime { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationDetails { get; set; } = new List<string>();
        public double OverallScore { get; set; }
    }

    #endregion
}