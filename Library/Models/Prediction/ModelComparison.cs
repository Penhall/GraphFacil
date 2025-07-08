// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelComparison.cs - Modelos de dados
using System;

namespace LotoLibrary.Models.Prediction
{
    public class ModelComparison
    {
        public ModelComparison()
        {
        }

        public string ModelName { get; set; }
        public double Accuracy { get; set; }
        public double Confidence { get; set; }
        public TimeSpan AverageProcessingTime { get; set; }
        public PerformanceGrade Grade { get; set; }
        public int Rank { get; set; }
    }
}
