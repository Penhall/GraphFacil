﻿﻿﻿// Dashboard/ViewModels/Specialized/ConfigurationViewModel.cs - Gerencia configurações globais
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Configuration;

    /// <summary>
    /// ViewModel especializado para configurações do sistema
    /// Responsabilidade única: gerenciar configurações globais
    /// </summary>
    public partial class ConfigurationViewModel : ModelOperationBase
    {
        #region Observable Properties
    [ObservableProperty]
        private ObservableCollection<ConfigurationItem> _systemConfigurations;

        [ObservableProperty]
        private bool _autoSaveEnabled = true;

        [ObservableProperty]
        private int _validationSampleSize = 50;

        [ObservableProperty]
        private bool _enableLogging = true;

        [ObservableProperty]
        private string _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        #endregion

    [ObservableProperty]
    private object _selectedModel; // Pode ser IPredictionModel ou ModelInfo

    [ObservableProperty]
    private ObservableCollection<ModelParameter> _modelParameters = new();

    #region Constructor
    public ConfigurationViewModel(Lances historicalData, IModelFactory modelFactory) : base(historicalData)
    {
        _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
        _systemConfigurations = new ObservableCollection<ConfigurationItem>();
    }
    #endregion
    #region Initialization Override
    protected override async Task InitializeSpecificAsync()
    {
        SetStatus("ConfigurationViewModel inicializado");
        await Task.CompletedTask;
    }
    #endregion
    #region Configuration Management
    private void LoadSystemConfigurations()
    {
        _systemConfigurations.Clear();
        _systemConfigurations.Add(new ConfigurationItem { Name = "Auto Save", Description = "Salvar automaticamente predições", Value = AutoSaveEnabled, Type = ConfigurationType.Boolean });
        _systemConfigurations.Add(new ConfigurationItem { Name = "Validation Sample Size", Description = "Tamanho da amostra para validação", Value = ValidationSampleSize, Type = ConfigurationType.Integer });
        _systemConfigurations.Add(new ConfigurationItem { Name = "Enable Logging", Description = "Habilitar sistema de logs", Value = EnableLogging, Type = ConfigurationType.Boolean });
        _systemConfigurations.Add(new ConfigurationItem { Name = "Output Directory", Description = "Diretório de saída dos arquivos", Value = OutputDirectory, Type = ConfigurationType.String });
    }
    #endregion
    #region Commands
    [RelayCommand]
    private async Task SaveConfiguration()
    {
        await ExecuteWithLoadingAsync(async () =>
    {
            // Simulação de salvamento
            await Task.Delay(500);
            await ShowSuccessMessageAsync("Configurações salvas com sucesso");
    }, "Salvando configurações...");
    }
    [RelayCommand]
    private async Task LoadConfiguration()
    {
        await ExecuteWithLoadingAsync(async () =>
    {
            // Simulação de carregamento
            await Task.Delay(500);
            LoadSystemConfigurations();
            await ShowSuccessMessageAsync("Configurações carregadas");
    }, "Carregando configurações...");
    }
    [RelayCommand]
    private void ResetToDefaults()
    {
        if (System.Windows.MessageBox.Show("Restaurar configurações padrão?", "Confirmar", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
        {
            AutoSaveEnabled = true;
            ValidationSampleSize = 50;
            EnableLogging = true;
            OutputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            LoadSystemConfigurations();
            SetStatus("Configurações restauradas ao padrão");
        }
    }
    #endregion
    #region Model Configuration
    partial void OnSelectedModelChanged(object value)
    {
        if (value is ModelInfo modelInfo)
        {
            var model = _modelFactory.CreateModel(modelInfo.Type);
            LoadModelParameters(model);
        }
        else if (value is IPredictionModel model)
        {
            LoadModelParameters(model);
        }
        else
        {
            ModelParameters.Clear();
        }
    }
    private void LoadModelParameters(IPredictionModel model)
    {
        ModelParameters.Clear();
        if (model is IConfigurableModel configurableModel)
        {
            foreach (var param in configurableModel.DefaultParameters)
            {
                ModelParameters.Add(new ModelParameter { Name = param.Key, Value = param.Value });
            }
        }
    }
    [RelayCommand]
    private async Task SaveModelConfiguration()
    {
        if (SelectedModel is not IPredictionModel model || model is not IConfigurableModel configurableModel)
        {
            SetStatus("Modelo não configurável selecionado.", true);
            return;
        }
        await ExecuteWithLoadingAsync(async () =>
    {
            foreach (var param in ModelParameters)
            {
                configurableModel.SetParameter(param.Name, param.Value);
            }
            await ShowSuccessMessageAsync($"Configurações do modelo {model.ModelName} salvas.");
    }, "Salvando configurações do modelo...");
    }
    #endregion
}
public class ModelParameter
{
    public string Name { get; set; }
    public object Value { get; set; }
}
