// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\StatisticalDebtCalculator.cs - Cálculos de frequência para modelos anti-frequencistas
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Models.Core;

namespace LotoLibrary.Utilities.AntiFrequency;

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

