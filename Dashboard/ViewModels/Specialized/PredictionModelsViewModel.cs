using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceInfra = LotoLibrary.Services.Infra;

namespace Dashboard.ViewModels.Specialized
{
    public partial class PredictionModelsViewModel : ModelOperationBase
    {
        private readonly ServiceInfra _infraService;
        private readonly UINotificationService _notificationService;
        private readonly List<IPredictionModel> _availableModels = new();

        [ObservableProperty]
        private IPredictionModel? _selectedModel;

        [ObservableProperty]
        private bool _isLoading;

        public IReadOnlyList<IPredictionModel> AvailableModels => _availableModels;

        public PredictionModelsViewModel(Lances historico, UINotificationService notificationService)
            : base(historico)
        {
            _infraService = new ServiceInfra();
            _notificationService = notificationService;
        }

        [RelayCommand]
        public async Task LoadModelsAsync()
        {
            try
            {
                IsLoading = true;
                _notificationService.ShowInfo("Carregando modelos...");

                // Simular carregamento de modelos
                await Task.Delay(500);

                _availableModels.Clear();
                // Adicionar modelos disponíveis aqui
                
                _notificationService.ShowSuccess("Modelos carregados com sucesso!");
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Erro ao carregar modelos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task PredictAsync()
        {
            if (SelectedModel == null)
            {
                _notificationService.ShowWarning("Nenhum modelo selecionado");
                return;
            }

            try
            {
                IsLoading = true;
                _notificationService.ShowInfo($"Executando previsão com {SelectedModel.Name}...");

                var result = await SelectedModel.ValidateAsync(Historico);
                
                _notificationService.ShowSuccess($"Previsão concluída: {result}");
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Erro na previsão: {ex.Message}");
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
            _notificationService.ShowInfo("PredictionModelsViewModel inicializado");
        }

        public override async Task Cleanup()
        {
            await base.Cleanup();
            _notificationService.ShowInfo("PredictionModelsViewModel finalizado");
        }
    }
}
