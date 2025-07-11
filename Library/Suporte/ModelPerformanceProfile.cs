// D:\PROJETOS\GraphFacil\Library\PredictionModels\Ensemble\ModelPerformanceProfile.cs - Primeiro modelo da Fase 3: Meta-Learning
using System.Collections.Generic;

namespace LotoLibrary.Suporte;
#region Supporting Classes

public class ModelPerformanceProfile
{
    public string ModelName { get; set; }
    public double HistoricalAccuracy { get; set; }
    public Dictionary<string, double> PerformanceByContext { get; set; }
    public List<string> OptimalRegimes { get; set; }
    public List<string> WeakRegimes { get; set; }
    public Dictionary<string, double> CorrelationWithOthers { get; set; }
    public double AdaptabilityScore { get; set; }

    public void UpdateWithNewResult(ConcursoResult result)
    {
        // Implementar atualização incremental
    }
}

#endregion
