// Dashboard/ViewModels/Specialized/ComparisonViewModel.cs - Gerencia comparações entre modelos
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Enums;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// ViewModel especializado para comparações entre modelos
    /// Responsabilidade única: comparar performance e diversificação
    /// </summary>
    public partial class ComparisonViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<ModelComparison> _comparisons;

        [ObservableProperty]
        private string _comparisonSummary = "";

        [ObservableProperty]
        private ObservableCollection<ModelType> _availableModelsForComparison;

        [ObservableProperty]
        private ObservableCollection<ModelType> _selectedModelsForComparison;

        [ObservableProperty]
        private string _targetConcursoForComparison = "";

        [ObservableProperty]
        private bool _comparisonInProgress;
        #endregion

        #region Constructor
        public ComparisonViewModel(Lances historicalData) : base(historicalData)
        {
            Comparisons = new ObservableCollection<ModelComparison>();
            AvailableModelsForComparison = new ObservableCollection<ModelType>();
            SelectedModelsForComparison = new ObservableCollection<ModelType>();

            LoadAvailableModels();
        }
        #endregion

        #region Initialization
        private void LoadAvailableModels()
        {
            var availableTypes = _modelFactory.GetAvailableModelTypes();
            AvailableModelsForComparison.Clear();

            foreach (var modelType in availableTypes)
            {
                AvailableModelsForComparison.Add(modelType);
            }
        }
        #endregion

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecuteComparison))]
        private async Task CompareSelectedModels()
        {
            if (SelectedModelsForComparison.Count < 2)
            {
                UINotificationService.Instance.ShowWarning("Selecione pelo menos 2 modelos para comparar");
                return;
            }

            await ExecuteWithLoadingAsync(async () =>
            {
                ComparisonInProgress = true;
                Comparisons.Clear();

                var targetConcurso = GetTargetConcursoForComparison();
                var predictions = new Dictionary<ModelType, PredictionResult>();

                // Gerar predições para todos os modelos selecionados
                foreach (var modelType in SelectedModelsForComparison)
                {
                    var model = await CreateAndTrainModelAsync(modelType);
                    predictions[modelType] = await model.PredictAsync(targetConcurso);
                }

                // Criar comparações par a par
                for (int i = 0; i < SelectedModelsForComparison.Count; i++)
                {
                    for (int j = i + 1; j < SelectedModelsForComparison.Count; j++)
                    {
                        var model1 = SelectedModelsForComparison[i];
                        var model2 = SelectedModelsForComparison[j];

                        var prediction1 = predictions[model1];
                        var prediction2 = predictions[model2];

                        if (prediction1.Success && prediction2.Success)
                        {
                            var comparison = CreateModelComparison(model1, model2, prediction1, prediction2, targetConcurso);
                            Comparisons.Add(comparison);
                        }
                    }
                }

                ComparisonSummary = $"Comparação de {SelectedModelsForComparison.Count} modelos concluída para concurso {targetConcurso}";
                ComparisonInProgress = false;

                await ShowSuccessMessageAsync("Comparação concluída com sucesso");

            }, "Comparando modelos selecionados...");
        }

        [RelayCommand]
        private void AddModelToComparison(ModelType modelType)
        {
            if (!SelectedModelsForComparison.Contains(modelType))
            {
                SelectedModelsForComparison.Add(modelType);
                SetStatus($"Modelo {modelType} adicionado à comparação");
            }
        }

        [RelayCommand]
        private void RemoveModelFromComparison(ModelType modelType)
        {
            if (SelectedModelsForComparison.Contains(modelType))
            {
                SelectedModelsForComparison.Remove(modelType);
                SetStatus($"Modelo {modelType} removido da comparação");
            }
        }

        [RelayCommand]
        private void ClearModelSelection()
        {
            SelectedModelsForComparison.Clear();
            SetStatus("Seleção de modelos limpa");
        }

        [RelayCommand]
        private void ClearComparisons()
        {
            if (UINotificationService.Instance.AskConfirmation("Limpar resultados de comparação?", "Confirmar"))
            {
                Comparisons.Clear();
                ComparisonSummary = "";
                SetStatus("Comparações limpas");
            }
        }

        [RelayCommand]
        private async Task ExportComparisons()
        {
            if (!Comparisons.Any())
            {
                UINotificationService.Instance.ShowWarning("Nenhuma comparação para exportar");
                return;
            }

            await ExecuteWithLoadingAsync(async () =>
            {
                var fileName = $"ModelComparisons_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var content = GenerateComparisonReport();

                await System.IO.File.WriteAllTextAsync(fileName, content);

                UINotificationService.Instance.ShowSuccess($"Comparações exportadas para: {fileName}");

            }, "Exportando comparações...");
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteComparison()
        {
            return CanExecute() && !ComparisonInProgress && SelectedModelsForComparison.Count >= 2;
        }
        #endregion

        #region Helper Methods
        private int GetTargetConcursoForComparison()
        {
            if (int.TryParse(TargetConcursoForComparison, out var concurso))
            {
                return concurso;
            }
            return GetNextConcurso();
        }

        private ModelComparison CreateModelComparison(ModelType model1, ModelType model2,
            PredictionResult prediction1, PredictionResult prediction2, int targetConcurso)
        {
            var commonNumbers = prediction1.PredictedNumbers.Intersect(prediction2.PredictedNumbers).Count();
            var diversificationRate = 1.0 - (double)commonNumbers / 15;

            return new ModelComparison
            {
                Model1Name = model1.ToString(),
                Model2Name = model2.ToString(),
                Model1Confidence = prediction1.Confidence,
                Model2Confidence = prediction2.Confidence,
                CommonNumbers = commonNumbers,
                DiversificationRate = diversificationRate,
                Model1Numbers = prediction1.PredictedNumbers.ToList(),
                Model2Numbers = prediction2.PredictedNumbers.ToList(),
                TargetConcurso = targetConcurso,
                ComparisonTime = DateTime.Now,
                Model1FormattedNumbers = Phase1Utilities.FormatPredictionNumbers(prediction1.PredictedNumbers.ToArray()),
                Model2FormattedNumbers = Phase1Utilities.FormatPredictionNumbers(prediction2.PredictedNumbers.ToArray())
            };
        }

        private string GenerateComparisonReport()
        {
            var report = $"RELATÓRIO DE COMPARAÇÃO DE MODELOS\n";
            report += $"Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
            report += $"Total de comparações: {Comparisons.Count}\n\n";

            foreach (var comparison in Comparisons)
            {
                report += $"COMPARAÇÃO: {comparison.Model1Name} vs {comparison.Model2Name}\n";
                report += $"Concurso: {comparison.TargetConcurso}\n";
                report += $"Confiança {comparison.Model1Name}: {comparison.Model1Confidence:P2}\n";
                report += $"Confiança {comparison.Model2Name}: {comparison.Model2Confidence:P2}\n";
                report += $"Números comuns: {comparison.CommonNumbers}/15\n";
                report += $"Taxa de diversificação: {comparison.DiversificationRate:P2}\n";
                report += $"{comparison.Model1Name}: {comparison.Model1FormattedNumbers}\n";
                report += $"{comparison.Model2Name}: {comparison.Model2FormattedNumbers}\n";
                report += new string('-', 60) + "\n";
            }

            return report;
        }
        #endregion
    }

    #region Supporting Classes
    public class ModelComparison
    {
        public string Model1Name { get; set; }
        public string Model2Name { get; set; }
        public double Model1Confidence { get; set; }
        public double Model2Confidence { get; set; }
        public int CommonNumbers { get; set; }
        public double DiversificationRate { get; set; }
        public List<int> Model1Numbers { get; set; }
        public List<int> Model2Numbers { get; set; }
        public int TargetConcurso { get; set; }
        public DateTime ComparisonTime { get; set; }
        public string Model1FormattedNumbers { get; set; }
        public string Model2FormattedNumbers { get; set; }
    }
    #endregion
}
