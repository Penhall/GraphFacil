// D:\PROJETOS\GraphFacil\Library\Enums\AntiFrequencyStrategy.cs - Enums específicos para modelos anti-frequencistas
namespace LotoLibrary.Enums;

/// <summary>
/// Estratégias de anti-frequência disponíveis
/// </summary>
public enum AntiFrequencyStrategy
{
    /// <summary>
    /// Estratégia suave - inversão moderada da frequência
    /// Aplica correção leve para dezenas sub-representadas
    /// </summary>
    Gentle,

    /// <summary>
    /// Estratégia moderada - balanceamento frequência vs anti-frequência
    /// Equilibra entre padrões frequencistas e anti-frequencistas
    /// </summary>
    Moderate,

    /// <summary>
    /// Estratégia forte - inversão total da frequência
    /// Prioriza completamente dezenas menos frequentes
    /// </summary>
    Strong,

    /// <summary>
    /// Estratégia adaptativa - muda baseada nos padrões detectados
    /// Ajusta automaticamente baseado no contexto atual
    /// </summary>
    Adaptive,

    /// <summary>
    /// Estratégia baseada em dívida estatística
    /// Foca em dezenas com maior "dívida" estatística
    /// </summary>
    StatisticalDebt,

    /// <summary>
    /// Estratégia baseada em saturação de padrões
    /// Evita padrões que estão "saturados" ou sobre-utilizados
    /// </summary>
    Saturation,

    /// <summary>
    /// Estratégia pendular - oscilação entre extremos
    /// Alterna entre diferentes abordagens de forma cíclica
    /// </summary>
    Pendular
}

