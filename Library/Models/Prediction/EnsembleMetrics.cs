// D:\PROJETOS\GraphFacil\Library\Models\Prediction\EnsembleMetrics.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class EnsembleMetrics
    {
        public Dictionary<string, double> ModelContributions { get; set; } = new Dictionary<string, double>();
        public double DiversityScore { get; set; }
        public double WeightOptimality { get; set; }
        public double OverallStability { get; set; }
        public DateTime LastOptimization { get; set; }
    }
}
