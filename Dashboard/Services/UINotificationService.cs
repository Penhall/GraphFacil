// D:\PROJETOS\GraphFacil\Dashboard\Services\UINotificationService.cs
using System;
using System.Windows;

namespace Dashboard.Services;

/// <summary>
/// Serviço para notificações na UI de forma thread-safe
/// </summary>
public static class UINotificationService
{
    /// <summary>
    /// Mostra mensagem de forma thread-safe
    /// </summary>
    public static void ShowMessage(string message, string title = "Informação", MessageBoxImage icon = MessageBoxImage.Information)
    {
        if (Application.Current?.Dispatcher != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, icon);
            });
        }
    }

    /// <summary>
    /// Mostra confirmação de forma thread-safe
    /// </summary>
    public static bool ShowConfirmation(string message, string title = "Confirmação")
    {
        bool result = false;

        if (Application.Current?.Dispatcher != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            });
        }

        return result;
    }

    /// <summary>
    /// Executa ação na UI thread
    /// </summary>
    public static void InvokeOnUI(Action action)
    {
        if (Application.Current?.Dispatcher != null)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }
    }

    /// <summary>
    /// Executa ação assíncrona na UI thread
    /// </summary>
    public static void BeginInvokeOnUI(Action action)
    {
        if (Application.Current?.Dispatcher != null)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
