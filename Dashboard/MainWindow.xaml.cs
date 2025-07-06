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
                "Erro de Inicializa��o",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Fechar a aplica��o se houver erro cr�tico na inicializa��o
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