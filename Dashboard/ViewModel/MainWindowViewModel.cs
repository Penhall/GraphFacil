// D:\PROJETOS\GraphFacil\Dashboard\ViewModel\MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LotoLibrary.Engines;
using LotoLibrary.Models;
using LotoLibrary.Services;
using LotoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.ViewModel
{
    /// <summary>
    /// ViewModel principal da aplicação - VERSÃO CORRIGIDA
    /// Integra sistema de Metrônomos Individuais e Osciladores
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        #region Private Fields
        private readonly MetronomoEngine? _metronomoEngine;
        private readonly OscillatorEngine? _engine;
        private readonly Lances _historico;
        private readonly ValidationMetricsService _validationService;
        private bool _isInitialized = false;
        #endregion

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<MetronomoIndividual> _metronomos = new();

        [ObservableProperty]
        private ObservableCollection<LotoLibrary.Services.DezenaOscilante> _dezenasOscilantes = new();

        [ObservableProperty]
        private bool _mostrarOsciladores = true;

        [ObservableProperty]
        private bool _mostrarMetronomos = true;

        [ObservableProperty]
        private string _textoConcurso = "3000";

        [ObservableProperty]
        private string _ultimoPalpite = string.Empty;

        [ObservableProperty]
        private bool _isProcessing = false;

        [ObservableProperty]
        private string _statusEngine = "Sistema pronto para uso";

        [ObservableProperty]
        private double _confiancaAtual = 0.0;

        [ObservableProperty]
        private int _concursoAlvo = 3000;

        [ObservableProperty]
        private MetronomoIndividual? _metronomoSelecionado;

        [ObservableProperty]
        private string _relatorioGeral = string.Empty;

        [ObservableProperty]
        private string _resultadosValidacao = string.Empty;

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            try
            {
                // Carregar dados históricos
                _historico = Infra.CarregarConcursosSeguro() ?? new Lances();

                // Inicializar serviços
                _validationService = new ValidationMetricsService();

                if (_historico.Any())
                {
                    _metronomoEngine = new MetronomoEngine(_historico);
                    _engine = new OscillatorEngine(_historico);
                }

                if (string.IsNullOrEmpty(StatusEngine))
                    StatusEngine = "Sistema inicializando...";


                // Inicialização assíncrona
                _ = InicializarAsync();

                StatusEngine = $"Sistema inicializado com {_historico.Count} concursos";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro na inicialização: {ex.Message}";
            }
        }
        private async Task InicializarAsync()
        {
            try
            {
                IsProcessing = true;
                StatusEngine = "Inicializando componentes...";

                await Task.Run(() =>
                {
                    // Inicializar Metrônomos
                    if (_metronomoEngine != null)
                    {
                        var metronomes = _metronomoEngine.InicializarMetronomosAsync().Result; // Fix: Use .Result to get the actual result from the Task
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Metronomos.Clear();
                            foreach (var m in _metronomoEngine.Metronomos.Values) // Ensure metronomes is a dictionary or collection with a Values property
                            {
                                Metronomos.Add(m);
                            }
                        });
                    }

                    // Inicializar Osciladores
                    if (_engine != null)
                    {
                        var oscillators = _engine.InicializarOsciladores();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            DezenasOscilantes.Clear();
                            foreach (var osc in oscillators)
                            {
                                DezenasOscilantes.Add(osc);
                            }
                        });
                    }
                });

                _isInitialized = true;
                StatusEngine = "Sistema inicializado com sucesso";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro na inicialização: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
        #endregion

        #region Commands - Estudos Originais

        [RelayCommand]
        private async Task Primeiro()
        {
            await ExecutarEstudo(1, "Estudo1");
        }

        [RelayCommand]
        private async Task Segundo()
        {
            await ExecutarEstudo(2, "Estudo2");
        }

        [RelayCommand]
        private async Task Terceiro()
        {
            await ExecutarEstudo(3, "Estudo3");
        }

        [RelayCommand]
        private async Task Quarto()
        {
            await ExecutarEstudo(4, "Análise Completa");
        }

        [RelayCommand]
        private async Task Quinto()
        {
            await ExecutarEstudo(5, "Estudo5");
        }

        [RelayCommand]
        private async Task Sexto()
        {
            await ExecutarEstudo(6, "Estudo6");
        }

        [RelayCommand]
        private async Task Setimo()
        {
            await ExecutarEstudo(7, "Estudo7");
        }

        [RelayCommand]
        private async Task Oitavo()
        {
            await ExecutarEstudo(8, "Estudo8");
        }

        #endregion

        #region Commands - Novos Recursos

        [RelayCommand]
        private async Task IniciarSincronizacao()
        {
            if (!_isInitialized)
            {
                StatusEngine = "Sistema não inicializado";
                return;
            }

            try
            {
                IsProcessing = true;
                StatusEngine = "Executando sincronização de osciladores...";

                await Task.Run(() =>
                {
                    if (_engine != null && DezenasOscilantes.Any())
                    {
                        // Aplicar estratégias de sincronização
                        var ultimosSorteios = _historico.TakeLast(10).ToList();
                        OscillatorStrategy.AplicarTendenciaCurtoPrazo(DezenasOscilantes.ToList(), ultimosSorteios);
                        OscillatorStrategy.AplicarQuentesFrios(DezenasOscilantes.ToList(), ultimosSorteios);

                        // Simular sincronização
                        for (int i = 0; i < 20; i++)
                        {
                            foreach (var dezena in DezenasOscilantes)
                            {
                                dezena.AtualizarFase();
                            }
                        }
                    }
                });

                StatusEngine = "Sincronização concluída";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro na sincronização: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private async Task GerarPalpite()
        {
            if (!_isInitialized)
            {
                StatusEngine = "Sistema não inicializado";
                return;
            }

            try
            {
                IsProcessing = true;
                StatusEngine = "Gerando palpite...";

                var palpite = await Task.Run(() =>
                {
                    if (MostrarMetronomos && Metronomos.Any())
                    {
                        return GerarPalpiteComMetronomos();
                    }
                    else if (MostrarOsciladores && DezenasOscilantes.Any())
                    {
                        return GerarPalpiteComOsciladores();
                    }
                    else
                    {
                        return GerarPalpiteAleatorio();
                    }
                });

                UltimoPalpite = $"[{string.Join(", ", palpite.Select(n => n.ToString("D2")))}]";
                StatusEngine = $"Palpite gerado para concurso {ConcursoAlvo}";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro na geração do palpite: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private async Task ValidarEstrategias()
        {
            if (!_isInitialized || !_historico.Any())
            {
                StatusEngine = "Dados insuficientes para validação";
                return;
            }

            try
            {
                IsProcessing = true;
                StatusEngine = "Executando validação de estratégias...";

                var resultado = await Task.Run(() =>
                {
                    var dadosTreino = _historico.Take(_historico.Count - 100).ToList();
                    var dadosValidacao = _historico.Skip(_historico.Count - 100).ToList();

                    return _validationService.CompararEstrategias(dadosTreino, dadosValidacao, 50);
                });

                // Formatar resultados
                var relatorio = FormatarRelatorioValidacao(resultado);
                ResultadosValidacao = relatorio;

                StatusEngine = "Validação concluída";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro na validação: {ex.Message}";
                ResultadosValidacao = $"Erro: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void SelecionarMetronomo(MetronomoIndividual? metronomo)
        {
            MetronomoSelecionado = metronomo;
            if (metronomo != null)
            {
                RelatorioGeral = metronomo.ObterAnaliseDetalhada();
            }
        }

        [RelayCommand]
        private async Task SalvarResultados()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"Resultados_{timestamp}.txt";
                var filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);

                var conteudo = $"""
                    RELATÓRIO DE RESULTADOS - {DateTime.Now:dd/MM/yyyy HH:mm:ss}
                    ================================================

                    ÚLTIMO PALPITE: {UltimoPalpite}
                    CONCURSO ALVO: {ConcursoAlvo}
                    CONFIANÇA: {ConfiancaAtual:P2}

                    STATUS DO SISTEMA:
                    {StatusEngine}

                    VALIDAÇÃO DE ESTRATÉGIAS:
                    {ResultadosValidacao}

                    ANÁLISE DETALHADA:
                    {RelatorioGeral}
                    """;

                await File.WriteAllTextAsync(filepath, conteudo);
                StatusEngine = $"Resultados salvos em: {filename}";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro ao salvar: {ex.Message}";
            }
        }

        [RelayCommand]
        private void TerminarPrograma()
        {
            var resultado = MessageBox.Show(
                "Deseja realmente encerrar o programa?",
                "Confirmar Saída",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        #endregion

        #region Private Methods

        private async Task ExecutarEstudo(int numeroEstudo, string nomeEstudo)
        {
            try
            {
                IsProcessing = true;
                StatusEngine = $"Executando {nomeEstudo}...";

                var alvo = int.Parse(TextoConcurso);

                await Task.Run(() =>
                {
                    Lances resultado = numeroEstudo switch
                    {
                        1 => Estudos.Estudo1(alvo),
                        2 => Estudos.Estudo2(alvo),
                        3 => Estudos.Estudo7(alvo),
                        4 => ExecutarAnaliseCompleta(),
                        5 => Estudos.Estudo5(alvo),
                        6 => Estudos.Estudo6(alvo),
                        7 => Estudos.Estudo7(alvo),
                        8 => Estudos.Estudo7(alvo),
                        _ => new Lances()
                    };

                    if (resultado.Any())
                    {
                        var nomeArquivo = Infra.NomeSaida($"Lista{nomeEstudo.Replace(" ", "")}", alvo);
                        Infra.SalvaSaidaW(resultado, nomeArquivo);
                    }
                });

                StatusEngine = $"{nomeEstudo} concluído";
            }
            catch (Exception ex)
            {
                StatusEngine = $"Erro em {nomeEstudo}: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private Lances ExecutarAnaliseCompleta()
        {
            // Executar análise completa do sistema
            AnaliseService.ExecutarAnalise();
            return new Lances(); // Retorna vazio pois AnaliseService gera seus próprios arquivos
        }

        private List<int> GerarPalpiteComMetronomos()
        {
            var concurso = ConcursoAlvo;

            return Metronomos
                .Select(m => new
                {
                    Numero = m.Numero,
                    Probabilidade = m.CalcularProbabilidadePara(concurso)
                })
                .OrderByDescending(x => x.Probabilidade)
                .Take(15)
                .Select(x => x.Numero)
                .OrderBy(n => n)
                .ToList();
        }

        private List<int> GerarPalpiteComOsciladores()
        {
            return DezenasOscilantes
                .Where(d => d.EstaSincronizada || d.Probabilidade > 0.6)
                .OrderByDescending(d => d.Probabilidade)
                .Take(15)
                .Select(d => d.Numero)
                .OrderBy(n => n)
                .ToList();
        }

        private List<int> GerarPalpiteAleatorio()
        {
            var random = new Random();
            return Enumerable.Range(1, 25)
                .OrderBy(x => random.Next())
                .Take(15)
                .OrderBy(x => x)
                .ToList();
        }

        private string FormatarRelatorioValidacao(Dictionary<string, MetricasPerformance> resultados)
        {
            var relatorio = "=== RELATÓRIO DE VALIDAÇÃO ===\n\n";

            foreach (var kvp in resultados.OrderByDescending(x => x.Value.TaxaAcertoMedia))
            {
                var estrategia = kvp.Key;
                var metricas = kvp.Value;

                relatorio += $"📊 {estrategia.ToUpper()}\n";
                relatorio += $"Taxa de Acerto: {metricas.TaxaAcertoMedia:P2}\n";
                relatorio += $"Desvio Padrão: {metricas.DesvioPadrao:F3}\n";
                relatorio += $"Melhor Resultado: {metricas.MelhorResultado} acertos\n";
                relatorio += $"Pior Resultado: {metricas.PiorResultado} acertos\n";

                // Fix: Remove the incorrect HasValue check for double type  
                if (metricas.GanhoSobreAleatorio != 0)
                {
                    relatorio += $"Ganho sobre Aleatório: {metricas.GanhoSobreAleatorio:P2}\n";
                }

                relatorio += "\n";
            }

            return relatorio;
        }

        #endregion

        #region Public Properties for View Binding

        public bool SistemaInicializado => _isInitialized;

        public string VersaoSistema => "2.0 - Metrônomos + Osciladores";

        public int TotalConcursos => _historico?.Count ?? 0;

        #endregion
    }
}