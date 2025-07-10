// D:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Simple\AntiFrequencySimpleModel.cs - Primeiro modelo anti-frequencista
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

namespace LotoLibrary.PredictionModels.AntiFrequency.Simple
{
    /// <summary>
    /// Modelo Anti-Frequencista Simples - Primeira implementação da estratégia anti-frequencista
    /// 
    /// CONCEITO: Inverte a lógica frequencista priorizando dezenas que aparecem MENOS
    /// ESTRATÉGIA: Quanto menor a frequência histórica, maior a probabilidade de seleção
    /// OBJETIVO: Explorar ineficiências deixadas pelos modelos frequencistas tradicionais
    /// </summary>
    public partial class AntiFrequencySimpleModel : AntiFrequencyModelBase
    {
        #region Fields
        private Dictionary<int, SimpleAntiFreqProfile> _simpleProfiles;
        private double _inversionFactor = 1.0;
        private double _minimumThreshold = 0.05;
        private int _recentAnalysisWindow = 50;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private double _currentInversionStrength;

        [ObservableProperty]
        private string _strategyDescription = "Inversão simples de frequência";

        [ObservableProperty]
        private int _underRepresentedCount;

        [ObservableProperty]
        private double _diversificationScore;
        #endregion

        #region Properties
        public override string ModelName => "Anti-Frequência Simples";
        public override AntiFrequencyStrategy DefaultStrategy => AntiFrequencyStrategy.Moderate;

        /// <summary>
        /// Perfis simplificados específicos deste modelo
        /// </summary>
        public Dictionary<int, SimpleAntiFreqProfile> SimpleProfiles => _simpleProfiles;

        /// <summary>
        /// Força da inversão aplicada (0.0 = sem inversão, 1.0 = inversão total)
        /// </summary>
        public double InversionStrength => _inversionFactor;
        #endregion

        #region Constructor
        public AntiFrequencySimpleModel()
        {
            _simpleProfiles = new Dictionary<int, SimpleAntiFreqProfile>();
            UpdateStrategyDescription();
        }
        #endregion

        #region AntiFrequencyModelBase Implementation

        protected override async Task<Dictionary<int, double>> CalculateAntiFrequencyScoresAsync(int targetConcurso)
        {
            var scores = new Dictionary<int, double>();

            try
            {
                UpdateStatus("Calculando scores anti-frequencistas simples...");

                // 1. Calcular frequências atuais de todas as dezenas
                var currentFrequencies = await CalculateCurrentFrequencies(targetConcurso);

                // 2. Aplicar inversão simples da frequência
                foreach (var kvp in currentFrequencies)
                {
                    var dezena = kvp.Key;
                    var frequency = kvp.Value;

                    // Score anti-frequencista = inversão da frequência normalizada
                    var antiFreqScore = CalculateSimpleAntiFrequencyScore(frequency);

                    // Aplicar fator de inversão configurável
                    antiFreqScore = ApplyInversionFactor(antiFreqScore, frequency);

                    // Aplicar threshold mínimo para evitar outliers extremos
                    antiFreqScore = ApplyMinimumThreshold(antiFreqScore, frequency);

                    scores[dezena] = antiFreqScore;
                }

                // 3. Aplicar ajustes baseados em análise recente
                scores = await ApplyRecentAnalysisAdjustments(scores, targetConcurso);

                // 4. Normalizar scores para range 0-1
                scores = NormalizeScores(scores);

                // 5. Atualizar métricas do modelo
                UpdateModelMetrics(scores);

                UpdateStatus($"✅ Scores anti-frequencistas calculados: {scores.Count} dezenas analisadas");
                return scores;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Erro no cálculo de scores: {ex.Message}");
                throw;
            }
        }

        protected override Dictionary<int, double> ApplyModelSpecificFilters(Dictionary<int, double> scores)
        {
            var filteredScores = new Dictionary<int, double>(scores);

            try
            {
                // 1. Filtro de Diversificação - evita concentração excessiva em grupos
                filteredScores = ApplyDiversificationFilter(filteredScores);

                // 2. Filtro de Recentidade - ajusta baseado em aparições muito recentes
                filteredScores = ApplyRecencyFilter(filteredScores);

                // 3. Filtro de Equilíbrio - garante representação mínima de todos os grupos
                filteredScores = ApplyBalanceFilter(filteredScores);

                // 4. Filtro de Volatilidade - ajusta baseado na estabilidade histórica
                filteredScores = ApplyVolatilityFilter(filteredScores);

                return filteredScores;
            }
            catch (Exception ex)
            {
                UpdateStatus($"⚠️ Erro nos filtros específicos: {ex.Message}");
                return scores; // Retorna scores originais em caso de erro
            }
        }

