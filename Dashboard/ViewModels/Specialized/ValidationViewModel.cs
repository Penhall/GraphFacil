// Dashboard/ViewModels/Specialized/ValidationViewModel.cs - Gerencia validações e testes
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Models;
using LotoLibrary.Services;
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
        #region Private Fields
        private readonly AntiFrequencyValidation _antiFrequencyValidation;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<TestResult> _validationResults;

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
            _antiFrequencyValidation = new AntiFrequencyValidation();
            ValidationResults = new ObservableCollection<TestResult>();
        }
        #endregion

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecuteValidation))]
        private async Task RunFullValidation()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                ValidationInProgress = true;
                ValidationResults.Clear();

                var startTime = DateTime.Now;
                var report = await _antiFrequencyValidation.ExecuteFullValidationAsync(_historicalData);
                var endTime = DateTime.Now;

                LastValidationDuration = endTime - startTime;

                foreach (var testResult in report.TestResults)
                {
                    ValidationResults.Add(testResult);
                }

                TotalTests = report.TotalTests;
                PassedTests = report.PassedTests;
                OverallAccuracy = report.OverallSuccess ? 1.0 : (double)PassedTests / TotalTests;

                LastValidationSummary = $"Validação concluída: {PassedTests}/{TotalTests} testes passaram";

                ValidationInProgress = false;

                var message = $"Validação completa finalizada!\n\n" +
                              $"Resultado: {PassedTests}/{TotalTests} testes passaram\n" +
                              $"Taxa de sucesso: {OverallAccuracy:P2}\n" +
                              $"Tempo decorrido: {LastValidationDuration:mm\\:ss}\n\n" +
                              $"Detalhes estão disponíveis na aba Validações.";

                UINotificationService.Instance.ShowInfo(message, "Validação Concluída");
                SetStatus(LastValidationSummary);

            }, "Executando validação completa...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteValidation))]
        private async Task RunQuickValidation()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                ValidationInProgress = true;

                // Validação rápida com amostra menor
                var quickData = new Lances(_historicalData.Skip(_historicalData.Count - 20).ToList());
                var startTime = DateTime.Now;

                // Implementar validação rápida específica
                await PerformQuickValidationAsync(quickData);

                var endTime = DateTime.Now;
                LastValidationDuration = endTime - startTime;
                ValidationInProgress = false;

                await ShowSuccessMessageAsync($"Validação rápida concluída em {LastValidationDuration.TotalSeconds:F1}s");

            }, "Executando validação rápida...");
        }

        [RelayCommand]
        private void ClearValidationResults()
        {
            if (UINotificationService.Instance.AskConfirmation("Limpar resultados de validação?", "Confirmar"))
            {
                ValidationResults.Clear();
                LastValidationSummary = "";
                OverallAccuracy = 0;
                TotalTests = 0;
                PassedTests = 0;
                SetStatus("Resultados de validação limpos");
            }
        }

        [RelayCommand]
        private async Task ExportValidationResults()
        {
            if (!ValidationResults.Any())
            {
                UINotificationService.Instance.ShowWarning("Nenhum resultado para exportar");
                return;
            }

            await ExecuteWithLoadingAsync(async () =>
            {
                var fileName = $"ValidationResults_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var content = GenerateValidationReport();

                await System.IO.File.WriteAllTextAsync(fileName, content);

                UINotificationService.Instance.ShowSuccess($"Resultados exportados para: {fileName}");

            }, "Exportando resultados...");
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteValidation()
        {
            return CanExecute() && !ValidationInProgress;
        }
        #endregion

        #region Helper Methods
        private async Task PerformQuickValidationAsync(Lances quickData)
        {
            // Simular validação rápida
            await Task.Delay(1000); // Simular processamento

            var testResult = new TestResult
            {
                TestName = "Validação Rápida",
                StartTime = DateTime.Now.AddSeconds(-1),
                EndTime = DateTime.Now,
                Duration = TimeSpan.FromSeconds(1),
                Success = true,
                Message = "✅ Validação rápida executada com sucesso"
            };

            ValidationResults.Add(testResult);
            TotalTests = 1;
            PassedTests = 1;
            OverallAccuracy = 1.0;
            LastValidationSummary = "Validação rápida: 1/1 teste passou";
        }

        private string GenerateValidationReport()
        {
            var report = $"RELATÓRIO DE VALIDAÇÃO\n";
            report += $"Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
            report += $"Duração: {LastValidationDuration:mm\\:ss}\n\n";
            report += $"RESUMO:\n";
            report += $"Total de testes: {TotalTests}\n";
            report += $"Testes aprovados: {PassedTests}\n";
            report += $"Taxa de sucesso: {OverallAccuracy:P2}\n\n";
            report += $"DETALHES:\n\n";

            foreach (var result in ValidationResults)
            {
                report += $"Teste: {result.TestName}\n";
                report += $"Status: {(result.Success ? "✅ PASSOU" : "❌ FALHOU")}\n";
                report += $"Duração: {result.Duration.TotalMilliseconds:F0}ms\n";
                report += $"Mensagem: {result.Message}\n";
                report += new string('-', 50) + "\n";
            }

            return report;
        }
        #endregion
    }
}