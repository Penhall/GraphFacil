// ExemploNovoViewModel.cs - Template para criar novos ViewModels facilmente
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using LotoLibrary.Models.Core;
using System;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// TEMPLATE: Como criar um novo ViewModel especializado
    /// Copie este template e modifique conforme necessário
    /// </summary>
    public partial class NovoExemploViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private string _exemploPropriedade = "";

        [ObservableProperty]
        private bool _exemploFlag;

        [ObservableProperty]
        private double _exemploValor;
        #endregion

        #region Constructor
        public NovoExemploViewModel(Lances historicalData) : base(historicalData)
        {
            // Inicialização específica se necessário
        }
        #endregion

        #region Initialization Override
        protected override async Task InitializeSpecificAsync()
        {
            // Sua inicialização específica aqui
            SetStatus("✅ NovoExemploViewModel inicializado");
            await Task.CompletedTask;
        }
        #endregion

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecuteExemplo))]
        private async Task ExecutarExemplo()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                // Sua lógica aqui
                ExemploPropriedade = $"Executado em {DateTime.Now:HH:mm:ss}";

                await ShowSuccessMessageAsync("Exemplo executado com sucesso!");

            }, "Executando exemplo...");
        }

        [RelayCommand]
        private void LimparExemplo()
        {
            ExemploPropriedade = "";
            ExemploFlag = false;
            ExemploValor = 0;

            SetStatus("Exemplo limpo");
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteExemplo()
        {
            return CanExecute() && !string.IsNullOrEmpty(ExemploPropriedade);
        }
        #endregion

        #region Helper Methods
        private void ExemploMetodoPrivado()
        {
            // Métodos auxiliares privados
        }
        #endregion
    }
}