// D:\PROJETOS\GraphFacil\Library\Services\TestResult2.cs - Teste completo do sistema tri-modelo
using System.Collections.Generic;
using System;
using LotoLibrary.Models.Prediction;

namespace LotoLibrary.Services.Auxiliar;

public class TestResult
{
    public string TestName { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string Details { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    // Dados específicos para análise
    public Dictionary<string, PerformanceReport> PerformanceData { get; set; }
    public Dictionary<string, double> CorrelationData { get; set; }
    public object EnsembleData { get; set; }
    public object DiversificationData { get; set; }
    public Dictionary<string, int> StrategyData { get; set; }
    public object AnalysisData { get; set; }
}

