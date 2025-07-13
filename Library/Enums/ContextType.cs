// D:\PROJETOS\GraphFacil\Library\Interfaces\ContextType.cs - Interface para modelos de meta-aprendizado
namespace LotoLibrary.Enums;

/// <summary>
/// Tipos de contexto para análise
/// </summary>
public enum ContextType
{
    /// <summary>
    /// Contexto baseado em tendências temporais
    /// </summary>
    Temporal,

    /// <summary>
    /// Contexto baseado em frequências estatísticas
    /// </summary>
    Statistical,

    /// <summary>
    /// Contexto baseado em volatilidade
    /// </summary>
    Volatility,

    /// <summary>
    /// Contexto baseado em padrões de saturação
    /// </summary>
    Saturation,

    /// <summary>
    /// Contexto misto ou indefinido
    /// </summary>
    Mixed
}
