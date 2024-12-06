using System.Collections.Generic;

namespace LotoLibrary.Services;

public class GrupoAvaliacao
{
    public int IdSS { get; set; }
    public int IdNS { get; set; }
    public string IdCC => $"{IdSS}-{IdNS}";
    public Dictionary<int, double> PercentuaisSS { get; set; }  // 3 a 9
    public Dictionary<int, double> PercentuaisNS { get; set; }  // 2 a 6
    public double PontuacaoCombinada { get; set; }

    public GrupoAvaliacao()
    {
        PercentuaisSS = new Dictionary<int, double>();
        PercentuaisNS = new Dictionary<int, double>();
    }
}
