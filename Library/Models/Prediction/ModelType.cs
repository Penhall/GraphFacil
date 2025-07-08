// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelType.cs - Modelos de dados
namespace LotoLibrary.Models.Prediction
{
    // Enums
    public enum ModelType
    {
        // Modelos Existentes
        Metronomo,
        Oscillator,
        MLNet,

        // Modelos Anti-Frequencistas
        AntiFrequency,
        StatisticalDebt,
        Saturation,
        PendularOscillator,

        // Modelos Avançados
        GraphNeuralNetwork,
        Autoencoder,
        ReinforcementLearning,

        // Ensemble e Meta
        Ensemble,
        MetaLearning,

        // Especiais
        Random,
        Frequency
    }
}
