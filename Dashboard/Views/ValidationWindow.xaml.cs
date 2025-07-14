using Dashboard.ViewModels;
using Dashboard.ViewModels.Services;
using LotoLibrary.Models;
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
    }
}
