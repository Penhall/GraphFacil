// D:\PROJETOS\GraphFacil\Library\Models\ModelMetrics.cs
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models
{
    public class ModelMetrics
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> CustomMetrics { get; set; } = new();
        
        public void AddCustomMetric(string name, double value)
        {
            CustomMetrics[name] = value;
        }
        
        public double GetCustomMetric(string name, double defaultValue = 0.0)
        {
            return CustomMetrics.GetValueOrDefault(name, defaultValue);
        }
    }
}
