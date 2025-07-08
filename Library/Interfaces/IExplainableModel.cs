// D:\PROJETOS\GraphFacil\Library\Interfaces\IExplainableModel.cs - Interface principal
using LotoLibrary.Models.Prediction;
using System.Collections.Generic;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para modelos que explicam suas predições
    /// </summary>
    public interface IExplainableModel : IPredictionModel
    {
        ModelExplanation ExplainPrediction(PredictionResult prediction);
        List<FeatureImportance> GetFeatureImportances();
        string GetModelDescription();
    }
}


