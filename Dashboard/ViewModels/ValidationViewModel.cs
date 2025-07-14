using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceInfra = LotoLibrary.Services.Infra;

namespace Dashboard.ViewModels
{
    public partial class ValidationViewModel : ModelOperationBase
    {
        private readonly ServiceInfra _infraService;
        private readonly UINotificationService _notificationService;
        private readonly List<IPredictionModel> _availableModels = new();
        private readonly Lances _historico;

        [ObservableProperty]
        private IPredictionModel? _selectedModel;

        [ObservableProperty]
        private bool _isLoading;

        public IReadOnlyList<IPredictionModel> AvailableModels => _availableModels;

        public ValidationViewModel(Lances historico, UINotificationService notificationService)
            : base(historico)
        {
            _infraService = new ServiceInfra();
            _notificationService = notificationService;
            _historico = historico;
        }

        [RelayCommand]
        public async Task ValidateAsync()
        {
            if (SelectedModel == null)
            {
                _notificationService.ShowWarning("Nenhum modelo selecionado");
                return;
            }

            try
            {
                IsLoading = true;
                _notificationService.ShowInfo($"Validando modelo {SelectedModel.Name}...");

                var validationResult = await SelectedModel.ValidateAsync(_historico);
                
                if (validationResult.IsValid)
                {
                    _notificationService.ShowSuccess($"Modelo validado com sucesso! Acurácia: {validationResult.Metrics.Accuracy:P}");
                }
                else
                {
                    _notificationService.ShowWarning($"Modelo validado com problemas: {validationResult.Message}");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                _notificationService.ShowError($"Erro na validação: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoadModelsAsync()
        {
            try
            {
                IsLoading = true;
                _notificationService.ShowInfo("Carregando modelos...");

                await Task.Delay(500); // Simular carregamento
                _availableModels.Clear();
                
                // TODO: Carregar modelos reais aqui
                
                _notificationService.ShowSuccess($"{_availableModels.Count} modelos carregados");
            }
            catch (Exception ex)
            {
                LogError(ex);
                _notificationService.ShowError($"Erro ao carregar modelos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadModelsAsync();
        }

        private void LogError(Exception ex)
        {
            Console.WriteLine($"ERRO: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}");
        }
    }
}
