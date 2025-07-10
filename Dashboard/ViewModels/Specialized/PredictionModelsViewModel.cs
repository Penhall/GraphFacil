// Dashboard/ViewModels/Specialized/PredictionModelsViewModel.cs - Gerencia TODOS os modelos de predição
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Enums;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// ViewModel especializado para gerenciar TODOS os modelos de predição
    /// Responsabilidade única: operações com modelos (criar, treinar, predizer, configurar)
    /// </summary>
    public partial class PredictionModelsViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<ModelInfo> _availableModels;

        [ObservableProperty]
        private ModelInfo _selectedModel;

        [ObservableProperty]
        private string _lastPredictionResult = "";

        [ObservableProperty]
        private double _selectedModelConfidence;

        [ObservableProperty]
        private bool _isPredictionEngineReady;

        [ObservableProperty]
        private Dictionary<string, object> _selectedModelParameters;

        [ObservableProperty]
        private ObservableCollection<PredictionHistory> _predictionHistory;

        [ObservableProperty]
        private string _targetConcurso = "";
        #endregion

        #region Constructor
        public PredictionModelsViewModel(Lances historicalData) : base(historicalData)
        {
            AvailableModels = new ObservableCollection<ModelInfo>();
            PredictionHistory = new ObservableCollection<PredictionHistory>();

            // Inicializar automaticamente
            _ = InitializeAsync();
        }
        #endregion

        #region Initialization
        protected override async Task InitializeSpecificAsync()
        {
            LoadAvailableModels();
            IsPredictionEngineReady = true;

            // Selecionar primeiro modelo disponível
            if (AvailableModels.Any())
            {
                SelectedModel = AvailableModels.First();
            }

            SetStatus("✅ Modelos de predição prontos");
        }

        private void LoadAvailableModels()
        {
            var modelTypes = _modelFactory.GetAvailableModelTypes();
            AvailableModels.Clear();

            foreach (var modelType in modelTypes)
            {
                var modelInfo = _modelFactory.GetModelInfo(modelType);
                if (modelInfo != null && modelInfo.Status == ModelStatus.Available)
                {
                    AvailableModels.Add(modelInfo);
                }
            }
        }
        #endregion

        #region Commands
        [RelayCommand(CanExecute = nameof(CanExecuteModelOperations))]
        private async Task GeneratePrediction()
        {
            if (SelectedModel == null)
            {
                UINotificationService.Instance.ShowWarning("Nenhum modelo selecionado");
                return;
            }

            if (!ValidateHistoricalData())
            {
                return;
            }

            await ExecuteWithLoadingAsync(async () =>
            {
                // Determinar concurso alvo
                var targetConcurso = GetTargetConcurso();

                // Criar e treinar modelo
                var model = await CreateAndTrainModelAsync(SelectedModel.Type);

                // Gerar predição
                var prediction = await model.PredictAsync(targetConcurso);

                if (prediction.Success)
                {
                    var numbers = FormatPredictionNumbers(prediction.PredictedNumbers.ToArray());
                    LastPredictionResult = $"Concurso {targetConcurso}: {numbers}";
                    SelectedModelConfidence = prediction.Confidence;

                    // Adicionar ao histórico
                    AddToHistory(prediction, targetConcurso);

                    // Salvar resultado
                    await SavePredictionAsync(prediction, targetConcurso);

                    await ShowSuccessMessageAsync($"Predição gerada com {prediction.Confidence:P2} confiança");
                }
                else
                {
                    SetStatus($"Erro na predição: {prediction.ErrorMessage}", true);
                    UINotificationService.Instance.ShowError(prediction.ErrorMessage, "Erro na Predição");
                }
            }, $"Gerando predição com {SelectedModel.Name}...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteModelOperations))]
        private async Task ValidateSelectedModel()
        {
            if (SelectedModel == null)
            {
                UINotificationService.Instance.ShowWarning("Nenhum modelo selecionado");
                return;
            }

            await ExecuteWithLoadingAsync(async () =>
            {
                var model = await CreateAndTrainModelAsync(SelectedModel.Type);
                var validationResult = await model.ValidateAsync(_historicalData);

                if (validationResult.Success)
                {
                    var message = $"Validação do {SelectedModel.Name}:\n\n" +
                                  $"Accuracy: {validationResult.Accuracy:P2}\n" +
                                  $"Testes: {validationResult.SuccessfulPredictions}/{validationResult.TotalTests}\n" +
                                  $"Tempo: {validationResult.TestEndTime - validationResult.TestStartTime:mm\\:ss}";

                    UINotificationService.Instance.ShowInfo(message, "Validação Concluída");
                    SetStatus($"✅ Validação: {validationResult.Accuracy:P2} accuracy");
                }
                else
                {
                    SetStatus($"Erro na validação: {validationResult.ErrorMessage}", true);
                    UINotificationService.Instance.ShowError(validationResult.ErrorMessage, "Erro na Validação");
                }
            }, $"Validando {SelectedModel.Name}...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteConfigurable))]
        private void LoadModelConfiguration()
        {
            if (SelectedModel == null || !SelectedModel.IsConfigurable)
            {
                return;
            }

            try
            {
                var model = _modelFactory.CreateModel(SelectedModel.Type);
                if (model is IConfigurableModel configurableModel)
                {
                    SelectedModelParameters = new Dictionary<string, object>(configurableModel.GetDefaultParameters());
                    SetStatus("Configuração carregada");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao carregar configuração: {ex.Message}", true);
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteConfigurable))]
        private void UpdateModelParameters()
        {
            if (SelectedModel == null || SelectedModelParameters == null)
            {
                UINotificationService.Instance.ShowWarning("Nenhuma configuração disponível");
                return;
            }

            try
            {
                var model = _modelFactory.CreateModel(SelectedModel.Type);
                if (model is IConfigurableModel configurableModel)
                {
                    if (configurableModel.ValidateParameters(SelectedModelParameters))
                    {
                        configurableModel.UpdateParameters(SelectedModelParameters);
                        SetStatus("✅ Parâmetros atualizados");
                        UINotificationService.Instance.ShowSuccess("Parâmetros atualizados com sucesso");
                    }
                    else
                    {
                        SetStatus("Parâmetros inválidos", true);
                        UINotificationService.Instance.ShowWarning("Os parâmetros fornecidos são inválidos");
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao atualizar parâmetros: {ex.Message}", true);
                UINotificationService.Instance.ShowError(ex.Message, "Erro nos Parâmetros");
            }
        }

        [RelayCommand]
        private void ClearHistory()
        {
            if (UINotificationService.Instance.AskConfirmation("Limpar histórico de predições?", "Confirmar"))
            {
                PredictionHistory.Clear();
                SetStatus("Histórico limpo");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteModelOperations))]
        private async Task QuickPredict()
        {
            // Predição rápida sem configurações
            TargetConcurso = GetNextConcurso().ToString();
            await GeneratePrediction();
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteModelOperations()
        {
            return CanExecute() && SelectedModel != null && IsPredictionEngineReady;
        }

        private bool CanExecuteConfigurable()
        {
            return CanExecuteModelOperations() && SelectedModel.IsConfigurable;
        }
        #endregion

        #region Model Selection Handler
        partial void OnSelectedModelChanged(ModelInfo value)
        {
            if (value != null)
            {
                SetStatus($"Modelo selecionado: {value.Name}");
                SelectedModelParameters = null;
                SelectedModelConfidence = 0;

                // Carregar configuração se disponível
                if (value.IsConfigurable)
                {
                    LoadModelConfiguration();
                }
            }
        }
        #endregion

        #region Helper Methods
        private int GetTargetConcurso()
        {
            if (int.TryParse(TargetConcurso, out var concurso))
            {
                return concurso;
            }
            return GetNextConcurso();
        }

        private void AddToHistory(PredictionResult prediction, int targetConcurso)
        {
            var historyItem = new PredictionHistory
            {
                Timestamp = DateTime.Now,
                ModelName = SelectedModel.Name,
                TargetConcurso = targetConcurso,
                PredictedNumbers = prediction.PredictedNumbers.ToList(),
                Confidence = prediction.Confidence,
                FormattedNumbers = FormatPredictionNumbers(prediction.PredictedNumbers.ToArray())
            };

            PredictionHistory.Insert(0, historyItem); // Mais recente primeiro

            // Manter apenas últimas 50 predições
            while (PredictionHistory.Count > 50)
            {
                PredictionHistory.RemoveAt(PredictionHistory.Count - 1);
            }
        }

        private async Task SavePredictionAsync(PredictionResult prediction, int targetConcurso)
        {
            try
            {
                var fileName = $"{SelectedModel.Name.Replace(" ", "")}_C{targetConcurso}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var content = $"PREDIÇÃO - {SelectedModel.Name}\n" +
                              $"Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                              $"Concurso Alvo: {targetConcurso}\n" +
                              $"Confiança: {prediction.Confidence:P2}\n\n" +
                              $"Números preditos:\n{FormatPredictionNumbers(prediction.PredictedNumbers.ToArray())}\n\n" +
                              $"Explicação:\n{prediction.Explanation}";

                await File.WriteAllTextAsync(fileName, content);

                // Também salvar no formato Lance para compatibilidade
                var lance = new Lance
                {
                    Concurso = targetConcurso,
                    DezenasSorteadas = prediction.PredictedNumbers.ToArray()
                };

                var lances = new Lances { lance };
                Infra.SalvaSaidaW(lances, fileName.Replace(".txt", "_Lance.txt"));
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Não falhar a operação principal por erro de salvamento
            }
        }
        #endregion
    }

    #region Supporting Classes
    public class PredictionHistory
    {
        public DateTime Timestamp { get; set; }
        public string ModelName { get; set; }
        public int TargetConcurso { get; set; }
        public List<int> PredictedNumbers { get; set; }
        public double Confidence { get; set; }
        public string FormattedNumbers { get; set; }
    }
    #endregion
}



