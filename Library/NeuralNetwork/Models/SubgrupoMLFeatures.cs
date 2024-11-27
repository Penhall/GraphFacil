using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

public class SubgrupoMLFeatures
{
    [VectorType(7)]
    [ColumnName("Features")]
    public float[] Features { get; set; }

    [ColumnName("Label")]
    public float Label { get; set; }
}