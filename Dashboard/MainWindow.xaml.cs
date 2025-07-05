using Dashboard.ViewModel;
using LotoLibrary.Services;
using System.Windows;

namespace Dashboard
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Infra.CarregarConcursos();

            DataContext = new MainWindowViewModel(Infra.arLoto);
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
}