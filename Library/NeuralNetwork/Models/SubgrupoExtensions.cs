using LotoLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.NeuralNetwork.Models;

// Extensões para facilitar conversões
public static class SubgrupoExtensions
{
    public static IEnumerable<SubgrupoMLInput> ToMLInputs(this IEnumerable<Subgrupo> subgrupos)
    {
        return subgrupos.Select(s => SubgrupoMLInput.FromSubgrupo(s));
    }

    public static IEnumerable<SubgrupoMLData> ToMLData(this IEnumerable<Subgrupo> subgrupos)
    {
        return SubgrupoMLData.FromSubgrupos(subgrupos);
    }
}
