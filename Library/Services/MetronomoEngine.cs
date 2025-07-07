// LotoLibrary/Services/MetronomoEngine.cs
using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Motor principal que gerencia todos os metrônomos individuais
    /// Substitui o sistema de osciladores por análise probabilística real
    /// </summary>
    public partial class MetronomoEngine : ObservableObject
    {
        #region Properties
        [ObservableProperty]
        private Dictionary<int, MetronomoIndividual> _metronomos = new();

        [ObservableProperty]
        private List<int> _ultimoPalpite = new();

        [ObservableProperty]
        private double _confiancaGeralPalpite;

        [ObservableProperty]
        private int _concursoAlvo;

        [ObservableProperty]
        private bool _isInicializado;

        [ObservableProperty]
        private string _statusEngine = "Aguardando inicialização...";

        private readonly Lances _historicoCompleto;
        private readonly List<Lance> _dadosTreino;
        private readonly List<Lance> _dadosValidacao;
        #endregion

        #region Constructor
        public MetronomoEngine()
        {
            _historicoCompleto = new Lances();
        }

        public MetronomoEngine(Lances historico)
        {
            _historicoCompleto = historico;
            (_dadosTreino, _dadosValidacao) = SplitData(historico, 100);
            ConcursoAlvo = (_historicoCompleto.LastOrDefault()?.Id ?? 3000) + 1;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Inicializa todos os 25 metrônomos com base no histórico real
        /// </summary>
        public async Task<bool> InicializarMetronomosAsync()
        {
            try
            {
                StatusEngine = "Inicializando metrônomos...";
                Metronomos.Clear();

                await Task.Run(() =>
                {
                    // Criar metrônomo para cada dezena (1-25)
                    for (int dezena = 1; dezena <= 25; dezena++)
                    {
                        var historicoAparicoes = ExtrairHistoricoAparicoes(dezena);
                        var metronomo = new MetronomoIndividual(dezena, historicoAparicoes);
                        
                        Metronomos[dezena] = metronomo;
                        
                        // Update status para mostrar progresso
                        StatusEngine = $"Analisando dezena {dezena}/25...";
                    }
                });

                // Atualizar estado atual para o concurso alvo
                AtualizarEstadoAtual();
                
                IsInicializado = true;
                StatusEngine = $"✅ {Metronomos.Count} metrônomos inicializados";
                
                return true;
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na inicialização: {ex.Message}";
                return false;
            }
        }

        private List<int> ExtrairHistoricoAparicoes(int dezena)
        {
            return _dadosTreino
                .Where(lance => lance.Lista.Contains(dezena))
                .Select(lance => lance.Id)
                .OrderBy(num => num)
                .ToList();
        }

        private (List<Lance> treino, List<Lance> validacao) SplitData(Lances historico, int validacaoSize)
        {
            var lista = historico.ToList();
            var treino = lista.SkipLast(validacaoSize).ToList();
            var validacao = lista.TakeLast(validacaoSize).ToList();
            return (treino, validacao);
        }
        #endregion

        #region Core Functionality
        /// <summary>
        /// Gera palpite para o concurso alvo baseado nas probabilidades individuais
        /// </summary>
        public List<int> GerarPalpiteOtimizado()
        {
            if (!IsInicializado || !Metronomos.Any())
            {
                StatusEngine = "❌ Engine não inicializado";
                return new List<int>();
            }

            try
            {
                // 1. Calcular probabilidades para todas as dezenas
                var probabilidades = new Dictionary<int, double>();
                foreach (var metronomo in Metronomos.Values)
                {
                    probabilidades[metronomo.Numero] = metronomo.CalcularProbabilidadePara(ConcursoAlvo);
                }

                // 2. Aplicar estratégias de otimização
                probabilidades = AplicarEstrategiaGrupos(probabilidades);
                probabilidades = AplicarEstrategiaTendencias(probabilidades);
                probabilidades = AplicarEstrategiaEquilibrio(probabilidades);

                // 3. Selecionar top 15 com base nas probabilidades ajustadas
                UltimoPalpite = probabilidades
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(15)
                    .Select(kvp => kvp.Key)
                    .OrderBy(x => x)
                    .ToList();

                // 4. Calcular confiança geral do palpite
                ConfiancaGeralPalpite = CalcularConfiancaPalpite(UltimoPalpite);

                StatusEngine = $"✅ Palpite gerado para concurso {ConcursoAlvo} (Confiança: {ConfiancaGeralPalpite:P1})";
                
                return UltimoPalpite;
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro ao gerar palpite: {ex.Message}";
                return new List<int>();
            }
        }

        /// <summary>
        /// Atualiza todos os metrônomos com resultado de um sorteio
        /// </summary>
        public void ProcessarNovoSorteio(Lance novoSorteio)
        {
            if (!IsInicializado) return;

            foreach (var metronomo in Metronomos.Values)
            {
                bool foiSorteada = novoSorteio.Lista.Contains(metronomo.Numero);
                metronomo.AtualizarComSorteio(novoSorteio.Id, foiSorteada);
            }

            // Atualizar concurso alvo
            ConcursoAlvo = novoSorteio.Id + 1;
            AtualizarEstadoAtual();

            StatusEngine = $"✅ Processado sorteio {novoSorteio.Id}";
        }

        /// <summary>
        /// Simula múltiplos sorteios para validação
        /// </summary>
        public async Task<ResultadoValidacao> ValidarModeloAsync()
        {
            if (!IsInicializado || !_dadosValidacao.Any())
            {
                return new ResultadoValidacao { Sucesso = false, Erro = "Dados insuficientes para validação" };
            }

            var resultado = new ResultadoValidacao();
            
            try
            {
                await Task.Run(() =>
                {
                    var acertos = new List<int>();
                    var palpitesGerados = new List<List<int>>();

                    foreach (var sorteio in _dadosValidacao)
                    {
                        // Gerar palpite para este sorteio
                        ConcursoAlvo = sorteio.Id;
                        var palpite = GerarPalpiteOtimizado();
                        palpitesGerados.Add(palpite);

                        // Contar acertos
                        int numAcertos = palpite.Intersect(sorteio.Lista).Count();
                        acertos.Add(numAcertos);

                        // Simular atualização com o resultado real
                        ProcessarNovoSorteio(sorteio);
                    }

                    // Calcular estatísticas
                    resultado.Sucesso = true;
                    resultado.TotalSorteios = _dadosValidacao.Count;
                    resultado.AcertosPorSorteio = acertos;
                    resultado.MediaAcertos = acertos.Average();
                    resultado.MelhorAcerto = acertos.Max();
                    resultado.PiorAcerto = acertos.Min();
                    resultado.DesvioPadrao = CalcularDesvioPadrao(acertos);
                    resultado.TaxaSucesso = acertos.Count(a => a >= 11) / (double)acertos.Count;
                    resultado.PalpitesGerados = palpitesGerados;
                });

                StatusEngine = $"✅ Validação concluída: {resultado.MediaAcertos:F1} acertos em média";
            }
            catch (Exception ex)
            {
                resultado.Sucesso = false;
                resultado.Erro = ex.Message;
                StatusEngine = $"❌ Erro na validação: {ex.Message}";
            }

            return resultado;
        }
        #endregion

        #region Optimization Strategies
        private Dictionary<int, double> AplicarEstrategiaGrupos(Dictionary<int, double> probabilidades)
        {
            // Estratégia: Dezenas que costumam sair juntas recebem boost
            var gruposFrequentes = new Dictionary<string, List<int>>
            {
                { "baixas", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 } },
                { "medias", new List<int> { 9, 10, 11, 12, 13, 14, 15, 16, 17 } },
                { "altas", new List<int> { 18, 19, 20, 21, 22, 23, 24, 25 } }
            };

            foreach (var grupo in gruposFrequentes)
            {
                var probabilidadeGrupo = grupo.Value.Average(d => probabilidades[d]);
                
                if (probabilidadeGrupo > 0.6) // Grupo forte
                {
                    foreach (var dezena in grupo.Value)
                    {
                        probabilidades[dezena] *= 1.1; // Boost de 10%
                    }
                }
            }

            return probabilidades;
        }

        private Dictionary<int, double> AplicarEstrategiaTendencias(Dictionary<int, double> probabilidades)
        {
            // Estratégia: Analisar tendências recentes dos últimos 5 sorteios
            var ultimosSorteios = _dadosTreino.TakeLast(5);
            var dezenasMuitoRecentes = ultimosSorteios
                .SelectMany(s => s.Lista)
                .GroupBy(d => d)
                .Where(g => g.Count() >= 2)
                .Select(g => g.Key);

            foreach (var dezena in dezenasMuitoRecentes)
            {
                probabilidades[dezena] *= 0.85; // Redução por repetição recente
            }

            return probabilidades;
        }

        private Dictionary<int, double> AplicarEstrategiaEquilibrio(Dictionary<int, double> probabilidades)
        {
            // Estratégia: Garantir distribuição equilibrada (pares/ímpares, soma, etc.)
            var pares = probabilidades.Where(kvp => kvp.Key % 2 == 0).ToList();
            var impares = probabilidades.Where(kvp => kvp.Key % 2 == 1).ToList();

            var mediaPares = pares.Average(p => p.Value);
            var mediaImpares = impares.Average(p => p.Value);

            // Equilibrar se há muito desbalanceamento
            if (Math.Abs(mediaPares - mediaImpares) > 0.2)
            {
                if (mediaPares > mediaImpares)
                {
                    foreach (var impar in impares)
                    {
                        probabilidades[impar.Key] *= 1.05;
                    }
                }
                else
                {
                    foreach (var par in pares)
                    {
                        probabilidades[par.Key] *= 1.05;
                    }
                }
            }

            return probabilidades;
        }

        private double CalcularConfiancaPalpite(List<int> palpite)
        {
            if (!palpite.Any()) return 0;

            var confiancas = palpite.Select(d => Metronomos[d].Confiabilidade);
            var probabilidades = palpite.Select(d => Metronomos[d].ProbabilidadeAtual);

            return (confiancas.Average() + probabilidades.Average()) / 2.0;
        }

        private double CalcularDesvioPadrao(List<int> valores)
        {
            var media = valores.Average();
            var variancia = valores.Sum(v => Math.Pow(v - media, 2)) / valores.Count;
            return Math.Sqrt(variancia);
        }
        #endregion

        #region State Management
        private void AtualizarEstadoAtual()
        {
            foreach (var metronomo in Metronomos.Values)
            {
                metronomo.AtualizarEstadoAtual(ConcursoAlvo);
            }
        }

        public void AlterarConcursoAlvo(int novoConcurso)
        {
            ConcursoAlvo = novoConcurso;
            AtualizarEstadoAtual();
            StatusEngine = $"🎯 Concurso alvo alterado para {ConcursoAlvo}";
        }

        public MetronomoIndividual? ObterMetronomo(int dezena)
        {
            return Metronomos.TryGetValue(dezena, out var metronomo) ? metronomo : null;
        }

        public List<MetronomoIndividual> ObterTop15Metronomos()
        {
            return Metronomos.Values
                .OrderByDescending(m => m.ProbabilidadeAtual)
                .Take(15)
                .ToList();
        }

        public List<MetronomoIndividual> ObterMetronomosPorTipo(TipoMetronomo tipo)
        {
            return Metronomos.Values
                .Where(m => m.TipoMetronomo == tipo)
                .OrderByDescending(m => m.ProbabilidadeAtual)
                .ToList();
        }
        #endregion

        #region Public Info Methods
        public string ObterRelatorioCompleto()
        {
            var relatorio = $"=== RELATÓRIO COMPLETO - METRÔNOMOS ===\n\n";
            
            relatorio += $"🎯 CONCURSO ALVO: {ConcursoAlvo}\n";
            relatorio += $"📊 STATUS: {StatusEngine}\n";
            relatorio += $"⚡ METRÔNOMOS ATIVOS: {Metronomos.Count}\n\n";

            if (UltimoPalpite.Any())
            {
                relatorio += $"🎲 ÚLTIMO PALPITE:\n";
                relatorio += $"Dezenas: [{string.Join(", ", UltimoPalpite.Select(d => d.ToString("D2")))}]\n";
                relatorio += $"Confiança Geral: {ConfiancaGeralPalpite:P1}\n\n";
            }

            relatorio += $"🔝 TOP 10 PROBABILIDADES:\n";
            var top10 = Metronomos.Values
                .OrderByDescending(m => m.ProbabilidadeAtual)
                .Take(10);

            foreach (var metronomo in top10)
            {
                relatorio += $"Dezena {metronomo.Numero:D2}: {metronomo.ProbabilidadeAtual:P1} " +
                           $"({metronomo.TipoMetronomo}, ±{metronomo.IntervalAtual})\n";
            }

            relatorio += $"\n📈 DISTRIBUIÇÃO POR TIPO:\n";
            var grupos = Metronomos.Values.GroupBy(m => m.TipoMetronomo);
            foreach (var grupo in grupos)
            {
                relatorio += $"{grupo.Key}: {grupo.Count()} metrônomos\n";
            }

            return relatorio;
        }
        #endregion
    }

    #region Helper Classes
    public class ResultadoValidacao
    {
        public bool Sucesso { get; set; }
        public string Erro { get; set; } = string.Empty;
        public int TotalSorteios { get; set; }
        public List<int> AcertosPorSorteio { get; set; } = new();
        public double MediaAcertos { get; set; }
        public int MelhorAcerto { get; set; }
        public int PiorAcerto { get; set; }
        public double DesvioPadrao { get; set; }
        public double TaxaSucesso { get; set; }
        public List<List<int>> PalpitesGerados { get; set; } = new();

        public override string ToString()
        {
            return $"Validação: {MediaAcertos:F1} acertos/sorteio (σ={DesvioPadrao:F1})";
        }
    }
    #endregion
}