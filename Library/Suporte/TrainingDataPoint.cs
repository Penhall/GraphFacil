// D:\PROJETOS\GraphFacil\Library\PredictionModels\Ensemble\TrainingDataPoint.cs - Primeiro modelo da Fase 3: Meta-Learning
using System.Collections.Generic;

namespace LotoLibrary.Suporte;

public class TrainingDataPoint
{
    public ContextAnalysis Context { get; set; }
    public Dictionary<string, double> ActualPerformances { get; set; }
}

#endregion
