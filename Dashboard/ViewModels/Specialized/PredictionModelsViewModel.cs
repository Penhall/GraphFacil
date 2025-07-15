// Dashboard/ViewModels/Specialized/PredictionModelsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
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
        #endregion

        #region Constructor
        public PredictionModelsViewModel(Lances historicalData) : base(historicalData)
        {
            PredictionHistory = new ObservableCollection<string>();
        }
        #endregion

        #region Initialization Override
        protected override async Task InitializeSpecificAsync()
        {
            SetStatus("✅ PredictionModelsViewModel inicializado");
            await LoadAvailableModelsAsync();
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
                        // CORREÇÃO: Usar método da classe base
                        await ShowSuccessMessageAsync($"Configuração carregada: {ModelConfigurationPath}");
                    }
                    else
                    {
                        SetStatus("Caminho de configuração não especificado", true);
                    }
                }
                catch (Exception ex)
                {
                    // CORREÇÃO: Usar SetStatus em vez de ShowErrorMessageAsync 
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
                    // Gerar predição rápida
                    // CORREÇÃO: Usar _historicalData em vez de HistoricalData
                    var concurso = _historicalData?.Max(l => l.Id) ?? 3000;
                    var result = await GeneratePredictionAsync(concurso + 1);

                    LastPredictionResult = result;
                    SelectedModelConfidence = Random.Shared.NextDouble() * 100;

                    // Adicionar ao histórico
                    var historyEntry = $"[{DateTime.Now:HH:mm:ss}] {result} (Confiança: {SelectedModelConfidence:F1}%)";
                    PredictionHistory.Add(historyEntry);

                    await ShowSuccessMessageAsync("Predição rápida gerada com sucesso!");
                }
                catch (Exception ex)
                {
                    // CORREÇÃO: Usar SetStatus e LogError
                    SetStatus($"Erro na predição: {ex.Message}", true);
                    LogError(ex);
                }
            }, "Gerando predição rápida...");
        }

        [RelayCommand]
        private void ClearHistory()
        {
            PredictionHistory.Clear();
            LastPredictionResult = "";
            SelectedModelConfidence = 0.0;
            SetStatus("Histórico de predições limpo");
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteQuickPredict()
        {
            // CORREÇÃO: Usar _historicalData
            return CanExecute() && IsModelLoaded && _historicalData != null;
        }
        #endregion

        #region Private Methods
        private async Task LoadAvailableModelsAsync()
        {
            try
            {
                await Task.Delay(500); // Simular carregamento
                SelectedModelName = "Modelo Padrão";
                IsModelLoaded = true; // Permitir uso imediato
                SetStatus("Modelos disponíveis carregados");
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao carregar modelos: {ex.Message}", true);
                LogError(ex);
            }
        }

        private async Task<string> GeneratePredictionAsync(int concurso)
        {
            // Simulação de predição - substituir pela lógica real
            await Task.Delay(1500);

            var numeros = new List<int>();
            var random = Random.Shared;

            while (numeros.Count < 15)
            {
                var numero = random.Next(1, 26);
                if (!numeros.Contains(numero))
                {
                    numeros.Add(numero);
                }
            }

            numeros.Sort();
            return string.Join(", ", numeros);
        }
        #endregion
    }
}