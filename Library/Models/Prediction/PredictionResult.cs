using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Prediction;

public class PredictionResult
{
    // Propriedades existentes
    public int Concurso { get; set; }
    public List<int> Dezenas { get; set; } = new();
    public List<int> PredictedNumbers { get => Dezenas; set => Dezenas = value; }
    public double Confidence { get; set; }
    public double OverallConfidence { get => Confidence; set => Confidence = value; }
    public string ModelName { get; set; } = string.Empty;
    public string ModelUsed { get => ModelName; set => ModelName = value; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Strategy { get; set; } = string.Empty;
    public string GenerationMethod { get => Strategy; set => Strategy = value; }
    public int TargetConcurso { get => Concurso; set => Concurso = value; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public Dictionary<string, object> ModelParameters { 
        get => Metadata;
        set => Metadata = value;
    }
    public DateTime PredictionTime { get; set; }
    public TimeSpan ProcessingTime { get; set; }

    // Novas propriedades
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    
    public PredictionResult()
    {
        Dezenas = new List<int>();
        Metadata = new Dictionary<string, object>();
    }

    public PredictionResult(int concurso, List<int> dezenas, double confidence, string modelName)
    {
        Concurso = concurso;
        Dezenas = dezenas ?? new List<int>();
        Confidence = confidence;
        ModelName = modelName;
        GeneratedAt = DateTime.Now;
        Timestamp = DateTime.Now;
        Metadata = new Dictionary<string, object>();
        Success = true; // Default para sucesso
    }
}

public static class PredictionResultExtensions
{
    public static string FormatPredictionNumbers(this PredictionResult result)
    {
        if (result?.Dezenas == null || !result.Dezenas.Any())
            return string.Empty;

        return string.Join(", ", result.Dezenas.OrderBy(d => d).Select(d => d.ToString("00")));
    }
}
