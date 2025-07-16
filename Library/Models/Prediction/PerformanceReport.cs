using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class PerformanceReport
{
    public string ModelName { get; set; } = string.Empty;
    public double Accuracy { get; set; }
    public double OverallAccuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public int TotalPredictions { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public Dictionary<string, double> DetailedMetrics { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public int TestSampleSize { get; set; }
}
