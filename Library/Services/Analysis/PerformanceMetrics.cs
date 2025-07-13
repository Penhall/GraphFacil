// D:\PROJETOS\GraphFacil\Library\Services\Analysis\PerformanceMetrics.cs - Comparador de performance entre modelos
using System;

namespace LotoLibrary.Services.Analysis;
public class PerformanceMetrics
{
    public string ModelName { get; set; }
    public int TotalPredictions { get; set; }
    public double AverageConfidence { get; set; }
    public double ConfidenceStability { get; set; }
    public double PredictionConsistency { get; set; }
    public double TemporalStability { get; set; }
    public DateTime LastUpdateTime { get; set; }
}

