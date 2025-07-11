// D:\PROJETOS\GraphFacil\Library\PredictionModels\Ensemble\ContextDetectionCriteria.cs - Primeiro modelo da Fase 3: Meta-Learning
namespace LotoLibrary.Suporte;

public class ContextDetectionCriteria
{
    public double VolatilityThreshold { get; set; }
    public double TrendThreshold { get; set; }
    public int WindowSize { get; set; }
}

#endregion
