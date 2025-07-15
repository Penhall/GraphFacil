// Dashboard/ViewModels/Specialized/PredictionModelsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// ViewModel especializado para gerenciar modelos de predição
    /// Responsabilidade única: operações relacionadas a predições
    /// </summary>
    public partial class PredictionModelsViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private string _lastPredictionResult = "";

        [ObservableProperty]
        private double _selectedModelConfidence = 0.0;

        [ObservableProperty]
        private ObservableCollection<string> _predictionHistory = new();

        [ObservableProperty]
        private string _selectedModelName = "";

        [ObservableProperty]
        private bool _isModelLoaded = false;

        [ObservableProperty]
        private string _modelConfigurationPath = "";

        [ObservableProperty]
        private ObservableCollection<ModelInfo> _availableModelInfos = new();

        [ObservableProperty]
        private ModelInfo? _selectedModelInfo;

        [ObservableProperty]
        private int _totalAvailableModels = 0;
        #endregion

        #region Constructor
        public PredictionModelsViewModel(Lances historicalData) : base(historicalData)
        {
            PredictionHistory = new ObservableCollection<string>();
            AvailableModelInfos = new ObservableCollection<ModelInfo>();
        }
        #endregion

        #region Initialization Override
        protected override async Task InitializeSpecificAsync()
        {
            SetStatus("Inicializando PredictionModelsViewModel...");
            await LoadAvailableModelsAsync();
            SetStatus("✅ PredictionModelsViewModel inicializado");
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task LoadModelConfiguration()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                try
                {
                    // Simular carregamento de configuração de modelo
                    await Task.Delay(1000);

                    if (!string.IsNullOrEmpty(ModelConfigurationPath))
                    {
                        IsModelLoaded = true;
                        SelectedModelConfidence = 85.5;
                        await ShowSuccessMessageAsync($"Configuração carregada: {ModelConfigurationPath}");
                    }
                    else
                    {
                        SetStatus("Caminho de configuração não especificado", true);
                    }
                }
                catch (Exception ex)
                {
                    SetStatus($"Erro ao carregar configuração: {ex.Message}", true);
                    LogError(ex);
                }
            }, "Carregando configuração do modelo...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteQuickPredict))]
        private async Task QuickPredict()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                try
                {
                    if (SelectedModelInfo == null)
                    {
                        SetStatus("Nenhum modelo selecionado", true);
                        return;
                    }

                    // Criar e usar o modelo real
                    var model = _modelFactory.CreateModel(SelectedModelInfo.Type);
                    if (!model.IsInitialized)
                    {
                        SetStatus("Inicializando modelo...");
                        await model.InitializeAsync(_historicalData);
                        await model.TrainAsync(_historicalData);
                    }

                    // Gerar predição real
                    var concurso = _historicalData?.Any() == true ? _historicalData.Max(l => l.Id) : 3000;
                    var predictionResult = await model.PredictAsync(concurso + 1);

                    LastPredictionResult = string.Join(", ", predictionResult.PredictedNumbers);
                    SelectedModelConfidence = predictionResult.Confidence * 100;

                    // Adicionar ao histórico
                    var historyEntry = $"[{DateTime.Now:HH:mm:ss}] {LastPredictionResult} (Confiança: {SelectedModelConfidence:F1}%)";
                    PredictionHistory.Add(historyEntry);

                    await ShowSuccessMessageAsync($"Predição gerada com modelo {SelectedModelInfo.Name}!");
                }
                catch (Exception ex)
                {
                    SetStatus($"Erro na predição: {ex.Message}", true);
                    LogError(ex);
                }
            }, "Gerando predição...");
        }

        [RelayCommand]
        private void ClearHistory()
        {
            PredictionHistory.Clear();
            LastPredictionResult = "";
            SelectedModelConfidence = 0.0;
            SetStatus("Histórico de predições limpo");
        }

        [RelayCommand]
        private async Task RefreshModels()
        {
            await LoadAvailableModelsAsync();
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteQuickPredict()
        {
            return CanExecute() && SelectedModelInfo != null && _historicalData != null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// CORREÇÃO: Carrega modelos reais usando o ModelFactory
        /// </summary>
        private async Task LoadAvailableModelsAsync()
        {
            try
            {
                SetStatus("Carregando modelos disponíveis...");
                await Task.Delay(500); // Simular tempo de carregamento

                AvailableModelInfos.Clear();

                // CORREÇÃO: Usar ModelFactory para obter tipos de modelos disponíveis
                var availableTypes = _modelFactory.GetAvailableModelTypes();

                foreach (var modelType in availableTypes)
                {
                    try
                    {
                        // Obter informações do modelo
                        var modelInfo = _modelFactory.GetModelInfo(modelType);
                        if (modelInfo != null)
                        {
                            AvailableModelInfos.Add(modelInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        SetStatus($"Erro ao carregar modelo {modelType}: {ex.Message}", true);
                        LogError(ex);
                    }
                }

                TotalAvailableModels = AvailableModelInfos.Count;

                // Selecionar primeiro modelo disponível automaticamente
                if (AvailableModelInfos.Any())
                {
                    SelectedModelInfo = AvailableModelInfos.First();
                    SelectedModelName = SelectedModelInfo.Name;
                    IsModelLoaded = true;
                }

                SetStatus($"✅ {TotalAvailableModels} modelo(s) carregado(s) com sucesso");
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao carregar modelos: {ex.Message}", true);
                LogError(ex);
                TotalAvailableModels = 0;
            }
        }

        /// <summary>
        /// Obtém modelos carregados no PredictionEngine
        /// </summary>
        private async Task LoadModelsFromEngine()
        {
            try
            {
                // Garantir que o PredictionEngine está inicializado
                if (!_predictionEngine.IsInitialized)
                {
                    await _predictionEngine.InitializeAsync(_historicalData);
                }

                var engineModels = _predictionEngine.Models;
                SetStatus($"PredictionEngine tem {engineModels.Count} modelo(s) registrado(s)");

                // Adicionar modelos do engine às informações disponíveis
                foreach (var kvp in engineModels)
                {
                    var modelName = kvp.Key;
                    var model = kvp.Value;

                    var historyEntry = $"Modelo registrado: {modelName} ({model.ModelName})";
                    if (PredictionHistory.Count < 10) // Limitar histórico
                    {
                        PredictionHistory.Add(historyEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao acessar modelos do engine: {ex.Message}", true);
                LogError(ex);
            }
        }
        #endregion

        #region Partial Methods for Property Changes
        partial void OnSelectedModelInfoChanged(ModelInfo? value)
        {
            if (value != null)
            {
                SelectedModelName = value.Name;
                SelectedModelConfidence = value.EstimatedAccuracy * 100;
                SetStatus($"Modelo selecionado: {value.Name}");
            }
        }
        #endregion
    }
}