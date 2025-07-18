// E:\PROJETOS\GraphFacil\Library\Services\Auxiliar\TriModelTestReport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models.Validation;

namespace LotoLibrary.Services.Auxiliar
{
    /// <summary>
    /// Relatório de teste do sistema tri-modelo
    /// </summary>
    public class TriModelTestReport
    {
        #region Properties
        public string ReportTitle { get; set; } = "Relatório de Teste Tri-Modelo";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public List<TestResult> MetronomoTests { get; set; } = new List<TestResult>();
        public List<TestResult> AntiFrequencyTests { get; set; } = new List<TestResult>();
        public List<TestResult> NeuralNetworkTests { get; set; } = new List<TestResult>();
        public List<TestResult> EnsembleTests { get; set; } = new List<TestResult>();
        public List<TestResult> IntegrationTests { get; set; } = new List<TestResult>();
        public List<TestResult> PerformanceTests { get; set; } = new List<TestResult>();
        public List<TestResult> CrossTestResults { get; set; } = new List<TestResult>();
        #endregion

        #region Computed Properties
        public int TotalTests => MetronomoTests.Count + AntiFrequencyTests.Count + NeuralNetworkTests.Count + 
                                EnsembleTests.Count + IntegrationTests.Count + PerformanceTests.Count + CrossTestResults.Count;

        public int PassedTests => GetAllTests().Count(t => t.Success);

        public int FailedTests => TotalTests - PassedTests;

        public double OverallSuccessRate => TotalTests > 0 ? (double)PassedTests / TotalTests : 0.0;

        public bool TriModelSystemReady => OverallSuccessRate >= 0.80; // 80% de sucesso mínimo

        public string SystemStatus => TriModelSystemReady ? "PRONTO" : "NECESSITA AJUSTES";
        #endregion

        #region Public Methods

