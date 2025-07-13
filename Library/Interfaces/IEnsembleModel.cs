// D:\PROJETOS\GraphFacil\Library\Interfaces\IEnsembleModel.cs - Interface principal
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para ensemble de modelos
    /// </summary>
    public interface IEnsembleModel : IPredictionModel
    {
        List<IPredictionModel> SubModels { get; }
        Dictionary<string, double> ModelWeights { get; set; }

        void AddModel(IPredictionModel model, double weight = 1.0);
        bool RemoveModel(string modelName);
        void UpdateWeights(Dictionary<string, double> weights);
        EnsembleMetrics GetEnsembleMetrics();
        Task OptimizeWeightsAsync(Lances validationData);
    }
}


