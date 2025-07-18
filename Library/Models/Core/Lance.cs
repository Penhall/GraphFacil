// D:\PROJETOS\GraphFacil\Library\Models\Lance.cs
using System;
using System.Collections.Generic;
using System.Linq;
namespace LotoLibrary.Models.Core;

public class Lance
{
    public int Id;
    public int Concurso => Id; // Compatibilidade
    public List<int> Dezenas => Lista; // Compatibilidade

    public int M = 0;
    public int N = 0;
    public int X = 0;
    public int Y = 0;
    public int PT = 0;

    public float F0 = 0;
    public float F1 = 0;
    public float F2 = 0;

    public Dictionary<int, int> ContagemAcerto = new();

    public int Anel = 0;

    public List<int> Lista;
    public List<int> ListaN = new();
    public List<int> ListaM = new();

    public List<double> MN = new();

    public Lances ListaX = new();

    public Lances ListaY = new();

    public string Saida;

    public string Nome;

    public Lance(int id, List<int> lista)
    {
        Id = id;
        Lista = lista;

        foreach (int o in lista)
        {
            Saida += Convert.ToString(o) + "\t";
            Nome += Convert.ToString(o);
        }
    }

    public Lance()
    {
        Id = 0;
        Lista = new List<int>();
    }

    public string Atualiza()
    {
        Saida = string.Empty;
        Nome = string.Empty;

        foreach (int o in Lista)
        {
            Saida += Convert.ToString(o) + "\t";
            Nome += Convert.ToString(o);
        }


        return Saida;
    }

    public void Ordena() { ListaM.Sort(); ListaN.Sort(); }

    public void LimpaListas() { ListaX.Clear(); ListaY.Clear(); }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        Lance other = obj as Lance;
        if (other == null) return 1;

        // Se algum valor for inválido, coloca no fim da lista
        if (float.IsNaN(F1) || float.IsInfinity(F1)) return -1;
        if (float.IsNaN(other.F1) || float.IsInfinity(other.F1)) return 1;

        // Ordem decrescente (maior primeiro)
        return other.F1.CompareTo(F1);
    }


    public override string ToString()
    {
        Atualiza();

        return Saida;
    }

}

// Classe Lances removida - usando a versão do arquivo Lances.cs

public static class EnumerableExtensions
{
    public static Lances ToLances1(this IEnumerable<Lance> source)
    {
        return new Lances(source);
    }

    public static Lances DistintostById(this Lances lances)
    {
        var distinctLances = lances
            .GroupBy(lance => lance.Id)
            .Select(group => group.First())
            .ToList();

        return new Lances(distinctLances);
    }

    public static Lances RemoveObjetosDuplicados(this Lances lances)
    {
        // Encontrar os IDs duplicados
        var duplicatedIds = lances
            .GroupBy(lance => lance.Id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        // Remover todos os lances com os IDs duplicados
        Lances result = new Lances(lances.Where(lance => !duplicatedIds.Contains(lance.Id)));

        return result;
    }


    // Método para encontrar o Lance com o maior valor X
    public static Lance ObterLanceMaiorX(this Lances lances)
    {
        if (lances == null || lances.Count == 0)
            return null;

        // Garantir que estamos retornando o objeto Lance completo
        return lances.OrderByDescending(lance => lance.X).First();
    }

    // Método para encontrar o Lance com o maior valor Y
    public static Lance ObterLanceMaiorY(this Lances lances)
    {
        if (lances == null || lances.Count == 0)
            return null;

        // Garantir que estamos retornando o objeto Lance completo
        return lances.OrderByDescending(lance => lance.Y).First();
    }


}
