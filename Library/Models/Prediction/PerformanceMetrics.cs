using System;

namespace LotoLibrary.Models.Prediction;

public class PerformanceMetrics
{
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public DateTime MeasuredAt { get; set; } = DateTime.Now;
}