        protected override Dictionary<string, object> GetModelSpecificParameters()
        {
            return new Dictionary<string, object>
            {
                ["InversionFactor"] = 0.8,
                ["MinimumThreshold"] = 0.05,
                ["RecentAnalysisWindow"] = 50,
                ["DiversificationWeight"] = 0.15,
                ["RecencyWeight"] = 0.10,
                ["BalanceWeight"] = 0.20,
                ["VolatilityWeight"] = 0.05,
                ["UnderRepresentationBonus"] = 0.25,
                ["OverRepresentationPenalty"] = 0.30
            };
        }
        #endregion

        #region Core Algorithm Implementation

        private async Task<Dictionary<int, double>> CalculateCurrentFrequencies(int targetConcurso)
        {
            var frequencies = new Dictionary<int, double>();
            var analysisWindow = GetParameter<int>("RecentAnalysisWindow");

            // Usar dados até o concurso anterior ao alvo
            var availableData = _historicalData
                .Where(l => l.Id < targetConcurso)
                .OrderByDescending(l => l.Id)
                .Take(analysisWindow)
                .ToList();

            if (!availableData.Any())
            {
                // Fallback: usar todos os dados disponíveis
                availableData = _historicalData.ToList();
            }

            // Calcular frequência de cada dezena na janela de análise
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var appearances = availableData.Count(l => l.Lista.Contains(dezena));
                frequencies[dezena] = (double)appearances / availableData.Count;

                // Atualizar perfil simples
                if (!_simpleProfiles.ContainsKey(dezena))
                {
                    _simpleProfiles[dezena] = new SimpleAntiFreqProfile { Dezena = dezena };
                }

                _simpleProfiles[dezena].CurrentFrequency = frequencies[dezena];
                _simpleProfiles[dezena].RecentAppearances = appearances;
                _simpleProfiles[dezena].AnalysisWindow = availableData.Count;
                _simpleProfiles[dezena].LastUpdate = DateTime.Now;
            }

            return frequencies;
        }

        private double CalculateSimpleAntiFrequencyScore(double frequency)
        {
            // Inversão simples: score = 1 - frequency normalizada
            var expectedFrequency = 15.0 / 25.0; // 60% é a frequência esperada base
            
            // Normalizar frequência em relação à expectativa
            var normalizedFrequency = Math.Min(1.0, frequency / expectedFrequency);
            
            // Inversão: quanto menor a frequência, maior o score
            var inversionScore = 1.0 - normalizedFrequency;
            
            // Aplicar curva para enfatizar diferenças
            return Math.Pow(inversionScore, 0.8);
        }

        private double ApplyInversionFactor(double antiFreqScore, double originalFrequency)
        {
            var factor = GetParameter<double>("InversionFactor");
            
            // Misturar score anti-frequencista com score neutro
            var neutralScore = 0.5; // Score neutro (sem bias)
            return (antiFreqScore * factor) + (neutralScore * (1.0 - factor));
        }

        private double ApplyMinimumThreshold(double score, double frequency)
        {
            var threshold = GetParameter<double>("MinimumThreshold");
            
            // Se frequência for muito baixa (potencial outlier), reduzir score
            if (frequency < threshold)
            {
                var penaltyFactor = frequency / threshold; // 0-1
                return score * (0.5 + (penaltyFactor * 0.5)); // Reduzir até 50%
            }
            
            return score;
        }

