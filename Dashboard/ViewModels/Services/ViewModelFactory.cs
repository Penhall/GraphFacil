// Dashboard/ViewModels/Services/ViewModelFactory.cs
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Models.Core;
using LotoLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.ViewModels.Services
{
    /// <summary>
    /// Factory para criação de ViewModels especializados
    /// Centraliza criação e garante configuração correta
    /// </summary>
    public class ViewModelFactory
    {
        #region Private Fields
        private readonly Lances _historicalData;
        private readonly UINotificationService _notificationService;
        #endregion

        #region Constructor
        public ViewModelFactory(Lances historicalData)
        {
            _historicalData = historicalData ?? throw new ArgumentNullException(nameof(historicalData));
            _notificationService = UINotificationService.Instance;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria PredictionModelsViewModel com dependências corretas
        /// </summary>
        public PredictionModelsViewModel CreatePredictionModelsViewModel()
        {
            try
            {
                var viewModel = new PredictionModelsViewModel(_historicalData);
                return viewModel;
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Erro ao criar PredictionModelsViewModel: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cria ValidationViewModel com UINotificationService
        /// </summary>
        public ValidationViewModel CreateValidationViewModel()
        {
            try
            {
                // CORREÇÃO: Adicionar UINotificationService como segundo parâmetro
                var viewModel = new ValidationViewModel(_historicalData, _notificationService);
                return viewModel;
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Erro ao criar ValidationViewModel: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cria ComparisonViewModel
        /// </summary>
        public ComparisonViewModel CreateComparisonViewModel()
        {
            try
            {
                var viewModel = new ComparisonViewModel(_historicalData);
                return viewModel;
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Erro ao criar ComparisonViewModel: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cria ConfigurationViewModel
        /// </summary>
        public ConfigurationViewModel CreateConfigurationViewModel()
        {
            try
            {
                // TODO: Implementar um modelo configurável padrão ou usar um modelo específico
                // Por enquanto, vamos criar um modelo configurável mock
                var mockConfigurableModel = new MockConfigurableModel();
                var viewModel = new ConfigurationViewModel(_historicalData, mockConfigurableModel);
                return viewModel;
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Erro ao criar ConfigurationViewModel: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Validation Methods
        /// <summary>
        /// Valida se os dados históricos são válidos para criação de ViewModels
        /// </summary>
        public bool ValidateHistoricalData()
        {
            if (_historicalData == null)
            {
                _notificationService.ShowError("Dados históricos não fornecidos");
                return false;
            }

            if (_historicalData.Count == 0)
            {
                _notificationService.ShowWarning("Dados históricos estão vazios");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtém informações sobre os dados históricos
        /// </summary>
        public string GetDataInfo()
        {
            if (_historicalData == null) return "Dados não carregados";

            // CORREÇÃO: Usar Lance.Id em vez de Lance.Numero
            var ultimoConcurso = _historicalData.Any() ? _historicalData.Max(l => l.Id) : 0;

            return $"Dados carregados: {_historicalData.Count} registros, " +
                   $"Último concurso: {ultimoConcurso}";
        }
        #endregion
    }

    /// <summary>
    /// Implementação mock de IConfigurableModel para testes
    /// </summary>
    public class MockConfigurableModel : IConfigurableModel
    {
        public Dictionary<string, object> CurrentParameters { get; private set; }
        public Dictionary<string, object> DefaultParameters { get; private set; }

        public MockConfigurableModel()
        {
            DefaultParameters = new Dictionary<string, object>
            {
                ["TestParameter"] = "DefaultValue",
                ["NumericParameter"] = 42,
                ["BooleanParameter"] = true,
                ["DoubleParameter"] = 3.14
            };
            CurrentParameters = new Dictionary<string, object>(DefaultParameters);
        }

        public object GetParameter(string name)
        {
            return CurrentParameters.TryGetValue(name, out var value) ? value : null;
        }

        public void SetParameter(string name, object value)
        {
            CurrentParameters[name] = value;
        }

        public void UpdateParameters(Dictionary<string, object> newParameters)
        {
            if (newParameters != null)
            {
                foreach (var param in newParameters)
                {
                    CurrentParameters[param.Key] = param.Value;
                }
            }
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            return parameters != null;
        }

        public string GetParameterDescription(string name)
        {
            return name switch
            {
                "TestParameter" => "Parâmetro de teste",
                "NumericParameter" => "Parâmetro numérico",
                "BooleanParameter" => "Parâmetro booleano",
                "DoubleParameter" => "Parâmetro decimal",
                _ => "Parâmetro desconhecido"
            };
        }

        public List<object> GetAllowedValues(string name)
        {
            return name switch
            {
                "TestParameter" => new List<object> { "Value1", "Value2", "Value3" },
                "NumericParameter" => new List<object> { 10, 20, 30, 40, 50 },
                "BooleanParameter" => new List<object> { true, false },
                "DoubleParameter" => new List<object> { 1.0, 2.0, 3.14, 4.0, 5.0 },
                _ => new List<object>()
            };
        }

        public void ResetToDefaults()
        {
            CurrentParameters = new Dictionary<string, object>(DefaultParameters);
        }
    }
}