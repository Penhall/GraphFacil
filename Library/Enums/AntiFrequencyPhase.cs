// D:\PROJETOS\GraphFacil\Library\Enums\AntiFrequencyPhase.cs - Enums específicos para modelos anti-frequencistas
namespace LotoLibrary.Enums
{
    /// <summary>
    /// Fases do modelo anti-frequencista
    /// </summary>
    public enum AntiFrequencyPhase
    {
        /// <summary>
        /// Fase de inicialização e análise inicial
        /// </summary>
        Initialization,

        /// <summary>
        /// Fase de análise de padrões
        /// </summary>
        PatternAnalysis,

        /// <summary>
        /// Fase de calibração de parâmetros
        /// </summary>
        Calibration,

        /// <summary>
        /// Fase de predição ativa
        /// </summary>
        ActivePrediction,

        /// <summary>
        /// Fase de adaptação baseada em resultados
        /// </summary>
        Adaptation,

        /// <summary>
        /// Fase de validação e ajuste
        /// </summary>
        Validation
    }
}

