using LotoLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.NeuralNetwork.Models;

// Classe para armazenar dados do subgrupo para ML.NET
public class SubgrupoMLData
{
    public int SubgrupoId { get; set; }
    public float[] Features { get; set; }
    public Dictionary<int, float> PercentuaisAcerto { get; set; }

    public static SubgrupoMLData FromSubgrupo(Subgrupo subgrupo)
    {
        return new SubgrupoMLData
        {
            SubgrupoId = subgrupo.Id,
            Features = subgrupo.ObterCaracteristicasML(),
            PercentuaisAcerto = subgrupo.ContagemPercentual
                .ToDictionary(k => k.Key, v => (float)v.Value)
        };
    }

    public static List<SubgrupoMLData> FromSubgrupos(IEnumerable<Subgrupo> subgrupos)
    {
        return subgrupos.Select(FromSubgrupo).ToList();
    }
}
