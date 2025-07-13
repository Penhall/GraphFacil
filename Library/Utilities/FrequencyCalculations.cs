// D:\PROJETOS\GraphFacil\Library\Utilities\FrequencyCalculations.cs
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Utilitários para cálculos de frequência e análise estatística
    /// Usado pelos modelos anti-frequencistas e de análise
    /// </summary>
    public static class FrequencyCalculations
    {
        /// <summary>
        /// Calcula frequência de cada dezena em uma lista de sorteios
        /// </summary>
        public static Dictionary<int, double> CalculateFrequencies(IEnumerable<List<int>> sorteios)
        {
            var frequencies = new Dictionary<int, double>();
            var totalSorteios = sorteios.Count();
            
            if (totalSorteios == 0) return frequencies;

            // Inicializar todas as dezenas (1-25) com frequência 0
            for (int i = 1; i <= 25; i++)
            {
                frequencies[i] = 0.0;
            }

            // Contar aparições de cada dezena
            foreach (var sorteio in sorteios)
            {
                foreach (var dezena in sorteio)
                {
                    if (dezena >= 1 && dezena <= 25)
                    {
                        frequencies[dezena]++;
                    }
                }
            }

            // Converter para frequência relativa (0.0 a 1.0)
            for (int i = 1; i <= 25; i++)
            {
                frequencies[i] = frequencies[i] / totalSorteios;
            }

            return frequencies;
        }

        /// <summary>
        /// Calcula frequência esperada teórica para Lotofácil
        /// </summary>
        public static double CalculateExpectedFrequency()
        {
            // Na Lotofácil, sorteia-se 15 dezenas de 25 possíveis
            // Frequência esperada = 15/25 = 0.6 = 60%
            return 15.0 / 25.0;
        }

        /// <summary>
        /// Calcula desvio estatístico da frequência observada vs esperada
        /// </summary>
        public static Dictionary<int, double> CalculateStatisticalDebt(Dictionary<int, double> observedFrequencies)
        {
            var expectedFreq = CalculateExpectedFrequency();
            var debt = new Dictionary<int, double>();

            foreach (var kvp in observedFrequencies)
            {
                // Dívida = Frequência esperada - Frequência observada
                // Positiva = dezena "deve" aparecer mais
                // Negativa = dezena apareceu "demais"
                debt[kvp.Key] = expectedFreq - kvp.Value;
            }

            return debt;
        }

        /// <summary>
        /// Calcula scores anti-frequencistas (inverso da frequência)
        /// </summary>
        public static Dictionary<int, double> CalculateAntiFrequencyScores(Dictionary<int, double> frequencies)
        {
            var maxFreq = frequencies.Values.Max();
            var scores = new Dictionary<int, double>();

            foreach (var kvp in frequencies)
            {
                // Score anti-freq = (Max Frequency - Current Frequency) / Max Frequency
                // Normalizado entre 0 e 1, onde 1 = menor frequência
                scores[kvp.Key] = maxFreq > 0 ? (maxFreq - kvp.Value) / maxFreq : 0.5;
            }

            return scores;
        }

        /// <summary>
        /// Aplica peso temporal aos cálculos (sorteios recentes têm mais peso)
        /// </summary>
        public static Dictionary<int, double> ApplyTemporalWeight(Dictionary<int, double> frequencies, double decayFactor = 0.95)
        {
            var weightedFreqs = new Dictionary<int, double>();
            
            foreach (var kvp in frequencies)
            {
                // Aplicar decay exponencial - sorteios mais recentes têm peso maior
                weightedFreqs[kvp.Key] = kvp.Value * Math.Pow(decayFactor, kvp.Key - 1);
            }

            return weightedFreqs;
        }

        /// <summary>
        /// Calcula volatilidade da frequência de uma dezena
        /// </summary>
        public static double CalculateVolatility(List<double> frequencyHistory)
        {
            if (frequencyHistory.Count < 2) return 0.0;

            var mean = frequencyHistory.Average();
            var variance = frequencyHistory.Sum(f => Math.Pow(f - mean, 2)) / (frequencyHistory.Count - 1);
            
            return Math.Sqrt(variance);
        }

        /// <summary>
        /// Detecta tendência na frequência (crescente, decrescente, estável)
        /// </summary>
        public static FrequencyTrend DetectTrend(List<double> frequencyHistory, int windowSize = 10)
        {
            if (frequencyHistory.Count < windowSize) return FrequencyTrend.Stable;

            var recentValues = frequencyHistory.TakeLast(windowSize).ToList();
            var olderValues = frequencyHistory.Skip(Math.Max(0, frequencyHistory.Count - 2 * windowSize))
                                              .Take(windowSize).ToList();

            if (!olderValues.Any()) return FrequencyTrend.Stable;

            var recentAvg = recentValues.Average();
            var olderAvg = olderValues.Average();
            var threshold = 0.05; // 5% de diferença para considerar mudança significativa

            if (recentAvg > olderAvg + threshold) return FrequencyTrend.Increasing;
            if (recentAvg < olderAvg - threshold) return FrequencyTrend.Decreasing;
            
            return FrequencyTrend.Stable;
        }

        /// <summary>
        /// Normaliza scores para soma = 1.0 (distribuição de probabilidade)
        /// </summary>
        public static Dictionary<int, double> NormalizeScores(Dictionary<int, double> scores)
        {
            var totalScore = scores.Values.Sum();
            var normalized = new Dictionary<int, double>();

            if (totalScore > 0)
            {
                foreach (var kvp in scores)
                {
                    normalized[kvp.Key] = kvp.Value / totalScore;
                }
            }
            else
            {
                // Se soma é 0, distribuir uniformemente
                var uniformScore = 1.0 / scores.Count;
                foreach (var kvp in scores)
                {
                    normalized[kvp.Key] = uniformScore;
                }
            }

            return normalized;
        }
    }

    /// <summary>
    /// Enum para direção da tendência de frequência
    /// </summary>
    public enum FrequencyTrend
    {
        Decreasing,
        Stable, 
        Increasing
    }
}
