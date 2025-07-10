// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelType.cs - Modelos de dados
namespace LotoLibrary.Enums
{
    // Enums
    public enum ModelType
    {
        // Modelos Individuais
        Metronomo,
        MLNet,
        Oscillator,

        // Modelos Anti-Frequencistas
        AntiFrequency,
        AntiFrequencySimple,
        StatisticalDebt,
        Saturation,
        PendularOscillator,

        // Modelos Avançados
        GraphNeuralNetwork,
        Autoencoder,
        ReinforcementLearning,

        // Ensemble e Meta
        Ensemble,
        BasicEnsemble,
        WeightedEnsemble,
        StackedEnsemble,
        MetaLearning,
        AdaptiveWeights,
        RegimeDetection,

        // Especiais
        Random,
        Frequency
    }
}
