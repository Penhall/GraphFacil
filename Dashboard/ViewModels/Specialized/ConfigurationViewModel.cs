
// Dashboard/ViewModels/Specialized/ConfigurationViewModel.cs - Gerencia configurações globais
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using LotoLibrary.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized;

/// <summary>
/// ViewModel especializado para configurações do sistema
/// Responsabilidade única: gerenciar configurações globais
/// </summary>
public partial class ConfigurationViewModel : ViewModelBase
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

    #region Constructor
    public ConfigurationViewModel(Lances historicalData)
    {
        SystemConfigurations = new ObservableCollection<ConfigurationItem>();
        LoadSystemConfigurations();
    }
    #endregion

    #region Initialization
    private void LoadSystemConfigurations()
    {
        SystemConfigurations.Clear();

        SystemConfigurations.Add(new ConfigurationItem
        {
            Name = "Auto Save",
            Description = "Salvar automaticamente predições",
            Value = AutoSaveEnabled,
            Type = ConfigurationType.Boolean
        });

        SystemConfigurations.Add(new ConfigurationItem
        {
            Name = "Validation Sample Size",
            Description = "Tamanho da amostra para validação",
            Value = ValidationSampleSize,
            Type = ConfigurationType.Integer
        });

        SystemConfigurations.Add(new ConfigurationItem
        {
            Name = "Enable Logging",
            Description = "Habilitar sistema de logs",
            Value = EnableLogging,
            Type = ConfigurationType.Boolean
        });

        SystemConfigurations.Add(new ConfigurationItem
        {
            Name = "Output Directory",
            Description = "Diretório de saída dos arquivos",
            Value = OutputDirectory,
            Type = ConfigurationType.String
        });
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async Task SaveConfiguration()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Aqui implementaria salvamento em arquivo/registry
            await Task.Delay(500); // Simular salvamento

            await ShowSuccessMessageAsync("Configurações salvas com sucesso");

        }, "Salvando configurações...");
    }

    [RelayCommand]
    private async Task LoadConfiguration()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Aqui implementaria carregamento de arquivo/registry
            await Task.Delay(500); // Simular carregamento

            LoadSystemConfigurations();
            await ShowSuccessMessageAsync("Configurações carregadas");

        }, "Carregando configurações...");
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        if (UINotificationService.Instance.AskConfirmation("Restaurar configurações padrão?", "Confirmar"))
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
}

#region Supporting Classes
public class ConfigurationItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public object Value { get; set; }
    public ConfigurationType Type { get; set; }
}

public enum ConfigurationType
{
    Boolean,
    Integer,
    String,
    Double
}
#endregion
