// D:\PROJETOS\GraphFacil\Library\Models\MetricasPerformance.cs
using System.Collections.Generic;

namespace LotoLibrary.Models
{
    /// <summary>
    /// Métricas consolidadas de performance
    /// </summary>
    public class MetricasPerformance
    {
        public string NomeEstrategia { get; set; } = string.Empty;
        public int TotalTestes { get; set; }
        public double TaxaAcertoMedia { get; set; }
        public double DesvioPadrao { get; set; }
        public double TaxaAcertoMinima { get; set; }
        public double TaxaAcertoMaxima { get; set; }
        public double MediaAcertos { get; set; }
        public int MelhorResultado { get; set; }
        public int PiorResultado { get; set; }

        // Distribuição de acertos
        public Dictionary<int, int> DistribuicaoAcertos { get; set; } = new();

        // Métricas avançadas
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }

        // Comparação com baseline
        public double GanhoSobreAleatorio { get; set; }
        public double GanhoSobreFrequencia { get; set; }

        // Consistência temporal
        public double VariabilidadeTemporal { get; set; }
        public List<double> TendenciaTemporeal { get; set; } = new();

        public override string ToString()
        {
            return $"{NomeEstrategia}: {TaxaAcertoMedia:P2} ± {DesvioPadrao:P2} " +
                   $"(Acertos: {MediaAcertos:F1}/15, Melhor: {MelhorResultado})";
        }
    }
}
