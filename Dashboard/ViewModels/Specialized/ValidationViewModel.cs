// Dashboard/ViewModels/Specialized/ValidationViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Services.Validation;
using LotoLibrary.Services.Diagnostic;
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

        #region Dependency Injection
        private readonly IValidationService _validationService;
        private readonly IModelFactory _modelFactory;
        #endregion

        #region Constructor
        public ValidationViewModel(Lances historicalData, IValidationService validationService, IModelFactory modelFactory) : base(historicalData)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
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
            // Instanciar e executar a suíte de validação real
            var validationSuite = _validationService.GetValidationSuite();
            int totalSteps = validationSuite.Count;
            int currentStep = 0;

            foreach (var testCase in validationSuite)
            {
                currentStep++;
                CurrentValidationStep = testCase.Name;
                ValidationProgress = (int)Math.Round((double)currentStep / totalSteps * 100);

                // Executar o teste real
                var testResult = await testCase.ExecuteAsync(_historicalData);

                // Mapear o resultado do backend para o ViewModel da UI
                var result = new ValidationResult
                {
                    TestName = testResult.TestName,
                    Status = testResult.Success ? "✅ Passou" : "❌ Falhou",
                    // A acurácia pode não se aplicar a todos os testes, ajuste conforme necessário
                    Accuracy = testResult.Success ? 100 : 0, 
                    Details = testResult.Details
                };

                ValidationResults.Add(result);

                // Se um teste crítico falhar, podemos optar por parar
                if (!testResult.Success && testCase.IsCritical)
                {
                    SetStatus($"Teste crítico '{testCase.Name}' falhou. Abortando.", true);
                    break;
                }
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