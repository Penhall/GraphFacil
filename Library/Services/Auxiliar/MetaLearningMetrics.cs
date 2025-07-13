// D:\PROJETOS\GraphFacil\Library\Services\MetaLearningMetrics.cs - Validação completa do sistema meta-learning
namespace LotoLibrary.Services.Auxiliar;

public class MetaLearningMetrics
{
    public MetaLearningMetrics()
    {
    }

    public double MetaConfidence { get; set; }
    public double AdaptationScore { get; set; }
    public double EnsembleOptimizationGain { get; set; }
    public string CurrentRegime { get; set; }
    public string RecommendedStrategy { get; set; }
    public int RegimesDetected { get; set; }
}

