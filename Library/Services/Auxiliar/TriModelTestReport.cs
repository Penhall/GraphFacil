// D:\PROJETOS\GraphFacil\Library\Services\TriModelTestReport.cs - Teste completo do sistema tri-modelo
using System.Linq;
using System;

namespace LotoLibrary.Services.Auxiliar;

public partial class TriModelTestReport
{
    public TestResult ModelInitializationTest { get; set; }
    public TestResult IndividualPerformanceTest { get; set; }
    public TestResult CrossCorrelationTest { get; set; }
    public TestResult AdvancedEnsembleTest { get; set; }
    public TestResult DiversificationTest { get; set; }
    public TestResult DynamicStrategyTest { get; set; }
    public TestResult ComparativeAnalysisTest { get; set; }

    public TimeSpan TotalExecutionTime { get; set; }
    public bool OverallSuccess { get; set; }
    public Exception CriticalError { get; set; }

    public bool AllTestsPassed()
    {
        var tests = new[]
        {
            ModelInitializationTest, IndividualPerformanceTest, CrossCorrelationTest,
            AdvancedEnsembleTest, DiversificationTest, DynamicStrategyTest, ComparativeAnalysisTest
        };

        return tests.All(test => test?.Success == true) && CriticalError == null;
    }
}

