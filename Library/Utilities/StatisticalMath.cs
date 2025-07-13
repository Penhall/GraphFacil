// D:\PROJETOS\GraphFacil\Library\Utilities\StatisticalMath.cs
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Utilitários para cálculos matemáticos e estatísticos avançados
    /// Usado pelos modelos de predição para análises complexas
    /// </summary>
    public static class StatisticalMath
    {
        /// <summary>
        /// Calcula correlação de Pearson entre duas séries
        /// </summary>
        public static double CalculatePearsonCorrelation(List<double> x, List<double> y)
        {
            if (x.Count != y.Count || x.Count < 2) return 0.0;

            var meanX = x.Average();
            var meanY = y.Average();

            var numerator = x.Zip(y, (xi, yi) => (xi - meanX) * (yi - meanY)).Sum();
            var denomX = Math.Sqrt(x.Sum(xi => Math.Pow(xi - meanX, 2)));
            var denomY = Math.Sqrt(y.Sum(yi => Math.Pow(yi - meanY, 2)));

            if (denomX == 0 || denomY == 0) return 0.0;

            return numerator / (denomX * denomY);
        }

        /// <summary>
        /// Calcula média móvel exponencial (EMA)
        /// </summary>
        public static List<double> CalculateEMA(List<double> values, double alpha = 0.2)
        {
            if (!values.Any()) return new List<double>();

            var ema = new List<double> { values[0] };

            for (int i = 1; i < values.Count; i++)
            {
                var newEma = alpha * values[i] + (1 - alpha) * ema[i - 1];
                ema.Add(newEma);
            }

            return ema;
        }

        /// <summary>
        /// Calcula RSI (Relative Strength Index) adaptado para loteria
        /// </summary>
        public static double CalculateRSI(List<double> values, int period = 14)
        {
            if (values.Count < period + 1) return 50.0; // RSI neutro

            var gains = new List<double>();
            var losses = new List<double>();

            for (int i = 1; i < values.Count; i++)
            {
                var change = values[i] - values[i - 1];
                gains.Add(change > 0 ? change : 0);
                losses.Add(change < 0 ? Math.Abs(change) : 0);
            }

            var avgGain = gains.TakeLast(period).Average();
            var avgLoss = losses.TakeLast(period).Average();

            if (avgLoss == 0) return 100.0; // Só ganhos
            
            var rs = avgGain / avgLoss;
            return 100.0 - (100.0 / (1.0 + rs));
        }

        /// <summary>
        /// Calcula Bandas de Bollinger adaptadas
        /// </summary>
        public static (double Lower, double Middle, double Upper) CalculateBollingerBands(
            List<double> values, int period = 20, double multiplier = 2.0)
        {
            if (values.Count < period)
            {
                var avg = values.Average();
                return (avg, avg, avg);
            }

            var recentValues = values.TakeLast(period).ToList();
            var sma = recentValues.Average();
            var variance = recentValues.Sum(v => Math.Pow(v - sma, 2)) / period;
            var stdDev = Math.Sqrt(variance);

            return (
                Lower: sma - (multiplier * stdDev),
                Middle: sma,
                Upper: sma + (multiplier * stdDev)
            );
        }

        /// <summary>
        /// Calcula Z-Score para detectar outliers
        /// </summary>
        public static double CalculateZScore(double value, List<double> dataset)
        {
            if (dataset.Count < 2) return 0.0;

            var mean = dataset.Average();
            var stdDev = CalculateStandardDeviation(dataset);

            return stdDev > 0 ? (value - mean) / stdDev : 0.0;
        }

        /// <summary>
        /// Calcula desvio padrão
        /// </summary>
        public static double CalculateStandardDeviation(List<double> values)
        {
            if (values.Count < 2) return 0.0;

            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
            
            return Math.Sqrt(variance);
        }

        /// <summary>
        /// Calcula entropia de Shannon para uma distribuição
        /// </summary>
        public static double CalculateEntropy(Dictionary<int, double> distribution)
        {
            var entropy = 0.0;
            var total = distribution.Values.Sum();

            if (total <= 0) return 0.0;

            foreach (var probability in distribution.Values)
            {
                if (probability > 0)
                {
                    var p = probability / total;
                    entropy -= p * Math.Log2(p);
                }
            }

            return entropy;
        }

        /// <summary>
        /// Testa normalidade usando teste de Shapiro-Wilk simplificado
        /// </summary>
        public static bool IsNormalDistribution(List<double> values, double significanceLevel = 0.05)
        {
            if (values.Count < 3) return false;

            // Implementação simplificada - para uso completo, usar biblioteca estatística
            var mean = values.Average();
            var stdDev = CalculateStandardDeviation(values);
            
            // Teste se aproximadamente 68% dos valores estão dentro de 1 desvio padrão
            var withinOneStdDev = values.Count(v => Math.Abs(v - mean) <= stdDev);
            var percentage = (double)withinOneStdDev / values.Count;
            
            return percentage >= 0.6 && percentage <= 0.75; // Aproximadamente 68%
        }

        /// <summary>
        /// Calcula coeficiente de Gini para medir desigualdade na distribuição
        /// </summary>
        public static double CalculateGiniCoefficient(List<double> values)
        {
            if (values.Count < 2) return 0.0;

            var sortedValues = values.OrderBy(x => x).ToList();
            var n = sortedValues.Count;
            var sum = sortedValues.Sum();

            if (sum == 0) return 0.0;

            var index = 0.0;
            foreach (var value in sortedValues)
            {
                index += value * (n - sortedValues.IndexOf(value));
            }

            return (2 * index) / (n * sum) - (n + 1) / n;
        }

        /// <summary>
        /// Calcula momentum de uma série temporal
        /// </summary>
        public static double CalculateMomentum(List<double> values, int period = 10)
        {
            if (values.Count < period + 1) return 0.0;

            var current = values[values.Count - 1];
            var previous = values[values.Count - 1 - period];

            return previous != 0 ? (current - previous) / previous : 0.0;
        }

        /// <summary>
        /// Detecta pontos de mudança em uma série temporal
        /// </summary>
        public static List<int> DetectChangePoints(List<double> values, double threshold = 2.0)
        {
            var changePoints = new List<int>();
            
            if (values.Count < 3) return changePoints;

            for (int i = 1; i < values.Count - 1; i++)
            {
                var before = values.Take(i).ToList();
                var after = values.Skip(i).ToList();

                if (before.Count > 0 && after.Count > 0)
                {
                    var meanBefore = before.Average();
                    var meanAfter = after.Average();
                    var stdBefore = CalculateStandardDeviation(before);
                    var stdAfter = CalculateStandardDeviation(after);
                    var pooledStd = Math.Sqrt((stdBefore * stdBefore + stdAfter * stdAfter) / 2);

                    if (pooledStd > 0)
                    {
                        var tStat = Math.Abs(meanAfter - meanBefore) / pooledStd;
                        if (tStat > threshold)
                        {
                            changePoints.Add(i);
                        }
                    }
                }
            }

            return changePoints;
        }

        /// <summary>
        /// Calcula índice de diversificação Herfindahl-Hirschman
        /// </summary>
        public static double CalculateHHI(Dictionary<int, double> distribution)
        {
            var total = distribution.Values.Sum();
            if (total <= 0) return 0.0;

            var hhi = 0.0;
            foreach (var value in distribution.Values)
            {
                var share = value / total;
                hhi += share * share;
            }

            return hhi;
        }
    }
}
