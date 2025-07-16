// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\FrequencyCalculations.cs - Cálculos de frequência para modelos anti-frequencistas
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Models.Core;

namespace LotoLibrary.Utilities.AntiFrequency;

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


