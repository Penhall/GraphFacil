// D:\PROJETOS\GraphFacil\Library\Services\DiagnosticReport.cs - Novo arquivo para diagnosticar o bug
using System.Collections.Generic;
using LotoLibrary.Enums;

namespace LotoLibrary.Services.Auxiliar;

// Modelos de dados para o diagnóstico
public class DiagnosticReport
{
    public int TotalPalpitesAnalisados { get; set; }
    public int TotalDezenasContadas { get; set; }
    public double MediaDezenasPorPalpite { get; set; }

    public int Dezenas1a9 { get; set; }
    public int Dezenas10a18 { get; set; }
    public int Dezenas19a25 { get; set; }

    public double Percentual1a9 { get; set; }
    public double Percentual10a18 { get; set; }
    public double Percentual19a25 { get; set; }

    public List<int> DezenasNuncaSelecionadas { get; set; } = new List<int>();
    public KeyValuePair<int, int> DezenaMaisFrequente { get; set; }
    public KeyValuePair<int, int> DezenaMenosFrequente { get; set; }

    public Dictionary<int, FrequenciaInfo> FrequenciaPorDezena { get; set; } = new Dictionary<int, FrequenciaInfo>();

    public bool TemProblemaDistribuicao { get; set; }
    public GravidadeProblema GravidadeProblema { get; set; }
    public string ErroExecucao { get; set; }
}
