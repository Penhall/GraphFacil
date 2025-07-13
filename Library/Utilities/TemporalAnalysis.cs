// D:\PROJETOS\GraphFacil\Library\Utilities\TemporalAnalysis.cs
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Utilitários para análise temporal de padrões em séries de sorteios
    /// Detecta ciclos, tendências e sazonalidade nos dados
    /// </summary>
    public static class TemporalAnalysis
    {
        /// <summary>
        /// Detecta ciclos temporais na frequência das dezenas
        /// </summary>
        public static Dictionary<int, CycleInfo> DetectCycles(List<(DateTime Date, List<int> Numbers)> historicalData, int maxCycleLength = 50)
        {
            var cycleInfo = new Dictionary<int, CycleInfo>();

            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var appearances = historicalData
                    .Where(h => h.Numbers.Contains(dezena))
                    .Select(h => h.Date)
                    .OrderBy(d => d)
                    .ToList();

                if (appearances.Count >= 3)
                {
                    var intervals = CalculateIntervals(appearances);
                    var cycles = DetectCyclicalPatterns(intervals, maxCycleLength);
                    
                    cycleInfo[dezena] = new CycleInfo
                    {
                        Dezena = dezena,
                        AverageInterval = intervals.Average(),
                        IntervalVariance = CalculateVariance(intervals),
                        DetectedCycles = cycles,
                        LastAppearance = appearances.Last(),
                        TotalAppearances = appearances.Count
                    };
                }
            }

            return cycleInfo;
        }

        /// <summary>
        /// Analisa tendências temporais (crescente, decrescente, estável)
        /// </summary>
        public static Dictionary<int, TrendInfo> AnalyzeTrends(List<(DateTime Date, List<int> Numbers)> historicalData, int windowSize = 20)
        {
            var trendInfo = new Dictionary<int, TrendInfo>();

            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var timeSeriesData = CreateTimeSeries(historicalData, dezena, windowSize);
                var trend = CalculateTrend(timeSeriesData);
                var momentum = CalculateMomentum(timeSeriesData);
                var volatility = CalculateVolatility(timeSeriesData);

                trendInfo[dezena] = new TrendInfo
                {
                    Dezena = dezena,
                    TrendDirection = trend,
                    TrendStrength = Math.Abs(trend),
                    Momentum = momentum,
                    Volatility = volatility,
                    IsAccelerating = momentum > 0.1,
                    IsStabilizing = volatility < 0.2
                };
            }

            return trendInfo;
        }

        /// <summary>
        /// Detecta sazonalidade nos padrões de sorteio
        /// </summary>
        public static SeasonalityInfo AnalyzeSeasonality(List<(DateTime Date, List<int> Numbers)> historicalData)
        {
            var monthlyPatterns = new Dictionary<int, List<int>>();
            var weeklyPatterns = new Dictionary<DayOfWeek, List<int>>();
            var yearlyPatterns = new Dictionary<int, List<int>>();

            foreach (var entry in historicalData)
            {
                var month = entry.Date.Month;
                var dayOfWeek = entry.Date.DayOfWeek;
                var year = entry.Date.Year;

                // Análise mensal
                if (!monthlyPatterns.ContainsKey(month))
                    monthlyPatterns[month] = new List<int>();
                monthlyPatterns[month].AddRange(entry.Numbers);

                // Análise semanal
                if (!weeklyPatterns.ContainsKey(dayOfWeek))
                    weeklyPatterns[dayOfWeek] = new List<int>();
                weeklyPatterns[dayOfWeek].AddRange(entry.Numbers);

                // Análise anual
                if (!yearlyPatterns.ContainsKey(year))
                    yearlyPatterns[year] = new List<int>();
                yearlyPatterns[year].AddRange(entry.Numbers);
            }

            return new SeasonalityInfo
            {
                MonthlyFrequencies = CalculateFrequencyByPeriod(monthlyPatterns),
                WeeklyFrequencies = CalculateFrequencyByDayOfWeek(weeklyPatterns),
                YearlyFrequencies = CalculateFrequencyByYear(yearlyPatterns),
                HasSeasonality = DetectSeasonalitySignificance(monthlyPatterns),
                SeasonalityStrength = CalculateSeasonalityStrength(monthlyPatterns)
            };
        }

        /// <summary>
        /// Calcula peso temporal decrescente para dados históricos
        /// </summary>
        public static Dictionary<DateTime, double> CalculateTemporalWeights(List<DateTime> dates, double decayFactor = 0.95)
        {
            var weights = new Dictionary<DateTime, double>();
            var sortedDates = dates.OrderByDescending(d => d).ToList();

            for (int i = 0; i < sortedDates.Count; i++)
            {
                weights[sortedDates[i]] = Math.Pow(decayFactor, i);
            }

            return weights;
        }

        /// <summary>
        /// Detecta pontos de mudança na série temporal
        /// </summary>
        public static List<ChangePoint> DetectChangePoints(List<(DateTime Date, double Value)> timeSeries, double threshold = 2.0)
        {
            var changePoints = new List<ChangePoint>();
            
            if (timeSeries.Count < 10) return changePoints;

            var sortedData = timeSeries.OrderBy(t => t.Date).ToList();

            for (int i = 5; i < sortedData.Count - 5; i++)
            {
                var beforeWindow = sortedData.Take(i).Select(t => t.Value).ToList();
                var afterWindow = sortedData.Skip(i).Select(t => t.Value).ToList();

                var meanBefore = beforeWindow.Average();
                var meanAfter = afterWindow.Average();
                var stdBefore = CalculateStandardDeviation(beforeWindow);
                var stdAfter = CalculateStandardDeviation(afterWindow);

                if (stdBefore > 0 && stdAfter > 0)
                {
                    var pooledStd = Math.Sqrt((Math.Pow(stdBefore, 2) + Math.Pow(stdAfter, 2)) / 2);
                    var tStat = Math.Abs(meanAfter - meanBefore) / pooledStd;

                    if (tStat > threshold)
                    {
                        changePoints.Add(new ChangePoint
                        {
                            Date = sortedData[i].Date,
                            Significance = tStat,
                            ChangeType = meanAfter > meanBefore ? ChangeType.Increase : ChangeType.Decrease,
                            ValueBefore = meanBefore,
                            ValueAfter = meanAfter
                        });
                    }
                }
            }

            return changePoints;
        }

        /// <summary>
        /// Prediz próximo valor baseado em análise temporal
        /// </summary>
        public static TemporalPrediction PredictNextValue(List<(DateTime Date, double Value)> timeSeries, DateTime targetDate)
        {
            if (timeSeries.Count < 3)
                return new TemporalPrediction { Confidence = 0.0, PredictedValue = 0.0 };

            var sortedData = timeSeries.OrderBy(t => t.Date).ToList();
            var values = sortedData.Select(t => t.Value).ToList();

            // Métodos de predição
            var linearTrend = PredictLinearTrend(sortedData, targetDate);
            var movingAverage = CalculateMovingAverage(values, Math.Min(5, values.Count));
            var exponentialSmoothing = CalculateExponentialSmoothing(values, 0.3);

            // Combinar predições com pesos
            var combinedPrediction = (linearTrend * 0.4) + (movingAverage * 0.3) + (exponentialSmoothing * 0.3);
            var confidence = CalculatePredictionConfidence(values, combinedPrediction);

            return new TemporalPrediction
            {
                PredictedValue = combinedPrediction,
                Confidence = confidence,
                TargetDate = targetDate,
                Method = "Combined (Linear + MA + ES)",
                TrendComponent = linearTrend,
                SeasonalComponent = 0.0 // Simplificado
            };
        }

        #region Helper Methods

        private static List<double> CalculateIntervals(List<DateTime> dates)
        {
            var intervals = new List<double>();
            for (int i = 1; i < dates.Count; i++)
            {
                intervals.Add((dates[i] - dates[i - 1]).TotalDays);
            }
            return intervals;
        }

        private static List<CyclicalPattern> DetectCyclicalPatterns(List<double> intervals, int maxCycleLength)
        {
            var patterns = new List<CyclicalPattern>();
            
            // Análise de autocorrelação simplificada
            for (int lag = 2; lag <= Math.Min(maxCycleLength, intervals.Count / 2); lag++)
            {
                var correlation = CalculateAutocorrelation(intervals, lag);
                if (correlation > 0.5) // Correlação significativa
                {
                    patterns.Add(new CyclicalPattern
                    {
                        CycleLength = lag,
                        Strength = correlation,
                        Confidence = correlation > 0.7 ? 0.8 : 0.5
                    });
                }
            }

            return patterns.OrderByDescending(p => p.Strength).ToList();
        }

        private static double CalculateAutocorrelation(List<double> values, int lag)
        {
            if (values.Count <= lag) return 0.0;

            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;

            if (variance == 0) return 0.0;

            var covariance = 0.0;
            for (int i = 0; i < values.Count - lag; i++)
            {
                covariance += (values[i] - mean) * (values[i + lag] - mean);
            }
            covariance /= (values.Count - lag);

            return covariance / variance;
        }

        private static List<(DateTime Date, double Value)> CreateTimeSeries(List<(DateTime Date, List<int> Numbers)> data, int dezena, int windowSize)
        {
            var timeSeries = new List<(DateTime Date, double Value)>();
            var sortedData = data.OrderBy(d => d.Date).ToList();

            for (int i = windowSize; i < sortedData.Count; i++)
            {
                var window = sortedData.Skip(i - windowSize).Take(windowSize);
                var frequency = window.Count(w => w.Numbers.Contains(dezena)) / (double)windowSize;
                timeSeries.Add((sortedData[i].Date, frequency));
            }

            return timeSeries;
        }

        private static double CalculateTrend(List<(DateTime Date, double Value)> timeSeries)
        {
            if (timeSeries.Count < 2) return 0.0;

            // Regressão linear simples
            var n = timeSeries.Count;
            var sumX = 0.0;
            var sumY = timeSeries.Sum(t => t.Value);
            var sumXY = 0.0;
            var sumX2 = 0.0;

            for (int i = 0; i < n; i++)
            {
                sumX += i;
                sumXY += i * timeSeries[i].Value;
                sumX2 += i * i;
            }

            var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope;
        }

        private static double CalculateMomentum(List<(DateTime Date, double Value)> timeSeries)
        {
            if (timeSeries.Count < 3) return 0.0;

            var recentValues = timeSeries.TakeLast(3).Select(t => t.Value).ToList();
            var olderValues = timeSeries.Take(3).Select(t => t.Value).ToList();

            var recentAvg = recentValues.Average();
            var olderAvg = olderValues.Average();

            return recentAvg - olderAvg;
        }

        private static double CalculateVolatility(List<(DateTime Date, double Value)> timeSeries)
        {
            if (timeSeries.Count < 2) return 0.0;

            var values = timeSeries.Select(t => t.Value).ToList();
            return CalculateStandardDeviation(values);
        }

        private static double CalculateVariance(List<double> values)
        {
            if (values.Count < 2) return 0.0;
            var mean = values.Average();
            return values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
        }

        private static double CalculateStandardDeviation(List<double> values)
        {
            return Math.Sqrt(CalculateVariance(values));
        }

        private static Dictionary<int, Dictionary<int, double>> CalculateFrequencyByPeriod(Dictionary<int, List<int>> patterns)
        {
            var result = new Dictionary<int, Dictionary<int, double>>();
            
            foreach (var period in patterns)
            {
                var frequencies = new Dictionary<int, double>();
                var total = period.Value.Count;
                
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    frequencies[dezena] = period.Value.Count(n => n == dezena) / (double)total;
                }
                
                result[period.Key] = frequencies;
            }
            
            return result;
        }

        private static Dictionary<DayOfWeek, Dictionary<int, double>> CalculateFrequencyByDayOfWeek(Dictionary<DayOfWeek, List<int>> patterns)
        {
            var result = new Dictionary<DayOfWeek, Dictionary<int, double>>();
            
            foreach (var day in patterns)
            {
                var frequencies = new Dictionary<int, double>();
                var total = day.Value.Count;
                
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    frequencies[dezena] = day.Value.Count(n => n == dezena) / (double)total;
                }
                
                result[day.Key] = frequencies;
            }
            
            return result;
        }

        private static Dictionary<int, Dictionary<int, double>> CalculateFrequencyByYear(Dictionary<int, List<int>> patterns)
        {
            return CalculateFrequencyByPeriod(patterns);
        }

        private static bool DetectSeasonalitySignificance(Dictionary<int, List<int>> monthlyPatterns)
        {
            // Análise ANOVA simplificada
            var allValues = monthlyPatterns.SelectMany(m => m.Value).ToList();
            var grandMean = allValues.Average();
            
            var betweenGroupVariance = 0.0;
            var withinGroupVariance = 0.0;
            var totalCount = 0;
            
            foreach (var month in monthlyPatterns)
            {
                var monthMean = month.Value.Average();
                var monthCount = month.Value.Count;
                
                betweenGroupVariance += monthCount * Math.Pow(monthMean - grandMean, 2);
                withinGroupVariance += month.Value.Sum(v => Math.Pow(v - monthMean, 2));
                totalCount += monthCount;
            }
            
            betweenGroupVariance /= (monthlyPatterns.Count - 1);
            withinGroupVariance /= (totalCount - monthlyPatterns.Count);
            
            var fStatistic = withinGroupVariance > 0 ? betweenGroupVariance / withinGroupVariance : 0;
            
            return fStatistic > 2.0; // F-test simplificado
        }

        private static double CalculateSeasonalityStrength(Dictionary<int, List<int>> monthlyPatterns)
        {
            var monthlyAverages = monthlyPatterns.Select(m => m.Value.Average()).ToList();
            var overallMean = monthlyAverages.Average();
            var variance = monthlyAverages.Sum(avg => Math.Pow(avg - overallMean, 2)) / monthlyAverages.Count;
            
            return Math.Sqrt(variance) / overallMean; // Coeficiente de variação
        }

        private static double PredictLinearTrend(List<(DateTime Date, double Value)> data, DateTime targetDate)
        {
            if (data.Count < 2) return 0.0;

            var trend = CalculateTrend(data);
            var lastValue = data.Last().Value;
            var daysDiff = (targetDate - data.Last().Date).TotalDays;
            
            return lastValue + (trend * daysDiff);
        }

        private static double CalculateMovingAverage(List<double> values, int window)
        {
            return values.TakeLast(window).Average();
        }

        private static double CalculateExponentialSmoothing(List<double> values, double alpha)
        {
            if (!values.Any()) return 0.0;
            
            var result = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                result = alpha * values[i] + (1 - alpha) * result;
            }
            
            return result;
        }

        private static double CalculatePredictionConfidence(List<double> historicalValues, double prediction)
        {
            if (historicalValues.Count < 3) return 0.5;
            
            var mean = historicalValues.Average();
            var stdDev = CalculateStandardDeviation(historicalValues);
            
            if (stdDev == 0) return 0.9;
            
            var zScore = Math.Abs(prediction - mean) / stdDev;
            
            // Confiança decresce com distância da média
            return Math.Max(0.1, 1.0 - (zScore * 0.2));
        }

        #endregion
    }

    #region Supporting Classes

    public class CycleInfo
    {
        public int Dezena { get; set; }
        public double AverageInterval { get; set; }
        public double IntervalVariance { get; set; }
        public List<CyclicalPattern> DetectedCycles { get; set; } = new List<CyclicalPattern>();
        public DateTime LastAppearance { get; set; }
        public int TotalAppearances { get; set; }
    }

    public class CyclicalPattern
    {
        public int CycleLength { get; set; }
        public double Strength { get; set; }
        public double Confidence { get; set; }
    }

    public class TrendInfo
    {
        public int Dezena { get; set; }
        public double TrendDirection { get; set; }
        public double TrendStrength { get; set; }
        public double Momentum { get; set; }
        public double Volatility { get; set; }
        public bool IsAccelerating { get; set; }
        public bool IsStabilizing { get; set; }
    }

    public class SeasonalityInfo
    {
        public Dictionary<int, Dictionary<int, double>> MonthlyFrequencies { get; set; }
        public Dictionary<DayOfWeek, Dictionary<int, double>> WeeklyFrequencies { get; set; }
        public Dictionary<int, Dictionary<int, double>> YearlyFrequencies { get; set; }
        public bool HasSeasonality { get; set; }
        public double SeasonalityStrength { get; set; }
    }

    public class ChangePoint
    {
        public DateTime Date { get; set; }
        public double Significance { get; set; }
        public ChangeType ChangeType { get; set; }
        public double ValueBefore { get; set; }
        public double ValueAfter { get; set; }
    }

    public class TemporalPrediction
    {
        public double PredictedValue { get; set; }
        public double Confidence { get; set; }
        public DateTime TargetDate { get; set; }
        public string Method { get; set; }
        public double TrendComponent { get; set; }
        public double SeasonalComponent { get; set; }
    }

    public enum ChangeType
    {
        Increase,
        Decrease,
        Stable
    }

    #endregion
}
