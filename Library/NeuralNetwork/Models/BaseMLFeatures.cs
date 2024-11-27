using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

public class BaseMLFeatures
{
    [ColumnName("Label")]
    public float Label { get; set; }
}
