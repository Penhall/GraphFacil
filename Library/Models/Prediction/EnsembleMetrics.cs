// D:\PROJETOS\GraphFacil\Library\Models\Prediction\EnsembleMetrics.cs
using System.Collections.Generic;
using System;

﻿// D:\PROJETOS\GraphFacil\Library\Models\Prediction\EnsembleMetrics.cs - Modelos de dados
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
