// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ComparisonReport.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class ComparisonReport
    {
        public DateTime ComparisonTime { get; set; }
        public List<ModelComparison> ModelComparisons { get; set; } = new List<ModelComparison>();
        public string BestModelName { get; set; }
        public string RecommendedStrategy { get; set; }
        public Dictionary<string, object> Statistics { get; set; } = new Dictionary<string, object>();
    }
}
