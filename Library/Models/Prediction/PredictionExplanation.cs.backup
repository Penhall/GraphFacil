using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class PredictionExplanation
{
    public int Concurso { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string MainReason { get; set; } = string.Empty;
    public List<string> KeyFactors { get; set; } = new();
    public Dictionary<int, string> NumberExplanations { get; set; } = new();
    public double ConfidenceLevel { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}