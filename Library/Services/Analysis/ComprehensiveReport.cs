// D:\PROJETOS\GraphFacil\Library\Services\Analysis\ComprehensiveReport.cs - Comparador de performance entre modelos
using System.Collections.Generic;
using System;

namespace LotoLibrary.Services.Analysis;

public class ComprehensiveReport
{
    public DateTime GenerationTime { get; set; }
    public List<string> ModelsAnalyzed { get; set; }
    public Dictionary<string, PerformanceMetrics> IndividualMetrics { get; set; }
    public CorrelationMatrix CorrelationMatrix { get; set; }
    public List<ModelPair> LowCorrelationPairs { get; set; }
    public List<string> RecommendedEnsemble { get; set; }
}

