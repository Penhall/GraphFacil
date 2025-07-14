using System;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.Services
{
    /// <summary>
    /// Serviço para exibir notificações na interface do usuário
    /// Implementa padrão Singleton para acesso global
    /// </summary>
    public class UINotificationService
    {
        #region Singleton Implementation
        private static readonly Lazy<UINotificationService> _instance = 
            new Lazy<UINotificationService>(() => new UINotificationService());

        public static UINotificationService Instance => _instance.Value;

        public UINotificationService() { }
        #endregion

        #region Notification Methods
        public Task ShowInfoAsync(string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() => 
            {
                MessageBox.Show(message, "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            }).Task;
        }

        public Task ShowSuccessAsync(string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() => 
            {
                MessageBox.Show(message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }).Task;
        }

        public Task ShowWarningAsync(string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() => 
            {
                MessageBox.Show(message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }).Task;
        }

        public Task ShowErrorAsync(string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() => 
            {
                MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }).Task;
        }

        public Task<bool> AskConfirmationAsync(string message, string title = "Confirmar")
        {
            return Application.Current.Dispatcher.InvokeAsync(() => 
            {
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }).Task;
        }
        #endregion

        #region Shortcut Methods (non-async)
        public void ShowInfo(string message) => ShowInfoAsync(message).ConfigureAwait(false);
        public void ShowSuccess(string message) => ShowSuccessAsync(message).ConfigureAwait(false);
        public void ShowWarning(string message) => ShowWarningAsync(message).ConfigureAwait(false);
        public void ShowError(string message) => ShowErrorAsync(message).ConfigureAwait(false);
        #endregion
    }
}
