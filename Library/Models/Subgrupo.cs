using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models;

public class Subgrupo
{
    public int Id { get; set; }
    public List<int> Numeros { get; set; }
    public int Frequencia { get; set; }
    public double ValorAssociado { get; set; }
    public Dictionary<int, int> ContagemAcertos { get; set; }
    public Dictionary<int, double> ContagemPercentual { get; set; }

    public Subgrupo(List<int> numeros)
    {
        Numeros = numeros;
        Frequencia = 0;
        ValorAssociado = 0.0;
        ContagemAcertos = new Dictionary<int, int>();
        ContagemPercentual = new Dictionary<int, double>();
    }

    public void IncrementarFrequencia()
    {
        Frequencia++;
    }

    public void AtualizarContagemAcertos(int acertos)
    {
        if (!ContagemAcertos.ContainsKey(acertos))
        {
            ContagemAcertos[acertos] = 0;
        }
        ContagemAcertos[acertos]++;
    }

    public void CalcularPercentual(int totalSorteios)
    {
        foreach (var acerto in ContagemAcertos.Keys.ToList())
        {
            ContagemPercentual[acerto] = (double)ContagemAcertos[acerto] / totalSorteios * 100;
        }
    }

    public float[] ObterCaracteristicasML()
    {
        List<float> caracteristicas = new List<float>();

        // Adicionar características básicas
        caracteristicas.Add((float)Frequencia);
        caracteristicas.Add((float)ValorAssociado);

        // Adicionar percentuais de acertos ordenados
        var percentuaisOrdenados = ContagemPercentual
            .OrderBy(x => x.Key)
            .Select(x => (float)x.Value);

        caracteristicas.AddRange(percentuaisOrdenados);

        return caracteristicas.ToArray();
    }

    public bool VerificarAcerto(Lance sorteio)
    {
        return Numeros.All(numero => sorteio.Lista.Contains(numero));
    }
}
