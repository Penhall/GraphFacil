using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using LotoLibrary.Models.Configuration;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Specialized
{
    /// <summary>
    /// ViewModel para configuração do sistema
    /// </summary>
    public partial class ConfigurationViewModel : ModelOperationBase
    {
        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<ConfigurationItem> _configurationItems = new();

        [ObservableProperty]
        private ConfigurationItem? _selectedConfigurationItem;

        [ObservableProperty]
        private string _configurationStatus = "Nenhuma configuração carregada";

        [ObservableProperty]
        private bool _isConfigurationModified = false;
        #endregion

        #region Dependency Injection
        private IConfigurableModel _configurableModel;
        #endregion

        #region Constructor
        public ConfigurationViewModel(Lances historicalData, IConfigurableModel configurableModel) : base(historicalData)
        {
            _configurableModel = configurableModel ?? throw new ArgumentNullException(nameof(configurableModel));
        }
        #endregion

        #region Initialization Override
        protected override async Task InitializeSpecificAsync()
        {
            await LoadConfigurationAsync();
            SetStatus("✅ ConfigurationViewModel inicializado");
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task LoadConfigurationAsync()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                try
                {
                    ConfigurationItems.Clear();

                    var defaultParams = _configurableModel.DefaultParameters;
                    var currentParams = _configurableModel.CurrentParameters;

                    foreach (var param in defaultParams)
                    {
                        ConfigurationItems.Add(new ConfigurationItem
                        {
                            Name = param.Key,
                            Description = _configurableModel.GetParameterDescription(param.Key),
                            Value = currentParams.ContainsKey(param.Key) ? currentParams[param.Key] : param.Value,
                            Type = GetConfigurationType(param.Value)
                        });
                    }

                    ConfigurationStatus = $"Configuração carregada - {ConfigurationItems.Count} itens";
                    IsConfigurationModified = false;
                }
                catch (Exception ex)
                {
                    SetStatus($"Erro ao carregar configuração: {ex.Message}", true);
                    LogError(ex);
                }
            }, "Carregando configurações...");
        }

        [RelayCommand]
        private async Task SaveConfigurationAsync()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                try
                {
                    var parameters = ConfigurationItems.ToDictionary(
                        item => item.Name,
                        item => item.Value);

                    _configurableModel.UpdateParameters(parameters);
                    IsConfigurationModified = false;
                    ConfigurationStatus = $"Configuração salva - {DateTime.Now:HH:mm:ss}";
                    await ShowSuccessMessageAsync("Configuração salva com sucesso!");
                }
                catch (Exception ex)
                {
                    SetStatus($"Erro ao salvar configuração: {ex.Message}", true);
                    LogError(ex);
                }
            }, "Salvando configurações...");
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            try
            {
                _configurableModel.ResetToDefaults();
                LoadConfigurationAsync().ConfigureAwait(false);
                SetStatus("Configurações restauradas para padrão");
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao restaurar padrões: {ex.Message}", true);
                LogError(ex);
            }
        }
        #endregion

        #region Private Methods
        private ConfigurationType GetConfigurationType(object value)
        {
            return value switch
            {
                bool _ => ConfigurationType.Boolean,
                int _ => ConfigurationType.Integer,
                double _ => ConfigurationType.Decimal,
                string _ => ConfigurationType.String,
                IEnumerable<object> _ => ConfigurationType.List,
                _ => ConfigurationType.Complex
            };
        }

        private void OnConfigurationItemChanged()
        {
            IsConfigurationModified = true;
            ConfigurationStatus = $"Configuração modificada - {DateTime.Now:HH:mm:ss}";
        }
        #endregion
    }
}
