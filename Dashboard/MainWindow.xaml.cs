// Dashboard/MainWindow.xaml.cs
using Dashboard.ViewModel;
using LotoLibrary.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace Dashboard
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Configurar ViewModel
            try 
            {
                DataContext = new MainWindowViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar a aplicação: {ex.Message}", 
                    "Erro de Inicialização", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                
                // Se falhar completamente, fechar aplicação
                Application.Current.Shutdown();
            }
        }

        #region Window Events
        /// <summary>
        /// Permite arrastar a janela pela barra de título
        /// </summary>
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
            catch (Exception ex)
            {
                // Log error mas não mostrar ao usuário (operação de UI)
                System.Diagnostics.Debug.WriteLine($"Erro ao mover janela: {ex.Message}");
            }
        }

        /// <summary>
        /// Minimizar janela
        /// </summary>
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Fechar aplicação
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Deseja realmente fechar a aplicação?", 
                "Confirmar", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion

        #region UI Events
        /// <summary>
        /// Evento de clique em um card de metrônomo
        /// </summary>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is FrameworkElement element && 
                    element.DataContext is MetronomoIndividual metronomo)
                {
                    // Executar comando de seleção do ViewModel
                    if (DataContext is MainWindowViewModel viewModel)
                    {
                        viewModel.SelecionarMetronomoCommand.Execute(metronomo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar metrônomo: {ex.Message}", 
                    "Erro", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Keyboard Shortcuts
        /// <summary>
        /// Atalhos de teclado
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    switch (e.Key)
                    {
                        case Key.F1:
                            viewModel.IniciarMetronomosCommand.Execute(null);
                            e.Handled = true;
                            break;

                        case Key.F2:
                            viewModel.GerarPalpiteCommand.Execute(null);
                            e.Handled = true;
                            break;

                        case Key.F3:
                            viewModel.ValidarModeloCommand.Execute(null);
                            e.Handled = true;
                            break;

                        case Key.F5:
                            // Refresh - recarregar dados
                            RefreshData();
                            e.Handled = true;
                            break;

                        case Key.Escape:
                            // Fechar janela
                            BtnClose_Click(this, new RoutedEventArgs());
                            e.Handled = true;
                            break;

                        case Key.F11:
                            // Toggle fullscreen
                            ToggleFullScreen();
                            e.Handled = true;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no atalho de teclado: {ex.Message}");
            }

            base.OnKeyDown(e);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Recarregar dados da aplicação
        /// </summary>
        private void RefreshData()
        {
            try
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    // Implementar lógica de refresh se necessário
                    MessageBox.Show("Dados atualizados!", "Refresh", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar dados: {ex.Message}", 
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Alternar tela cheia
        /// </summary>
        private void ToggleFullScreen()
        {
            if (WindowState == WindowState.Maximized && WindowStyle == WindowStyle.None)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
            }
            else
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
        }
        #endregion

        #region Window State Management
        /// <summary>
        /// Salvar estado da janela ao fechar
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Aqui você pode salvar configurações, posição da janela, etc.
                var settings = Properties.Settings.Default;
                settings.WindowWidth = Width;
                settings.WindowHeight = Height;
                settings.WindowLeft = Left;
                settings.WindowTop = Top;
                settings.WindowState = WindowState.ToString();
                settings.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar configurações: {ex.Message}");
            }

            base.OnClosing(e);
        }

        /// <summary>
        /// Restaurar estado da janela ao carregar
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            try
            {
                // Restaurar configurações salvas
                var settings = Properties.Settings.Default;
                
                if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
                {
                    Width = settings.WindowWidth;
                    Height = settings.WindowHeight;
                }

                if (settings.WindowLeft > 0 && settings.WindowTop > 0)
                {
                    Left = settings.WindowLeft;
                    Top = settings.WindowTop;
                }

                if (Enum.TryParse<WindowState>(settings.WindowState, out var savedState))
                {
                    WindowState = savedState;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao restaurar configurações: {ex.Message}");
            }
        }
        #endregion
    }
}