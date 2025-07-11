// D:\PROJETOS\GraphFacil\Library\Interfaces\ConfidenceLevel.cs - Interface para modelos de meta-aprendizado
// D:\PROJETOS\GraphFacil\Library\Enums\MetaLearningEnums.cs - Enums para meta-learning
namespace LotoLibrary.Enums
{
    /// <summary>
    /// Níveis de confiança para decisões
    /// </summary>
    public enum ConfidenceLevel
    {
        /// <summary>
        /// Confiança muito baixa (< 0.3)
        /// </summary>
        VeryLow,

        /// <summary>
        /// Confiança baixa (0.3 - 0.5)
        /// </summary>
        Low,

        /// <summary>
        /// Confiança média (0.5 - 0.7)
        /// </summary>
        Medium,

        /// <summary>
        /// Confiança alta (0.7 - 0.9)
        /// </summary>
        High,

        /// <summary>
        /// Confiança muito alta (> 0.9)
        /// </summary>
        VeryHigh
    }
}