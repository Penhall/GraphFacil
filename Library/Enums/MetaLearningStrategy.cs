// D:\PROJETOS\GraphFacil\Library\Interfaces\MetaLearningStrategy.cs - Interface para modelos de meta-aprendizado
// D:\PROJETOS\GraphFacil\Library\Enums\MetaLearningEnums.cs - Enums para meta-learning
namespace LotoLibrary.Enums
{
    /// <summary>
    /// Estratégias de meta-aprendizado
    /// </summary>
    public enum MetaLearningStrategy
    {
        /// <summary>
        /// Seleção baseada em performance histórica
        /// </summary>
        PerformanceBased,

        /// <summary>
        /// Seleção baseada em detecção de regime
        /// </summary>
        RegimeBased,

        /// <summary>
        /// Ensemble adaptativo com pesos otimizados
        /// </summary>
        AdaptiveEnsemble,

        /// <summary>
        /// Aprendizado online contínuo
        /// </summary>
        OnlineLearning,

        /// <summary>
        /// Combinação de múltiplas estratégias
        /// </summary>
        Hybrid
    }
}