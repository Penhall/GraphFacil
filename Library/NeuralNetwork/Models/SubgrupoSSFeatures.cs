using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

// Classe específica para SS (7 dimensões)
public class SubgrupoSSFeatures : BaseMLFeatures
{
    [VectorType(7)]
    [ColumnName("Features")]
    public float[] Features { get; set; }
}
