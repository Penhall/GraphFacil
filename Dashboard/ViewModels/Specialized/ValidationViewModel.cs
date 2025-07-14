// D:\PROJETOS\GraphFacil\Dashboard\ViewModels\Specialized\ValidationViewModel.cs - Correção do CanExecute
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Models;
using LotoLibrary.Services;
using LotoLibrary.Suporte; // Usar apenas este namespace
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// ViewModel especializado para validações e testes
    /// Responsabilidade única: executar e gerenciar validações
    /// </summary>
    public partial class ValidationViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<LotoLibrary.Suporte.TestResult> _validationResults;

        [ObservableProperty]
        private string _lastValidationSummary = "";

        [ObservableProperty]
        private double _overallAccuracy;

        [ObservableProperty]
        private bool _validationInProgress;

        [ObservableProperty]
        private int _totalTests;

        [ObservableProperty]
        private int _passedTests;

        [ObservableProperty]
        private TimeSpan _lastValidationDuration;
        #endregion

        #region Constructor
        public ValidationViewModel(Lances historicalData) : base(historicalData)
        {
            ValidationResults = new ObservableCollection<LotoLibrary.Suporte.TestResult>();
        }
        #endregion

        #region Commands with CanExecute methods
        [RelayCommand(CanExecute = nameof(CanExecuteValidation))]
        private async Task RunFullValidation()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                ValidationInProgress = true;
                ValidationResults.Clear();

                var startTime = DateTime.Now;
                // Implementação temporária - substituir por lógica de validação real
                await Task.Delay(1000); // Simular processamento
                var endTime = DateTime.Now;

                LastValidationDuration = endTime - startTime;
                
                // Dados de exemplo - remover quando implementar validação real
                ValidationResults.Add(new LotoLibrary.Suporte.TestResult {
                    TestName = "Validação Simulada",
                    Success = true,
                    Message = "Implementar lógica real de validação aqui"
                });

                TotalTests = 1;
                PassedTests = 1;
                OverallAccuracy = 1.0;

                LastValidationSummary = "Validação simulada - implementar lógica real";
                
                ValidationInProgress = false;

            }, "Executando validação completa...");
        }

        // MÉTODO CANEXECUTE ADICIONADO
        private bool CanExecuteValidation()
        {
            return !ValidationInProgress && !IsLoading && _historicalData != null && _historicalData.Count > 50;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteQuickValidation))]
        private async Task RunQuickValidation()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                ValidationInProgress = true;
                
                // Implementar validação rápida aqui
                await Task.Delay(1000); // Simular processamento

                LastValidationSummary = "Validação rápida concluída";
                ValidationInProgress = false;

            }, "Executando validação rápida...");
        }

        private bool CanExecuteQuickValidation()
        {
            return !ValidationInProgress && !IsLoading && _historicalData != null;
        }

        [RelayCommand]
        private void ClearResults()
        {
            ValidationResults.Clear();
            LastValidationSummary = "";
            OverallAccuracy = 0.0;
            TotalTests = 0;
            PassedTests = 0;
        }
        #endregion

        #region UI Notification Methods  
        public async Task ShowSuccessMessageAsync(string message)
        {
            // Implementar notificação de sucesso
            await Task.Run(() => 
            {
                // Simular notificação
                Console.WriteLine($"✅ {message}");
            });
        }

        public async Task ShowErrorMessageAsync(string message)
        {
            // Implementar notificação de erro
            await Task.Run(() => 
            {
                // Simular notificação
                Console.WriteLine($"❌ {message}");
            });
        }
        #endregion

        #region Overrides
        protected override async Task InitializeSpecificAsync()
        {
            SetStatus("ValidationViewModel inicializado");
            await Task.CompletedTask;
        }

        public override async Task Cleanup()
        {
            ValidationResults?.Clear();
            await base.Cleanup();
        }
        #endregion
    }
}
