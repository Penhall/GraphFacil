// D:\PROJETOS\GraphFacil\Library\Interfaces\IPredictionModel.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Models.Validation;
using LotoLibrary.Enums;
using LotoLibrary.Models.Core;
using LotoLibrary.Models.Prediction;

namespace LotoLibrary.Interfaces
{
    public interface IPredictionModel : IDisposable
    {
        string ModelName { get; }
        string ModelVersion { get; }
        ModelType ModelType { get; }
        bool IsInitialized { get; }
        bool IsTrained { get; }
        double Confidence { get; }
        string Description { get; }

        Task<bool> InitializeAsync(Lances historicalData);
        Task<bool> TrainAsync(Lances historicalData);
        Task<PredictionResult> PredictAsync(int concurso);
        Task<ValidationResult> ValidateAsync(Lances testData);
        void Reset();
    }
}
