// D:\PROJETOS\GraphFacil\Library\Suporte\ContextAnalysis.cs - EXPANDIDA
using System.Collections.Generic;
using System.Linq;
using System;
using LotoLibrary.Suporte;

namespace LotoLibrary.Suporte
{
    /// <summary>
    /// Análise expandida de contexto para meta-learning
    /// Versão robusta com análise detalhada de padrões e contexto
    /// </summary>
    public class ContextAnalysis
    {
        #region Core Context Properties
        /// <summary>
        /// Nome identificador do contexto analisado
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Volatilidade detectada nos padrões (0.0 a 1.0)
        /// </summary>
        public double Volatility { get; set; }

        /// <summary>
        /// Força da tendência temporal detectada (0.0 a 1.0)
        /// </summary>
        public double TrendStrength { get; set; }

        /// <summary>
        /// Complexidade dos padrões detectados (0.0 a 1.0)
        /// </summary>
        public double PatternComplexity { get; set; }

        /// <summary>
        /// Performance recente de cada modelo
        /// </summary>
        public Dictionary<string, double> RecentPerformance { get; set; } = new Dictionary<string, double>();
        #endregion

        #region Extended Analysis Properties
        /// <summary>
        /// Momento da análise
        /// </summary>
        public DateTime AnalysisTimestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Tamanho da janela de análise utilizada
        /// </summary>
        public int AnalysisWindow { get; set; } = 50;

        /// <summary>
        /// Regime de mercado detectado
        /// </summary>
        public string DetectedRegime { get; set; } = "Unknown";

        /// <summary>
        /// Confiança na detecção do regime (0.0 a 1.0)
        /// </summary>
        public double RegimeConfidence { get; set; }

