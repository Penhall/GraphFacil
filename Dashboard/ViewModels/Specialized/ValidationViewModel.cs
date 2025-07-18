// Dashboard/ViewModels/Specialized/ValidationViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Core;
using LotoLibrary.Models.Validation;
using LotoLibrary.Services.Analysis;
using LotoLibrary.Suporte;
using ValidationResult = LotoLibrary.Models.Validation.ValidationResult;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    public class ValidationViewModel : ObservableObject
    {
        private Lances _historicalData;
        private readonly IValidationService _validationService;

        public ValidationViewModel(Lances historicalData, IValidationService validationService)
        {
            _historicalData = historicalData;
            _validationService = validationService;

            ValidationResults = new ObservableCollection<ValidationResult>();
            LastValidationSummary = "Nenhuma validação executada";

            RunQuickValidationCommand = new AsyncRelayCommand(RunQuickValidationAsync, CanRunValidation);
            RunFullValidationCommand = new AsyncRelayCommand(RunFullValidationAsync, CanRunValidation);
            CompareModelsCommand = new AsyncRelayCommand(CompareModelsAsync, CanCompareModels);
        }

        private bool _isValidationRunning;
        public bool IsValidationRunning
        {
            get => _isValidationRunning;
            set => SetProperty(ref _isValidationRunning, value);
        }

        private ObservableCollection<ValidationResult> _validationResults;
        public ObservableCollection<ValidationResult> ValidationResults
        {
            get => _validationResults;
            set => SetProperty(ref _validationResults, value);
        }

        private string _lastValidationSummary;
        public string LastValidationSummary
        {
            get => _lastValidationSummary;
            set => SetProperty(ref _lastValidationSummary, value);
        }

        private double _overallAccuracy;
        public double OverallAccuracy
        {
            get => _overallAccuracy;
            set => SetProperty(ref _overallAccuracy, value);
        }

        private int _validationProgress;
        public int ValidationProgress
        {
            get => _validationProgress;
            set => SetProperty(ref _validationProgress, value);
        }

        private bool _isComparisonRunning;
        public bool IsComparisonRunning
        {
            get => _isComparisonRunning;
            set => SetProperty(ref _isComparisonRunning, value);
        }

        private ObservableCollection<LotoLibrary.Suporte.ModelComparisonResult> _comparisonResults;
        public ObservableCollection<LotoLibrary.Suporte.ModelComparisonResult> ComparisonResults
        {
            get => _comparisonResults;
            set => SetProperty(ref _comparisonResults, value);
        }

        public AsyncRelayCommand RunQuickValidationCommand { get; }
        public AsyncRelayCommand RunFullValidationCommand { get; }
        public AsyncRelayCommand CompareModelsCommand { get; }

        private bool CanRunValidation() => !IsValidationRunning && _historicalData != null && _historicalData.Any();

        private bool CanCompareModels() => !IsComparisonRunning && _comparisonResults != null && _comparisonResults.Count > 1;

        private async Task RunValidationAsync(Func<Lances, Task<ValidationResult>> validationMethod)
        {
            if (!CanRunValidation()) return;

            IsValidationRunning = true;
            ValidationProgress = 0;

            try
            {
                var result = await validationMethod(_historicalData);
                ValidationResults = new ObservableCollection<ValidationResult> { result };
                LastValidationSummary = $"Validação: {result.TotalTests} testes, {result.PassedTests} passaram, Acurácia: {result.Accuracy:P2}";
                OverallAccuracy = result.Accuracy;
            }
            catch (Exception ex)
            {
                // Trate exceções (log, notificação ao usuário, etc.)
                Console.WriteLine($"Erro durante validação: {ex.Message}");
            }
            finally
            {
                IsValidationRunning = false;
                ValidationProgress = 100;
            }
        }

        private async Task RunQuickValidationAsync() => await RunValidationAsync(_validationService.RunQuickValidationAsync);

        private async Task RunFullValidationAsync() => await RunValidationAsync(_validationService.RunFullValidationAsync);

        private async Task CompareModelsAsync()
        {
            //  Implementação da comparação de modelos
            IsComparisonRunning = true;
            try
            {
                var performanceComparer = new PerformanceComparer();
                var results = await performanceComparer.CompareModelsAsync(_comparisonResults.Select(x => x.ModelValidationResult).ToList());
                ComparisonResults = new ObservableCollection<LotoLibrary.Suporte.ModelComparisonResult>(results);
            }
            catch (Exception ex)
            {
                // Tratar exceções
                Console.WriteLine($"Erro ao comparar modelos: {ex.Message}");
            }
            finally
            {
                IsComparisonRunning = false;
            }
        }
    }
}

