using System;
using System.Collections.Generic;

namespace LotoLibrary.Utilities
{
    public class PredictionExplanation
    {
        public int Concurso { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string MainReason { get; set; } = string.Empty;
        public double ConfidenceLevel { get; set; }
        public Dictionary<string, double> ContributingFactors { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}
