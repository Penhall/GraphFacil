using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

public class MLOutput
{
    [ColumnName("Score")]
    public float Score { get; set; }
}