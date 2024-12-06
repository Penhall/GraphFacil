using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

// Classe específica para NS (5 dimensões)
public class SubgrupoNSFeatures : MLFeatures
{
    [VectorType(5)]
    [ColumnName("Features")]
    public new float[] Features { get; set; }
}