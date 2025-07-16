using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class ComparisonReport
{
    public string BestModelName { get; set; } = string.Empty;
    public Dictionary<string, PerformanceReport> ModelReports { get; set; } = new();
    public Dictionary<string, double> RankingScores { get; set; } = new();
    public string ComparisonCriteria { get; set; } = "Accuracy";
    public List<string> Insights { get; set; } = new();
    public DateTime ComparedAt { get; set; } = DateTime.Now;
}