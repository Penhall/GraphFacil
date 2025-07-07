using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Resultado da validação de um palpite individual
    /// </summary>
    public class ResultadoValidacao
    {
        public int ConcursoId { get; set; }
        public List<int> PalpiteGerado { get; set; } = new();
        public List<int> ResultadoReal { get; set; } = new();
        public int Acertos { get; set; }
        public double TaxaAcerto { get; set; }
        public List<int> NumerosAcertados { get; set; } = new();
        public List<int> NumerosPerdidos { get; set; } = new();
        public DateTime DataTeste { get; set; }
        public string TipoEstrategia { get; set; } = "Osciladores";
    }

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

    /// <summary>
    /// Sistema completo de validação e métricas para osciladores
    /// </summary>
    public partial class ValidationMetricsService
    {
        private readonly Random _random = new Random(42); // Seed fixo para reprodutibilidade

        /// <summary>
        /// Executa validação completa dos osciladores
        /// </summary>
        public MetricasPerformance ValidarOsciladores(
            List<Lance> dadosTreino,
            List<Lance> dadosValidacao,
            int numeroTestes = 50)
        {
            var resultados = new List<ResultadoValidacao>();
            var engine = new OscillatorEngine(new Lances(dadosTreino));

            Console.WriteLine($"Iniciando validação com {numeroTestes} testes...");

            for (int i = 0; i < Math.Min(numeroTestes, dadosValidacao.Count); i++)
            {
                var concursoTeste = dadosValidacao[i];

                // Usar dados até o concurso anterior para treinar
                var dadosAteAgora = dadosTreino.Concat(dadosValidacao.Take(i)).ToList();
                var oscillators = engine.InicializarOsciladores();

                // Simular estratégias nos dados históricos
                SimularEstrategias(oscillators, dadosAteAgora.TakeLast(20).ToList());

                // Gerar palpite
                var palpite = OscillatorStrategy.GerarPalpiteValidacao(oscillators, dadosAteAgora);

                // Calcular resultado
                var resultado = new ResultadoValidacao
                {
                    ConcursoId = concursoTeste.Id,
                    PalpiteGerado = palpite,
                    ResultadoReal = concursoTeste.Lista,
                    Acertos = palpite.Intersect(concursoTeste.Lista).Count(),
                    DataTeste = DateTime.Now,
                    TipoEstrategia = "Osciladores"
                };

                resultado.TaxaAcerto = resultado.Acertos / 15.0;
                resultado.NumerosAcertados = palpite.Intersect(concursoTeste.Lista).ToList();
                resultado.NumerosPerdidos = concursoTeste.Lista.Except(palpite).ToList();

                resultados.Add(resultado);

                Console.WriteLine($"Teste {i + 1}/{numeroTestes}: {resultado.Acertos} acertos");
            }

            return CalcularMetricas(resultados, "Osciladores Sincronizados");
        }

        /// <summary>
        /// Compara osciladores com estratégias baseline
        /// </summary>
        public Dictionary<string, MetricasPerformance> CompararEstrategias(
            List<Lance> dadosTreino,
            List<Lance> dadosValidacao,
            int numeroTestes = 50)
        {
            var resultados = new Dictionary<string, MetricasPerformance>();

            // 1. Validar Osciladores
            resultados["Osciladores"] = ValidarOsciladores(dadosTreino, dadosValidacao, numeroTestes);

            // 2. Baseline Aleatório
            resultados["Aleatório"] = ValidarEstrategiaAleatoria(dadosValidacao, numeroTestes);

            // 3. Baseline por Frequência
            resultados["Frequência"] = ValidarEstrategiaFrequencia(dadosTreino, dadosValidacao, numeroTestes);

            // 4. Baseline Últimos Sorteados
            resultados["Últimos"] = ValidarEstrategiaUltimos(dadosTreino, dadosValidacao, numeroTestes);

            // Calcular ganhos relativos
            var baselineAleatorio = resultados["Aleatório"].TaxaAcertoMedia;
            var baselineFrequencia = resultados["Frequência"].TaxaAcertoMedia;

            foreach (var estrategia in resultados.Values)
            {
                estrategia.GanhoSobreAleatorio = (estrategia.TaxaAcertoMedia - baselineAleatorio) / baselineAleatorio;
                estrategia.GanhoSobreFrequencia = (estrategia.TaxaAcertoMedia - baselineFrequencia) / baselineFrequencia;
            }

            return resultados;
        }

        /// <summary>
        /// Valida estratégia aleatória como baseline
        /// </summary>
        private MetricasPerformance ValidarEstrategiaAleatoria(List<Lance> dadosValidacao, int numeroTestes)
        {
            var resultados = new List<ResultadoValidacao>();

            for (int i = 0; i < Math.Min(numeroTestes, dadosValidacao.Count); i++)
            {
                var concursoTeste = dadosValidacao[i];
                var palpiteAleatorio = GerarPalpiteAleatorio();

                var resultado = new ResultadoValidacao
                {
                    ConcursoId = concursoTeste.Id,
                    PalpiteGerado = palpiteAleatorio,
                    ResultadoReal = concursoTeste.Lista,
                    Acertos = palpiteAleatorio.Intersect(concursoTeste.Lista).Count(),
                    TaxaAcerto = palpiteAleatorio.Intersect(concursoTeste.Lista).Count() / 15.0,
                    TipoEstrategia = "Aleatório"
                };

                resultados.Add(resultado);
            }

            return CalcularMetricas(resultados, "Aleatório");
        }

        /// <summary>
        /// Valida estratégia baseada em frequência histórica
        /// </summary>
        private MetricasPerformance ValidarEstrategiaFrequencia(
            List<Lance> dadosTreino,
            List<Lance> dadosValidacao,
            int numeroTestes)
        {
            var resultados = new List<ResultadoValidacao>();

            // Calcular frequências dos números nos dados de treino
            var frequencias = dadosTreino
                .SelectMany(l => l.Lista)
                .GroupBy(n => n)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(kvp => kvp.Value)
                .ToList();

            var numerosMaisFrequentes = frequencias.Take(15).Select(kvp => kvp.Key).ToList();

            for (int i = 0; i < Math.Min(numeroTestes, dadosValidacao.Count); i++)
            {
                var concursoTeste = dadosValidacao[i];

                var resultado = new ResultadoValidacao
                {
                    ConcursoId = concursoTeste.Id,
                    PalpiteGerado = numerosMaisFrequentes,
                    ResultadoReal = concursoTeste.Lista,
                    Acertos = numerosMaisFrequentes.Intersect(concursoTeste.Lista).Count(),
                    TaxaAcerto = numerosMaisFrequentes.Intersect(concursoTeste.Lista).Count() / 15.0,
                    TipoEstrategia = "Frequência"
                };

                resultados.Add(resultado);
            }

            return CalcularMetricas(resultados, "Frequência Histórica");
        }

        /// <summary>
        /// Valida estratégia baseada nos últimos números sorteados
        /// </summary>
        private MetricasPerformance ValidarEstrategiaUltimos(
            List<Lance> dadosTreino,
            List<Lance> dadosValidacao,
            int numeroTestes)
        {
            var resultados = new List<ResultadoValidacao>();

            for (int i = 0; i < Math.Min(numeroTestes, dadosValidacao.Count); i++)
            {
                var concursoTeste = dadosValidacao[i];

                // Usar os últimos 5 sorteios para gerar palpite
                var ultimosSorteios = dadosTreino.Concat(dadosValidacao.Take(i))
                    .TakeLast(5)
                    .SelectMany(l => l.Lista)
                    .GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .Take(15)
                    .Select(g => g.Key)
                    .ToList();

                var resultado = new ResultadoValidacao
                {
                    ConcursoId = concursoTeste.Id,
                    PalpiteGerado = ultimosSorteios,
                    ResultadoReal = concursoTeste.Lista,
                    Acertos = ultimosSorteios.Intersect(concursoTeste.Lista).Count(),
                    TaxaAcerto = ultimosSorteios.Intersect(concursoTeste.Lista).Count() / 15.0,
                    TipoEstrategia = "Últimos"
                };

                resultados.Add(resultado);
            }

            return CalcularMetricas(resultados, "Últimos Sorteados");
        }

        /// <summary>
        /// Calcula todas as métricas de performance
        /// </summary>
        private MetricasPerformance CalcularMetricas(List<ResultadoValidacao> resultados, string nomeEstrategia)
        {
            if (!resultados.Any()) return new MetricasPerformance { NomeEstrategia = nomeEstrategia };

            var acertos = resultados.Select(r => r.Acertos).ToList();
            var taxasAcerto = resultados.Select(r => r.TaxaAcerto).ToList();

            var metricas = new MetricasPerformance
            {
                NomeEstrategia = nomeEstrategia,
                TotalTestes = resultados.Count,
                TaxaAcertoMedia = taxasAcerto.Average(),
                DesvioPadrao = CalcularDesvioPadrao(taxasAcerto),
                TaxaAcertoMinima = taxasAcerto.Min(),
                TaxaAcertoMaxima = taxasAcerto.Max(),
                MediaAcertos = acertos.Average(),
                MelhorResultado = acertos.Max(),
                PiorResultado = acertos.Min(),
                DistribuicaoAcertos = acertos.GroupBy(a => a).ToDictionary(g => g.Key, g => g.Count())
            };

            // Calcular métricas avançadas
            CalcularMetricasAvancadas(metricas, resultados);

            // Calcular variabilidade temporal
            metricas.VariabilidadeTemporal = CalcularDesvioPadrao(taxasAcerto);
            metricas.TendenciaTemporeal = CalcularTendencia(taxasAcerto);

            return metricas;
        }

        /// <summary>
        /// Calcula métricas avançadas (Precision, Recall, F1)
        /// </summary>
        private void CalcularMetricasAvancadas(MetricasPerformance metricas, List<ResultadoValidacao> resultados)
        {
            // Para loteria, adaptamos as métricas tradicionais
            var totalNumerosEscolhidos = resultados.Sum(r => r.PalpiteGerado.Count);
            var totalNumerosCorretos = resultados.Sum(r => r.NumerosAcertados.Count);
            var totalNumerosReais = resultados.Sum(r => r.ResultadoReal.Count);

            metricas.Precision = totalNumerosEscolhidos > 0 ?
                totalNumerosCorretos / (double)totalNumerosEscolhidos : 0;

            metricas.Recall = totalNumerosReais > 0 ?
                totalNumerosCorretos / (double)totalNumerosReais : 0;

            metricas.F1Score = (metricas.Precision + metricas.Recall) > 0 ?
                2 * (metricas.Precision * metricas.Recall) / (metricas.Precision + metricas.Recall) : 0;
        }

        /// <summary>
        /// Simula aplicação de estratégias nos osciladores
        /// </summary>
        private void SimularEstrategias(List<DezenaOscilante> oscillators, List<Lance> historico)
        {
            if (!historico.Any()) return;

            // Aplicar estratégias disponíveis
            OscillatorStrategy.AplicarTendenciaCurtoPrazo(oscillators, historico);
            OscillatorStrategy.AplicarQuentesFrios(oscillators, historico);

            // Simular algumas iterações de sincronização
            for (int i = 0; i < 10; i++)
            {
                foreach (var dezena in oscillators)
                {
                    var influencia = oscillators
                        .Where(d => d.Numero != dezena.Numero)
                        .Sum(d => Math.Sin((d.Fase - dezena.Fase) * Math.PI / 180) * d.ForcaSincronizacao * 0.1);

                    dezena.AplicarInfluencia(influencia);
                    dezena.AtualizarFase();
                }
            }
        }

        /// <summary>
        /// Gera palpite completamente aleatório
        /// </summary>
        private List<int> GerarPalpiteAleatorio()
        {
            return Enumerable.Range(1, 25)
                .OrderBy(x => _random.Next())
                .Take(15)
                .OrderBy(x => x)
                .ToList();
        }

        /// <summary>
        /// Calcula desvio padrão
        /// </summary>
        private double CalcularDesvioPadrao(List<double> valores)
        {
            if (!valores.Any()) return 0;

            var media = valores.Average();
            var somaQuadrados = valores.Sum(v => Math.Pow(v - media, 2));
            return Math.Sqrt(somaQuadrados / valores.Count);
        }

        /// <summary>
        /// Calcula tendência temporal usando regressão linear simples
        /// </summary>
        private List<double> CalcularTendencia(List<double> valores)
        {
            if (valores.Count < 2) return valores;

            var n = valores.Count;
            var x = Enumerable.Range(0, n).Select(i => (double)i).ToList();
            var y = valores;

            var mediaX = x.Average();
            var mediaY = y.Average();

            var numerador = x.Zip(y, (xi, yi) => (xi - mediaX) * (yi - mediaY)).Sum();
            var denominador = x.Sum(xi => Math.Pow(xi - mediaX, 2));

            if (denominador == 0) return valores;

            var inclinacao = numerador / denominador;
            var intercepto = mediaY - inclinacao * mediaX;

            return x.Select(xi => inclinacao * xi + intercepto).ToList();
        }
    }
}