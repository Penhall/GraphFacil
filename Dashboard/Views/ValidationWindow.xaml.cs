using Dashboard.ViewModels;
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
            ViewModel = new ValidationViewModel(historico, new UINotificationService());
            DataContext = ViewModel;
        }
    }
}
