// D:\PROJETOS\GraphFacil\Library\Suporte\ContextAnalyzer.cs - VERSÃO EXPANDIDA
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Utilities;
using LotoLibrary.Models.Core;

namespace LotoLibrary.Suporte
{
    /// <summary>
    /// Analisador de contexto robusto para meta-learning
    /// Versão expandida com análise detalhada de padrões e detecção de regimes
    /// </summary>
    public class ContextAnalyzer
    {
        #region Fields
        private readonly int _defaultAnalysisWindow = 50;
        private readonly double _volatilityThreshold = 0.15;
        private readonly double _trendThreshold = 0.1;
        private Dictionary<string, ContextAnalysis> _contextHistory;
        private List<string> _knownRegimes;
        #endregion

        #region Constructor
        public ContextAnalyzer()
        {
            _contextHistory = new Dictionary<string, ContextAnalysis>();
            InitializeKnownRegimes();
        }
        #endregion

        #region Main Analysis Methods
        /// <summary>
        /// Analisa contexto atual baseado nos dados históricos e concurso alvo
        /// </summary>
        public async Task<ContextAnalysis> AnalyzeCurrentContext(List<ConcursoResult> dados, int targetConcurso)
        {
            if (dados == null || !dados.Any())
            {
                return ContextAnalysis.CreateDefault();
            }

            var analysis = new ContextAnalysis
            {
                Name = $"Context_{targetConcurso}",
                AnalysisTimestamp = DateTime.Now,
                AnalysisWindow = Math.Min(_defaultAnalysisWindow, dados.Count)
            };

            var analysisData = dados.TakeLast(analysis.AnalysisWindow).ToList();

            // Análises paralelas para performance
            var tasks = new List<Task>
            {
                Task.Run(() => AnalyzeVolatility(analysis, analysisData)),
                Task.Run(() => AnalyzeTrends(analysis, analysisData)),
                Task.Run(() => AnalyzePatternComplexity(analysis, analysisData)),
                Task.Run(() => DetectCurrentRegime(analysis, analysisData)),
                Task.Run(() => AnalyzeStatisticalIndicators(analysis, analysisData)),
                Task.Run(() => AnalyzeDiversification(analysis, analysisData)),
                Task.Run(() => AnalyzeTemporalContext(analysis, analysisData))
            };

            await Task.WhenAll(tasks);

            analysis.RecentPerformance = await SimulateRecentModelPerformance(analysisData);
            analysis.FuturePrediction = PredictFutureContext(analysis, analysisData);
            _contextHistory[analysis.Name] = analysis;

            return analysis;
        }

        public async Task<ContextAnalysis> AnalyzeCurrentContext(List<Lance> dados, int targetConcurso)
        {
            var converted = ConvertToConcursoResults(dados);
            return await AnalyzeCurrentContext(converted, targetConcurso);
        }

        private List<ConcursoResult> ConvertToConcursoResults(List<Lance> dados)
        {
            return dados?.Select(d => new ConcursoResult {
                Concurso = d.Id,
                DezenasOrdenadas = d.Lista.OrderBy(x => x).ToList(),
                DataSorteio = DateTime.Now // Usar data atual como padrão
            }).ToList() ?? new List<ConcursoResult>();
        }

