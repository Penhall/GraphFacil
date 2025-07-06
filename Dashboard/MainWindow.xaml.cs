using Dashboard.ViewModel;
using System.Windows;

namespace Dashboard;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        try
        {
            this.DataContext = new MainWindowViewModel();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Erro ao inicializar o ViewModel: {ex.Message}",
                "Erro de Inicialização",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Fechar a aplicação se houver erro crítico na inicialização
            this.Close();
        }
    }

    private void GridBarraTitulo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void BtnFechar_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}