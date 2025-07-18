// D:\PROJETOS\GraphFacil\Library\Enums\AntiFrequencyEnums.cs - Enums específicos para modelos anti-frequencistas
using LotoLibrary.Services.Analysis;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models.Validation;
using System;

namespace LotoLibrary.Suporte
{
    public partial class ModelComparisonResult
    {
        public string Model1Name { get; set; }
        public string Model2Name { get; set; }
        public PerformanceMetrics Model1Metrics { get; set; }
        public PerformanceMetrics Model2Metrics { get; set; }
        public double Correlation { get; set; }
        public double DiversificationScore { get; set; }
        public double RecommendedWeight1 { get; set; }
        public double RecommendedWeight2 => 1.0 - RecommendedWeight1;
        public DateTime ComparisonTimestamp { get; set; }
        public ValidationResult ModelValidationResult { get; set; }
    }

}
