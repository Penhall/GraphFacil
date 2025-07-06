using Dashboard.ViewModel;
using System.Windows;

namespace Dashboard.Views
{
    /// <summary>
    /// Lógica de interação para ValidationWindow.xaml
    /// </summary>
    public partial class ValidationWindow : Window
    {
        public ValidationWindow()
        {
            InitializeComponent();

            try
            {
                this.DataContext = new ValidationViewModel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar a validação: {ex.Message}",
                "Erro de Inicialização",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

                this.Close();
            }
        }

        private void FecharWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}