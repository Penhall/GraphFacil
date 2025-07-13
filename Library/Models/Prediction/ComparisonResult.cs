using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class ComparisonResult
{
    public string BestModelName { get; set; } = string.Empty;
    public Dictionary<string, double> ModelScores { get; set; } = new();
    public DateTime ComparedAt { get; set; } = DateTime.Now;
    public string ComparisonMethod { get; set; } = "Standard";
}