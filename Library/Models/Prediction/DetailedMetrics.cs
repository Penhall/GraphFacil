using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class DetailedMetrics
{
    public string ModelName { get; set; } = string.Empty;
    public Dictionary<string, double> Metrics { get; set; } = new();
    public List<string> Insights { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}