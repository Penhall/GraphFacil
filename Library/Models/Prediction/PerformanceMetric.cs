using System;

namespace LotoLibrary.Models.Prediction;

public class PerformanceMetric
{
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public DateTime MeasuredAt { get; set; } = DateTime.Now;
}