// Dashboard/ViewModels/Base/ModelOperationBase.cs
using Dashboard.ViewModels.Services;
using LotoLibrary.Engines;
using LotoLibrary.Enums;
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Core;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.ViewModels.Base
{
    /// <summary>
    /// Classe base para ViewModels que trabalham com modelos de predição
    /// Centraliza operações comuns como inicialização de engine, factory, etc.
    /// </summary>
    public abstract partial class ModelOperationBase : ViewModelBase
    {
        #region Protected Fields
        protected readonly Lances _historicalData;
        protected readonly PredictionEngine _predictionEngine;
        protected readonly ModelFactory _modelFactory;
        #endregion

        #region Constructor
        protected ModelOperationBase(Lances historicalData)
        {
            _historicalData = historicalData ?? throw new ArgumentNullException(nameof(historicalData));
            _predictionEngine = new PredictionEngine();
            _modelFactory = new ModelFactory();
        }
        #endregion

        #region Initialization
        public override async Task InitializeAsync()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                await InitializePredictionEngineAsync();
                await InitializeSpecificAsync();

            }, "Inicializando sistema de predição...");
        }

        /// <summary>
        /// Inicializa o PredictionEngine
        /// </summary>
        protected virtual async Task InitializePredictionEngineAsync()
        {
            if (!_predictionEngine.IsInitialized)
            {
                var success = await _predictionEngine.InitializeAsync(_historicalData);
                if (success)
                {
                    SetStatus("PredictionEngine inicializado com sucesso");
                }
                else
                {
                    SetStatus("Erro ao inicializar PredictionEngine", true);
                }
            }
        }

        /// <summary>
        /// Inicialização específica do ViewModel (override se necessário)
        /// </summary>
        protected virtual async Task InitializeSpecificAsync()
        {
            await Task.CompletedTask;
        }
        #endregion

        #region Common Model Operations
        /// <summary>
        /// Cria e treina um modelo
        /// </summary>
        protected async Task<IPredictionModel> CreateAndTrainModelAsync(ModelType modelType)
        {
            var model = _modelFactory.CreateModel(modelType);

            if (!model.IsInitialized)
            {
                await model.InitializeAsync(_historicalData);
            }

            await model.TrainAsync(_historicalData);
            return model;
        }

        /// <summary>
        /// Obtém todos os tipos de modelos disponíveis
        /// </summary>
        protected List<ModelType> GetAvailableModelTypes()
        {
            try
            {
                return _modelFactory.GetAvailableModelTypes();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new List<ModelType>();
            }
        }

        /// <summary>
        /// Obtém informações sobre um modelo específico
        /// </summary>
        protected ModelInfo GetModelInfo(ModelType modelType)
        {
            try
            {
                return _modelFactory.GetModelInfo(modelType);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Cria instância de um modelo sem treinar
        /// </summary>
        protected IPredictionModel CreateModel(ModelType modelType, Dictionary<string, object> parameters = null)
        {
            try
            {
                return _modelFactory.CreateModel(modelType, parameters);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Valida se dados históricos são suficientes
        /// </summary>
        protected bool ValidateHistoricalData(int minimumRequired = 50)
        {
            if (_historicalData == null || _historicalData.Count < minimumRequired)
            {
                SetStatus($"Dados insuficientes. Mínimo: {minimumRequired}, Atual: {_historicalData?.Count ?? 0}", true);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Obtém informações sobre um tipo de modelo
        /// </summary>
        protected ModelInfo GetModelInformation(ModelType modelType)
        {
            return _modelFactory.GetModelInfo(modelType);
        }

        /// <summary>
        /// Verifica se um modelo está disponível
        /// </summary>
        protected bool IsModelAvailable(ModelType modelType)
        {
            try
            {
                var availableTypes = _modelFactory.GetAvailableModelTypes();
                return availableTypes.Contains(modelType);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Registra um modelo no PredictionEngine
        /// </summary>
        protected async Task<bool> RegisterModelInEngineAsync(string name, IPredictionModel model)
        {
            try
            {
                if (!_predictionEngine.IsInitialized)
                {
                    await _predictionEngine.InitializeAsync(_historicalData);
                }

                return await _predictionEngine.RegisterModelAsync(name, model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Obtém modelos registrados no engine
        /// </summary>
        protected Dictionary<string, IPredictionModel> GetRegisteredModels()
        {
            try
            {
                return _predictionEngine.Models.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return new Dictionary<string, IPredictionModel>();
            }
        }

        /// <summary>
        /// Cria múltiplos modelos de uma vez
        /// </summary>
        protected async Task<List<IPredictionModel>> CreateMultipleModelsAsync(List<ModelType> modelTypes)
        {
            var models = new List<IPredictionModel>();

            foreach (var modelType in modelTypes)
            {
                try
                {
                    var model = CreateModel(modelType);
                    if (model != null)
                    {
                        if (!model.IsInitialized)
                        {
                            await model.InitializeAsync(_historicalData);
                        }
                        models.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    SetStatus($"Erro ao criar modelo {modelType}: {ex.Message}", true);
                }
            }

            return models;
        }
        #endregion

        #region Common Methods
        /// <summary>
        /// Exibe mensagem de sucesso
        /// CORREÇÃO: Usar método síncrono ShowSuccess em vez de ShowSuccessAsync
        /// </summary>
        protected virtual async Task ShowSuccessMessageAsync(string message)
        {
            SetStatus(message);
            UINotificationService.Instance.ShowSuccess(message); // CORREÇÃO: Método síncrono
            await Task.CompletedTask;
        }

        /// <summary>
        /// Exibe mensagem de erro
        /// CORREÇÃO: Usar método síncrono ShowError em vez de ShowErrorAsync
        /// </summary>
        protected virtual async Task ShowErrorMessageAsync(string message)
        {
            SetStatus(message, true);
            UINotificationService.Instance.ShowError(message); // CORREÇÃO: Método síncrono
            await Task.CompletedTask;
        }

        /// <summary>
        /// Verifica se operações podem ser executadas
        /// </summary>
        protected virtual bool CanExecute()
        {
            return _historicalData != null && _historicalData.Count > 0 && !IsLoading;
        }

        /// <summary>
        /// Obtém o próximo concurso disponível
        /// </summary>
        protected virtual int GetNextConcurso()
        {
            return _historicalData?.Max(l => l.Id) + 1 ?? 1;
        }

        /// <summary>
        /// Registra erro no log
        /// </summary>
        protected virtual void LogError(Exception ex)
        {
            Console.WriteLine($"ERRO: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"ERRO: {ex}");
        }

        /// <summary>
        /// Obtém estatísticas dos dados históricos
        /// </summary>
        protected string GetDataStatistics()
        {
            if (_historicalData == null || !_historicalData.Any())
                return "Nenhum dado disponível";

            var firstConcurso = _historicalData.Min(l => l.Id);
            var lastConcurso = _historicalData.Max(l => l.Id);
            var totalConcursos = _historicalData.Count;

            return $"Dados: {totalConcursos} concursos ({firstConcurso} a {lastConcurso})";
        }

        /// <summary>
        /// Força atualização de status do PredictionEngine
        /// </summary>
        protected void RefreshEngineStatus()
        {
            if (_predictionEngine != null)
            {
                var status = _predictionEngine.IsInitialized ? "✅ Inicializado" : "⏳ Aguardando";
                var modelCount = _predictionEngine.Models?.Count ?? 0;
                SetStatus($"Engine: {status} | Modelos: {modelCount}");
            }
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Cleanup virtual que pode ser sobrescrito
        /// </summary>
        public virtual async Task Cleanup()
        {
            SetStatus("Limpeza concluída");
            await Task.CompletedTask;
        }
        #endregion
    }
}