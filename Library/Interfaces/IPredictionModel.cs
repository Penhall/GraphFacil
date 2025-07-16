// D:\PROJETOS\GraphFacil\Library\Interfaces\IPredictionModel.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Models.Validation;
using LotoLibrary.Enums;
using LotoLibrary.Models.Core;

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
        Task<List<int>> PredictAsync(int concurso);
        Task<ValidationResult> ValidateAsync(Lances testData);
    }

    public interface IConfigurableModel
    {
        Dictionary<string, object> Parameters { get; }
        object GetParameter(string name);
        void SetParameter(string name, object value);
        void UpdateParameters(Dictionary<string, object> newParameters);
        bool ValidateParameters(Dictionary<string, object> parameters);
        string GetParameterDescription(string name);
        List<object> GetAllowedValues(string name);
        void ResetToDefaults();
    }
}
