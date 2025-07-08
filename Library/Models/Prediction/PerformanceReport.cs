// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PerformanceReport.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class PerformanceReport
    {
        public string ModelName { get; set; }
        public DateTime ReportTime { get; set; }
        public ValidationResult ValidationResults { get; set; }
        public List<PerformanceMetric> Metrics { get; set; } = new List<PerformanceMetric>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public PerformanceGrade Grade { get; set; }
    }
}
