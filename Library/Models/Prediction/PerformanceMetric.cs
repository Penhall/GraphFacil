// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PerformanceMetric.cs - Modelos de dados
namespace LotoLibrary.Models.Prediction
{
    public class PerformanceMetric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public MetricType Type { get; set; }
        public bool IsGoodHigh { get; set; } // True se valor alto for bom
    }
}
