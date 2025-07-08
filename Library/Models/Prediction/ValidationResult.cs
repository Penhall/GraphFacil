// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class ValidationResult
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public List<int> CorrectPredictions { get; set; } = new List<int>();
        public List<int> IncorrectPredictions { get; set; } = new List<int>();
        public TimeSpan ValidationTime { get; set; }
        public int TotalTests { get; set; }
        public Dictionary<string, double> DetailedMetrics { get; set; } = new Dictionary<string, double>();
    }
}
