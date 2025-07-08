// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
namespace LotoLibrary.Models.Prediction
{
    public class FeatureImportance
    {
        public string FeatureName { get; set; }
        public double Importance { get; set; }
        public string Description { get; set; }
        public PredictionModels Type { get; set; }
    }
}
