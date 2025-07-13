// D:\PROJETOS\GraphFacil\Library\Suporte\ConcursoResult.cs
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotoLibrary.Suporte
{
    /// <summary>
    /// Resultado de um concurso da Lotofácil para análise e validação
    /// Usado pelos sistemas de meta-learning e validação
    /// </summary>
    public class ConcursoResult
    {
        #region Basic Properties
        public int Concurso { get; set; }
        public DateTime DataSorteio { get; set; }
        public List<int> DezenasOrdenadas { get; set; } = new List<int>();
        public List<int> DezenasNaoSorteadas { get; set; } = new List<int>();
        #endregion

        #region Prediction Context
        public List<int> PredicaoModelo { get; set; } = new List<int>();
        public string ModeloUtilizado { get; set; } = "";
        public double ConfiancaModelo { get; set; }
        public int Acertos { get; set; }
        public double TaxaAcerto { get; set; }
        #endregion

        #region Analysis Properties
        public Dictionary<string, double> MetricasEstatisticas { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, object> DadosComplementares { get; set; } = new Dictionary<string, object>();
        public string ContextoDetectado { get; set; } = "";
        public double VolatilidadeDetectada { get; set; }
        #endregion

        #region Constructors
        public ConcursoResult()
        {
            InitializeNonDrawnNumbers();
        }

        public ConcursoResult(int concurso, DateTime dataSorteio, List<int> dezenas)
        {
            Concurso = concurso;
            DataSorteio = dataSorteio;
            DezenasOrdenadas = dezenas?.OrderBy(x => x).ToList() ?? new List<int>();
            InitializeNonDrawnNumbers();
            CalculateBasicMetrics();
        }

        /// <summary>
        /// Construtor completo para resultados de validação
        /// </summary>
        public ConcursoResult(int concurso, DateTime dataSorteio, List<int> dezenasReais, 
                             List<int> predicao, string modelo, double confianca)
        {
            Concurso = concurso;
            DataSorteio = dataSorteio;
            DezenasOrdenadas = dezenasReais?.OrderBy(x => x).ToList() ?? new List<int>();
            PredicaoModelo = predicao?.OrderBy(x => x).ToList() ?? new List<int>();
            ModeloUtilizado = modelo;
            ConfiancaModelo = confianca;
            
            InitializeNonDrawnNumbers();
            CalculateAccuracy();
            CalculateBasicMetrics();
        }
        #endregion

        #region Helper Methods
        private void InitializeNonDrawnNumbers()
        {
            DezenasNaoSorteadas = Enumerable.Range(1, 25)
                .Except(DezenasOrdenadas)
                .ToList();
        }

        private void CalculateAccuracy()
        {
            if (DezenasOrdenadas.Any() && PredicaoModelo.Any())
            {
                Acertos = DezenasOrdenadas.Intersect(PredicaoModelo).Count();
                TaxaAcerto = Acertos / 15.0;
            }
        }

        private void CalculateBasicMetrics()
        {
            if (!DezenasOrdenadas.Any()) return;

            // Métricas estatísticas básicas
            MetricasEstatisticas["Media"] = DezenasOrdenadas.Average();
            MetricasEstatisticas["Mediana"] = CalculateMedian(DezenasOrdenadas);
            MetricasEstatisticas["DesvioPadrao"] = CalculateStandardDeviation(DezenasOrdenadas);
            MetricasEstatisticas["Amplitude"] = DezenasOrdenadas.Max() - DezenasOrdenadas.Min();
            
            // Distribuição por faixas
            MetricasEstatisticas["Dezenas1a10"] = DezenasOrdenadas.Count(d => d <= 10);
            MetricasEstatisticas["Dezenas11a20"] = DezenasOrdenadas.Count(d => d >= 11 && d <= 20);
            MetricasEstatisticas["Dezenas21a25"] = DezenasOrdenadas.Count(d => d >= 21);
            
            // Análise de sequências
            MetricasEstatisticas["SequenciasConsecutivas"] = CountConsecutiveSequences();
            MetricasEstatisticas["MaiorGap"] = CalculateLargestGap();
        }

        private double CalculateMedian(List<int> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int count = sorted.Count;
            
            if (count % 2 == 0)
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            else
                return sorted[count / 2];
        }

        private double CalculateStandardDeviation(List<int> values)
        {
            if (values.Count < 2) return 0.0;
            
            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
            return Math.Sqrt(variance);
        }

        private int CountConsecutiveSequences()
        {
            if (DezenasOrdenadas.Count < 2) return 0;
            
            var sorted = DezenasOrdenadas.OrderBy(x => x).ToList();
            int sequences = 0;
            
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                if (sorted[i + 1] == sorted[i] + 1)
                    sequences++;
            }
            
            return sequences;
        }

        private int CalculateLargestGap()
        {
            if (DezenasOrdenadas.Count < 2) return 0;
            
            var sorted = DezenasOrdenadas.OrderBy(x => x).ToList();
            int maxGap = 0;
            
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                int gap = sorted[i + 1] - sorted[i] - 1;
                maxGap = Math.Max(maxGap, gap);
            }
            
            return maxGap;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria ConcursoResult a partir de um Lance existente
        /// </summary>
        public static ConcursoResult FromLance(Models.Lance lance)
        {
            return new ConcursoResult
            {
                Concurso = lance.Id,
                DataSorteio = DateTime.Now, // Lance não tem data, usar atual
                DezenasOrdenadas = lance.Lista?.OrderBy(x => x).ToList() ?? new List<int>()
            };
        }

        /// <summary>
        /// Cria ConcursoResult a partir de um Lotofacil
        /// </summary>
        public static ConcursoResult FromLotofacil(Models.Lotofacil lotofacil)
        {
            var dezenas = new List<int>();
            
            if (lotofacil.listaDezenas != null)
            {
                foreach (var dezenaStr in lotofacil.listaDezenas)
                {
                    if (int.TryParse(dezenaStr, out int dezena))
                        dezenas.Add(dezena);
                }
            }
            
            DateTime.TryParse(lotofacil.dataApuracao, out DateTime data);
            
            return new ConcursoResult(lotofacil.numero, data, dezenas);
        }

        /// <summary>
        /// Cria lista de ConcursoResult para validação de modelo
        /// </summary>
        public static List<ConcursoResult> CreateValidationResults(
            List<Models.Lance> historico, 
            Dictionary<int, List<int>> predicoes, 
            string modelName)
        {
            var results = new List<ConcursoResult>();
            
            foreach (var lance in historico)
            {
                if (predicoes.ContainsKey(lance.Id))
                {
                    var result = new ConcursoResult(
                        lance.Id,
                        DateTime.Now,
                        lance.Lista,
                        predicoes[lance.Id],
                        modelName,
                        0.75 // Confiança padrão
                    );
                    results.Add(result);
                }
            }
            
            return results;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Verifica se é um resultado válido
        /// </summary>
        public bool IsValid()
        {
            return Concurso > 0 && 
                   DezenasOrdenadas.Count == 15 && 
                   DezenasOrdenadas.All(d => d >= 1 && d <= 25) &&
                   DezenasOrdenadas.Distinct().Count() == 15;
        }

        /// <summary>
        /// Gera resumo textual do resultado
        /// </summary>
        public string GetSummary()
        {
            var summary = $"Concurso {Concurso} ({DataSorteio:dd/MM/yyyy})\n";
            summary += $"Dezenas: {string.Join(", ", DezenasOrdenadas)}\n";
            
            if (PredicaoModelo.Any())
            {
                summary += $"Predição ({ModeloUtilizado}): {string.Join(", ", PredicaoModelo)}\n";
                summary += $"Acertos: {Acertos}/15 ({TaxaAcerto:P2})\n";
            }
            
            return summary;
        }

        /// <summary>
        /// Calcula similaridade com outro resultado
        /// </summary>
        public double CalculateSimilarity(ConcursoResult other)
        {
            if (other == null || !IsValid() || !other.IsValid())
                return 0.0;
                
            var intersection = DezenasOrdenadas.Intersect(other.DezenasOrdenadas).Count();
            return intersection / 15.0;
        }
        #endregion

        #region Object Overrides
        public override string ToString()
        {
            return $"Concurso {Concurso}: {string.Join(",", DezenasOrdenadas)}";
        }

        public override bool Equals(object obj)
        {
            if (obj is ConcursoResult other)
                return Concurso == other.Concurso;
            return false;
        }

        public override int GetHashCode()
        {
            return Concurso.GetHashCode();
        }
        #endregion
    }
}
