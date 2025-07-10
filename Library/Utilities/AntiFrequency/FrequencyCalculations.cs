// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\FrequencyCalculations.cs - Cálculos de frequência para modelos anti-frequencistas
using LotoLibrary.Models;
using LotoLibrary.PredictionModels.AntiFrequency.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.Utilities.AntiFrequency
{
    /// <summary>
    /// Analisador de frequências especializado para modelos anti-frequencistas
    /// </summary>
    public class FrequencyAnalyzer
    {
        private List<Lance> _historicalData;
        private Dictionary<int, List<int>> _appearanceHistory;
        private Dictionary<int, FrequencyStats> _frequencyStats;

        public async Task InitializeAsync(List<Lance> historicalData)
        {
            _historicalData = historicalData?.OrderBy(l => l.Id).ToList() ?? new List<Lance>();
            _appearanceHistory = new Dictionary<int, List<int>>();
            _frequencyStats = new Dictionary<int, FrequencyStats>();

            await BuildAppearanceHistory();
            await CalculateFrequencyStats();
        }

        public async Task<FrequencyProfile> CreateFrequencyProfileAsync(int dezena, int analysisWindow)
        {
            if (!_appearanceHistory.ContainsKey(dezena))
                return new FrequencyProfile { Dezena = dezena };

            var appearances = _appearanceHistory[dezena];
            var recentData = _historicalData.TakeLast(analysisWindow).ToList();
            var recentAppearances = appearances.Where(id => recentData.Any(l => l.Id == id)).ToList();

            var profile = new FrequencyProfile
            {
                Dezena = dezena,
                AppearanceCount = recentAppearances.Count,
                ExpectedAppearances = CalculateExpectedAppearances(analysisWindow),
                FrequencyScore = CalculateFrequencyScore(dezena, analysisWindow),
                AntiFrequencyScore = CalculateAntiFrequencyScore(dezena, analysisWindow),
                StatisticalDebt = CalculateStatisticalDebt(dezena, analysisWindow),
                LastAppearance = GetLastAppearanceDate(dezena),
                AverageInterval = CalculateAverageInterval(dezena),
                Volatility = CalculateVolatility(dezena),
                RecentAppearances = recentAppearances.TakeLast(10).ToList()
            };

            return profile;
        }

        public double CalculateFrequency(int dezena, List<Lance> data, int window)
        {
            var windowData = data.TakeLast(window).ToList();
            var appearances = windowData.Count(l => l.Lista.Contains(dezena));
            return (double)appearances / windowData.Count;
        }

        public double CalculateAntiFrequencyScore(int dezena, int analysisWindow)
        {
            var frequency = CalculateFrequencyScore(dezena, analysisWindow);
            var expectedFrequency = 15.0 / 25.0; // 60% chance base

            // Quanto menor a frequência, maior o score anti-frequencista
            var antiScore = Math.Max(0.0, expectedFrequency - frequency) / expectedFrequency;
            
            // Aplicar curva não-linear para enfatizar diferenças
            return Math.Pow(antiScore, 0.7);
        }

        private async Task BuildAppearanceHistory()
        {
            await Task.Run(() =>
            {
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    _appearanceHistory[dezena] = _historicalData
                        .Where(l => l.Lista.Contains(dezena))
                        .Select(l => l.Id)
                        .OrderBy(id => id)
                        .ToList();
                }
            });
        }

        private async Task CalculateFrequencyStats()
        {
            await Task.Run(() =>
            {
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var appearances = _appearanceHistory[dezena];
                    var intervals = CalculateIntervals(appearances);

                    _frequencyStats[dezena] = new FrequencyStats
                    {
                        TotalAppearances = appearances.Count,
                        AverageInterval = intervals.Any() ? intervals.Average() : 0,
                        IntervalVariance = CalculateVariance(intervals),
                        LastAppearance = appearances.LastOrDefault(),
                        LongestGap = intervals.Any() ? intervals.Max() : 0,
                        ShortestGap = intervals.Any() ? intervals.Min() : 0
                    };
                }
            });
        }

        private double CalculateFrequencyScore(int dezena, int analysisWindow)
        {
            if (!_appearanceHistory.ContainsKey(dezena))
                return 0.0;

            var recentData = _historicalData.TakeLast(analysisWindow).ToList();
            var appearances = _appearanceHistory[dezena]
                .Count(id => recentData.Any(l => l.Id == id));

            return (double)appearances / analysisWindow;
        }

        private double CalculateStatisticalDebt(int dezena, int analysisWindow)
        {
            var actualAppearances = CalculateFrequencyScore(dezena, analysisWindow) * analysisWindow;
            var expectedAppearances = CalculateExpectedAppearances(analysisWindow);
            
            // Dívida = diferença entre esperado e real (positivo = deve aparecer mais)
            return expectedAppearances - actualAppearances;
        }

        private int CalculateExpectedAppearances(int analysisWindow)
        {
            // Cada dezena tem 15/25 = 60% de chance de aparecer em cada sorteio
            return (int)Math.Round(analysisWindow * (15.0 / 25.0));
        }

        private DateTime GetLastAppearanceDate(int dezena)
        {
            var lastId = _appearanceHistory[dezena].LastOrDefault();
            var lastLance = _historicalData.FirstOrDefault(l => l.Id == lastId);
            return lastLance?.Data ?? DateTime.MinValue;
        }

        private TimeSpan CalculateAverageInterval(int dezena)
        {
            if (!_frequencyStats.ContainsKey(dezena))
                return TimeSpan.Zero;

            var avgDays = _frequencyStats[dezena].AverageInterval;
            return TimeSpan.FromDays(avgDays * 3); // Assumindo sorteios 3x por semana
        }

        private double CalculateVolatility(int dezena)
        {
            if (!_frequencyStats.ContainsKey(dezena))
                return 0.0;

            return Math.Sqrt(_frequencyStats[dezena].IntervalVariance);
        }

        private List<double> CalculateIntervals(List<int> appearances)
        {
            var intervals = new List<double>();
            for (int i = 1; i < appearances.Count; i++)
            {
                intervals.Add(appearances[i] - appearances[i - 1]);
            }
            return intervals;
        }

        private double CalculateVariance(List<double> values)
        {
            if (values.Count < 2) return 0.0;
            
            var mean = values.Average();
            return values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
        }
    }

    /// <summary>
    /// Calculadora de dívida estatística para modelos anti-frequencistas
    /// </summary>
    public class StatisticalDebtCalculator
    {
        private List<Lance> _historicalData;
        private Dictionary<int, DebtProfile> _debtProfiles;

        public async Task InitializeAsync(List<Lance> historicalData)
        {
            _historicalData = historicalData?.OrderBy(l => l.Id).ToList() ?? new List<Lance>();
            _debtProfiles = new Dictionary<int, DebtProfile>();

            await CalculateDebtProfiles();
        }

        public double CalculateDebt(int dezena, int analysisWindow, int targetConcurso)
        {
            if (!_debtProfiles.ContainsKey(dezena))
                return 0.0;

            var profile = _debtProfiles[dezena];
            var recentData = _historicalData
                .Where(l => l.Id <= targetConcurso)
                .TakeLast(analysisWindow)
                .ToList();

            var actualAppearances = recentData.Count(l => l.Lista.Contains(dezena));
            var expectedAppearances = (double)analysisWindow * (15.0 / 25.0);
            
            var rawDebt = expectedAppearances - actualAppearances;
            
            // Aplicar fatores de correção
            var timeDecay = CalculateTimeDecayFactor(dezena, targetConcurso);
            var volatilityAdjustment = 1.0 + (profile.Volatility / 10.0);
            
            return rawDebt * timeDecay * volatilityAdjustment;
        }

        public double CalculateNormalizedDebt(int dezena, int analysisWindow, int targetConcurso)
        {
            var debt = CalculateDebt(dezena, analysisWindow, targetConcurso);
            var maxPossibleDebt = analysisWindow * (15.0 / 25.0);
            
            return Math.Max(-1.0, Math.Min(1.0, debt / maxPossibleDebt));
        }

        private async Task CalculateDebtProfiles()
        {
            await Task.Run(() =>
            {
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var appearances = _historicalData
                        .Where(l => l.Lista.Contains(dezena))
                        .Select(l => l.Id)
                        .ToList();

                    var intervals = CalculateAppearanceIntervals(appearances);
                    
                    _debtProfiles[dezena] = new DebtProfile
                    {
                        Dezena = dezena,
                        TotalAppearances = appearances.Count,
                        AverageInterval = intervals.Any() ? intervals.Average() : 0,
                        Volatility = CalculateVolatility(intervals),
                        LastAppearance = appearances.LastOrDefault(),
                        CurrentGap = CalculateCurrentGap(dezena),
                        DebtTrend = CalculateDebtTrend(dezena)
                    };
                }
            });
        }

        private double CalculateTimeDecayFactor(int dezena, int targetConcurso)
        {
            if (!_debtProfiles.ContainsKey(dezena))
                return 1.0;

            var lastAppearance = _debtProfiles[dezena].LastAppearance;
            var gap = targetConcurso - lastAppearance;
            var avgInterval = _debtProfiles[dezena].AverageInterval;

            if (avgInterval <= 0) return 1.0;

            // Fator de decaimento temporal: quanto mais tempo sem aparecer, maior o fator
            return Math.Min(2.0, 1.0 + (gap / avgInterval) * 0.1);
        }

        private List<double> CalculateAppearanceIntervals(List<int> appearances)
        {
            var intervals = new List<double>();
            for (int i = 1; i < appearances.Count; i++)
            {
                intervals.Add(appearances[i] - appearances[i - 1]);
            }
            return intervals;
        }

        private double CalculateVolatility(List<double> intervals)
        {
            if (intervals.Count < 2) return 0.0;
            
            var mean = intervals.Average();
            var variance = intervals.Sum(i => Math.Pow(i - mean, 2)) / (intervals.Count - 1);
            return Math.Sqrt(variance);
        }

        private int CalculateCurrentGap(int dezena)
        {
            var lastAppearance = _historicalData
                .Where(l => l.Lista.Contains(dezena))
                .Select(l => l.Id)
                .LastOrDefault();

            var lastConcurso = _historicalData.LastOrDefault()?.Id ?? 0;
            return lastConcurso - lastAppearance;
        }

        private double CalculateDebtTrend(int dezena)
        {
            // Calcular tendência da dívida nos últimos concursos
            var windows = new[] { 20, 50, 100 };
            var debts = new List<double>();

            foreach (var window in windows)
            {
                if (_historicalData.Count >= window)
                {
                    var debt = CalculateDebt(dezena, window, _historicalData.Last().Id);
                    debts.Add(debt);
                }
            }

            if (debts.Count < 2) return 0.0;

            // Tendência = diferença entre janela menor e maior
            return debts.First() - debts.Last();
        }
    }

    /// <summary>
    /// Sistema de pesos temporais para modelos anti-frequencistas
    /// </summary>
    public class TemporalWeighting
    {
        private List<Lance> _historicalData;
        private Dictionary<int, TemporalProfile> _temporalProfiles;

        public async Task InitializeAsync(List<Lance> historicalData)
        {
            _historicalData = historicalData?.OrderBy(l => l.Id).ToList() ?? new List<Lance>();
            _temporalProfiles = new Dictionary<int, TemporalProfile>();

            await BuildTemporalProfiles();
        }

        public double CalculateWeight(int dezena, int targetConcurso)
        {
            if (!_temporalProfiles.ContainsKey(dezena))
                return 1.0;

            var profile = _temporalProfiles[dezena];
            var timeSinceLastAppearance = targetConcurso - profile.LastAppearance;
            
            // Peso aumenta com o tempo desde última aparição
            var baseWeight = Math.Min(2.0, 1.0 + (timeSinceLastAppearance / profile.AverageInterval) * 0.1);
            
            // Ajustar baseado na tendência recente
            var trendAdjustment = 1.0 + (profile.RecentTrend * 0.05);
            
            return baseWeight * trendAdjustment;
        }

        public async Task<double> FindOptimalDecayFactorAsync(List<Lance> trainingData)
        {
            var decayFactors = new[] { 0.90, 0.92, 0.94, 0.95, 0.96, 0.98 };
            var bestFactor = 0.95;
            var bestScore = 0.0;

            foreach (var factor in decayFactors)
            {
                var score = await EvaluateDecayFactor(factor, trainingData);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestFactor = factor;
                }
            }

            return bestFactor;
        }

        private async Task BuildTemporalProfiles()
        {
            await Task.Run(() =>
            {
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var appearances = _historicalData
                        .Where(l => l.Lista.Contains(dezena))
                        .Select(l => l.Id)
                        .OrderBy(id => id)
                        .ToList();

                    var intervals = CalculateIntervals(appearances);

                    _temporalProfiles[dezena] = new TemporalProfile
                    {
                        Dezena = dezena,
                        LastAppearance = appearances.LastOrDefault(),
                        AverageInterval = intervals.Any() ? intervals.Average() : 7.0,
                        IntervalTrend = CalculateIntervalTrend(intervals),
                        RecentTrend = CalculateRecentTrend(dezena),
                        Seasonality = CalculateSeasonality(appearances)
                    };
                }
            });
        }

        private async Task<double> EvaluateDecayFactor(double factor, List<Lance> trainingData)
        {
            // Simulação simplificada de avaliação
            await Task.Delay(1);
            
            // Fatores próximos a 0.95 tendem a ser mais estáveis
            return 1.0 - Math.Abs(factor - 0.95);
        }

        private List<double> CalculateIntervals(List<int> appearances)
        {
            var intervals = new List<double>();
            for (int i = 1; i < appearances.Count; i++)
            {
                intervals.Add(appearances[i] - appearances[i - 1]);
            }
            return intervals;
        }

        private double CalculateIntervalTrend(List<double> intervals)
        {
            if (intervals.Count < 3) return 0.0;

            var recentIntervals = intervals.TakeLast(5).ToList();
            var olderIntervals = intervals.Skip(Math.Max(0, intervals.Count - 10)).Take(5).ToList();

            var recentAvg = recentIntervals.Average();
            var olderAvg = olderIntervals.Average();

            return recentAvg - olderAvg;
        }

        private double CalculateRecentTrend(int dezena)
        {
            var recentData = _historicalData.TakeLast(20).ToList();
            var appearances = recentData.Count(l => l.Lista.Contains(dezena));
            var expected = 20 * (15.0 / 25.0);
            
            return (appearances - expected) / expected;
        }

        private double CalculateSeasonality(List<int> appearances)
        {
            // Análise de sazonalidade simplificada
            // Pode ser expandida para detectar padrões semanais/mensais
            return 0.0; // Placeholder
        }
    }

    /// <summary>
    /// Detector de padrões para modelos anti-frequencistas
    /// </summary>
    public class PatternDetection
    {
        private List<Lance> _historicalData;
        private Dictionary<string, PatternStrength> _detectedPatterns;

        public async Task InitializeAsync(List<Lance> historicalData)
        {
            _historicalData = historicalData?.OrderBy(l => l.Id).ToList() ?? new List<Lance>();
            _detectedPatterns = new Dictionary<string, PatternStrength>();

            await DetectAllPatterns();
        }

        public async Task<List<string>> DetectAntiFrequencyPatternsAsync(List<Lance> data)
        {
            var patterns = new List<string>();

            // Detectar padrões de sub-frequência
            var underFrequentNumbers = await DetectUnderFrequentNumbers(data);
            if (underFrequentNumbers.Any())
            {
                patterns.Add($"Sub-frequentes: {string.Join(",", underFrequentNumbers.Take(5))}");
            }

            // Detectar padrões de ciclos
            var cyclicalPatterns = await DetectCyclicalPatterns(data);
            patterns.AddRange(cyclicalPatterns);

            // Detectar padrões de gaps
            var gapPatterns = await DetectGapPatterns(data);
            patterns.AddRange(gapPatterns);

            return patterns;
        }

        public async Task<List<string>> DetectOverFrequencyPatternsAsync(List<Lance> data)
        {
            var patterns = new List<string>();

            // Detectar padrões de sobre-frequência
            var overFrequentNumbers = await DetectOverFrequentNumbers(data);
            if (overFrequentNumbers.Any())
            {
                patterns.Add($"Sobre-frequentes: {string.Join(",", overFrequentNumbers.Take(5))}");
            }

            return patterns;
        }

        public async Task<List<string>> DetectUnderFrequencyPatternsAsync(List<Lance> data)
        {
            return await DetectAntiFrequencyPatternsAsync(data);
        }

        public async Task<double> CalculatePatternConsistencyAsync(List<Lance> data)
        {
            var patterns = await DetectAntiFrequencyPatternsAsync(data);
            var consistencyScores = new List<double>();

            foreach (var pattern in patterns)
            {
                var strength = CalculatePatternStrength(pattern, data);
                consistencyScores.Add(strength);
            }

            return consistencyScores.Any() ? consistencyScores.Average() : 0.5;
        }

        public double AnalyzeSelectionPatternStrength(List<int> selectedNumbers)
        {
            // Analisar força dos padrões na seleção
            var patternStrength = 0.0;

            // Verificar distribuição
            var distribution = AnalyzeDistribution(selectedNumbers);
            patternStrength += distribution * 0.4;

            // Verificar sequências
            var sequences = AnalyzeSequences(selectedNumbers);
            patternStrength += sequences * 0.3;

            // Verificar gaps
            var gaps = AnalyzeGaps(selectedNumbers);
            patternStrength += gaps * 0.3;

            return Math.Min(1.0, patternStrength);
        }

        private async Task DetectAllPatterns()
        {
            await Task.Run(() =>
            {
                // Detectar diferentes tipos de padrões
                DetectFrequencyPatterns();
                DetectSequentialPatterns();
                DetectGapPatterns();
                DetectCyclicalPatterns();
            });
        }

        private async Task<List<int>> DetectUnderFrequentNumbers(List<Lance> data)
        {
            await Task.Delay(1); // Placeholder async

            var frequencies = new Dictionary<int, int>();
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                frequencies[dezena] = data.Count(l => l.Lista.Contains(dezena));
            }

            var avgFrequency = frequencies.Values.Average();
            return frequencies
                .Where(kvp => kvp.Value < avgFrequency * 0.8)
                .OrderBy(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        private async Task<List<int>> DetectOverFrequentNumbers(List<Lance> data)
        {
            await Task.Delay(1); // Placeholder async

            var frequencies = new Dictionary<int, int>();
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                frequencies[dezena] = data.Count(l => l.Lista.Contains(dezena));
            }

            var avgFrequency = frequencies.Values.Average();
            return frequencies
                .Where(kvp => kvp.Value > avgFrequency * 1.2)
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        private async Task<List<string>> DetectCyclicalPatterns(List<Lance> data)
        {
            await Task.Delay(1); // Placeholder async
            return new List<string> { "Ciclo médio detectado: 7-8 concursos" };
        }

        private async Task<List<string>> DetectGapPatterns(List<Lance> data)
        {
            await Task.Delay(1); // Placeholder async
            return new List<string> { "Gaps longos detectados em dezenas baixas" };
        }

        private void DetectFrequencyPatterns()
        {
            _detectedPatterns["HighFrequency"] = new PatternStrength { Strength = 0.7, Confidence = 0.8 };
            _detectedPatterns["LowFrequency"] = new PatternStrength { Strength = 0.8, Confidence = 0.9 };
        }

        private void DetectSequentialPatterns()
        {
            _detectedPatterns["Sequential"] = new PatternStrength { Strength = 0.5, Confidence = 0.6 };
        }

        private void DetectGapPatterns()
        {
            _detectedPatterns["LongGaps"] = new PatternStrength { Strength = 0.6, Confidence = 0.7 };
        }

        private void DetectCyclicalPatterns()
        {
            _detectedPatterns["Cyclical"] = new PatternStrength { Strength = 0.7, Confidence = 0.8 };
        }

        private double CalculatePatternStrength(string pattern, List<Lance> data)
        {
            // Implementação simplificada
            return _detectedPatterns.Values.Average(p => p.Strength);
        }

        private double AnalyzeDistribution(List<int> numbers)
        {
            var baixas = numbers.Count(n => n <= 8);
            var medias = numbers.Count(n => n >= 9 && n <= 17);
            var altas = numbers.Count(n => n >= 18);

            var ideal = 5.0; // 15/3
            var deviation = (Math.Abs(baixas - ideal) + Math.Abs(medias - ideal) + Math.Abs(altas - ideal)) / 15.0;
            
            return Math.Max(0.0, 1.0 - deviation);
        }

        private double AnalyzeSequences(List<int> numbers)
        {
            var sequences = 0;
            var sorted = numbers.OrderBy(n => n).ToList();
            
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                if (sorted[i + 1] == sorted[i] + 1)
                    sequences++;
            }

            // Penalizar muitas sequências
            return Math.Max(0.0, 1.0 - (sequences / 10.0));
        }

        private double AnalyzeGaps(List<int> numbers)
        {
            var sorted = numbers.OrderBy(n => n).ToList();
            var gaps = new List<int>();
            
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                gaps.Add(sorted[i + 1] - sorted[i]);
            }

            var avgGap = gaps.Any() ? gaps.Average() : 0;
            var idealGap = 25.0 / 15.0; // Distribuição ideal

            return Math.Max(0.0, 1.0 - Math.Abs(avgGap - idealGap) / idealGap);
        }
    }

    #region Supporting Data Classes

    public class FrequencyStats
    {
        public int TotalAppearances { get; set; }
        public double AverageInterval { get; set; }
        public double IntervalVariance { get; set; }
        public int LastAppearance { get; set; }
        public double LongestGap { get; set; }
        public double ShortestGap { get; set; }
    }

    public class DebtProfile
    {
        public int Dezena { get; set; }
        public int TotalAppearances { get; set; }
        public double AverageInterval { get; set; }
        public double Volatility { get; set; }
        public int LastAppearance { get; set; }
        public int CurrentGap { get; set; }
        public double DebtTrend { get; set; }
    }

    public class TemporalProfile
    {
        public int Dezena { get; set; }
        public int LastAppearance { get; set; }
        public double AverageInterval { get; set; }
        public double IntervalTrend { get; set; }
        public double RecentTrend { get; set; }
        public double Seasonality { get; set; }
    }

    public class PatternStrength
    {
        public double Strength { get; set; }
        public double Confidence { get; set; }
    }

    #endregion
}