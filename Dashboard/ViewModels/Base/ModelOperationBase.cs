// D:\PROJETOS\GraphFacil\Dashboard\ViewModels\Base\ModelOperationBase.cs - Correção dos métodos virtuais
using LotoLibrary.Engines;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using System;
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
                await _predictionEngine.InitializeAsync(_historicalData);
                SetStatus("PredictionEngine inicializado");
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
            await model.TrainAsync(_historicalData);
            return model;
        }

        /// <summary>
        /// Valida se dados históricos são suficientes
        /// </summary>
        protected bool ValidateHistoricalData(int minimumRequired = 50)
        {
            if (_historicalData == null || _historicalData.Count < minimumRequired)
            {
                SetStatus($"Dados insuficientes. Mínimo: {minimumRequired}, Atual: {_historicalData?.Count ?? 0}");
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
