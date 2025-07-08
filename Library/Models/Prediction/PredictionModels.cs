// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class PredictionResult
    {
        public List<int> PredictedNumbers { get; set; } = new List<int>();
        public Dictionary<int, double> NumberProbabilities { get; set; } = new Dictionary<int, double>();
        public double OverallConfidence { get; set; }
        public string ModelUsed { get; set; }
        public DateTime PredictionTime { get; set; }
        public int TargetConcurso { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        // Métricas adicionais
        public TimeSpan ProcessingTime { get; set; }
        public string Version { get; set; } = "1.0";
    }

    public enum PredictionModels
    {
        Statistical,
        Temporal,
        Pattern,
        Frequency,
        Technical,
        Meta
    }
}