        /// <summary>
        /// Indicadores estatísticos calculados
        /// </summary>
        public Dictionary<string, double> MetricasEstatisticas { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Fatores contextuais específicos detectados
        /// </summary>
        public List<string> ContextualFactors { get; set; } = new List<string>();

        /// <summary>
        /// Métricas de diversificação
        /// </summary>
        public DiversificationMetrics Diversification { get; set; } = new DiversificationMetrics();

        /// <summary>
        /// Análise temporal específica
        /// </summary>
        public TemporalContext TemporalInfo { get; set; } = new TemporalContext();

        /// <summary>
        /// Predições de contexto futuro
        /// </summary>
        public ContextPrediction FuturePrediction { get; set; } = new ContextPrediction();
        #endregion

        #region Analysis Methods
        /// <summary>
        /// Calcula score geral de contexto baseado em múltiplos fatores
        /// </summary>
        public double CalculateOverallContextScore()
        {
            var scores = new List<double>
            {
                1.0 - Volatility, // Menor volatilidade = melhor score
                TrendStrength,    // Maior força de tendência = melhor
                1.0 - PatternComplexity, // Menor complexidade = melhor
                RegimeConfidence  // Maior confiança = melhor
            };

            return scores.Average();
        }

        /// <summary>
        /// Determina se o contexto é favorável para estratégias anti-frequencistas
        /// </summary>
        public bool IsFavorableForAntiFrequency()
        {
            return MetricasEstatisticas.GetValueOrDefault("DebtAccumulation", 0.0) > 0.6 ||
                   Volatility > 0.7 ||
                   DetectedRegime.Contains("Reversão") ||
                   MetricasEstatisticas.GetValueOrDefault("FrequencyImbalance", 0.0) > 0.5;
        }

        /// <summary>
        /// Determina se o contexto é favorável para modelos ensemble
        /// </summary>
        public bool IsFavorableForEnsemble()
        {
            return PatternComplexity > 0.6 ||
                   Volatility > 0.4 && Volatility < 0.8 ||
                   RecentPerformance.Values.Any() && RecentPerformance.Values.Max() - RecentPerformance.Values.Min() < 0.2;
        }

        /// <summary>
        /// Obtém recomendação de estratégia baseada no contexto
        /// </summary>
        public StrategyRecommendation GetStrategyRecommendation()
        {
            var recommendation = new StrategyRecommendation(
                name: Name,
                bestModel: "Conservative", // Valor padrão
                confidence: CalculateOverallContextScore(),
                rationale: "Contexto padrão"
            );

            // Lógica de recomendação baseada em contexto
            if (IsFavorableForAntiFrequency())
            {
                recommendation = new StrategyRecommendation(
                    name: Name,
                    bestModel: "Anti-Frequency",
                    confidence: CalculateOverallContextScore(),
                    rationale: "Alto potencial de reversão estatística detectado"
                );
            }
            else if (IsFavorableForEnsemble())
            {
                recommendation = new StrategyRecommendation(
                    name: Name,
                    bestModel: "Ensemble",
                    confidence: CalculateOverallContextScore(),
                    rationale: "Contexto complexo favorece combinação de modelos"
                );
            }
            else if (TrendStrength > 0.7)
            {
                recommendation = new StrategyRecommendation(
                    name: Name,
                    bestModel: "Trend-Following",
                    confidence: CalculateOverallContextScore(),
                    rationale: "Tendência forte detectada"
                );
            }

            return recommendation;
        }

        /// <summary>
        /// Atualiza contexto com novos dados
        /// </summary>
        public void UpdateContext(List<ConcursoResult> newData)
        {
            if (!newData.Any()) return;

            // Recalcular indicadores com novos dados
            UpdateStatisticalIndicators(newData);
            UpdateVolatilityMetrics(newData);
            UpdateTrendAnalysis(newData);
            UpdateRegimeDetection(newData);

            AnalysisTimestamp = DateTime.Now;
        }

        /// <summary>
        /// Compara similaridade com outro contexto
        /// </summary>
        public double CalculateSimilarity(ContextAnalysis other)
        {
            if (other == null) return 0.0;

            var similarities = new List<double>
            {
                1.0 - Math.Abs(Volatility - other.Volatility),
                1.0 - Math.Abs(TrendStrength - other.TrendStrength),
                1.0 - Math.Abs(PatternComplexity - other.PatternComplexity),
                DetectedRegime == other.DetectedRegime ? 1.0 : 0.5
            };

            return similarities.Average();
        }
        #endregion

        #region Helper Methods
        private void UpdateStatisticalIndicators(List<ConcursoResult> data)
        {
            if (!data.Any()) return;

            // Indicador de acumulação de dívida estatística
            var frequencies = CalculateFrequencies(data);
            var expectedFreq = 15.0 / 25.0;
            var debtSum = frequencies.Values.Sum(f => Math.Max(0, expectedFreq - f));
            MetricasEstatisticas["DebtAccumulation"] = Math.Min(1.0, debtSum / 5.0);

            // Indicador de desequilíbrio de frequência
            var maxFreq = frequencies.Values.Max();
            var minFreq = frequencies.Values.Min();
            MetricasEstatisticas["FrequencyImbalance"] = (maxFreq - minFreq) / maxFreq;

            // Indicador de entropia
            var entropy = CalculateEntropy(frequencies);
            MetricasEstatisticas["Entropy"] = entropy / Math.Log2(25); // Normalizado

            // Indicador de momentum
            if (data.Count >= 10)
            {
                var recentAvg = data.TakeLast(5).Average(d => d.DezenasOrdenadas.Average());
                var olderAvg = data.Skip(Math.Max(0, data.Count - 10)).Take(5).Average(d => d.DezenasOrdenadas.Average());
                MetricasEstatisticas["Momentum"] = (recentAvg - olderAvg) / 25.0;
            }
        }

        private void UpdateVolatilityMetrics(List<ConcursoResult> data)
        {
            if (data.Count < 5) return;

            var averages = data.Select(d => d.DezenasOrdenadas.Average()).ToList();
            var mean = averages.Average();
            var variance = averages.Sum(avg => Math.Pow(avg - mean, 2)) / averages.Count;
            
            Volatility = Math.Min(1.0, Math.Sqrt(variance) / 5.0); // Normalizado
        }

        private void UpdateTrendAnalysis(List<ConcursoResult> data)
        {
            if (data.Count < 3) return;

            var values = data.Select(d => d.DezenasOrdenadas.Average()).ToList();
            var trend = CalculateLinearTrend(values);
            TrendStrength = Math.Min(1.0, Math.Abs(trend) * 10.0); // Normalizado
        }

        private void UpdateRegimeDetection(List<ConcursoResult> data)
        {
            // Lógica simplificada de detecção de regime
            var vol = MetricasEstatisticas.GetValueOrDefault("FrequencyImbalance", 0.0);
            var debt = MetricasEstatisticas.GetValueOrDefault("DebtAccumulation", 0.0);
            var momentum = MetricasEstatisticas.GetValueOrDefault("Momentum", 0.0);

            if (debt > 0.6)
            {
                DetectedRegime = "Reversão Estatística";
                RegimeConfidence = debt;
            }
            else if (vol > 0.7)
            {
                DetectedRegime = "Alta Volatilidade";
                RegimeConfidence = vol;
            }
            else if (Math.Abs(momentum) > 0.3)
            {
                DetectedRegime = momentum > 0 ? "Tendência Ascendente" : "Tendência Descendente";
                RegimeConfidence = Math.Abs(momentum);
            }
            else
            {
                DetectedRegime = "Estável";
                RegimeConfidence = 1.0 - vol; // Estabilidade = baixa volatilidade
            }

            RegimeConfidence = Math.Min(1.0, RegimeConfidence);
        }

        private Dictionary<int, double> CalculateFrequencies(List<ConcursoResult> data)
        {
            var frequencies = new Dictionary<int, double>();
            var totalDraws = data.Count;

            for (int i = 1; i <= 25; i++)
            {
                var appearances = data.Count(d => d.DezenasOrdenadas.Contains(i));
                frequencies[i] = appearances / (double)totalDraws;
            }

            return frequencies;
        }

        private double CalculateEntropy(Dictionary<int, double> frequencies)
        {
            var entropy = 0.0;
            foreach (var freq in frequencies.Values)
            {
                if (freq > 0)
                    entropy -= freq * Math.Log2(freq);
            }
            return entropy;
        }

        private double CalculateLinearTrend(List<double> values)
        {
            if (values.Count < 2) return 0.0;

            var n = values.Count;
            var sumX = n * (n - 1) / 2;
            var sumY = values.Sum();
            var sumXY = values.Select((y, x) => x * y).Sum();
            var sumX2 = n * (n - 1) * (2 * n - 1) / 6;

            return (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Cria análise de contexto a partir de dados históricos
        /// </summary>
        public static ContextAnalysis CreateFromHistoricalData(List<ConcursoResult> data, string contextName = "")
        {
            var context = new ContextAnalysis
            {
                Name = string.IsNullOrEmpty(contextName) ? $"Context_{DateTime.Now:yyyyMMdd_HHmmss}" : contextName,
                AnalysisWindow = data.Count
            };

            context.UpdateContext(data);
            return context;
        }

        /// <summary>
        /// Cria contexto padrão para testes
        /// </summary>
        public static ContextAnalysis CreateDefault()
        {
            return new ContextAnalysis
            {
                Name = "Default",
                Volatility = 0.5,
                TrendStrength = 0.3,
                PatternComplexity = 0.4,
                DetectedRegime = "Neutro",
                RegimeConfidence = 0.6
            };
        }
        #endregion
    }

    #region Supporting Classes
    public class DiversificationMetrics
    {
        public double HerfindahlIndex { get; set; }
        public double Shannon { get; set; }
        public double Gini { get; set; }
        public double EffectiveNumberOfDezenas { get; set; }
    }

    public class TemporalContext
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalDraws { get; set; }
        public double DrawFrequency { get; set; }
        public bool HasSeasonality { get; set; }
        public double SeasonalityStrength { get; set; }
    }

    public class ContextPrediction
    {
        public double PredictedVolatility { get; set; }
        public double PredictedTrendStrength { get; set; }
        public string PredictedRegime { get; set; }
        public double Confidence { get; set; }
        public DateTime PredictionDate { get; set; }
    }

    #endregion
}
