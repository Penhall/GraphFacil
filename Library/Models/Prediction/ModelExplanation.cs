// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class ModelExplanation
    {
        public string Reasoning { get; set; }
        public List<DecisionFactor> Factors { get; set; } = new List<DecisionFactor>();
        public Dictionary<int, string> NumberReasons { get; set; } = new Dictionary<int, string>();
        public double ConfidenceLevel { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
