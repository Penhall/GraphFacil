// D:\PROJETOS\GraphFacil\Library\Interfaces\EnsembleOptimizationStrategy.cs - Interface para modelos de meta-aprendizado
namespace LotoLibrary.Enums;

/// <summary>
/// Estratégias de otimização de ensemble
/// </summary>
public enum EnsembleOptimizationStrategy
{
    /// <summary>
    /// Maximiza performance média ponderada
    /// </summary>
    MaximizePerformance,

    /// <summary>
    /// Maximiza ratio de Sharpe (performance/volatilidade)
    /// </summary>
    MaximizeSharpeRatio,

    /// <summary>
    /// Maximiza diversificação
    /// </summary>
    MaximizeDiversification,

    /// <summary>
    /// Abordagem balanceada (performance + diversificação)
    /// </summary>
    Balanced,

    /// <summary>
    /// Minimiza correlação máxima
    /// </summary>
    MinimizeMaxCorrelation
}

