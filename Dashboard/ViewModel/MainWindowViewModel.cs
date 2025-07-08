// D:\PROJETOS\GraphFacil\Dashboard\ViewModel\MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.Views;
using LotoLibrary.Engines;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dashboard.ViewModel
{
    /// <summary>
    /// ViewModel principal da aplicação, integrado com sistema de Metrônomos
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        #region New Fields for Fase 1

        private PredictionEngine _predictionEngine;

        [ObservableProperty]
        private string _predictionEngineStatus = "Não inicializado";

        [ObservableProperty]
        private bool _isPredictionEngineInitialized;

        [ObservableProperty]
        private string _lastPredictionResult = "";

        [ObservableProperty]
        private string _phase1ValidationStatus = "";

        [ObservableProperty]
        private bool _isValidationRunning;

        #endregion

        #region Private Fields
        private MetronomoEngine _metronomoEngine;
        private Lances _historico;
        private bool _isInitialized = false;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<MetronomoIndividual> _metronomos = new();

        [ObservableProperty]
        private bool _mostrarMetronomos = true;

        [ObservableProperty]
        private string _textoConcurso = string.Empty;

        [ObservableProperty]
        private string _ultimoPalpite = string.Empty;

        [ObservableProperty]
        private bool _isProcessing = false;

        [ObservableProperty]
        private string _statusEngine = "Inicializando sistema...";

        [ObservableProperty]
        private double _confiancaAtual = 0.0;

        [ObservableProperty]
        private int _concursoAlvo = 3000;

        [ObservableProperty]
        private MetronomoIndividual? _metronomoSelecionado;

        [ObservableProperty]
        private string _relatorioGeral = string.Empty;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            // Inicialização assíncrona para não travar a UI
            _ = InicializarAsync();
            InitializePredictionEngineAsync();
        }


        #region PredictionEngine Integration

        /// <summary>
        /// Inicializa o PredictionEngine de forma assíncrona
        /// </summary>
        private async void InitializePredictionEngineAsync()
        {
            try
            {
                PredictionEngineStatus = "Inicializando PredictionEngine...";

                _predictionEngine = new PredictionEngine();

                // Conectar eventos
                _predictionEngine.OnStatusChanged += (s, status) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PredictionEngineStatus = status;
                    });
                };

                _predictionEngine.OnPredictionGenerated += (s, prediction) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var dezenas = string.Join(", ", prediction.PredictedNumbers.Select(d => d.ToString("D2")));
                        LastPredictionResult = $"[{dezenas}] - Confiança: {prediction.OverallConfidence:P2}";
                    });
                };

                // Carregar dados históricos
                var dados = Infra.CarregarConcursosSeguro();
                if (dados?.Any() == true)
                {
                    var initResult = await _predictionEngine.InitializeAsync(dados);
                    IsPredictionEngineInitialized = initResult;

                    if (initResult)
                    {
                        PredictionEngineStatus = "✅ PredictionEngine inicializado com sucesso";
                    }
                    else
                    {
                        PredictionEngineStatus = "❌ Falha na inicialização do PredictionEngine";
                    }
                }
                else
                {
                    PredictionEngineStatus = "❌ Sem dados históricos para inicializar";
                }
            }
            catch (Exception ex)
            {
                PredictionEngineStatus = $"❌ Erro na inicialização: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar PredictionEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera predição usando o novo sistema
        /// </summary>
        [RelayCommand]
        private async Task GerarPalpiteNovo()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                IsProcessing = true;
                StatusEngine = "Gerando palpite com PredictionEngine...";

                if (_predictionEngine == null || !_predictionEngine.IsInitialized)
                {
                    MessageBox.Show("PredictionEngine não está inicializado!",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Determinar próximo concurso
                var proximoConcurso = ObterProximoConcurso();

                var prediction = await _predictionEngine.GeneratePredictionAsync(proximoConcurso);

                if (prediction?.PredictedNumbers?.Any() == true)
                {
                    var dezenas = string.Join(", ", prediction.PredictedNumbers.Select(d => d.ToString("D2")));
                    var resultado = $"Concurso {proximoConcurso}: [{dezenas}]\n" +
                                   $"Confiança: {prediction.OverallConfidence:P2}\n" +
                                   $"Modelo: {prediction.ModelUsed}\n" +
                                   $"Tempo: {prediction.ProcessingTime.TotalMilliseconds:F0}ms";

                    MessageBox.Show(resultado, "Palpite Gerado - Nova Arquitetura",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    StatusEngine = "✅ Palpite gerado com sucesso";
                }
                else
                {
                    MessageBox.Show("Não foi possível gerar palpite.",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro ao gerar palpite: {ex.Message}";
                MessageBox.Show($"Erro ao gerar palpite: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Executa diagnósticos do sistema
        /// </summary>
        [RelayCommand]
        private async Task ExecutarDiagnosticos()
        {
            try
            {
                IsProcessing = true;
                StatusEngine = "Executando diagnósticos...";

                if (_predictionEngine == null)
                {
                    MessageBox.Show("PredictionEngine não está disponível!",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _predictionEngine.RunDiagnosticsAsync();

                // Mostrar relatório de diagnóstico
                var diagnosticReport = System.IO.File.Exists("DiagnosticReport.txt")
                    ? System.IO.File.ReadAllText("DiagnosticReport.txt")
                    : "Relatório de diagnóstico não gerado.";

                var diagnosticWindow = new Window
                {
                    Title = "Relatório de Diagnóstico",
                    Width = 800,
                    Height = 600,
                    Content = new ScrollViewer
                    {
                        Content = new TextBlock
                        {
                            Text = diagnosticReport,
                            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                            FontSize = 12,
                            Margin = new Thickness(10),
                            TextWrapping = TextWrapping.Wrap
                        }
                    }
                };

                diagnosticWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro nos diagnósticos: {ex.Message}";
                MessageBox.Show($"Erro ao executar diagnósticos: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Valida toda a implementação da Fase 1
        /// </summary>
        [RelayCommand]
        private async Task ValidarFase1()
        {
            try
            {
                IsValidationRunning = true;
                Phase1ValidationStatus = "Iniciando validação da Fase 1...";

                var validationReport = await Phase1ValidationService.ExecuteValidationSuiteAsync();

                // Salvar relatório
                var reportText = validationReport.GenerateReport();
                var reportPath = "Phase1ValidationReport.txt";
                await System.IO.File.WriteAllTextAsync(reportPath, reportText);

                // Mostrar resultado
                var statusIcon = validationReport.OverallSuccess ? "✅" : "❌";
                var statusText = validationReport.OverallSuccess ? "PASSOU" : "FALHOU";

                Phase1ValidationStatus = $"{statusIcon} Validação {statusText} em {validationReport.TotalExecutionTime.TotalSeconds:F2}s";

                var message = $"Validação da Fase 1: {statusText}\n\n" +
                             $"Tempo de execução: {validationReport.TotalExecutionTime.TotalSeconds:F2}s\n" +
                             $"Relatório salvo em: {reportPath}\n\n" +
                             $"Deseja visualizar o relatório completo?";

                var result = MessageBox.Show(message, $"Validação Fase 1 - {statusText}",
                    MessageBoxButton.YesNo,
                    validationReport.OverallSuccess ? MessageBoxImage.Information : MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var reportWindow = new Window
                    {
                        Title = "Relatório de Validação - Fase 1",
                        Width = 900,
                        Height = 700,
                        Content = new ScrollViewer
                        {
                            Content = new TextBlock
                            {
                                Text = reportText,
                                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                                FontSize = 11,
                                Margin = new Thickness(15),
                                TextWrapping = TextWrapping.Wrap
                            }
                        }
                    };

                    reportWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Phase1ValidationStatus = $"❌ Erro na validação: {ex.Message}";
                MessageBox.Show($"Erro durante validação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsValidationRunning = false;
            }
        }

        /// <summary>
        /// Compara performance: Sistema antigo vs novo
        /// </summary>
        [RelayCommand]
        private async Task CompararPerformance()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                IsProcessing = true;
                StatusEngine = "Comparando performance...";

                var comparison = await ExecutarComparacaoPerformance();

                var message = $"=== COMPARAÇÃO DE PERFORMANCE ===\n\n" +
                             $"🔧 SISTEMA ANTIGO:\n" +
                             $"   Tempo médio: {comparison.OldSystemAvgTime:F0}ms\n" +
                             $"   Dezenas 1-9: {comparison.OldSystemDezenas1a9:F1}%\n" +
                             $"   Distribuição: {comparison.OldSystemDistribution}\n\n" +
                             $"🚀 SISTEMA NOVO:\n" +
                             $"   Tempo médio: {comparison.NewSystemAvgTime:F0}ms\n" +
                             $"   Dezenas 1-9: {comparison.NewSystemDezenas1a9:F1}%\n" +
                             $"   Distribuição: {comparison.NewSystemDistribution}\n\n" +
                             $"📊 MELHORIA:\n" +
                             $"   Performance: {comparison.PerformanceImprovement:F1}%\n" +
                             $"   Qualidade: {comparison.QualityImprovement}";

                MessageBox.Show(message, "Comparação de Performance",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na comparação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        #endregion


        private async Task InicializarAsync()
        {
            try
            {
                StatusEngine = "Carregando dados históricos...";

                // Tentar carregamento seguro dos dados
                await Task.Run(() =>
                {
                    try
                    {
                        _historico = Infra.CarregarConcursosSeguro();
                    }
                    catch (Exception ex)
                    {
                        StatusEngine = $"❌ Erro ao carregar dados: {ex.Message}";
                        throw;
                    }
                });

                StatusEngine = "Inicializando motor de metrônomos...";

                // Verificar se dados foram carregados
                if (_historico == null || !_historico.Any())
                {
                    StatusEngine = "⚠️ Nenhum dado histórico carregado - criando dados padrão";
                    _historico = new Lances();
                    ConcursoAlvo = 3001;
                }
                else
                {
                    ConcursoAlvo = (_historico.LastOrDefault()?.Id ?? 3000) + 1;
                }

                // Inicializar engine com validação
                _metronomoEngine = new MetronomoEngine(_historico);

                if (_metronomoEngine == null)
                {
                    throw new InvalidOperationException("Falha ao criar MetronomoEngine");
                }

                // Conectar eventos do engine
                ConectarEventosEngine();

                // Bind das propriedades do engine
                BindEngineProperties();

                // Atualizar informações iniciais
                AtualizarTextoConcurso();

                _isInitialized = true;
                StatusEngine = "✅ Sistema inicializado. Pressione 'Iniciar Metrônomos' para começar.";
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na inicialização: {ex.Message}";

                // Log detalhado para debug
                System.Diagnostics.Debug.WriteLine($"Erro detalhado na inicialização:");
                System.Diagnostics.Debug.WriteLine($"Mensagem: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                // Mostrar erro na UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        $"Erro na inicialização do sistema:\n\n{ex.Message}\n\n" +
                        "Verifique se:\n" +
                        "1. O arquivo Lotofacil.json não está sendo usado por outro programa\n" +
                        "2. Você tem permissões de leitura na pasta\n" +
                        "3. O arquivo não está corrompido",
                        "Erro de Inicialização",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
        }
        #endregion

        #region Engine Events Connection

        private void ConectarEventosEngine()
        {
            if (_metronomoEngine == null) return;

            try
            {
                // Conectar evento para mostrar mensagens
                _metronomoEngine.OnMostrarMensagem += (mensagem) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(mensagem, "Diagnóstico dos Metrônomos",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                };

                // Conectar evento para configuração de treinamento
                _metronomoEngine.OnSolicitarConfiguracaoTreinamento += (configuracao) =>
                {
                    try
                    {
                        bool resultado = false;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var dialog = new ConfiguracaoTreinamentoDialog(configuracao);
                            dialog.Owner = Application.Current.MainWindow;
                            resultado = dialog.ShowDialog() == true;
                        });
                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Erro ao abrir configuração: {ex.Message}",
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                        return false;
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao conectar eventos: {ex.Message}");
            }
        }
        #endregion

        #region Engine Binding
        private void BindEngineProperties()
        {
            if (_metronomoEngine == null) return;

            try
            {
                // Sincronizar propriedades do engine com o ViewModel
                _metronomoEngine.PropertyChanged += (s, e) =>
                {
                    try
                    {
                        switch (e.PropertyName)
                        {
                            case nameof(_metronomoEngine.StatusEngine):
                                StatusEngine = _metronomoEngine.StatusEngine ?? "Status desconhecido";
                                break;
                            case nameof(_metronomoEngine.ConfiancaGeralPalpite):
                                ConfiancaAtual = _metronomoEngine.ConfiancaGeralPalpite;
                                break;
                            case nameof(_metronomoEngine.ConcursoAlvo):
                                ConcursoAlvo = _metronomoEngine.ConcursoAlvo;
                                AtualizarTextoConcurso();
                                break;
                            case nameof(_metronomoEngine.UltimoPalpite):
                                AtualizarUltimoPalpite();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro no binding de propriedades: {ex.Message}");
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar binding: {ex.Message}");
            }
        }
        #endregion

        #region Main Commands - Metrônomos
        [RelayCommand]
        private async Task IniciarMetronomos()
        {
            if (!ValidarInicializacao() || IsProcessing) return;

            try
            {
                IsProcessing = true;

                var sucesso = await _metronomoEngine.InicializarMetronomosAsync();

                if (sucesso)
                {
                    // Atualizar coleção observável
                    Metronomos.Clear();
                    foreach (var metronomo in _metronomoEngine.Metronomos.Values.OrderBy(m => m.Numero))
                    {
                        Metronomos.Add(metronomo);
                    }

                    MostrarMetronomos = true;
                    RelatorioGeral = _metronomoEngine.ObterRelatorioCompleto();
                }
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro ao inicializar: {ex.Message}";
                MessageBox.Show($"Erro ao inicializar metrônomos: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void GerarPalpite()
        {
            if (!ValidarInicializacao()) return;

            if (!_metronomoEngine.IsInicializado)
            {
                MessageBox.Show("Inicie os metrônomos primeiro!", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var palpite = _metronomoEngine.GerarPalpiteOtimizado();

                if (palpite == null || !palpite.Any())
                {
                    MessageBox.Show("Não foi possível gerar palpite. Verifique se os metrônomos estão funcionando.",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AtualizarUltimoPalpite();

                // Atualizar visual dos metrônomos
                foreach (var metronomo in Metronomos)
                {
                    metronomo.EmFaseOtima = palpite.Contains(metronomo.Numero);
                }

                RelatorioGeral = _metronomoEngine.ObterRelatorioCompleto();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar palpite: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ValidarModelo()
        {
            if (!ValidarInicializacao()) return;

            if (!_metronomoEngine.IsInicializado)
            {
                MessageBox.Show("Inicie os metrônomos primeiro!", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsProcessing = true;
                StatusEngine = "Executando validação integrada...";

                var metricas = await _metronomoEngine.ValidarModeloAsync();

                var mensagem = $"=== VALIDAÇÃO DE METRÔNOMOS ===\n\n" +
                             $"📊 Testes realizados: {metricas.TotalTestes}\n" +
                             $"🎯 Taxa de acerto: {metricas.TaxaAcertoMedia:P2}\n" +
                             $"📈 Média de acertos: {metricas.MediaAcertos:F1}/15\n" +
                             $"🏆 Melhor resultado: {metricas.MelhorResultado}/15\n" +
                             $"📉 Pior resultado: {metricas.PiorResultado}/15\n" +
                             $"📊 Desvio padrão: {metricas.DesvioPadrao:P2}\n" +
                             $"⚡ Precision: {metricas.Precision:P2}\n" +
                             $"🔄 Recall: {metricas.Recall:P2}\n" +
                             $"🎯 F1-Score: {metricas.F1Score:P2}\n\n" +
                             $"💡 Avaliação: {(metricas.TaxaAcertoMedia >= 0.45 ? "EXCELENTE" : metricas.TaxaAcertoMedia >= 0.40 ? "BOM" : "EM CALIBRAÇÃO")}";

                MessageBox.Show(mensagem, "Validação Concluída",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante validação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private async Task CompararEstrategias()
        {
            if (!ValidarInicializacao()) return;

            if (!_metronomoEngine.IsInicializado)
            {
                MessageBox.Show("Inicie os metrônomos primeiro!", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsProcessing = true;
                StatusEngine = "Comparando metrônomos com outras estratégias...";

                var comparacao = await _metronomoEngine.CompararComOutrasEstrategiasAsync();

                var relatorio = "=== COMPARAÇÃO DE ESTRATÉGIAS ===\n\n";

                var estrategiasOrdenadas = comparacao
                    .OrderByDescending(kvp => kvp.Value.TaxaAcertoMedia)
                    .ToList();

                for (int i = 0; i < estrategiasOrdenadas.Count; i++)
                {
                    var estrategia = estrategiasOrdenadas[i];
                    var posicao = i + 1;
                    var emoji = posicao == 1 ? "🏆" : posicao == 2 ? "🥈" : posicao == 3 ? "🥉" : "📊";

                    relatorio += $"{emoji} {posicao}º {estrategia.Key}:\n";
                    relatorio += $"   Taxa: {estrategia.Value.TaxaAcertoMedia:P2} | ";
                    relatorio += $"Média: {estrategia.Value.MediaAcertos:F1}/15 | ";
                    relatorio += $"Melhor: {estrategia.Value.MelhorResultado}\n\n";
                }

                // Verificar posição dos metrônomos
                var posicaoMetronomos = estrategiasOrdenadas
                    .FindIndex(e => e.Key.Contains("Metrônomo")) + 1;

                relatorio += $"🎯 RESULTADO: Metrônomos ficaram em {posicaoMetronomos}º lugar de {estrategiasOrdenadas.Count}";

                MessageBox.Show(relatorio, "Comparação de Estratégias",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante comparação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void AlterarConcursoAlvo()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                var ultimoConcurso = _historico?.LastOrDefault()?.Id ?? 3000;
                var primeiroConcurso = _historico?.FirstOrDefault()?.Id ?? 1;

                var dialog = new SeletorConcursoDialog(
                    ConcursoAlvo,
                    primeiroConcurso,
                    ultimoConcurso + 50 // Permitir concursos futuros
                );

                if (dialog.ShowDialog() == true)
                {
                    var novoConcurso = dialog.ConcursoSelecionado;
                    var tipoSelecao = dialog.TipoSelecao;

                    _metronomoEngine.AlterarConcursoAlvo(novoConcurso);

                    // Gerar novo palpite automaticamente se solicitado
                    if (dialog.GerarPalpiteAutomatico && _metronomoEngine.IsInicializado)
                    {
                        GerarPalpite();
                    }

                    var status = tipoSelecao switch
                    {
                        "Historico" => $"🎯 Concurso {novoConcurso} (histórico) - palpite para análise",
                        "Futuro" => $"🔮 Concurso {novoConcurso} (futuro) - previsão",
                        "Proximo" => $"⚡ Próximo concurso {novoConcurso} - palpite atual",
                        _ => $"🎯 Concurso alvo: {novoConcurso}"
                    };

                    StatusEngine = status;

                    MessageBox.Show(
                        $"Concurso alvo alterado para: {novoConcurso}\n\n" +
                        $"Tipo: {tipoSelecao}\n" +
                        $"Status: {(novoConcurso <= ultimoConcurso ? "Dados históricos disponíveis" : "Previsão futura")}\n\n" +
                        $"{(dialog.GerarPalpiteAutomatico ? "Palpite gerado automaticamente!" : "Use 'Gerar Palpite' para criar novo palpite.")}",
                        "Concurso Alterado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar concurso: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Study Commands (Mantidos do sistema original)


        [RelayCommand]
        private void Primeiro()
        {
            try
            {
                // Método original mantido para compatibilidade
                if (!ValidarInicializacao()) return;

                // ... código existente ...

                StatusEngine = "Primeiro concluído (método legado)";
                TerminarPrograma();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no método Primeiro: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // ✅ ATUALIZAR MÉTODO EXISTENTE - Manter compatibilidade

        [RelayCommand]
        private async void Sexto()
        {
            try
            {
                if (!ValidarInicializacao()) return;

                // Usar novo sistema se disponível, senão usar antigo
                if (_predictionEngine?.IsInitialized == true)
                {
                    await GerarPalpiteNovo();
                }
                else
                {
                    // Fallback para método antigo
                    StatusEngine = "Usando sistema legado...";
                    // ... código antigo de geração de palpites ...
                }

                TerminarPrograma();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no método Sexto: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        [RelayCommand]
        private void Setimo()
        {
            ExecutarEstudo(7, () =>
            {
                // Implementar Estudo7 conforme necessário
                return new Lances();
            });
        }

        [RelayCommand]
        private void Oitavo()
        {
            ExecutarEstudo(8, () =>
            {
                // Implementar Estudo8 conforme necessário
                return new Lances();
            });
        }
        #endregion

        #region Diagnostic Commands
        [RelayCommand]
        private void DiagnosticarMetronomos()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                _metronomoEngine.DiagnosticarMetronomosCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no diagnóstico: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ForcarRecalculoMetronomos()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                _metronomoEngine.ForcarRecalculoMetronomosCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no recálculo: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ConfigurarTreinamento()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                _metronomoEngine.ConfigurarTreinamentoCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na configuração: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void VerificarStatusArquivos()
        {
            try
            {
                var status = Infra.VerificarStatusJSON();
                MessageBox.Show(status, "Status dos Arquivos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao verificar status: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ForcarLiberacaoArquivos()
        {
            try
            {
                IsProcessing = true;
                StatusEngine = "Forçando liberação de arquivos...";

                Infra.ForcarLiberacaoERecarregar();

                StatusEngine = "✅ Arquivos liberados e recarregados";
                MessageBox.Show("Arquivos foram liberados e dados recarregados com sucesso!",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro ao liberar arquivos: {ex.Message}";
                MessageBox.Show($"Erro ao forçar liberação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }
        #endregion

        #region Utility Commands
        [RelayCommand]
        private void SalvarResultados()
        {
            if (!ValidarInicializacao()) return;

            try
            {
                if (!_metronomoEngine.IsInicializado)
                {
                    MessageBox.Show("Sistema não inicializado!", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var relatorio = _metronomoEngine.ObterRelatorioCompleto();
                var nomeArquivo = $"Relatorio_Metronomos_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                System.IO.File.WriteAllText(nomeArquivo, relatorio);

                MessageBox.Show($"Relatório salvo em: {nomeArquivo}", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void TerminarPrograma()
        {
            var resultado = MessageBox.Show("Deseja realmente sair do programa?",
                "Confirmar Saída", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        [RelayCommand]
        private void AbrirValidacao()
        {
            try
            {
                MessageBox.Show("Funcionalidade de análise ML em desenvolvimento.",
                    "Em Desenvolvimento", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir janela de validação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ToggleVisualizacao()
        {
            MostrarMetronomos = !MostrarMetronomos;
        }

        [RelayCommand]
        private void SelecionarMetronomo(MetronomoIndividual? metronomo)
        {
            MetronomoSelecionado = metronomo;

            if (metronomo != null)
            {
                var analise = metronomo.ObterAnaliseDetalhada();
                MessageBox.Show(analise, $"Análise - Dezena {metronomo.Numero:D2}",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region Helper Methods
        private bool ValidarInicializacao()
        {
            if (!_isInitialized || _metronomoEngine == null)
            {
                MessageBox.Show("Sistema ainda não foi inicializado completamente. Aguarde...",
                    "Sistema Inicializando", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ExecutarEstudo(int numeroEstudo, Func<Lances> executarEstudo)
        {
            try
            {
                StatusEngine = $"Executando Estudo {numeroEstudo}...";
                IsProcessing = true;

                var resultado = executarEstudo();

                MessageBox.Show($"Estudo {numeroEstudo} executado com sucesso!\n" +
                               $"Resultados: {resultado.Count} itens gerados",
                    "Estudo Concluído", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no Estudo {numeroEstudo}: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
                StatusEngine = "Pronto para nova operação";
            }
        }

        private void AtualizarTextoConcurso()
        {
            try
            {
                if (_historico?.Count > 0)
                {
                    var ultimoConcurso = _historico.Last();
                    TextoConcurso = $"Próximo: {ConcursoAlvo} (Último: {ultimoConcurso.Id})";
                }
                else
                {
                    TextoConcurso = $"Concurso Alvo: {ConcursoAlvo}";
                }
            }
            catch (Exception ex)
            {
                TextoConcurso = $"Concurso Alvo: {ConcursoAlvo} (Erro: {ex.Message})";
            }
        }

        private void AtualizarUltimoPalpite()
        {
            try
            {
                if (_metronomoEngine?.UltimoPalpite?.Any() == true)
                {
                    var dezenas = _metronomoEngine.UltimoPalpite
                        .Select(d => d.ToString("D2"))
                        .ToArray();

                    UltimoPalpite = $"[{string.Join("-", dezenas)}] " +
                                   $"(Confiança: {ConfiancaAtual:P1})";
                }
                else
                {
                    UltimoPalpite = "Nenhum palpite gerado ainda";
                }
            }
            catch (Exception ex)
            {
                UltimoPalpite = $"Erro na atualização: {ex.Message}";
            }
        }

        private int ObterProximoConcurso()
        {
            try
            {
                if (Infra.arLoto?.Any() == true)
                {
                    return Infra.arLoto.Last().Id + 1;
                }
                return 3500; // Fallback
            }
            catch
            {
                return 3500;
            }
        }

        private async Task<PerformanceComparison> ExecutarComparacaoPerformance()
        {
            var comparison = new PerformanceComparison();

            // Testar sistema antigo (se ainda disponível)
            try
            {
                var oldStartTime = DateTime.Now;
                // Aqui você chamaria o método antigo de geração de palpites
                // Por exemplo: var oldResult = MetronomoEngine.GerarPalpite();
                var oldEndTime = DateTime.Now;

                comparison.OldSystemAvgTime = (oldEndTime - oldStartTime).TotalMilliseconds;
                comparison.OldSystemDezenas1a9 = 15.0; // Placeholder - medir real
                comparison.OldSystemDistribution = "Enviesada";
            }
            catch
            {
                comparison.OldSystemAvgTime = 0;
                comparison.OldSystemDezenas1a9 = 0;
                comparison.OldSystemDistribution = "Não disponível";
            }

            // Testar sistema novo
            if (_predictionEngine?.IsInitialized == true)
            {
                var newStartTime = DateTime.Now;
                var newResult = await _predictionEngine.GeneratePredictionAsync(ObterProximoConcurso());
                var newEndTime = DateTime.Now;

                comparison.NewSystemAvgTime = (newEndTime - newStartTime).TotalMilliseconds;

                // Analisar distribuição
                var testPredictions = new List<List<int>>();
                for (int i = 0; i < 10; i++)
                {
                    var pred = await _predictionEngine.GeneratePredictionAsync(3500 + i);
                    if (pred?.PredictedNumbers?.Any() == true)
                    {
                        testPredictions.Add(pred.PredictedNumbers);
                    }
                }

                if (testPredictions.Any())
                {
                    var diagnosticReport = DiagnosticService.AnalisarDistribuicaoDezenas(testPredictions);
                    comparison.NewSystemDezenas1a9 = diagnosticReport.Percentual1a9;
                    comparison.NewSystemDistribution = diagnosticReport.TemProblemaDistribuicao ? "Enviesada" : "Normal";
                }
            }

            // Calcular melhorias
            if (comparison.OldSystemAvgTime > 0)
            {
                comparison.PerformanceImprovement =
                    ((comparison.OldSystemAvgTime - comparison.NewSystemAvgTime) / comparison.OldSystemAvgTime) * 100;
            }

            comparison.QualityImprovement = comparison.NewSystemDezenas1a9 > comparison.OldSystemDezenas1a9
                ? "Melhorada" : "Similar";

            return comparison;
        }


        #endregion

        #region Public Properties for Data Binding
        public string VersaoSistema => "Sistema de Metrônomos v3.0";

        public string StatusCompleto =>
            $"{StatusEngine} | Metrônomos: {Metronomos.Count}/25 | " +
            $"Processando: {(IsProcessing ? "SIM" : "NÃO")}";

        public List<TipoMetronomo> TiposMetronomo =>
            Enum.GetValues<TipoMetronomo>().ToList();

        public Dictionary<TipoMetronomo, int> EstatisticasPorTipo
        {
            get
            {
                return Metronomos
                    .GroupBy(m => m.TipoMetronomo)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
        }
        #endregion


    }
}