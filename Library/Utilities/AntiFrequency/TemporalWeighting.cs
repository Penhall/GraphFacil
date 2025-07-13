// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\TemporalWeighting.cs - Cálculos de frequência para modelos anti-frequencistas
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Models;

namespace LotoLibrary.Utilities.AntiFrequency;

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

