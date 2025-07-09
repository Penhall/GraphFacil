// Dashboard/ViewModels/Base/ViewModelBase.cs - Base comum para todos os ViewModels
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Base
{
    /// <summary>
    /// Classe base para todos os ViewModels com funcionalidades comuns
    /// </summary>
    public abstract partial class ViewModelBase : ObservableObject
    {
        #region Observable Properties
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusMessage = "";

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = "";
        #endregion

        #region Protected Methods
        protected void SetStatus(string message, bool isError = false)
        {
            StatusMessage = message;
            HasError = isError;
            if (isError) ErrorMessage = message;
        }

        protected async Task ExecuteWithLoadingAsync(Func<Task> operation, string loadingMessage = "Processando...")
        {
            try
            {
                IsLoading = true;
                SetStatus(loadingMessage);
                await operation();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                SetStatus($"Erro: {ex.Message}", true);
                throw;
            }
        }

        protected void ClearError()
        {
            HasError = false;
            ErrorMessage = "";
        }
        #endregion
    }
}