        /// <summary>
        /// Analisa contexto atual com dados Lance (compatibilidade)
        /// </summary>
        public async Task<ContextAnalysis> AnalyzeCurrentContext(Lances dados, int targetConcurso)
        {
            if (dados == null || !dados.Any())
            {
                return ContextAnalysis.CreateDefault();
            }

            var analysis = new ContextAnalysis
            {
                Name = $"Context_{targetConcurso}",
                AnalysisTimestamp = DateTime.Now,
                AnalysisWindow = Math.Min(_defaultAnalysisWindow, dados.Count)
            };

            // Usar janela de análise apropriada
            var analysisWindow = dados.TakeLast(analysis.AnalysisWindow).ToList();
            var analysisData = analysisWindow.Select(ConcursoResult.FromLance).ToList();

            // Análises paralelas para performance
            var tasks = new List<Task>
            {
                Task.Run(() => AnalyzeVolatility(analysis, analysisData)),
                Task.Run(() => AnalyzeTrends(analysis, analysisData)),
                Task.Run(() => AnalyzePatternComplexity(analysis, analysisData)),
                Task.Run(() => DetectCurrentRegime(analysis, analysisData)),
                Task.Run(() => AnalyzeStatisticalIndicators(analysis, analysisData)),
                Task.Run(() => AnalyzeDiversification(analysis, analysisData)),
                Task.Run(() => AnalyzeTemporalContext(analysis, analysisData))
            };

            await Task.WhenAll(tasks);

            // Análise de performance recente de modelos (simulada)
            analysis.RecentPerformance = await SimulateRecentModelPerformance(analysisData);

            // Predição de contexto futuro
            analysis.FuturePrediction = PredictFutureContext(analysis, analysisData);

            // Armazenar no histórico
            _contextHistory[analysis.Name] = analysis;

            return analysis;
        }

        /// <summary>
        /// Compara contexto atual com contextos históricos
        /// </summary>
        public async Task<List<ContextSimilarity>> FindSimilarContexts(ContextAnalysis currentContext, int maxResults = 5)
        {
            var similarities = new List<ContextSimilarity>();

            foreach (var historical in _contextHistory.Values)
            {
                if (historical.Name != currentContext.Name)
                {
                    var similarity = currentContext.CalculateSimilarity(historical);
                    similarities.Add(new ContextSimilarity
                    {
                        HistoricalContext = historical,
                        SimilarityScore = similarity,
                        CommonFactors = FindCommonFactors(currentContext, historical)
                    });
                }
            }

            return similarities.OrderByDescending(s => s.SimilarityScore)
                             .Take(maxResults)
                             .ToList();
        }

        /// <summary>
        /// Detecta mudanças de regime em tempo real
        /// </summary>
        public async Task<RegimeChangeResult> DetectRegimeChange(List<ConcursoResult> recentData, ContextAnalysis previousContext)
        {
            if (recentData.Count < 5 || previousContext == null)
            {
                return new RegimeChangeResult { HasChanged = false };
            }

            var currentAnalysis = await AnalyzeCurrentContext(recentData, recentData.Last().Concurso);
            
            var volatilityChange = Math.Abs(currentAnalysis.Volatility - previousContext.Volatility);
            var trendChange = Math.Abs(currentAnalysis.TrendStrength - previousContext.TrendStrength);
            var regimeChange = currentAnalysis.DetectedRegime != previousContext.DetectedRegime;

            var changeSignificance = CalculateChangeSignificance(currentAnalysis, previousContext);

            return new RegimeChangeResult
            {
                HasChanged = changeSignificance > 0.3 || regimeChange,
                ChangeSignificance = changeSignificance,
                PreviousRegime = previousContext.DetectedRegime,
                NewRegime = currentAnalysis.DetectedRegime,
                ChangeFactors = IdentifyChangeFactors(currentAnalysis, previousContext),
                Confidence = Math.Min(currentAnalysis.RegimeConfidence, previousContext.RegimeConfidence)
            };
        }
        #endregion

        #region Specific Analysis Methods
        private void AnalyzeVolatility(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            if (data.Count < 3) 
            {
                analysis.Volatility = 0.5;
                return;
            }

            // Análise de volatilidade baseada em múltiplos fatores
            var averages = data.Select(d => d.DezenasOrdenadas.Average()).ToList();
            var frequencies = CalculateFrequencyDistribution(data);
            
            // Volatilidade de médias
            var avgVolatility = StatisticalMath.CalculateStandardDeviation(averages) / 25.0;
            
            // Volatilidade de frequências
            var freqVolatility = frequencies.Values.Select(f => Math.Abs(f - 0.6)).Average();
            
            // Volatilidade de padrões espaciais
            var spatialVolatility = CalculateSpatialVolatility(data);
            
            // Combinar medidas de volatilidade
            analysis.Volatility = Math.Min(1.0, (avgVolatility + freqVolatility + spatialVolatility) / 3.0);
            
            // Adicionar contexto detalhado
            analysis.MetricasEstatisticas["AvgVolatility"] = avgVolatility;
            analysis.MetricasEstatisticas["FreqVolatility"] = freqVolatility;
            analysis.MetricasEstatisticas["SpatialVolatility"] = spatialVolatility;
        }

