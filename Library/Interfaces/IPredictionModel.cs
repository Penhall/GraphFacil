// D:\PROJETOS\GraphFacil\Library\Interfaces\IPredictionModel.cs - Interface principal
using System.Threading.Tasks;
using System;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface base para todos os modelos de predição
    /// </summary>
    public interface IPredictionModel : IDisposable
    {
        #region Properties
        string ModelName { get; }
        string ModelType { get; }
        bool IsInitialized { get; }
        double Confidence { get; }
        DateTime LastTrainingTime { get; }
        int TrainingDataSize { get; }
        #endregion

        #region Core Methods
        Task<bool> InitializeAsync(Lances historicalData);
        Task<bool> TrainAsync(Lances trainingData);
        Task<PredictionResult> PredictAsync(int targetConcurso);
        Task<ValidationResult> ValidateAsync(Lances validationData);
        void Reset();
        #endregion

        #region Events
        event EventHandler<string> OnStatusChanged;
        event EventHandler<double> OnConfidenceChanged;
        #endregion
    }
}
