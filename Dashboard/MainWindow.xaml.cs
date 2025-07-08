// D:\PROJETOS\GraphFacil\Dashboard\MainWindow.xaml.cs
using Dashboard.ViewModel;
using LotoLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dashboard
{
    /// <summary>
    /// Lógica de interação para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Configurar ViewModel como DataContext
            Infra.CarregarConcursos();

            var historico = Infra.arLoto; ;
            DataContext = new MainWindowViewModel();
        }

        #region Event Handlers da Janela
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Deseja realmente sair do programa?",
                "Confirmar Saída", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion

        #region Event Handlers dos Controles
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is MetronomoIndividual metronomo)
            {
                // Selecionar metrônomo e mostrar detalhes
                var viewModel = DataContext as MainWindowViewModel;
                viewModel?.SelecionarMetronomoCommand.Execute(metronomo);
            }
        }
        #endregion
    }
}