        private void AnalyzeTrends(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            if (data.Count < 5)
            {
                analysis.TrendStrength = 0.3;
                return;
            }

            var frequencies = CalculateRollingFrequencies(data, 10);
            var trendScores = new List<double>();

            foreach (var dezena in frequencies.Keys)
            {
                var values = frequencies[dezena];
                if (values.Count >= 3)
                {
                    var trend = StatisticalMath.CalculateMomentum(values, Math.Min(5, values.Count - 1));
                    trendScores.Add(Math.Abs(trend));
                }
            }

            analysis.TrendStrength = trendScores.Any() ? Math.Min(1.0, trendScores.Average() * 5.0) : 0.3;
            analysis.MetricasEstatisticas["TrendConsistency"] = CalculateTrendConsistency(frequencies);
        }

        private void AnalyzePatternComplexity(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            if (data.Count < 5)
            {
                analysis.PatternComplexity = 0.4;
                return;
            }

            // Múltiplas medidas de complexidade
            var entropyComplexity = CalculateEntropyComplexity(data);
            var sequentialComplexity = CalculateSequentialComplexity(data);
            var distributionalComplexity = CalculateDistributionalComplexity(data);
            var temporalComplexity = CalculateTemporalComplexity(data);

            analysis.PatternComplexity = Math.Min(1.0, (entropyComplexity + sequentialComplexity + 
                                                       distributionalComplexity + temporalComplexity) / 4.0);

            analysis.MetricasEstatisticas["EntropyComplexity"] = entropyComplexity;
            analysis.MetricasEstatisticas["SequentialComplexity"] = sequentialComplexity;
            analysis.MetricasEstatisticas["DistributionalComplexity"] = distributionalComplexity;
            analysis.MetricasEstatisticas["TemporalComplexity"] = temporalComplexity;
        }

        private void DetectCurrentRegime(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            var regimeScores = new Dictionary<string, double>();

            foreach (var regime in _knownRegimes)
            {
                regimeScores[regime] = CalculateRegimeScore(regime, analysis, data);
            }

            var bestRegime = regimeScores.OrderByDescending(kvp => kvp.Value).First();
            analysis.DetectedRegime = bestRegime.Key;
            analysis.RegimeConfidence = bestRegime.Value;

            // Adicionar fatores contextuais específicos do regime
            analysis.ContextualFactors.AddRange(GetRegimeFactors(bestRegime.Key, analysis));
        }

