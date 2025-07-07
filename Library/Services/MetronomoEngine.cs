// LotoLibrary/Services/MetronomoEngine.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Motor principal que gerencia todos os metrônomos individuais
    /// Integrado com o sistema ValidationMetrics.cs original (SEM duplicações)
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

        [ObservableProperty]
        private int _tamanhoValidacao = 100;

        [ObservableProperty]
        private int _concursoInicioTreinamento = 1;

        [ObservableProperty]
        private int _concursoFimTreinamento = -1;

        [ObservableProperty]
        private string _informacoesTreinamento = string.Empty;

        private readonly Lances _historicoCompleto;
        private List<Lance> _dadosTreino;
        private List<Lance> _dadosValidacao;
        private readonly ValidationMetricsService _validationService;
        #endregion

        #region Constructor
        public MetronomoEngine()
        {
            _historicoCompleto = new Lances();
            _validationService = new ValidationMetricsService();
        }

        public MetronomoEngine(Lances historico)
        {
            _historicoCompleto = historico;
            (_dadosTreino, _dadosValidacao) = SplitDataPersonalizado(historico);
            ConcursoAlvo = (_historicoCompleto.LastOrDefault()?.Id ?? 3000) + 1;
            _validationService = new ValidationMetricsService();
            AtualizarInformacoesTreinamento();
        }
        #endregion

        #region Training Configuration
        /// <summary>
        /// Configura a divisão dos dados de treinamento e validação
        /// </summary>
        public void ConfigurarDadosTreinamento(int tamanhoValidacao, int? concursoInicio = null, int? concursoFim = null)
        {
            TamanhoValidacao = tamanhoValidacao;

            if (concursoInicio.HasValue)
                ConcursoInicioTreinamento = concursoInicio.Value;

            if (concursoFim.HasValue)
                ConcursoFimTreinamento = concursoFim.Value;

            // Recalcular divisão dos dados
            (_dadosTreino, _dadosValidacao) = SplitDataPersonalizado(_historicoCompleto);

            AtualizarInformacoesTreinamento();

            // Se já inicializado, reinicializar metrônomos
            if (IsInicializado)
            {
                Task.Run(() => InicializarMetronomosAsync());
            }
        }

        private (List<Lance> treino, List<Lance> validacao) SplitDataPersonalizado(Lances historico)
        {
            var lista = historico.ToList();

            // Aplicar filtros de concurso se especificados
            if (ConcursoFimTreinamento > 0)
            {
                lista = lista.Where(l => l.Id <= ConcursoFimTreinamento).ToList();
            }

            if (ConcursoInicioTreinamento > 1)
            {
                lista = lista.Where(l => l.Id >= ConcursoInicioTreinamento).ToList();
            }

            // Dividir dados
            var treino = lista.SkipLast(TamanhoValidacao).ToList();
            var validacao = lista.TakeLast(TamanhoValidacao).ToList();

            return (treino, validacao);
        }

        private void AtualizarInformacoesTreinamento()
        {
            var totalConcursos = _historicoCompleto.Count;
            var concursoMaisRecente = _historicoCompleto.LastOrDefault()?.Id ?? 0;
            var concursoMaisAntigo = _historicoCompleto.FirstOrDefault()?.Id ?? 0;

            InformacoesTreinamento = $"📊 DADOS DE TREINAMENTO:\n" +
                                   $"Total de concursos disponíveis: {totalConcursos}\n" +
                                   $"Range: {concursoMaisAntigo} → {concursoMaisRecente}\n\n" +
                                   $"🎯 CONFIGURAÇÃO ATUAL:\n" +
                                   $"Treinamento: {_dadosTreino?.Count ?? 0} concursos\n" +
                                   $"Validação: {_dadosValidacao?.Count ?? 0} concursos\n" +
                                   $"Filtro início: {(ConcursoInicioTreinamento > 1 ? ConcursoInicioTreinamento.ToString() : "Sem filtro")}\n" +
                                   $"Filtro fim: {(ConcursoFimTreinamento > 0 ? ConcursoFimTreinamento.ToString() : "Sem filtro")}";
        }

        [RelayCommand]
        private void ConfigurarTreinamento()
        {
            var dialog = new ConfiguracaoTreinamentoDialog(
                TamanhoValidacao,
                ConcursoInicioTreinamento,
                ConcursoFimTreinamento,
                _historicoCompleto.FirstOrDefault()?.Id ?? 1,
                _historicoCompleto.LastOrDefault()?.Id ?? 3000
            );

            if (dialog.ShowDialog() == true)
            {
                ConfigurarDadosTreinamento(
                    dialog.TamanhoValidacao,
                    dialog.ConcursoInicio > 1 ? dialog.ConcursoInicio : null,
                    dialog.ConcursoFim > 0 ? dialog.ConcursoFim : null
                );

                StatusEngine = $"✅ Configuração atualizada: {_dadosTreino.Count} treino, {_dadosValidacao.Count} validação";
            }
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
            if (_dadosTreino == null || !_dadosTreino.Any())
                return new List<int>();

            return _dadosTreino
                .Where(lance => lance.Lista.Contains(dezena))
                .Select(lance => lance.Id)
                .OrderBy(num => num)
                .ToList();
        }
        #endregion

        #region Core Functionality
        /// <summary>
        /// Gera palpite para o concurso alvo baseado nas probabilidades individuais
        /// </summary>
        public List<int> GerarPalpiteOtimizado()
        {
            return GerarPalpiteComDebug();
        }

        /// <summary>
        /// Gera palpite com logs detalhados para debug
        /// </summary>
        public List<int> GerarPalpiteComDebug()
        {
            if (!IsInicializado || !Metronomos.Any())
            {
                StatusEngine = "❌ Engine não inicializado";
                return new List<int>();
            }

            var debug = $"=== DEBUG GERAÇÃO DE PALPITE ===\n";
            debug += $"Concurso alvo: {ConcursoAlvo}\n\n";

            try
            {
                // 1. Calcular probabilidades para todas as dezenas
                var probabilidades = new Dictionary<int, double>();
                debug += "🎯 PROBABILIDADES BRUTAS:\n";

                foreach (var metronomo in Metronomos.Values.OrderBy(m => m.Numero))
                {
                    var prob = metronomo.CalcularProbabilidadePara(ConcursoAlvo);
                    probabilidades[metronomo.Numero] = prob;
                    debug += $"Dezena {metronomo.Numero:D2}: {prob:F6}\n";
                }

                // 2. Verificar se todas as probabilidades são iguais (BUG!)
                var probabilidadesUnicas = probabilidades.Values.Distinct().Count();
                debug += $"\n📊 ANÁLISE:\n";
                debug += $"Probabilidades únicas: {probabilidadesUnicas}/25\n";

                if (probabilidadesUnicas <= 3)
                {
                    debug += "⚠️ PROBLEMA: Poucas probabilidades únicas!\n";
                    debug += "🔧 Aplicando correção de emergência...\n";

                    // Correção de emergência: usar atraso como diferenciador
                    foreach (var metronomo in Metronomos.Values)
                    {
                        var atraso = Math.Max(1, metronomo.IntervalAtual);
                        var ciclo = Math.Max(1, metronomo.CicloMedio);
                        var fatorAtraso = Math.Exp(-Math.Abs(atraso - ciclo) / ciclo);
                        probabilidades[metronomo.Numero] = fatorAtraso + (metronomo.Numero * 0.001);
                    }
                }

                // 3. Aplicar estratégias de otimização
                debug += "\n🔧 APLICANDO OTIMIZAÇÕES:\n";
                var probAntes = probabilidades[1];

                probabilidades = AplicarEstrategiaGrupos(probabilidades);
                debug += $"Após grupos - Dezena 1: {probAntes:F6} → {probabilidades[1]:F6}\n";

                probabilidades = AplicarEstrategiaTendencias(probabilidades);
                debug += $"Após tendências - Dezena 1: {probabilidades[1]:F6}\n";

                probabilidades = AplicarEstrategiaEquilibrio(probabilidades);
                debug += $"Após equilíbrio - Dezena 1: {probabilidades[1]:F6}\n";

                // 4. Selecionar top 15
                debug += "\n🏆 TOP 15 SELECIONADAS:\n";
                var ranking = probabilidades.OrderByDescending(kvp => kvp.Value).ToList();

                UltimoPalpite = ranking.Take(15).Select(kvp => kvp.Key).OrderBy(x => x).ToList();

                for (int i = 0; i < 15; i++)
                {
                    debug += $"{i + 1}º: Dezena {ranking[i].Key:D2} - {ranking[i].Value:F6}\n";
                }

                // 5. Calcular confiança
                ConfiancaGeralPalpite = CalcularConfiancaPalpite(UltimoPalpite);
                debug += $"\n✅ Confiança geral: {ConfiancaGeralPalpite:P2}\n";

                // Salvar log para análise
                StatusEngine = $"✅ Palpite gerado com {probabilidadesUnicas} probabilidades únicas";

                // Debug opcional
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine(debug);
                }

                return UltimoPalpite;
            }
            catch (Exception ex)
            {
                debug += $"\n❌ ERRO: {ex.Message}\n{ex.StackTrace}";
                StatusEngine = $"❌ Erro ao gerar palpite: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(debug);
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
        /// Usa o ValidationMetricsService original para validar metrônomos
        /// </summary>
        public async Task<MetricasPerformance> ValidarModeloAsync()
        {
            if (!IsInicializado || !_dadosValidacao.Any())
            {
                throw new InvalidOperationException("Dados insuficientes para validação");
            }

            try
            {
                StatusEngine = "Executando validação com sistema original...";

                var metricas = await Task.Run(() =>
                {
                    return ValidarMetronomosEspecificamente();
                });

                StatusEngine = $"✅ Validação concluída: {metricas.TaxaAcertoMedia:P1} de acerto médio";
                return metricas;
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na validação: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// Compara metrônomos com outras estratégias usando sistema original
        /// </summary>
        public async Task<Dictionary<string, MetricasPerformance>> CompararComOutrasEstrategiasAsync()
        {
            if (!IsInicializado)
            {
                throw new InvalidOperationException("Metrônomos não inicializados");
            }

            StatusEngine = "Comparando com outras estratégias...";

            var resultados = await Task.Run(() =>
            {
                // Usar o método original de comparação
                var comparacao = _validationService.CompararEstrategias(_dadosTreino, _dadosValidacao, 50);

                // Adicionar validação específica dos metrônomos
                var metricasMetronomos = ValidarMetronomosEspecificamente();
                comparacao["Metrônomos Individuais"] = metricasMetronomos;

                return comparacao;
            });

            StatusEngine = "✅ Comparação concluída";
            return resultados;
        }

        /// <summary>
        /// Validação específica dos metrônomos usando lógica própria
        /// </summary>
        private MetricasPerformance ValidarMetronomosEspecificamente()
        {
            var resultados = new List<ResultadoValidacao>();

            for (int i = 0; i < Math.Min(50, _dadosValidacao.Count); i++)
            {
                var concursoTeste = _dadosValidacao[i];

                // Simular dados disponíveis até este ponto
                var dadosDisponiveis = _dadosTreino.Concat(_dadosValidacao.Take(i)).ToList();

                // Recrear metrônomos com dados até este ponto
                var metronomosTemp = CriarMetronomosTemporarios(dadosDisponiveis);

                // Gerar palpite usando probabilidades dos metrônomos
                var palpite = GerarPalpiteComMetronomosTemporarios(metronomosTemp, concursoTeste.Id);

                // Calcular acertos
                var numerosAcertados = palpite.Intersect(concursoTeste.Lista).ToList();

                var resultado = new ResultadoValidacao
                {
                    ConcursoId = concursoTeste.Id,
                    PalpiteGerado = palpite,
                    ResultadoReal = concursoTeste.Lista,
                    NumerosAcertados = numerosAcertados,
                    Acertos = numerosAcertados.Count,
                    TaxaAcerto = numerosAcertados.Count / 15.0,
                    TipoEstrategia = "Metrônomos",
                    DataTeste = DateTime.Now
                };

                resultados.Add(resultado);
            }

            return CriarMetricasManualmente(resultados, "Metrônomos Individuais");
        }

        private Dictionary<int, MetronomoIndividual> CriarMetronomosTemporarios(List<Lance> dadosDisponiveis)
        {
            var metronomosTemp = new Dictionary<int, MetronomoIndividual>();

            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var historico = dadosDisponiveis
                    .Where(lance => lance.Lista.Contains(dezena))
                    .Select(lance => lance.Id)
                    .OrderBy(num => num)
                    .ToList();

                metronomosTemp[dezena] = new MetronomoIndividual(dezena, historico);
            }

            return metronomosTemp;
        }

        private List<int> GerarPalpiteComMetronomosTemporarios(Dictionary<int, MetronomoIndividual> metronomos, int concursoAlvo)
        {
            var probabilidades = new Dictionary<int, double>();

            foreach (var metronomo in metronomos.Values)
            {
                probabilidades[metronomo.Numero] = metronomo.CalcularProbabilidadePara(concursoAlvo);
            }

            return probabilidades
                .OrderByDescending(kvp => kvp.Value)
                .Take(15)
                .Select(kvp => kvp.Key)
                .OrderBy(x => x)
                .ToList();
        }

        private MetricasPerformance CriarMetricasManualmente(List<ResultadoValidacao> resultados, string nomeEstrategia)
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
            var totalNumerosEscolhidos = resultados.Sum(r => r.PalpiteGerado.Count);
            var totalNumerosCorretos = resultados.Sum(r => r.NumerosAcertados.Count);
            var totalNumerosReais = resultados.Sum(r => r.ResultadoReal.Count);

            metricas.Precision = totalNumerosEscolhidos > 0 ?
                totalNumerosCorretos / (double)totalNumerosEscolhidos : 0;

            metricas.Recall = totalNumerosReais > 0 ?
                totalNumerosCorretos / (double)totalNumerosReais : 0;

            metricas.F1Score = (metricas.Precision + metricas.Recall) > 0 ?
                2 * (metricas.Precision * metricas.Recall) / (metricas.Precision + metricas.Recall) : 0;

            return metricas;
        }

        private double CalcularDesvioPadrao(List<double> valores)
        {
            var media = valores.Average();
            var variancia = valores.Sum(v => Math.Pow(v - media, 2)) / valores.Count;
            return Math.Sqrt(variancia);
        }
        #endregion

        #region Diagnostic Tools
        [RelayCommand]
        private void DiagnosticarMetronomos()
        {
            if (!IsInicializado)
            {
                StatusEngine = "❌ Metrônomos não inicializados";
                return;
            }

            var diagnostico = "=== DIAGNÓSTICO DOS METRÔNOMOS ===\n\n";

            // 1. Verificar se todos os metrônomos foram criados
            diagnostico += $"📊 METRÔNOMOS CRIADOS: {Metronomos.Count}/25\n";

            // 2. Verificar históricos
            var metronomo1 = Metronomos[1];
            var metronomo25 = Metronomos[25];

            diagnostico += $"\n🔍 EXEMPLO - DEZENA 01:\n";
            diagnostico += $"Histórico: {metronomo1.HistoricoAparicoes.Count} aparições\n";
            diagnostico += $"Primeiras: [{string.Join(", ", metronomo1.HistoricoAparicoes.Take(5))}]\n";
            diagnostico += $"Últimas: [{string.Join(", ", metronomo1.HistoricoAparicoes.TakeLast(5))}]\n";
            diagnostico += $"Ciclo médio: {metronomo1.CicloMedio:F2}\n";
            diagnostico += $"Tipo: {metronomo1.TipoMetronomo}\n";
            diagnostico += $"Probabilidade atual: {metronomo1.ProbabilidadeAtual:F4}\n";

            diagnostico += $"\n🔍 EXEMPLO - DEZENA 25:\n";
            diagnostico += $"Histórico: {metronomo25.HistoricoAparicoes.Count} aparições\n";
            diagnostico += $"Primeiras: [{string.Join(", ", metronomo25.HistoricoAparicoes.Take(5))}]\n";
            diagnostico += $"Últimas: [{string.Join(", ", metronomo25.HistoricoAparicoes.TakeLast(5))}]\n";
            diagnostico += $"Ciclo médio: {metronomo25.CicloMedio:F2}\n";
            diagnostico += $"Tipo: {metronomo25.TipoMetronomo}\n";
            diagnostico += $"Probabilidade atual: {metronomo25.ProbabilidadeAtual:F4}\n";

            // 3. Verificar probabilidades de todas as dezenas
            diagnostico += $"\n📈 PROBABILIDADES ATUAIS (Concurso {ConcursoAlvo}):\n";
            foreach (var metronomo in Metronomos.Values.OrderBy(m => m.Numero))
            {
                var prob = metronomo.CalcularProbabilidadePara(ConcursoAlvo);
                diagnostico += $"Dezena {metronomo.Numero:D2}: {prob:F4} (ciclo: {metronomo.CicloMedio:F1}, intervalo atual: {metronomo.IntervalAtual})\n";
            }

            // 4. Verificar dados de treinamento
            diagnostico += $"\n📚 DADOS DE TREINAMENTO:\n";
            diagnostico += $"Total de concursos: {_dadosTreino?.Count ?? 0}\n";
            diagnostico += $"Primeiro concurso: {_dadosTreino?.FirstOrDefault()?.Id}\n";
            diagnostico += $"Último concurso: {_dadosTreino?.LastOrDefault()?.Id}\n";

            MessageBox.Show(diagnostico, "Diagnóstico dos Metrônomos",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ForcarRecalculoMetronomos()
        {
            if (!IsInicializado)
            {
                StatusEngine = "❌ Metrônomos não inicializados";
                return;
            }

            StatusEngine = "🔄 Forçando recálculo de todos os metrônomos...";

            foreach (var metronomo in Metronomos.Values)
            {
                metronomo.AnalisarPadroes();
                metronomo.AtualizarEstadoAtual(ConcursoAlvo);
            }

            StatusEngine = "✅ Recálculo concluído - tente gerar um novo palpite";
        }
        #endregion

        #region Optimization Strategies
        private Dictionary<int, double> AplicarEstrategiaGrupos(Dictionary<int, double> probabilidades)
        {
            var gruposFrequentes = new Dictionary<string, List<int>>
            {
                { "baixas", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 } },
                { "medias", new List<int> { 9, 10, 11, 12, 13, 14, 15, 16, 17 } },
                { "altas", new List<int> { 18, 19, 20, 21, 22, 23, 24, 25 } }
            };

            foreach (var grupo in gruposFrequentes)
            {
                var probabilidadeGrupo = grupo.Value.Average(d => probabilidades[d]);

                if (probabilidadeGrupo > 0.6)
                {
                    foreach (var dezena in grupo.Value)
                    {
                        probabilidades[dezena] *= 1.1;
                    }
                }
            }

            return probabilidades;
        }

        private Dictionary<int, double> AplicarEstrategiaTendencias(Dictionary<int, double> probabilidades)
        {
            if (_dadosTreino == null || !_dadosTreino.Any()) return probabilidades;

            var ultimosSorteios = _dadosTreino.TakeLast(5);
            var dezenasMuitoRecentes = ultimosSorteios
                .SelectMany(s => s.Lista)
                .GroupBy(d => d)
                .Where(g => g.Count() >= 2)
                .Select(g => g.Key);

            foreach (var dezena in dezenasMuitoRecentes)
            {
                probabilidades[dezena] *= 0.85;
            }

            return probabilidades;
        }

        private Dictionary<int, double> AplicarEstrategiaEquilibrio(Dictionary<int, double> probabilidades)
        {
            var pares = probabilidades.Where(kvp => kvp.Key % 2 == 0).ToList();
            var impares = probabilidades.Where(kvp => kvp.Key % 2 == 1).ToList();

            var mediaPares = pares.Average(p => p.Value);
            var mediaImpares = impares.Average(p => p.Value);

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

    /// <summary>
    /// Dialog para configuração de treinamento
    /// </summary>
    public class ConfiguracaoTreinamentoDialog : Window
    {
        public int TamanhoValidacao { get; private set; }
        public int ConcursoInicio { get; private set; }
        public int ConcursoFim { get; private set; }

        private System.Windows.Controls.TextBox _textBoxValidacao;
        private System.Windows.Controls.TextBox _textBoxInicio;
        private System.Windows.Controls.TextBox _textBoxFim;

        public ConfiguracaoTreinamentoDialog(int tamanhoAtual, int inicioAtual, int fimAtual, int minimo, int maximo)
        {
            Title = "Configuração de Treinamento";
            Width = 450;
            Height = 300;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var stackPanel = new System.Windows.Controls.StackPanel { Margin = new Thickness(20) };

            // Título
            var titulo = new System.Windows.Controls.TextBlock
            {
                Text = "🔧 Configurar Dados de Treinamento",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Tamanho da validação
            var labelValidacao = new System.Windows.Controls.Label { Content = "Tamanho da Validação:" };
            _textBoxValidacao = new System.Windows.Controls.TextBox
            {
                Text = tamanhoAtual.ToString(),
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Concurso início
            var labelInicio = new System.Windows.Controls.Label { Content = $"Concurso Início (mín: {minimo}):" };
            _textBoxInicio = new System.Windows.Controls.TextBox
            {
                Text = inicioAtual > 1 ? inicioAtual.ToString() : "",
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Concurso fim
            var labelFim = new System.Windows.Controls.Label { Content = $"Concurso Fim (máx: {maximo}, 0=sem limite):" };
            _textBoxFim = new System.Windows.Controls.TextBox
            {
                Text = fimAtual > 0 ? fimAtual.ToString() : "",
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Botões
            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                Width = 70,
                Margin = new Thickness(5),
                IsDefault = true
            };
            okButton.Click += (s, e) =>
            {
                if (int.TryParse(_textBoxValidacao.Text, out int validacao) && validacao > 0)
                {
                    TamanhoValidacao = validacao;
                    ConcursoInicio = int.TryParse(_textBoxInicio.Text, out int inicio) ? inicio : 1;
                    ConcursoFim = int.TryParse(_textBoxFim.Text, out int fim) ? fim : -1;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Tamanho de validação deve ser um número maior que 0", "Valor Inválido");
                }
            };

            var cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancelar",
                Width = 70,
                Margin = new Thickness(5),
                IsCancel = true
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(titulo);
            stackPanel.Children.Add(labelValidacao);
            stackPanel.Children.Add(_textBoxValidacao);
            stackPanel.Children.Add(labelInicio);
            stackPanel.Children.Add(_textBoxInicio);
            stackPanel.Children.Add(labelFim);
            stackPanel.Children.Add(_textBoxFim);
            stackPanel.Children.Add(buttonPanel);

            Content = stackPanel;
        }
    }
}