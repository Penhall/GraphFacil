// E:\PROJETOS\GraphFacil\Library\Services\Auxiliar\Phase1CompletionReport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models.Validation;

namespace LotoLibrary.Services.Auxiliar
{
    /// <summary>
    /// Relatório de conclusão da Fase 1 do projeto
    /// </summary>
    public class Phase1CompletionReport
    {
        #region Properties
        public string ReportTitle { get; set; } = "Relatório de Conclusão - Fase 1";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public List<TestResult> TestResults { get; set; } = new List<TestResult>();
        public List<TestResult> ComponentTests { get; set; } = new List<TestResult>();
        public List<TestResult> IntegrationTests { get; set; } = new List<TestResult>();
        public List<TestResult> PerformanceTests { get; set; } = new List<TestResult>();
        public List<TestResult> EngineTests { get; set; } = new List<TestResult>();
        public List<TestResult> ModelTests { get; set; } = new List<TestResult>();
        public List<TestResult> SystemTests { get; set; } = new List<TestResult>();
        #endregion

        #region Computed Properties
        public int TotalTests => TestResults.Count + ComponentTests.Count + IntegrationTests.Count + 
                                PerformanceTests.Count + EngineTests.Count + ModelTests.Count + SystemTests.Count;

        public int PassedTests => GetAllTests().Count(t => t.Success);

        public int FailedTests => TotalTests - PassedTests;

        public double SuccessRate => TotalTests > 0 ? (double)PassedTests / TotalTests : 0.0;

        public bool Phase1Complete => SuccessRate >= 0.85; // 85% de sucesso mínimo

        public string Status => Phase1Complete ? "COMPLETA" : "INCOMPLETA";
        #endregion

        #region Public Methods

        /// <summary>
        /// Adiciona teste de validação
        /// </summary>
        public void AddTestResult(TestResult test)
        {
            TestResults.Add(test);
        }

