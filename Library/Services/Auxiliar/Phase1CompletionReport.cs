// D:\PROJETOS\GraphFacil\Library\Services\Phase1CompletionReport.cs - Validação Final da Fase 1
using System.Linq;
using System;

namespace LotoLibrary.Services.Auxiliar;
#region Supporting Classes

public class Phase1CompletionReport
{
    public ValidationTest DataLoadingTest { get; set; }
    public ValidationTest PredictionEngineTest { get; set; }
    public ValidationTest MetronomoModelTest { get; set; }
    public ValidationTest IntegrationTest { get; set; }
    public ValidationTest BugFixValidation { get; set; }
    public ValidationTest PerformanceTest { get; set; }
    public ValidationTest Phase2ReadinessTest { get; set; }
    
    public TimeSpan TotalExecutionTime { get; set; }
    public bool OverallSuccess { get; set; }
    public Exception CriticalError { get; set; }

    public bool AllTestsPassed()
    {
        var tests = new[]
        {
            DataLoadingTest, PredictionEngineTest, MetronomoModelTest,
            IntegrationTest, BugFixValidation, PerformanceTest, Phase2ReadinessTest
        };

        return tests.All(test => test?.Success == true) && CriticalError == null;
    }
}

#endregion
