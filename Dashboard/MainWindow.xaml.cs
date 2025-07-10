// Dashboard/MainWindow.xaml.cs - Code-behind SIMPLIFICADO
using Dashboard.ViewModels;
using LotoLibrary.Utilities;
using System;
using System.Windows;
using System.Windows.Input;

namespace Dashboard
{
    /// <summary>
    /// Code-behind SIMPLIFICADO - apenas eventos essenciais de janela
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            InitializeViewModel();
        }
        #endregion

        #region Initialization
        private void InitializeViewModel()
        {
            try
            {
                // Carregar dados históricos
                Infra.CarregarConcursos();


                var historico = Infra.arLoto;

                if (historico == null || historico.Count == 0)
                {
                    MessageBox.Show("Erro ao carregar dados históricos!", "Erro Crítico",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    return;
                }

                // Criar ViewModel principal refatorado
                DataContext = new MainWindowViewModel(historico);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na inicialização: {ex.Message}", "Erro Crítico",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
        #endregion

        #region Window Events - APENAS o essencial
        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
            catch (Exception ex)
            {
                // Log silencioso - não interromper UX
                Console.WriteLine($"Erro no drag: {ex.Message}");
            }
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cleanup do ViewModel se necessário
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.Cleanup();
                }

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no fechamento: {ex.Message}");
                Environment.Exit(0); // Forçar saída se necessário
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Qualquer inicialização adicional após load
                Title = $"Sistema Lotofácil - Arquitetura Modular - {DateTime.Now:yyyy}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no load: {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Cleanup antes de fechar
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.Cleanup();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no closing: {ex.Message}");
            }
        }
        #endregion
    }
}
