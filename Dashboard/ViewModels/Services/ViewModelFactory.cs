// Dashboard/ViewModels/Services/ViewModelFactory.cs
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Models;
using System;
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
                var viewModel = new ConfigurationViewModel(_historicalData);
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
}