// D:\PROJETOS\GraphFacil\Library\Suporte\ContextDetectionCriteria.cs
using System.Collections.Generic;
using System;

namespace LotoLibrary.Suporte;

/// <summary>
/// Critérios para detecção de contexto em regimes de mercado
/// Usado pelo MetaLearningModel para identificar padrões de comportamento
/// </summary>
public class ContextDetectionCriteria
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, double> Thresholds { get; set; }
    public List<string> RequiredIndicators { get; set; }
    public double MinimumConfidence { get; set; }
    public int MinimumSamples { get; set; }
    public TimeSpan DetectionWindow { get; set; }

    public ContextDetectionCriteria()
    {
        Thresholds = new Dictionary<string, double>();
        RequiredIndicators = new List<string>();
        MinimumConfidence = 0.7;
        MinimumSamples = 10;
        DetectionWindow = TimeSpan.FromDays(30);
    }

    /// <summary>
    /// Verifica se o contexto atual atende aos critérios
    /// </summary>
    public bool Matches(ContextAnalysis context)
    {
        if (context == null) return false;

        // Verificar cada threshold configurado
        foreach (var threshold in Thresholds)
        {
            if (!CheckThreshold(context, threshold.Key, threshold.Value))
                return false;
        }

        return true;
    }

    private bool CheckThreshold(ContextAnalysis context, string indicator, double threshold)
    {
        return indicator switch
        {
            "Volatility" => context.Volatility >= threshold,
            "TrendStrength" => context.TrendStrength >= threshold,
            "PatternComplexity" => context.PatternComplexity >= threshold,
            _ => true
        };
    }
}
