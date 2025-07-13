// D:\PROJETOS\GraphFacil\Library\Enums\CorrectionType.cs - Enums específicos para modelos anti-frequencistas
namespace LotoLibrary.Enums;

/// <summary>
/// Tipos de correção aplicáveis
/// </summary>
public enum CorrectionType
{
    /// <summary>
    /// Sem correção aplicada
    /// </summary>
    None,

    /// <summary>
    /// Correção de distribuição baixas/altas
    /// </summary>
    Distribution,

    /// <summary>
    /// Correção de balanceamento par/ímpar
    /// </summary>
    EvenOdd,

    /// <summary>
    /// Correção de sequências excessivas
    /// </summary>
    Sequential,

    /// <summary>
    /// Correção de gaps inadequados
    /// </summary>
    Gap,

    /// <summary>
    /// Correção de sobre-concentração
    /// </summary>
    Concentration,

    /// <summary>
    /// Correção múltipla (várias correções aplicadas)
    /// </summary>
    Multiple
}

;