        /// <summary>
        /// Adiciona teste do modelo Metrônomo
        /// </summary>
        public void AddMetronomoTest(TestResult test)
        {
            MetronomoTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste do modelo AntiFrequency
        /// </summary>
        public void AddAntiFrequencyTest(TestResult test)
        {
            AntiFrequencyTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste do modelo Neural Network
        /// </summary>
        public void AddNeuralNetworkTest(TestResult test)
        {
            NeuralNetworkTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste do sistema Ensemble
        /// </summary>
        public void AddEnsembleTest(TestResult test)
        {
            EnsembleTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste de integração
        /// </summary>
        public void AddIntegrationTest(TestResult test)
        {
            IntegrationTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste de performance
        /// </summary>
        public void AddPerformanceTest(TestResult test)
        {
            PerformanceTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste de validação cruzada
        /// </summary>
        public void AddCrossTestResult(TestResult test)
        {
            CrossTestResults.Add(test);
        }

        /// <summary>
        /// Obtém todos os testes
        /// </summary>
        public List<TestResult> GetAllTests()
        {
            var allTests = new List<TestResult>();
            allTests.AddRange(MetronomoTests);
            allTests.AddRange(AntiFrequencyTests);
            allTests.AddRange(NeuralNetworkTests);
            allTests.AddRange(EnsembleTests);
            allTests.AddRange(IntegrationTests);
            allTests.AddRange(PerformanceTests);
            allTests.AddRange(CrossTestResults);
            return allTests;
        }

        /// <summary>
        /// Obtém performance por modelo
        /// </summary>
        public Dictionary<string, ModelPerformanceSummary> GetModelPerformance()
        {
            return new Dictionary<string, ModelPerformanceSummary>
            {
                ["Metronomo"] = CreateModelSummary(MetronomoTests, "Modelo Metrônomo"),
                ["AntiFrequency"] = CreateModelSummary(AntiFrequencyTests, "Modelo Anti-Frequência"),
                ["NeuralNetwork"] = CreateModelSummary(NeuralNetworkTests, "Rede Neural"),
                ["Ensemble"] = CreateModelSummary(EnsembleTests, "Sistema Ensemble"),
                ["Integration"] = CreateModelSummary(IntegrationTests, "Testes de Integração"),
                ["Performance"] = CreateModelSummary(PerformanceTests, "Testes de Performance"),
                ["CrossValidation"] = CreateModelSummary(CrossTestResults, "Validação Cruzada")
            };
        }

        /// <summary>
        /// Gera relatório executivo do tri-modelo
        /// </summary>
        public string GenerateTriModelSummary()
        {
            var summary = $"""
                🎯 RELATÓRIO TRI-MODELO
                ================================================
                
                📊 STATUS GERAL:
                • Status do Sistema: {SystemStatus}
                • Taxa de Sucesso Geral: {OverallSuccessRate:P1}
                • Testes Executados: {TotalTests}
                • Testes Aprovados: {PassedTests}
                • Testes Reprovados: {FailedTests}
                • Data: {GeneratedAt:dd/MM/yyyy HH:mm}
                
                🤖 PERFORMANCE DOS MODELOS:
                """;

            var modelPerformance = GetModelPerformance();
            foreach (var model in modelPerformance)
            {
                var perf = model.Value;
                if (perf.TotalTests > 0)
                {
                    var status = perf.SuccessRate >= 0.75 ? "✅" : "⚠️";
                    var avgScore = perf.AverageScore > 0 ? $" (Score: {perf.AverageScore:P1})" : "";
                    summary += $"\n• {status} {perf.ModelName}: {perf.PassedTests}/{perf.TotalTests} ({perf.SuccessRate:P1}){avgScore}";
                }
            }

            summary += $"""
                
                
                🔧 ANÁLISE TÉCNICA:
                {GenerateTechnicalAnalysis()}
                
                🎯 RECOMENDAÇÕES:
                {GenerateRecommendations()}
                
                ================================================
                """;

            return summary;
        }

        /// <summary>
        /// Gera relatório detalhado do tri-modelo
        /// </summary>
        public string GenerateDetailedTriModelReport()
        {
            var report = GenerateTriModelSummary();
            
            report += "\n\n📝 DETALHES POR MODELO:\n";
            report += "================================================\n";

            var modelPerformance = GetModelPerformance();
            foreach (var model in modelPerformance)
            {
                if (model.Value.TotalTests > 0)
                {
                    report += $"\n🔍 {model.Value.ModelName.ToUpper()}\n";
                    report += new string('-', 40) + "\n";
                    
                    var tests = GetTestsByModel(model.Key);
                    foreach (var test in tests)
                    {
                        var status = test.Success ? "✅ PASS" : "❌ FAIL";
                        var score = test.Score > 0 ? $" [{test.Score:P1}]" : "";
                        report += $"{status} | {test.TestName}{score}\n";
                        
                        if (!test.Success && !string.IsNullOrEmpty(test.Message))
                        {
                            report += $"      Erro: {test.Message}\n";
                        }
                        if (!string.IsNullOrEmpty(test.Details))
                        {
                            report += $"      Detalhes: {test.Details}\n";
                        }
                        if (test.ExecutionTime.TotalMilliseconds > 0)
                        {
                            report += $"      Tempo: {test.ExecutionTime.TotalMilliseconds:F0}ms\n";
                        }
                    }
                }
            }

            return report;
        }

        #endregion

        #region Private Methods

        private ModelPerformanceSummary CreateModelSummary(List<TestResult> tests, string modelName)
        {
            return new ModelPerformanceSummary
            {
                ModelName = modelName,
                TotalTests = tests.Count,
                PassedTests = tests.Count(t => t.Success),
                FailedTests = tests.Count(t => !t.Success),
                SuccessRate = tests.Count > 0 ? (double)tests.Count(t => t.Success) / tests.Count : 0.0,
                AverageScore = tests.Count > 0 ? tests.Average(t => t.Score) : 0.0,
                AverageExecutionTime = tests.Count > 0 ? TimeSpan.FromMilliseconds(tests.Average(t => t.ExecutionTime.TotalMilliseconds)) : TimeSpan.Zero
            };
        }

        private List<TestResult> GetTestsByModel(string model)
        {
            return model.ToLower() switch
            {
                "metronomo" => MetronomoTests,
                "antifrequency" => AntiFrequencyTests,
                "neuralnetwork" => NeuralNetworkTests,
                "ensemble" => EnsembleTests,
                "integration" => IntegrationTests,
                "performance" => PerformanceTests,
                "crossvalidation" => CrossTestResults,
                _ => new List<TestResult>()
            };
        }

        private string GenerateTechnicalAnalysis()
        {
            var modelPerf = GetModelPerformance();
            var bestModel = modelPerf.Where(m => m.Value.TotalTests > 0).OrderByDescending(m => m.Value.SuccessRate).FirstOrDefault();
            var worstModel = modelPerf.Where(m => m.Value.TotalTests > 0).OrderBy(m => m.Value.SuccessRate).FirstOrDefault();

            var analysis = "";
            
            if (bestModel.Value != null)
            {
                analysis += $"• Melhor Performance: {bestModel.Value.ModelName} ({bestModel.Value.SuccessRate:P1})\n";
            }
            
            if (worstModel.Value != null && worstModel.Key != bestModel.Key)
            {
                analysis += $"• Necessita Atenção: {worstModel.Value.ModelName} ({worstModel.Value.SuccessRate:P1})\n";
            }

            var avgExecutionTime = GetAllTests().Where(t => t.ExecutionTime.TotalMilliseconds > 0)
                                                 .Average(t => t.ExecutionTime.TotalMilliseconds);
            if (avgExecutionTime > 0)
            {
                analysis += $"• Tempo Médio de Execução: {avgExecutionTime:F0}ms\n";
            }

            return analysis.TrimEnd('\n');
        }

        private string GenerateRecommendations()
        {
            var recommendations = new List<string>();

            if (TriModelSystemReady)
            {
                recommendations.Add("✅ Sistema tri-modelo pronto para produção");
                recommendations.Add("→ Implementar monitoramento contínuo");
                recommendations.Add("→ Configurar alertas de performance");
            }
            else
            {
                recommendations.Add("⚠️ Sistema necessita otimizações antes da produção");
                
                var modelPerf = GetModelPerformance();
                foreach (var model in modelPerf.Where(m => m.Value.SuccessRate < 0.75 && m.Value.TotalTests > 0))
                {
                    recommendations.Add($"→ Otimizar {model.Value.ModelName}");
                }
            }

            return string.Join("\n", recommendations);
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Resumo de performance de um modelo
    /// </summary>
    public class ModelPerformanceSummary
    {
        public string ModelName { get; set; }
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public double SuccessRate { get; set; }
        public double AverageScore { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
    }

    #endregion
}