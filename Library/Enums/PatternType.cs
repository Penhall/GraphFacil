// D:\PROJETOS\GraphFacil\Library\Enums\PatternType.cs - Enums específicos para modelos anti-frequencistas
namespace LotoLibrary.Enums;

/// <summary>
/// Tipos de padrões detectáveis
/// </summary>
public enum PatternType
{
    /// <summary>
    /// Padrões de frequência simples
    /// </summary>
    Frequency,

    /// <summary>
    /// Padrões sequenciais (números consecutivos)
    /// </summary>
    Sequential,

    /// <summary>
    /// Padrões de gaps (intervalos entre números)
    /// </summary>
    Gap,

    /// <summary>
    /// Padrões cíclicos (repetição temporal)
    /// </summary>
    Cyclical,

    /// <summary>
    /// Padrões de distribuição (baixas/médias/altas)
    /// </summary>
    Distribution,

    /// <summary>
    /// Padrões de volatilidade (estabilidade/instabilidade)
    /// </summary>
    Volatility,

    /// <summary>
    /// Padrões sazonais (variação temporal)
    /// </summary>
    Seasonal,

    /// <summary>
    /// Padrões de correlação entre dezenas
    /// </summary>
    Correlation
}


