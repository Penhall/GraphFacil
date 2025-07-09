// Dashboard/ViewModels/Services/UINotificationService.cs - Serviço para notificações
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.ViewModels.Services;

/// <summary>
/// Serviço centralizado para notificações na UI
/// </summary>
public class UINotificationService
{
    #region Singleton Pattern
    private static UINotificationService _instance;
    public static UINotificationService Instance => _instance ??= new UINotificationService();

    private UINotificationService() { }
    #endregion

    #region Notification Methods
    /// <summary>
    /// Mostra mensagem de informação
    /// </summary>
    public void ShowInfo(string message, string title = "Informação")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    /// <summary>
    /// Mostra mensagem de sucesso
    /// </summary>
    public void ShowSuccess(string message, string title = "Sucesso")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show($"✅ {message}", title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    /// <summary>
    /// Mostra mensagem de erro
    /// </summary>
    public void ShowError(string message, string title = "Erro")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show($"❌ {message}", title, MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }

    /// <summary>
    /// Mostra mensagem de aviso
    /// </summary>
    public void ShowWarning(string message, string title = "Aviso")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show($"⚠️ {message}", title, MessageBoxButton.OK, MessageBoxImage.Warning);
        });
    }

    /// <summary>
    /// Pergunta confirmação ao usuário
    /// </summary>
    public bool AskConfirmation(string message, string title = "Confirmação")
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        });
    }

    /// <summary>
    /// Mostra progresso (placeholder para implementação futura)
    /// </summary>
    public async Task ShowProgressAsync(string message, Func<Task> operation)
    {
        // Por enquanto, apenas executa a operação
        // Futuro: implementar dialog de progresso
        await operation();
    }
    #endregion

    #region Events
    /// <summary>
    /// Evento para notificações que não usam MessageBox
    /// </summary>
    public event EventHandler<NotificationEventArgs> NotificationRequested;

    /// <summary>
    /// Dispara notificação via evento
    /// </summary>
    public void NotifyViaEvent(string message, NotificationType type = NotificationType.Info)
    {
        NotificationRequested?.Invoke(this, new NotificationEventArgs
        {
            Message = message,
            Type = type,
            Timestamp = DateTime.Now
        });
    }
    #endregion
}

#region Supporting Classes
public class NotificationEventArgs : EventArgs
{
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}
#endregion
