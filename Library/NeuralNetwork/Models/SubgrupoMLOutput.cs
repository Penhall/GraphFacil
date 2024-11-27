using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

public class SubgrupoMLOutput
{
    [ColumnName("Score")]
    public float Score { get; set; }
}
