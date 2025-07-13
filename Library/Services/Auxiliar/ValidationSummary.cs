// D:\PROJETOS\GraphFacil\Library\Services\ValidationSummary.cs - Validação completa do sistema meta-learning
using System.Collections.Generic;
using System;

namespace LotoLibrary.Services.Auxiliar;

public class ValidationSummary
{
    public bool Success { get; set; }
    public double OverallScore { get; set; }
    public Dictionary<string, TestResult> TestResults { get; set; }
    public string TestLog { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public string ErrorMessage { get; set; }
    public MetaLearningMetrics MetaLearningMetrics { get; set; }
}