        /// <summary>
        /// Adiciona teste de componente
        /// </summary>
        public void AddComponentTest(TestResult test)
        {
            ComponentTests.Add(test);
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
        /// Adiciona teste de engine
        /// </summary>
        public void AddEngineTest(TestResult test)
        {
            EngineTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste de modelo
        /// </summary>
        public void AddModelTest(TestResult test)
        {
            ModelTests.Add(test);
        }

        /// <summary>
        /// Adiciona teste de sistema
        /// </summary>
        public void AddSystemTest(TestResult test)
        {
            SystemTests.Add(test);
        }

        /// <summary>
        /// Obtém todos os testes
        /// </summary>
        public List<TestResult> GetAllTests()
        {
            var allTests = new List<TestResult>();
            allTests.AddRange(TestResults);
            allTests.AddRange(ComponentTests);
            allTests.AddRange(IntegrationTests);
            allTests.AddRange(PerformanceTests);
            allTests.AddRange(EngineTests);
            allTests.AddRange(ModelTests);
            allTests.AddRange(SystemTests);
            return allTests;
        }

        /// <summary>
        /// Obtém resumo por categoria
        /// </summary>
        public Dictionary<string, TestCategorySummary> GetCategorySummary()
        {
            return new Dictionary<string, TestCategorySummary>
            {
                ["Validation"] = CreateCategorySummary(TestResults, "Testes de Validação"),
                ["Component"] = CreateCategorySummary(ComponentTests, "Testes de Componente"),
                ["Integration"] = CreateCategorySummary(IntegrationTests, "Testes de Integração"),
                ["Performance"] = CreateCategorySummary(PerformanceTests, "Testes de Performance"),
                ["Engine"] = CreateCategorySummary(EngineTests, "Testes de Engine"),
                ["Model"] = CreateCategorySummary(ModelTests, "Testes de Modelo"),
                ["System"] = CreateCategorySummary(SystemTests, "Testes de Sistema")
            };
        }

        /// <summary>
        /// Gera relatório executivo
        /// </summary>
        public string GenerateExecutiveSummary()
        {
            var summary = $"""
                🎯 RELATÓRIO EXECUTIVO - FASE 1
                ================================================
                
                📊 RESUMO GERAL:
                • Status da Fase 1: {Status}
                • Taxa de Sucesso: {SuccessRate:P1}
                • Testes Executados: {TotalTests}
                • Testes Aprovados: {PassedTests}
                • Testes Reprovados: {FailedTests}
                • Data: {GeneratedAt:dd/MM/yyyy HH:mm}
                
                📋 RESUMO POR CATEGORIA:
                """;

            var categories = GetCategorySummary();
            foreach (var category in categories)
            {
                var cat = category.Value;
                var status = cat.SuccessRate >= 0.8 ? "✅" : "❌";
                summary += $"\n• {status} {cat.Name}: {cat.PassedTests}/{cat.TotalTests} ({cat.SuccessRate:P1})";
            }

            summary += $"""
                
                
                🎯 PRÓXIMOS PASSOS:
                {GetNextStepsRecommendation()}
                
                ================================================
                Relatório gerado automaticamente
                """;

            return summary;
        }

        /// <summary>
        /// Gera relatório detalhado
        /// </summary>
        public string GenerateDetailedReport()
        {
            var report = GenerateExecutiveSummary();
            
            report += "\n\n📝 DETALHES DOS TESTES:\n";
            report += "================================================\n";

            var categories = GetCategorySummary();
            foreach (var category in categories)
            {
                if (category.Value.TotalTests > 0)
                {
                    report += $"\n🔍 {category.Value.Name.ToUpper()}\n";
                    report += new string('-', 40) + "\n";
                    
                    var tests = GetTestsByCategory(category.Key);
                    foreach (var test in tests)
                    {
                        var status = test.Success ? "✅ PASS" : "❌ FAIL";
                        report += $"{status} | {test.TestName}\n";
                        if (!test.Success && !string.IsNullOrEmpty(test.Message))
                        {
                            report += $"      Erro: {test.Message}\n";
                        }
                        if (!string.IsNullOrEmpty(test.Details))
                        {
                            report += $"      Detalhes: {test.Details}\n";
                        }
                    }
                }
            }

            return report;
        }

        #endregion

        #region Private Methods

        private TestCategorySummary CreateCategorySummary(List<TestResult> tests, string categoryName)
        {
            return new TestCategorySummary
            {
                Name = categoryName,
                TotalTests = tests.Count,
                PassedTests = tests.Count(t => t.Success),
                FailedTests = tests.Count(t => !t.Success),
                SuccessRate = tests.Count > 0 ? (double)tests.Count(t => t.Success) / tests.Count : 0.0
            };
        }

        private List<TestResult> GetTestsByCategory(string category)
        {
            return category.ToLower() switch
            {
                "validation" => TestResults,
                "component" => ComponentTests,
                "integration" => IntegrationTests,
                "performance" => PerformanceTests,
                "engine" => EngineTests,
                "model" => ModelTests,
                "system" => SystemTests,
                _ => new List<TestResult>()
            };
        }

        private string GetNextStepsRecommendation()
        {
            if (Phase1Complete)
            {
                return """
                    ✅ Fase 1 CONCLUÍDA com sucesso!
                    → Proceder para Fase 2: Ensemble Methods
                    → Implementar BasicEnsembleModel
                    → Configurar sistema de pesos adaptativos
                    """;
            }
            else
            {
                var failedCategories = GetCategorySummary()
                    .Where(c => c.Value.SuccessRate < 0.8 && c.Value.TotalTests > 0)
                    .Select(c => c.Value.Name)
                    .ToList();

                return $"""
                    ❌ Fase 1 INCOMPLETA - Ação necessária:
                    → Corrigir falhas em: {string.Join(", ", failedCategories)}
                    → Re-executar testes após correções
                    → Atingir mínimo de 85% de aprovação
                    """;
            }
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Resumo de uma categoria de testes
    /// </summary>
    public class TestCategorySummary
    {
        public string Name { get; set; }
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
        public double SuccessRate { get; set; }
    }

    #endregion
}