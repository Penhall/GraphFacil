using System;

namespace LotoLibrary.Suporte;

public class StrategyRecommendation
{
    public string Name { get; set; } = string.Empty;
    public string BestModel { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Rationale { get; set; } = string.Empty;

    public StrategyRecommendation() { }

    public StrategyRecommendation(string name, string bestModel, double confidence, string rationale)
    {
        Name = name;
        BestModel = bestModel;
        Confidence = confidence;
        Rationale = rationale;
    }
}