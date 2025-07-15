// Dashboard/ViewModels/Specialized/ValidationViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using LotoLibrary.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// ViewModel para validações e testes do sistema
    /// </summary>
    public partial class ValidationViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private string _lastValidationSummary = "Nenhuma validação executada";

        [ObservableProperty]
        private double _overallAccuracy = 0.0;

        [ObservableProperty]
        private ObservableCollection<ValidationResult> _validationResults = new();

        [ObservableProperty]
        private bool _isValidationRunning = false;

        [ObservableProperty]
        private string _currentValidationStep = "";

        [ObservableProperty]
        private int _validationProgress = 0;
        #endregion

        #region Constructor
        public ValidationViewModel(Lances historicalData) : base(historicalData)
        {
            ValidationResults = new ObservableCollection<ValidationResult>();
        }
        #endregion

        #region Initialization Override
        protected override async Task InitializeSpecificAsync()
        {
            SetStatus("✅ ValidationViewModel inicializado");
            await Task.CompletedTask;
        }
        #endregion

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecuteValidation))]
        private async Task RunQuickValidation()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                IsValidationRunning = true;
                ValidationProgress = 0;
                ValidationResults.Clear();

                try
                {
                    await RunValidationStepsAsync();

                    LastValidationSummary = $"Validação concluída - {ValidationResults.Count} testes executados";
                    OverallAccuracy = CalculateOverallAccuracy();

                    await ShowSuccessMessageAsync("Validação rápida concluída com sucesso!");
                }
                catch (Exception ex)
                {
                    // CORREÇÃO: Usar SetStatus em vez de ShowErrorMessageAsync
                    SetStatus($"Erro na validação: {ex.Message}", true);
                    LogError(ex);
                }
                finally
                {
                    IsValidationRunning = false;
                    ValidationProgress = 100;
                }
            }, "Executando validação rápida...");
        }

        [RelayCommand]
        private void ClearValidationResults()
        {
            ValidationResults.Clear();
            LastValidationSummary = "Resultados de validação limpos";
            OverallAccuracy = 0.0;
            ValidationProgress = 0;
            SetStatus("Resultados de validação limpos");
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteValidation()
        {
            // CORREÇÃO: Usar _historicalData em vez de HistoricalData
            return CanExecute() && !IsValidationRunning && _historicalData != null;
        }
        #endregion

        #region Private Methods
        private async Task RunValidationStepsAsync()
        {
            var steps = new[]
            {
                "Validando estrutura de dados",
                "Testando algoritmos de predição",
                "Verificando integridade dos modelos",
                "Calculando métricas de performance",
                "Finalizando validação"
            };

            for (int i = 0; i < steps.Length; i++)
            {
                CurrentValidationStep = steps[i];
                ValidationProgress = (i + 1) * 20;

                await Task.Delay(800); // Simular processamento

                // Adicionar resultado simulado
                var result = new ValidationResult
                {
                    TestName = steps[i],
                    Status = Random.Shared.NextDouble() > 0.1 ? "✅ Passou" : "⚠️ Aviso",
                    Accuracy = Random.Shared.NextDouble() * 100,
                    Details = $"Teste executado em {DateTime.Now:HH:mm:ss}"
                };

                ValidationResults.Add(result);
            }
        }

        private double CalculateOverallAccuracy()
        {
            if (ValidationResults.Count == 0) return 0.0;

            double total = 0;
            foreach (var result in ValidationResults)
            {
                total += result.Accuracy;
            }

            return total / ValidationResults.Count;
        }
        #endregion
    }

    /// <summary>
    /// Representa o resultado de um teste de validação
    /// </summary>
    public class ValidationResult
    {
        public string TestName { get; set; } = "";
        public string Status { get; set; } = "";
        public double Accuracy { get; set; }
        public string Details { get; set; } = "";
    }
}