// D:\PROJETOS\GraphFacil\Library\Services\TestResult2.cs - Serviço de validação da Fase 1
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Resultado base para operações de teste
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Indica se o teste foi bem-sucedido
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem descritiva sobre o resultado
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Extensão do TestResult2 com detalhes adicionais e métricas
    /// </summary>
    public class TestResultXtras : TestResult
    {
        /// <summary>
        /// Detalhes adicionais sobre o resultado do teste
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Métricas e valores numéricos relacionados ao teste
        /// </summary>
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Relatório de validação contendo múltiplos resultados de teste
    /// </summary>
    public class Phase1ValidationReport
    {
        public TestResult DataLoadingTest { get; set; }
        public TestResult DezenaBugFixTest { get; set; }
        public TestResult InterfaceImplementationTest { get; set; }
        public TestResult MetronomoModelTest { get; set; }
        public TestResult PredictionEngineTest { get; set; }
        public TestResult PerformanceTest { get; set; }

        public TimeSpan TotalExecutionTime { get; set; }
        public bool OverallSuccess { get; set; }
        public string FatalError { get; set; }

        public bool AllTestsPassed()
        {
            return DataLoadingTest?.Success == true &&
                   DezenaBugFixTest?.Success == true &&
                   InterfaceImplementationTest?.Success == true &&
                   MetronomoModelTest?.Success == true &&
                   PredictionEngineTest?.Success == true &&
                   PerformanceTest?.Success == true &&
                   string.IsNullOrEmpty(FatalError);
        }

        public string GenerateReport()
        {
            var report = "=== RELATÓRIO DE VALIDAÇÃO - FASE 1 ===\n\n";
            report += $"Tempo Total de Execução: {TotalExecutionTime.TotalSeconds:F2}s\n";
            report += $"Status Geral: {(OverallSuccess ? "✅ PASSOU" : "❌ FALHOU")}\n\n";

            if (!string.IsNullOrEmpty(FatalError))
            {
                report += $"❌ ERRO FATAL: {FatalError}\n\n";
            }

            report += "RESULTADOS DOS TESTES:\n\n";
            report += FormatTestResult("1. Carregamento de Dados", DataLoadingTest);
            report += FormatTestResult("2. Correção Bug Dezenas 1-9", DezenaBugFixTest);
            report += FormatTestResult("3. Implementação de Interfaces", InterfaceImplementationTest);
            report += FormatTestResult("4. MetronomoModel Refatorado", MetronomoModelTest);
            report += FormatTestResult("5. PredictionEngine", PredictionEngineTest);
            report += FormatTestResult("6. Performance Geral", PerformanceTest);

            return report + "\n=== FIM DO RELATÓRIO ===";
        }

        private string FormatTestResult(string testName, TestResult result)
        {
            if (result == null) return $"{testName}: ❌ NÃO EXECUTADO\n\n";

            var status = result.Success ? "✅ PASSOU" : "❌ FALHOU";
            var text = $"{testName}: {status}\n  Mensagem: {result.Message}\n";

            if (result is TestResultXtras extras && !string.IsNullOrEmpty(extras.Details))
            {
                text += $"  Detalhes: {extras.Details}\n";
            }

            if (result is TestResultXtras extrasWithMetrics && extrasWithMetrics.Metrics?.Count > 0)
            {
                text += "  Métricas:\n";
                foreach (var metric in extrasWithMetrics.Metrics)
                {
                    text += $"    • {metric.Key}: {metric.Value}\n";
                }
            }

            return text + "\n";
        }
    }

}
