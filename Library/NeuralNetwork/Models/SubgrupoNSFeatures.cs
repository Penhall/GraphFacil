using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

// Classe específica para NS (5 dimensões)
public class SubgrupoNSFeatures : BaseMLFeatures
{
    [VectorType(5)]
    [ColumnName("Features")]
    public float[] Features { get; set; }
}