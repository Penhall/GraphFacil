// Dashboard/ViewModel/MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    /// ViewModel principal da aplicação, agora integrado com sistema de Metrônomos
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        #region Private Fields
        private readonly MetronomoEngine _metronomoEngine;
        private readonly Lances _historico;
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
        private string _statusEngine = "Aguardando inicialização...";

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
            try
            {
                // Inicialização dos dados históricos
                StatusEngine = "Carregando dados históricos...";
                Infra.CarregarConcursos();
                _historico = Infra.arLoto;

                // Inicialização do Motor de Metrônomos
                _metronomoEngine = new MetronomoEngine(_historico);
                
                // Bind das propriedades do engine
                BindEngineProperties();

                // Atualizar informações iniciais
                AtualizarTextoConcurso();
                StatusEngine = "Sistema inicializado. Pressione 'Iniciar Metrônomos' para começar.";
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na inicialização: {ex.Message}";
                MessageBox.Show($"Erro na inicialização: {ex.Message}", 
                    "Erro de Inicialização", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Engine Binding
        private void BindEngineProperties()
        {
            // Sincronizar propriedades do engine com o ViewModel
            _metronomoEngine.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_metronomoEngine.StatusEngine):
                        StatusEngine = _metronomoEngine.StatusEngine;
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
            };
        }
        #endregion

        #region Main Commands - Metrônomos
        [RelayCommand]
        private async Task IniciarMetronomos()
        {
            if (IsProcessing) return;

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
            if (!_metronomoEngine.IsInicializado)
            {
                MessageBox.Show("Inicie os metrônomos primeiro!", "Aviso", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var palpite = _metronomoEngine.GerarPalpiteOtimizado();
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
            if (!_metronomoEngine.IsInicializado)
            {
                MessageBox.Show("Inicie os metrônomos primeiro!", "Aviso", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsProcessing = true;
                StatusEngine = "Executando validação com 100 sorteios...";

                var resultado = await _metronomoEngine.ValidarModeloAsync();

                if (resultado.Sucesso)
                {
                    var mensagem = $"=== RESULTADO DA VALIDAÇÃO ===\n\n" +
                                 $"📊 Sorteios testados: {resultado.TotalSorteios}\n" +
                                 $"🎯 Média de acertos: {resultado.MediaAcertos:F1}/15\n" +
                                 $"📈 Melhor resultado: {resultado.MelhorAcerto}/15\n" +
                                 $"📉 Pior resultado: {resultado.PiorAcerto}/15\n" +
                                 $"📊 Desvio padrão: ±{resultado.DesvioPadrao:F1}\n" +
                                 $"✅ Taxa de sucesso (≥11): {resultado.TaxaSucesso:P1}\n\n" +
                                 $"💡 Sistema {(resultado.MediaAcertos >= 8 ? "APROVADO" : "EM CALIBRAÇÃO")}";

                    MessageBox.Show(mensagem, "Validação Concluída", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Erro na validação: {resultado.Erro}", 
                        "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
        private void AlterarConcursoAlvo()
        {
            var inputDialog = new InputDialog("Alterar Concurso Alvo", 
                $"Concurso atual: {ConcursoAlvo}", ConcursoAlvo.ToString());
            
            if (inputDialog.ShowDialog() == true)
            {
                if (int.TryParse(inputDialog.Answer, out int novoConcurso) && novoConcurso > 0)
                {
                    _metronomoEngine.AlterarConcursoAlvo(novoConcurso);
                    
                    // Atualizar metrônomos visualmente
                    foreach (var metronomo in Metronomos)
                    {
                        metronomo.AtualizarEstadoAtual(novoConcurso);
                    }
                }
                else
                {
                    MessageBox.Show("Número de concurso inválido!", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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

        #region Utility Commands
        [RelayCommand]
        private void SalvarResultados()
        {
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
                var validationWindow = new Dashboard.Views.ValidationWindow();
                validationWindow.ShowDialog();
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
            if (_historico?.Count > 0)
            {
                var ultimoConcurso = _historico.Last();
                TextoConcurso = $"Próximo: {ConcursoAlvo} (Último: {ultimoConcurso.Numero})";
            }
            else
            {
                TextoConcurso = $"Concurso Alvo: {ConcursoAlvo}";
            }
        }

        private void AtualizarUltimoPalpite()
        {
            if (_metronomoEngine.UltimoPalpite.Any())
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

    #region Helper Classes
    /// <summary>
    /// Dialog simples para entrada de texto
    /// </summary>
    public partial class InputDialog : Window
    {
        public string Answer => AnswerTextBox.Text;

        public InputDialog(string title, string question, string defaultAnswer = "")
        {
            Width = 350;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Title = title;

            var stackPanel = new System.Windows.Controls.StackPanel 
            { 
                Margin = new Thickness(15) 
            };

            var questionLabel = new System.Windows.Controls.Label 
            { 
                Content = question 
            };
            
            AnswerTextBox = new System.Windows.Controls.TextBox 
            { 
                Text = defaultAnswer,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var buttonPanel = new System.Windows.Controls.StackPanel 
            { 
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new System.Windows.Controls.Button 
            { 
                Content = "OK", 
                Width = 70, 
                Margin = new Thickness(5)
            };
            okButton.Click += (s, e) => { DialogResult = true; Close(); };

            var cancelButton = new System.Windows.Controls.Button 
            { 
                Content = "Cancelar", 
                Width = 70, 
                Margin = new Thickness(5)
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(questionLabel);
            stackPanel.Children.Add(AnswerTextBox);
            stackPanel.Children.Add(buttonPanel);

            Content = stackPanel;
        }

        private System.Windows.Controls.TextBox AnswerTextBox;
    }
    #endregion
}