using System.Collections.Generic;
using System.Linq;
using TF = Tensorflow.NumPy;


namespace LotoLibrary.Services;

public class Subgrupo
{

    public int Id { get; set; }
    public List<int> Numeros { get; set; }
    public int Frequencia { get; set; }
    public double ValorAssociado { get; set; }
    public Dictionary<int, int> ContagemAcertos { get; set; } // Contagem de acertos (e.g., 9, 8, 7...)
    public Dictionary<int, double> ContagemPercentual { get; set; } // Contagem percentual para cada acerto

    public Subgrupo(List<int> numeros)
    {
        Numeros = numeros;
        Frequencia = 0;
        ValorAssociado = 0.0;
        ContagemAcertos = new Dictionary<int, int>();
        ContagemPercentual = new Dictionary<int, double>();
    }

    // Método para incrementar a frequência quando o subgrupo aparece em um sorteio
    public void IncrementarFrequencia()
    {
        Frequencia++;
    }

    // Método para atualizar a contagem de acertos
    public void AtualizarContagemAcertos(int acertos)
    {
        if (ContagemAcertos.ContainsKey(acertos))
        {
            ContagemAcertos[acertos]++;
        }
        else
        {
            ContagemAcertos[acertos] = 1;
        }
    }

    // Método para calcular a porcentagem de acertos
    public void CalcularPercentual(int totalSorteios)
    {
        foreach (var acerto in ContagemAcertos.Keys.ToList())
        {
            ContagemPercentual[acerto] = (double)ContagemAcertos[acerto] / totalSorteios * 100;
        }
    }

    // Método para obter um vetor de características do subgrupo
    public TF.NDArray ObterVetorCaracteristicas()
    {
        var caracteristicas = new List<double> { Frequencia, ValorAssociado };
        caracteristicas.AddRange(ContagemPercentual.Values);
        return TF.np.array(caracteristicas.ToArray());
    }
}