        private void AnalyzeStatisticalIndicators(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            var frequencies = CalculateFrequencyDistribution(data);
            var expectedFreq = 0.6; // 15/25

            // Dívida estatística acumulada
            var totalDebt = frequencies.Values.Sum(f => Math.Max(0, expectedFreq - f));
            analysis.MetricasEstatisticas["StatisticalDebt"] = Math.Min(1.0, totalDebt / 5.0);

            // Desequilíbrio de frequência
            var maxFreq = frequencies.Values.Max();
            var minFreq = frequencies.Values.Min();
            analysis.MetricasEstatisticas["FrequencyImbalance"] = (maxFreq - minFreq) / maxFreq;

            // Entropia normalizada
            var entropy = StatisticalMath.CalculateEntropy(frequencies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            analysis.MetricasEstatisticas["NormalizedEntropy"] = entropy / Math.Log2(25);

            // Momentum geral
            if (data.Count >= 10)
            {
                var recentAvg = data.TakeLast(5).Average(d => d.DezenasOrdenadas.Average());
                var olderAvg = data.Skip(Math.Max(0, data.Count - 10)).Take(5).Average(d => d.DezenasOrdenadas.Average());
                analysis.MetricasEstatisticas["OverallMomentum"] = (recentAvg - olderAvg) / 25.0;
            }

            // Índice de dispersão
            var dispersionIndex = CalculateDispersionIndex(data);
            analysis.MetricasEstatisticas["DispersionIndex"] = dispersionIndex;
        }

        private void AnalyzeDiversification(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            var frequencies = CalculateFrequencyDistribution(data);
            
            // Índice Herfindahl-Hirschman
            analysis.Diversification.HerfindahlIndex = StatisticalMath.CalculateHHI(frequencies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            
            // Índice de Gini
            analysis.Diversification.Gini = StatisticalMath.CalculateGiniCoefficient(frequencies.Values.ToList());
            
            // Entropia de Shannon
            analysis.Diversification.Shannon = StatisticalMath.CalculateEntropy(frequencies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            
            // Número efetivo de dezenas (baseado na entropia)
            analysis.Diversification.EffectiveNumberOfDezenas = Math.Exp(analysis.Diversification.Shannon);
        }

        private void AnalyzeTemporalContext(ContextAnalysis analysis, List<ConcursoResult> data)
        {
            if (!data.Any()) return;

            analysis.TemporalInfo.PeriodStart = data.First().DataSorteio;
            analysis.TemporalInfo.PeriodEnd = data.Last().DataSorteio;
            analysis.TemporalInfo.TotalDraws = data.Count;

            if (data.Count > 1)
            {
                var totalDays = (analysis.TemporalInfo.PeriodEnd - analysis.TemporalInfo.PeriodStart).TotalDays;
                analysis.TemporalInfo.DrawFrequency = totalDays / data.Count;
            }

            // Análise de sazonalidade usando TemporalAnalysis
            var historicalData = data.Select(d => (d.DataSorteio, d.DezenasOrdenadas)).ToList();
            var seasonality = TemporalAnalysis.AnalyzeSeasonality(historicalData);
            
            analysis.TemporalInfo.HasSeasonality = seasonality.HasSeasonality;
            analysis.TemporalInfo.SeasonalityStrength = seasonality.SeasonalityStrength;
        }
        #endregion

        #region Helper Methods
        private List<Lance> ConvertToLances(List<ConcursoResult> dados)
        {
            return dados.Select(d => new Lance(d.Concurso, d.DezenasOrdenadas)).ToList();
        }

        private Dictionary<int, double> CalculateFrequencyDistribution(List<ConcursoResult> data)
        {
            return FrequencyCalculations.CalculateFrequencies(data.Select(d => d.DezenasOrdenadas));
        }

        private Dictionary<int, List<double>> CalculateRollingFrequencies(List<ConcursoResult> data, int windowSize)
        {
            var rollingFreqs = new Dictionary<int, List<double>>();
            
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                rollingFreqs[dezena] = new List<double>();
                
                for (int i = windowSize; i <= data.Count; i++)
                {
                    var window = data.Skip(i - windowSize).Take(windowSize);
                    var freq = window.Count(d => d.DezenasOrdenadas.Contains(dezena)) / (double)windowSize;
                    rollingFreqs[dezena].Add(freq);
                }
            }
            
            return rollingFreqs;
        }

        private double CalculateSpatialVolatility(List<ConcursoResult> data)
        {
            var gaps = data.Select(d => CalculateAverageGap(d.DezenasOrdenadas)).ToList();
            return StatisticalMath.CalculateStandardDeviation(gaps) / 25.0;
        }

        private double CalculateAverageGap(List<int> dezenas)
        {
            if (dezenas.Count < 2) return 0.0;
            
            var sorted = dezenas.OrderBy(x => x).ToList();
            var gaps = new List<int>();
            
            for (int i = 1; i < sorted.Count; i++)
            {
                gaps.Add(sorted[i] - sorted[i - 1]);
            }
            
            return gaps.Average();
        }

        private double CalculateEntropyComplexity(List<ConcursoResult> data)
        {
            var frequencies = CalculateFrequencyDistribution(data);
            var entropy = StatisticalMath.CalculateEntropy(frequencies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            return entropy / Math.Log2(25); // Normalizado
        }

        private double CalculateSequentialComplexity(List<ConcursoResult> data)
        {
            var consecutivePatterns = new List<int>();
            
            foreach (var result in data)
            {
                var sorted = result.DezenasOrdenadas.OrderBy(x => x).ToList();
                var consecutiveCount = 0;
                
                for (int i = 1; i < sorted.Count; i++)
                {
                    if (sorted[i] == sorted[i - 1] + 1)
                        consecutiveCount++;
                }
                
                consecutivePatterns.Add(consecutiveCount);
            }
            
            return StatisticalMath.CalculateStandardDeviation(consecutivePatterns.Select(p => (double)p).ToList()) / 15.0;
        }

        private double CalculateDistributionalComplexity(List<ConcursoResult> data)
        {
            var distributions = data.Select(d => new
            {
                Low = d.DezenasOrdenadas.Count(x => x <= 8),
                Mid = d.DezenasOrdenadas.Count(x => x >= 9 && x <= 17),
                High = d.DezenasOrdenadas.Count(x => x >= 18)
            }).ToList();
            
            var lowVar = StatisticalMath.CalculateStandardDeviation(distributions.Select(d => (double)d.Low).ToList());
            var midVar = StatisticalMath.CalculateStandardDeviation(distributions.Select(d => (double)d.Mid).ToList());
            var highVar = StatisticalMath.CalculateStandardDeviation(distributions.Select(d => (double)d.High).ToList());
            
            return (lowVar + midVar + highVar) / 15.0;
        }

        private double CalculateTemporalComplexity(List<ConcursoResult> data)
        {
            if (data.Count < 3) return 0.3;
            
            var changes = new List<double>();
            
            for (int i = 1; i < data.Count; i++)
            {
                var intersection = data[i].DezenasOrdenadas.Intersect(data[i - 1].DezenasOrdenadas).Count();
                changes.Add(15 - intersection); // Número de mudanças
            }
            
            return StatisticalMath.CalculateStandardDeviation(changes) / 15.0;
        }

        private double CalculateRegimeScore(string regime, ContextAnalysis analysis, List<ConcursoResult> data)
        {
            return regime switch
            {
                "Tendência Forte" => analysis.TrendStrength,
                "Reversão Estatística" => analysis.MetricasEstatisticas.GetValueOrDefault("StatisticalDebt", 0.0),
                "Alta Volatilidade" => analysis.Volatility,
                "Estável" => 1.0 - analysis.Volatility,
                "Padrão Complexo" => analysis.PatternComplexity,
                _ => 0.5
            };
        }

        private List<string> GetRegimeFactors(string regime, ContextAnalysis analysis)
        {
            var factors = new List<string>();
            
            switch (regime)
            {
                case "Tendência Forte":
                    factors.Add($"Força da tendência: {analysis.TrendStrength:P1}");
                    factors.Add("Momentum consistente detectado");
                    break;
                case "Reversão Estatística":
                    factors.Add($"Dívida estatística: {analysis.MetricasEstatisticas.GetValueOrDefault("StatisticalDebt", 0.0):P1}");
                    factors.Add("Pressão para normalização");
                    break;
                case "Alta Volatilidade":
                    factors.Add($"Volatilidade: {analysis.Volatility:P1}");
                    factors.Add("Padrões instáveis");
                    break;
            }
            
            return factors;
        }

        private double CalculateTrendConsistency(Dictionary<int, List<double>> frequencies)
        {
            var consistencyScores = new List<double>();
            
            foreach (var freqSeries in frequencies.Values)
            {
                if (freqSeries.Count >= 3)
                {
                    var trends = new List<double>();
                    for (int i = 2; i < freqSeries.Count; i++)
                    {
                        trends.Add(freqSeries[i] - freqSeries[i - 1]);
                    }
                    
                    var consistency = 1.0 - StatisticalMath.CalculateStandardDeviation(trends);
                    consistencyScores.Add(Math.Max(0.0, consistency));
                }
            }
            
            return consistencyScores.Any() ? consistencyScores.Average() : 0.5;
        }

        private double CalculateDispersionIndex(List<ConcursoResult> data)
        {
            var allDezenas = data.SelectMany(d => d.DezenasOrdenadas).ToList();
            var variance = StatisticalMath.CalculateStandardDeviation(allDezenas.Select(d => (double)d).ToList());
            var mean = allDezenas.Average();
            
            return variance / mean; // Índice de dispersão
        }

        private double CalculateChangeSignificance(ContextAnalysis current, ContextAnalysis previous)
        {
            var changes = new List<double>
            {
                Math.Abs(current.Volatility - previous.Volatility),
                Math.Abs(current.TrendStrength - previous.TrendStrength),
                Math.Abs(current.PatternComplexity - previous.PatternComplexity),
                Math.Abs(current.RegimeConfidence - previous.RegimeConfidence)
            };
            
            return changes.Average();
        }

        private List<string> IdentifyChangeFactors(ContextAnalysis current, ContextAnalysis previous)
        {
            var factors = new List<string>();
            
            if (Math.Abs(current.Volatility - previous.Volatility) > 0.2)
                factors.Add($"Mudança significativa na volatilidade ({previous.Volatility:P1} → {current.Volatility:P1})");
                
            if (Math.Abs(current.TrendStrength - previous.TrendStrength) > 0.2)
                factors.Add($"Mudança na força da tendência ({previous.TrendStrength:P1} → {current.TrendStrength:P1})");
                
            if (current.DetectedRegime != previous.DetectedRegime)
                factors.Add($"Mudança de regime ({previous.DetectedRegime} → {current.DetectedRegime})");
            
            return factors;
        }

        private List<string> FindCommonFactors(ContextAnalysis context1, ContextAnalysis context2)
        {
            var commonFactors = new List<string>();
            
            if (context1.DetectedRegime == context2.DetectedRegime)
                commonFactors.Add($"Mesmo regime: {context1.DetectedRegime}");
                
            if (Math.Abs(context1.Volatility - context2.Volatility) < 0.1)
                commonFactors.Add("Volatilidade similar");
                
            if (Math.Abs(context1.TrendStrength - context2.TrendStrength) < 0.1)
                commonFactors.Add("Força de tendência similar");
            
            return commonFactors;
        }

        private async Task<Dictionary<string, double>> SimulateRecentModelPerformance(List<ConcursoResult> data)
        {
            // Simulação de performance recente de modelos
            return await Task.FromResult(new Dictionary<string, double>
            {
                ["MetronomoModel"] = 0.62 + (data.Count * 0.001),
                ["AntiFrequencySimpleModel"] = 0.65 + (data.LastOrDefault()?.MetricasEstatisticas.GetValueOrDefault("StatisticalDebt", 0.0) ?? 0.0) * 0.1,
                ["StatisticalDebtModel"] = 0.68 + (data.LastOrDefault()?.MetricasEstatisticas.GetValueOrDefault("FrequencyImbalance", 0.0) ?? 0.0) * 0.05,
                ["EnsembleModel"] = 0.71
            });
        }

        private ContextPrediction PredictFutureContext(ContextAnalysis current, List<ConcursoResult> data)
        {
            return new ContextPrediction
            {
                PredictedVolatility = Math.Max(0.1, Math.Min(0.9, current.Volatility + 
                    (current.MetricasEstatisticas.GetValueOrDefault("OverallMomentum", 0.0) * 0.1))),
                PredictedTrendStrength = Math.Max(0.1, current.TrendStrength * 0.9), // Tendências decaem
                PredictedRegime = current.DetectedRegime,
                Confidence = current.RegimeConfidence * 0.8, // Confiança decresce com tempo
                PredictionDate = DateTime.Now.AddDays(7)
            };
        }

        private void InitializeKnownRegimes()
        {
            _knownRegimes = new List<string>
            {
                "Tendência Forte",
                "Reversão Estatística", 
                "Alta Volatilidade",
                "Estável",
                "Padrão Complexo"
            };
        }
        #endregion
    }

    #region Supporting Classes
    public class ContextSimilarity
    {
        public ContextAnalysis HistoricalContext { get; set; }
        public double SimilarityScore { get; set; }
        public List<string> CommonFactors { get; set; } = new List<string>();
    }

    public class RegimeChangeResult
    {
        public bool HasChanged { get; set; }
        public double ChangeSignificance { get; set; }
        public string PreviousRegime { get; set; }
        public string NewRegime { get; set; }
        public List<string> ChangeFactors { get; set; } = new List<string>();
        public double Confidence { get; set; }
    }
    #endregion
}
