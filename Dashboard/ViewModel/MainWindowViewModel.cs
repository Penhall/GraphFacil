// D:\PROJETOS\GraphFacil\Dashboard\ViewModel\MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.Views;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.ViewModel
{
    /// <summary>
    /// ViewModel principal da aplicação, integrado com sistema de Metrônomos
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
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
        }

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
            ExecutarEstudo(1, () =>
            {
                int alvo = ConcursoAlvo;
                var resultado = Estudos.Estudo1(alvo);
                Infra.SalvaSaidaW(resultado, Infra.NomeSaida("ListaEstudo1", alvo));
                return resultado;
            });
        }

        [RelayCommand]
        private void Segundo()
        {
            ExecutarEstudo(2, () =>
            {
                int alvo = ConcursoAlvo;
                var resultado = Estudos.Estudo2(alvo);
                Infra.SalvaSaidaW(resultado, Infra.NomeSaida("ListaEstudo2", alvo));
                return resultado;
            });
        }

        [RelayCommand]
        private void Terceiro()
        {
            ExecutarEstudo(3, () =>
            {
                // Implementar Estudo3 conforme necessário
                return new Lances();
            });
        }

        [RelayCommand]
        private void Quarto()
        {
            ExecutarEstudo(4, () =>
            {
                // Implementar Estudo4 conforme necessário
                return new Lances();
            });
        }

        [RelayCommand]
        private void Quinto()
        {
            ExecutarEstudo(5, () =>
            {
                // Implementar Estudo5 conforme necessário
                return new Lances();
            });
        }

        [RelayCommand]
        private void Sexto()
        {
            ExecutarEstudo(6, () =>
            {
                // Implementar Estudo6 conforme necessário
                return new Lances();
            });
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
        #endregion

        #region Public Properties for Data Binding
        public string VersaoSistema => "Sistema de Metrônomos v2.0";

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

    /// <summary>
    /// Dialog para seleção de concurso (mantido do código original)
    /// </summary>
    public class SeletorConcursoDialog : Window
    {
        public int ConcursoSelecionado { get; private set; }
        public string TipoSelecao { get; private set; } = "Personalizado";
        public bool GerarPalpiteAutomatico { get; private set; }

        private System.Windows.Controls.TextBox _textBoxConcurso;
        private System.Windows.Controls.ComboBox _comboTipo;
        private System.Windows.Controls.CheckBox _checkGerarPalpite;

        public SeletorConcursoDialog(int concursoAtual, int minimo, int maximo)
        {
            Title = "Selecionar Concurso Alvo";
            Width = 400;
            Height = 280;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var stackPanel = new System.Windows.Controls.StackPanel
            {
                Margin = new Thickness(20)
            };

            // Título
            var titulo = new System.Windows.Controls.TextBlock
            {
                Text = "🎯 Selecionar Concurso para Análise",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Informações
            var info = new System.Windows.Controls.TextBlock
            {
                Text = $"Concurso atual: {concursoAtual}\nRange disponível: {minimo} - {maximo}",
                Margin = new Thickness(0, 0, 0, 15),
                Foreground = System.Windows.Media.Brushes.Gray
            };

            // Tipo de seleção
            var labelTipo = new System.Windows.Controls.Label { Content = "Tipo de Análise:" };
            _comboTipo = new System.Windows.Controls.ComboBox
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Próximo Concurso", Tag = "Proximo" });
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Concurso Histórico", Tag = "Historico" });
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Previsão Futura", Tag = "Futuro" });
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Personalizado", Tag = "Personalizado" });
            _comboTipo.SelectedIndex = 0;

            // Número do concurso
            var labelConcurso = new System.Windows.Controls.Label { Content = "Número do Concurso:" };
            _textBoxConcurso = new System.Windows.Controls.TextBox
            {
                Text = concursoAtual.ToString(),
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Checkbox para gerar palpite automaticamente
            _checkGerarPalpite = new System.Windows.Controls.CheckBox
            {
                Content = "Gerar palpite automaticamente",
                IsChecked = true,
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
                if (int.TryParse(_textBoxConcurso.Text, out int numero) && numero >= minimo && numero <= maximo + 50)
                {
                    ConcursoSelecionado = numero;
                    var selectedItem = (System.Windows.Controls.ComboBoxItem)_comboTipo.SelectedItem;
                    TipoSelecao = selectedItem.Tag.ToString();
                    GerarPalpiteAutomatico = _checkGerarPalpite.IsChecked == true;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Digite um número entre {minimo} e {maximo + 50}", "Valor Inválido");
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

            // Eventos
            _comboTipo.SelectionChanged += (s, e) =>
            {
                var selectedItem = (System.Windows.Controls.ComboBoxItem)_comboTipo.SelectedItem;
                switch (selectedItem.Tag.ToString())
                {
                    case "Proximo":
                        _textBoxConcurso.Text = (maximo + 1).ToString();
                        break;
                    case "Historico":
                        _textBoxConcurso.Text = maximo.ToString();
                        break;
                    case "Futuro":
                        _textBoxConcurso.Text = (maximo + 10).ToString();
                        break;
                }
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(titulo);
            stackPanel.Children.Add(info);
            stackPanel.Children.Add(labelTipo);
            stackPanel.Children.Add(_comboTipo);
            stackPanel.Children.Add(labelConcurso);
            // stackPanel.Children.Add(_textBoxFim);
            stackPanel.Children.Add(_textBoxConcurso);
            stackPanel.Children.Add(_checkGerarPalpite);
            stackPanel.Children.Add(buttonPanel);

            Content = stackPanel;
        }
    }
}