        private async Task<Dictionary<int, double>> ApplyRecentAnalysisAdjustments(Dictionary<int, double> scores, int targetConcurso)
        {
            var adjustedScores = new Dictionary<int, double>(scores);
            
            // Analisar tendências recentes (últimos 10 concursos)
            var recentData = _historicalData
                .Where(l => l.Id < targetConcurso)
                .OrderByDescending(l => l.Id)
                .Take(10)
                .ToList();

            if (recentData.Count < 5) return adjustedScores; // Dados insuficientes

            foreach (var dezena in scores.Keys.ToList())
            {
                var recentAppearances = recentData.Count(l => l.Lista.Contains(dezena));
                var recentFrequency = (double)recentAppearances / recentData.Count;
                var expectedRecent = 15.0 / 25.0;

                // Se dezena apareceu muito recentemente, reduzir score ligeiramente
                if (recentFrequency > expectedRecent * 1.5)
                {
                    adjustedScores[dezena] *= 0.9; // Reduzir 10%
                }
                // Se dezena não apareceu recentemente, aumentar score
                else if (recentFrequency < expectedRecent * 0.5)
                {
                    adjustedScores[dezena] *= 1.1; // Aumentar 10%
                }

                // Atualizar perfil
                if (_simpleProfiles.ContainsKey(dezena))
                {
                    _simpleProfiles[dezena].RecentTrend = recentFrequency - expectedRecent;
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> NormalizeScores(Dictionary<int, double> scores)
        {
            if (!scores.Any()) return scores;

            var minScore = scores.Values.Min();
            var maxScore = scores.Values.Max();
            var range = maxScore - minScore;

            if (range == 0) return scores; // Todos os scores são iguais

            var normalizedScores = new Dictionary<int, double>();
            foreach (var kvp in scores)
            {
                var normalizedScore = (kvp.Value - minScore) / range;
                normalizedScores[kvp.Key] = normalizedScore;
            }

            return normalizedScores;
        }

        private void UpdateModelMetrics(Dictionary<int, double> scores)
        {
            // Calcular força da inversão atual
            var expectedFreq = 15.0 / 25.0;
            var avgScore = scores.Values.Average();
            CurrentInversionStrength = Math.Abs(avgScore - 0.5) * 2.0; // 0-1

            // Contar dezenas sub-representadas
            UnderRepresentedCount = _simpleProfiles.Values
                .Count(p => p.CurrentFrequency < expectedFreq * 0.8);

            // Calcular score de diversificação
            var scoreVariance = CalculateVariance(scores.Values.ToList());
            DiversificationScore = Math.Min(1.0, scoreVariance * 4.0); // Normalizar

            // Atualizar descrição da estratégia
            UpdateStrategyDescription();
        }

        private void UpdateStrategyDescription()
        {
            var strength = CurrentInversionStrength;
            if (strength < 0.3)
                StrategyDescription = "Inversão suave - correção moderada";
            else if (strength < 0.7)
                StrategyDescription = "Inversão moderada - balanceamento ativo";
            else
                StrategyDescription = "Inversão forte - priorização anti-frequencista";
        }
        #endregion

        #region Model-Specific Filters

        private Dictionary<int, double> ApplyDiversificationFilter(Dictionary<int, double> scores)
        {
            var weight = GetParameter<double>("DiversificationWeight");
            var adjustedScores = new Dictionary<int, double>(scores);

            // Definir grupos de dezenas
            var grupos = new Dictionary<string, List<int>>
            {
                ["Baixas"] = Enumerable.Range(1, 8).ToList(),
                ["Médias"] = Enumerable.Range(9, 9).ToList(),
                ["Altas"] = Enumerable.Range(18, 8).ToList()
            };

            foreach (var grupo in grupos)
            {
                var groupScores = grupo.Value.Select(d => scores[d]).ToList();
                var groupAvg = groupScores.Average();
                var overallAvg = scores.Values.Average();

                // Se grupo está sub-representado, aumentar scores
                if (groupAvg < overallAvg * 0.8)
                {
                    foreach (var dezena in grupo.Value)
                    {
                        adjustedScores[dezena] += weight;
                    }
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyRecencyFilter(Dictionary<int, double> scores)
        {
            var weight = GetParameter<double>("RecencyWeight");
            var adjustedScores = new Dictionary<int, double>(scores);

            foreach (var dezena in scores.Keys)
            {
                if (_simpleProfiles.ContainsKey(dezena))
                {
                    var profile = _simpleProfiles[dezena];
                    
                    // Se teve aparição muito recente, aplicar pequena penalidade
                    if (profile.RecentTrend > 0.2)
                    {
                        adjustedScores[dezena] -= weight;
                    }
                    // Se não apareceu recentemente, dar pequeno bônus
                    else if (profile.RecentTrend < -0.2)
                    {
                        adjustedScores[dezena] += weight;
                    }
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyBalanceFilter(Dictionary<int, double> scores)
        {
            var weight = GetParameter<double>("BalanceWeight");
            var adjustedScores = new Dictionary<int, double>(scores);

            // Balanceamento par/ímpar
            var pares = scores.Where(kvp => kvp.Key % 2 == 0).ToList();
            var impares = scores.Where(kvp => kvp.Key % 2 != 0).ToList();

            var avgPares = pares.Average(kvp => kvp.Value);
            var avgImpares = impares.Average(kvp => kvp.Value);

            if (avgPares > avgImpares * 1.3)
            {
                // Favorecer ímpares
                foreach (var impar in impares)
                {
                    adjustedScores[impar.Key] += weight;
                }
            }
            else if (avgImpares > avgPares * 1.3)
            {
                // Favorecer pares
                foreach (var par in pares)
                {
                    adjustedScores[par.Key] += weight;
                }
            }

            return adjustedScores;
        }

        private Dictionary<int, double> ApplyVolatilityFilter(Dictionary<int, double> scores)
        {
            var weight = GetParameter<double>("VolatilityWeight");
            var adjustedScores = new Dictionary<int, double>(scores);

            // Analisar volatilidade de cada dezena e ajustar scores
            foreach (var dezena in scores.Keys)
            {
                if (_simpleProfiles.ContainsKey(dezena))
                {
                    var profile = _simpleProfiles[dezena];
                    
                    // Calcular volatilidade simplificada baseada na tendência recente
                    var volatility = Math.Abs(profile.RecentTrend);
                    
                    // Dezenas com alta volatilidade recebem pequeno ajuste
                    if (volatility > 0.3)
                    {
                        adjustedScores[dezena] += weight * 0.5; // Bônus por instabilidade
                    }
                }
            }

            return adjustedScores;
        }
        #endregion

        #region Validation and Explanation

        /// <summary>
        /// Valida se o modelo está funcionando corretamente
        /// </summary>
        public async Task<AntiFreqValidationResult> ValidateModelPerformanceAsync()
        {
            try
            {
                UpdateStatus("Validando performance do modelo anti-frequencista...");

                var result = new AntiFreqValidationResult
                {
                    ModelName = ModelName,
                    ValidationTime = DateTime.Now,
                    IsValid = true,
                    ValidationDetails = new List<string>()
                };

                // 1. Validar configuração de parâmetros
                ValidateParameters(result);

                // 2. Validar perfis criados
                ValidateProfiles(result);

                // 3. Validar anti-correlação com frequencismo
                await ValidateAntiCorrelation(result);

                // 4. Validar distribuição de scores
                ValidateScoreDistribution(result);

                result.OverallScore = CalculateOverallValidationScore(result);

                UpdateStatus($"✅ Validação concluída: {result.OverallScore:P1} de qualidade");
                return result;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Erro na validação: {ex.Message}");
                return new AntiFreqValidationResult
                {
                    ModelName = ModelName,
                    IsValid = false,
                    ValidationDetails = new List<string> { $"Erro: {ex.Message}" }
                };
            }
        }

        /// <summary>
        /// Gera explicação detalhada sobre uma predição específica
        /// </summary>
        public override ModelExplanation ExplainPrediction(PredictionResult prediction)
        {
            var explanation = base.ExplainPrediction(prediction);

            // Adicionar explicações específicas do modelo simples
            explanation.MainFactors.Add($"Força de inversão aplicada: {CurrentInversionStrength:P1}");
            explanation.MainFactors.Add($"Dezenas sub-representadas identificadas: {UnderRepresentedCount}");
            explanation.MainFactors.Add($"Score de diversificação: {DiversificationScore:P1}");

            // Detalhes técnicos específicos
            explanation.TechnicalDetails["InversionFactor"] = GetParameter<double>("InversionFactor");
            explanation.TechnicalDetails["MinimumThreshold"] = GetParameter<double>("MinimumThreshold");
            explanation.TechnicalDetails["CurrentInversionStrength"] = CurrentInversionStrength;
            explanation.TechnicalDetails["UnderRepresentedCount"] = UnderRepresentedCount;

            // Top 5 dezenas com maior score anti-frequencista
            var topAntiFreq = prediction.PredictedNumbers.Take(5);
            foreach (var dezena in topAntiFreq)
            {
                if (_simpleProfiles.ContainsKey(dezena))
                {
                    var profile = _simpleProfiles[dezena];
                    explanation.TechnicalDetails[$"Dezena{dezena:D2}_Profile"] = new
                    {
                        FrequenciaAtual = profile.CurrentFrequency,
                        TendenciaRecente = profile.RecentTrend,
                        AparicaesRecentes = profile.RecentAppearances
                    };
                }
            }

            return explanation;
        }
        #endregion

        #region Helper Methods

        private void ValidateParameters(AntiFreqValidationResult result)
        {
            var inversionFactor = GetParameter<double>("InversionFactor");
            if (inversionFactor < 0.0 || inversionFactor > 1.0)
            {
                result.IsValid = false;
                result.ValidationDetails.Add("InversionFactor fora do range válido (0-1)");
            }

            var threshold = GetParameter<double>("MinimumThreshold");
            if (threshold < 0.0 || threshold > 0.5)
            {
                result.IsValid = false;
                result.ValidationDetails.Add("MinimumThreshold fora do range válido (0-0.5)");
            }

            if (result.IsValid)
            {
                result.ValidationDetails.Add("✅ Parâmetros validados com sucesso");
            }
        }

        private void ValidateProfiles(AntiFreqValidationResult result)
        {
            if (_simpleProfiles.Count != 25)
            {
                result.IsValid = false;
                result.ValidationDetails.Add($"Número incorreto de perfis: {_simpleProfiles.Count} (esperado: 25)");
                return;
            }

            var invalidProfiles = _simpleProfiles.Values
                .Count(p => p.CurrentFrequency < 0 || p.CurrentFrequency > 1);

            if (invalidProfiles > 0)
            {
                result.IsValid = false;
                result.ValidationDetails.Add($"{invalidProfiles} perfis com frequências inválidas");
                return;
            }

            result.ValidationDetails.Add("✅ Perfis validados com sucesso");
        }

        private async Task ValidateAntiCorrelation(AntiFreqValidationResult result)
        {
            // Simular validação de anti-correlação
            await Task.Delay(1);

            var avgFrequency = _simpleProfiles.Values.Average(p => p.CurrentFrequency);
            var expectedFreq = 15.0 / 25.0;

            // Modelo deve ter bias para dezenas menos frequentes
            if (avgFrequency < expectedFreq * 0.9)
            {
                result.ValidationDetails.Add("✅ Anti-correlação detectada corretamente");
            }
            else
            {
                result.ValidationDetails.Add("⚠️ Anti-correlação fraca - modelo pode estar muito conservador");
            }
        }

        private void ValidateScoreDistribution(AntiFreqValidationResult result)
        {
            if (!_simpleProfiles.Any()) return;

            var frequencies = _simpleProfiles.Values.Select(p => p.CurrentFrequency).ToList();
            var variance = CalculateVariance(frequencies);

            if (variance > 0.01) // Boa diversidade de frequências
            {
                result.ValidationDetails.Add("✅ Distribuição de scores adequada");
            }
            else
            {
                result.ValidationDetails.Add("⚠️ Distribuição de scores muito uniforme");
            }
        }

        private double CalculateOverallValidationScore(AntiFreqValidationResult result)
        {
            if (!result.IsValid) return 0.0;

            var successfulChecks = result.ValidationDetails.Count(d => d.StartsWith("✅"));
            var totalChecks = result.ValidationDetails.Count;

            return totalChecks > 0 ? (double)successfulChecks / totalChecks : 0.0;
        }

        private double CalculateVariance(List<double> values)
        {
            if (values.Count < 2) return 0.0;

            var mean = values.Average();
            return values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
        }

        private void UpdateStatus(string status)
        {
            AntiFrequencyStatus = status;
            System.Diagnostics.Debug.WriteLine($"[AntiFrequencySimpleModel] {status}");
        }
        #endregion
    }

    #region Supporting Data Classes

    /// <summary>
    /// Perfil simplificado para o modelo anti-frequencista simples
    /// </summary>
    public class SimpleAntiFreqProfile
    {
        public int Dezena { get; set; }
        public double CurrentFrequency { get; set; }
        public double RecentTrend { get; set; }
        public int RecentAppearances { get; set; }
        public int AnalysisWindow { get; set; }
        public DateTime LastUpdate { get; set; }
        public double AntiFrequencyScore { get; set; }
    }

    /// <summary>
    /// Resultado da validação do modelo anti-frequencista
    /// </summary>
    public class AntiFreqValidationResult
    {
        public string ModelName { get; set; }
        public DateTime ValidationTime { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationDetails { get; set; } = new List<string>();
        public double OverallScore { get; set; }
    }

    #endregion
}