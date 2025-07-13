// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\FrequencyAnalyzer.cs - Cálculos de frequência para modelos anti-frequencistas
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Models;
using LotoLibrary.PredictionModels.AntiFrequency.Base;
using LotoLibrary.Suporte;

namespace LotoLibrary.Utilities.AntiFrequency;

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
        var lastConcurso = ConcursoResult.FromLance(lastLance);
        return lastConcurso.DataSorteio;
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
