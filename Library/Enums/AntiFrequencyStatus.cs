// D:\PROJETOS\GraphFacil\Library\Enums\AntiFrequencyStatus.cs
namespace LotoLibrary.Enums;

/// <summary>
/// Status do sistema anti-frequencista
/// </summary>
public enum AntiFrequencyStatus
{
    /// <summary>
    /// Sistema não inicializado
    /// </summary>
    NotInitialized,

    /// <summary>
    /// Inicializando componentes
    /// </summary>
    Initializing,

    /// <summary>
    /// Analisando dados históricos
    /// </summary>
    AnalyzingData,

    /// <summary>
    /// Detectando padrões
    /// </summary>
    DetectingPatterns,

    /// <summary>
    /// Calibrando parâmetros
    /// </summary>
    Calibrating,

    /// <summary>
    /// Pronto para predições
    /// </summary>
    Ready,

    /// <summary>
    /// Gerando predição
    /// </summary>
    Predicting,

    /// <summary>
    /// Validando resultados
    /// </summary>
    Validating,

    /// <summary>
    /// Adaptando estratégia
    /// </summary>
    Adapting,

    /// <summary>
    /// Erro no sistema
    /// </summary>
    Error
}
