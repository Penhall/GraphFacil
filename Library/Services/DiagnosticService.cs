// D:\PROJETOS\GraphFacil\Library\Services\DiagnosticService.cs - Novo arquivo para diagnosticar o bug
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Serviço para diagnosticar e corrigir o bug das dezenas 1-9
    /// </summary>
    public static class DiagnosticService
    {
        /// <summary>
        /// Analisa a distribuição de dezenas nos palpites gerados
        /// </summary>
        public static DiagnosticReport AnalisarDistribuicaoDezenas(List<List<int>> palpitesGerados, int amostraSize = 100)
        {
            var report = new DiagnosticReport();
            
            // Contador de frequência por dezena
            var frequenciaDezenas = new Dictionary<int, int>();
            for (int i = 1; i <= 25; i++)
            {
                frequenciaDezenas[i] = 0;
            }

            int totalPalpites = Math.Min(palpitesGerados.Count, amostraSize);
            int totalDezenasSelecionadas = 0;

            foreach (var palpite in palpitesGerados.Take(totalPalpites))
            {
                if (palpite?.Any() == true)
                {
                    foreach (var dezena in palpite)
                    {
                        if (dezena >= 1 && dezena <= 25)
                        {
                            frequenciaDezenas[dezena]++;
                            totalDezenasSelecionadas++;
                        }
                    }
                }
            }

            // Análise estatística
            report.TotalPalpitesAnalisados = totalPalpites;
            report.TotalDezenasContadas = totalDezenasSelecionadas;
            report.MediaDezenasPorPalpite = totalPalpites > 0 ? (double)totalDezenasSelecionadas / totalPalpites : 0;

            // Distribuição por faixa
            report.Dezenas1a9 = frequenciaDezenas.Where(kv => kv.Key <= 9).Sum(kv => kv.Value);
            report.Dezenas10a18 = frequenciaDezenas.Where(kv => kv.Key >= 10 && kv.Key <= 18).Sum(kv => kv.Value);
            report.Dezenas19a25 = frequenciaDezenas.Where(kv => kv.Key >= 19).Sum(kv => kv.Value);

            // Percentuais
            if (totalDezenasSelecionadas > 0)
            {
                report.Percentual1a9 = (double)report.Dezenas1a9 / totalDezenasSelecionadas * 100;
                report.Percentual10a18 = (double)report.Dezenas10a18 / totalDezenasSelecionadas * 100;
                report.Percentual19a25 = (double)report.Dezenas19a25 / totalDezenasSelecionadas * 100;
            }

            // Dezenas nunca selecionadas
            report.DezenasNuncaSelecionadas = frequenciaDezenas.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToList();
            
            // Dezenas mais/menos frequentes
            report.DezenaMaisFrequente = frequenciaDezenas.OrderByDescending(kv => kv.Value).First();
            report.DezenaMenosFrequente = frequenciaDezenas.OrderBy(kv => kv.Value).First();

            // Detalhamento por dezena
            report.FrequenciaPorDezena = new Dictionary<int, FrequenciaInfo>();
            foreach (var kv in frequenciaDezenas)
            {
                report.FrequenciaPorDezena[kv.Key] = new FrequenciaInfo
                {
                    Dezena = kv.Key,
                    Frequencia = kv.Value,
                    Percentual = totalDezenasSelecionadas > 0 ? (double)kv.Value / totalDezenasSelecionadas * 100 : 0,
                    FrequenciaEsperada = (double)totalDezenasSelecionadas / 25, // Distribuição uniforme esperada
                    Desvio = kv.Value - ((double)totalDezenasSelecionadas / 25)
                };
            }

            // Avaliação do problema
            report.TemProblemaDistribuicao = report.DezenasNuncaSelecionadas.Count > 5 || 
                                           report.Percentual1a9 < 20 || // Esperado ~36% (9/25)
                                           report.Percentual10a18 < 20;  // Esperado ~36% (9/25)

            report.GravidadeProblema = CalcularGravidadeProblema(report);

            return report;
        }

        /// <summary>
        /// Testa especificamente o algoritmo atual de geração de palpites
        /// </summary>
        public static DiagnosticReport TestarAlgoritmoAtual(int numTestes = 100)
        {
            var palpitesGerados = new List<List<int>>();

            try
            {
                // Garantir que o sistema está inicializado
                if (!Infra.arLoto.Any())
                {
                    Infra.CarregarConcursos();
                }

                // Simular geração de múltiplos palpites
                for (int i = 0; i < numTestes; i++)
                {
                    try
                    {
                        // Aqui você deve chamar o método atual de geração de palpites
                        // TEMPORÁRIO: Vou criar um método genérico que pode ser substituído
                        var palpite = GerarPalpiteParaTeste();
                        if (palpite?.Any() == true)
                        {
                            palpitesGerados.Add(palpite);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao gerar palpite {i}: {ex.Message}");
                    }
                }

                return AnalisarDistribuicaoDezenas(palpitesGerados, numTestes);
            }
            catch (Exception ex)
            {
                var errorReport = new DiagnosticReport();
                errorReport.ErroExecucao = $"Erro durante teste: {ex.Message}";
                errorReport.TemProblemaDistribuicao = true;
                errorReport.GravidadeProblema = GravidadeProblema.Critica;
                return errorReport;
            }
        }

        /// <summary>
        /// Método temporário para testar - SUBSTITUIR pelo método real de geração
        /// </summary>
        private static List<int> GerarPalpiteParaTeste()
        {
            // ATENÇÃO: Este é um método temporário!
            // Substituir pela chamada real do seu algoritmo de geração de palpites
            
            // Exemplo usando o MetronomoEngine atual (adapte conforme necessário)
            var random = new Random();
            var palpite = new List<int>();
            
            // Simular seleção de 15 dezenas
            while (palpite.Count < 15)
            {
                var dezena = random.Next(1, 26);
                if (!palpite.Contains(dezena))
                {
                    palpite.Add(dezena);
                }
            }
            
            return palpite.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Gera relatório detalhado em formato texto
        /// </summary>
        public static string GerarRelatorioTexto(DiagnosticReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== RELATÓRIO DE DIAGNÓSTICO - DISTRIBUIÇÃO DE DEZENAS ===");
            sb.AppendLine();
            
            sb.AppendLine($"📊 RESUMO GERAL:");
            sb.AppendLine($"   Total de palpites analisados: {report.TotalPalpitesAnalisados}");
            sb.AppendLine($"   Total de dezenas contadas: {report.TotalDezenasContadas}");
            sb.AppendLine($"   Média de dezenas por palpite: {report.MediaDezenasPorPalpite:F2}");
            sb.AppendLine();

            sb.AppendLine($"📈 DISTRIBUIÇÃO POR FAIXA:");
            sb.AppendLine($"   Dezenas 1-9:   {report.Dezenas1a9,4} ({report.Percentual1a9:F1}%) - Esperado ~36%");
            sb.AppendLine($"   Dezenas 10-18: {report.Dezenas10a18,4} ({report.Percentual10a18:F1}%) - Esperado ~36%");
            sb.AppendLine($"   Dezenas 19-25: {report.Dezenas19a25,4} ({report.Percentual19a25:F1}%) - Esperado ~28%");
            sb.AppendLine();

            if (report.DezenasNuncaSelecionadas.Any())
            {
                sb.AppendLine($"🚨 DEZENAS NUNCA SELECIONADAS: {string.Join(", ", report.DezenasNuncaSelecionadas)}");
                sb.AppendLine();
            }

            sb.AppendLine($"🔝 EXTREMOS:");
            sb.AppendLine($"   Mais frequente: Dezena {report.DezenaMaisFrequente.Key} ({report.DezenaMaisFrequente.Value} vezes)");
            sb.AppendLine($"   Menos frequente: Dezena {report.DezenaMenosFrequente.Key} ({report.DezenaMenosFrequente.Value} vezes)");
            sb.AppendLine();

            sb.AppendLine($"⚠️ DIAGNÓSTICO:");
            sb.AppendLine($"   Tem problema de distribuição: {(report.TemProblemaDistribuicao ? "SIM" : "NÃO")}");
            sb.AppendLine($"   Gravidade: {report.GravidadeProblema}");
            
            if (!string.IsNullOrEmpty(report.ErroExecucao))
            {
                sb.AppendLine($"   Erro: {report.ErroExecucao}");
            }

            sb.AppendLine();
            sb.AppendLine("📋 DETALHAMENTO POR DEZENA:");
            foreach (var info in report.FrequenciaPorDezena.OrderBy(kv => kv.Key))
            {
                var dezena = info.Value;
                var status = dezena.Frequencia == 0 ? "NUNCA" : 
                           Math.Abs(dezena.Desvio) > 2 ? "ANÔMALO" : "OK";
                sb.AppendLine($"   Dezena {dezena.Dezena:D2}: {dezena.Frequencia,3} vezes ({dezena.Percentual:F1}%) " +
                             $"Desvio: {dezena.Desvio:+0.0;-0.0} [{status}]");
            }

            return sb.ToString();
        }

        private static GravidadeProblema CalcularGravidadeProblema(DiagnosticReport report)
        {
            if (!string.IsNullOrEmpty(report.ErroExecucao))
                return GravidadeProblema.Critica;

            if (report.DezenasNuncaSelecionadas.Count >= 10)
                return GravidadeProblema.Critica;

            if (report.DezenasNuncaSelecionadas.Count >= 5 || 
                report.Percentual1a9 < 15 || report.Percentual10a18 < 15)
                return GravidadeProblema.Alta;

            if (report.DezenasNuncaSelecionadas.Count >= 2 || 
                report.Percentual1a9 < 25 || report.Percentual10a18 < 25)
                return GravidadeProblema.Media;

            return GravidadeProblema.Baixa;
        }
    }

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

    public class FrequenciaInfo
    {
        public int Dezena { get; set; }
        public int Frequencia { get; set; }
        public double Percentual { get; set; }
        public double FrequenciaEsperada { get; set; }
        public double Desvio { get; set; }
    }

    public enum GravidadeProblema
    {
        Baixa,
        Media,
        Alta,
        Critica
    }
}