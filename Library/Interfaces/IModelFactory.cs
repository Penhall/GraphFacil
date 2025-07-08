// D:\PROJETOS\GraphFacil\Library\Interfaces\IModelFactory.cs - Interface principal
using LotoLibrary.Models.Prediction;
using System.Collections.Generic;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para factory de modelos
    /// </summary>
    public interface IModelFactory
    {
        IPredictionModel CreateModel(ModelType type, Dictionary<string, object> parameters = null);
        IEnsembleModel CreateEnsemble(List<ModelType> modelTypes, Dictionary<string, double> weights = null);
        List<ModelType> GetAvailableModelTypes();
        ModelInfo GetModelInfo(ModelType type);
    }
}


