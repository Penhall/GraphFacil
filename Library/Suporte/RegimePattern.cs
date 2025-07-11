// D:\PROJETOS\GraphFacil\Library\PredictionModels\Ensemble\RegimePattern.cs - Primeiro modelo da Fase 3: Meta-Learning
namespace LotoLibrary.Suporte;

public class RegimePattern
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string OptimalModel { get; set; }
    public ContextDetectionCriteria DetectionCriteria { get; set; }
    public double HistoricalFrequency { get; set; }
    public double AveragePerformance { get; set; }
    public double CurrentConfidence { get; set; }
}

#endregion
