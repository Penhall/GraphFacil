using LotoLibrary.Models;
using Microsoft.ML.Data;

namespace LotoLibrary.NeuralNetwork.Models;

// Classe para ML.NET input
public class SubgrupoMLInput
{
    [VectorType(7)] // Ajuste o tamanho conforme necessário
    public float[] Caracteristicas { get; set; }

    public static SubgrupoMLInput FromSubgrupo(Subgrupo subgrupo)
    {
        return new SubgrupoMLInput
        {
            Caracteristicas = subgrupo.ObterCaracteristicasML()
        };
    }
}
