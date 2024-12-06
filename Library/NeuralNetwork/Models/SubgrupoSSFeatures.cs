using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

// Classe específica para SS (7 dimensões)
public class SubgrupoSSFeatures : MLFeatures
{
    [VectorType(7)]
    [ColumnName("Features")]
    public new float[] Features { get; set; }
}
