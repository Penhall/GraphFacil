using Dashboard.ViewModels;
using Dashboard.ViewModels.Services;
using LotoLibrary.Models.Core;
using System.Windows;

namespace Dashboard.Views
{
    public partial class ValidationWindow : Window
    {
        public ValidationViewModel ViewModel { get; }

        public ValidationWindow(Lances historico)
        {
            InitializeComponent();
            ViewModel = new ValidationViewModel(historico, UINotificationService.Instance);
            DataContext = ViewModel;
        }

        private void FecharWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ValidateAsync();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar lógica para parar validação
            ViewModel.IsValidationRunning = false;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar exportação de resultados
            UINotificationService.Instance.ShowInfo("Funcionalidade de exportação em desenvolvimento");
        }
    }
}
