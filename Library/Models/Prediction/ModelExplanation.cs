using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class ModelExplanation
{
    public string ModelName { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<string> MainFactors { get; set; } = new();
    public List<string> TechnicalDetails { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    public ModelExplanation()
    {
        MainFactors = new List<string>();
        TechnicalDetails = new List<string>();
    }
}