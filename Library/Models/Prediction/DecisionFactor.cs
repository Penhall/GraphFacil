// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
namespace LotoLibrary.Models.Prediction
{
    public class DecisionFactor
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
    }
}
