// D:\PROJETOS\GraphFacil\Library\PredictionModels\Ensemble\ContextAnalysis.cs - Primeiro modelo da Fase 3: Meta-Learning
using System.Collections.Generic;

namespace LotoLibrary.Suporte;

public class ContextAnalysis
{
    public double Volatility { get; set; }
    public double TrendStrength { get; set; }
    public double PatternComplexity { get; set; }
    public Dictionary<string, double> RecentPerformance { get; set; }
}

#endregion
