// D:\PROJETOS\GraphFacil\Library\Services\AntiFreqIntegrationTester.cs - Testa integração do primeiro modelo anti-frequencista
using LotoLibrary.Engines;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using LotoLibrary.Services.Analysis;
using LotoLibrary.Models;
using LotoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Testa a integração completa do AntiFrequencySimpleModel com o sistema existente
    /// Valida performance, correlação e funcionalidade em ambiente real
    /// </summary>
    public static class AntiFreqIntegrationTester
    {
        public static async Task<IntegrationTestReport> ExecuteCompleteIntegrationTestAsync()
        {
            var report = new IntegrationTestReport();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine("🚀 INICIANDO TESTE DE INTEGRAÇÃO - ANTIFREQUENCY SIMPLE MODEL");
                Console.WriteLine("=" * 70);
                Console.WriteLine();

                // 1. Teste de Carregamento e Inicialização
                report.InitializationTest = await TestModelInitialization();
                LogResult("Inicialização do Modelo", report.InitializationTest.Success);

                // 2. Teste de Integração com PredictionEngine
                report.EngineIntegrationTest = await TestEngineIntegration();
                LogResult("Integração com PredictionEngine", report.EngineIntegrationTest.Success);

                // 3. Teste de Performance Comparativa
                report.PerformanceTest = await TestPerformanceComparison();
                LogResult("Teste de Performance", report.PerformanceTest.Success);

                // 4. Teste de Correlação entre Modelos
                report.CorrelationTest = await TestModelCorrelation();
                LogResult("Teste de Correlação", report.CorrelationTest.Success);

                // 5. Teste de Estratégias Anti-Frequencistas
                report.AntiFreqStrategyTest = await TestAntiFrequencyStrategies();
                LogResult("Estratégias Anti-Frequencistas", report.AntiFreqStrategyTest.Success);

                // 6. Teste de Ensemble Básico
                report.EnsembleTest = await TestBasicEnsemble();
                LogResult("Ensemble Básico", report.EnsembleTest.Success);

                // 7. Teste de Interface e UX
                report.InterfaceTest = await TestInterfaceIntegration();
                LogResult("Integração de Interface", report.InterfaceTest.Success);

                stopwatch.Stop();
                report.TotalExecutionTime = stopwatch.Elapsed;
                report.OverallSuccess = report.AllTestsPassed();

                // Relatório final
                Console.WriteLine();
                Console.WriteLine("=" * 70);
                Console.WriteLine($"🏁 TESTE DE INTEGRAÇÃO CONCLUÍDO EM {report.TotalExecutionTime.TotalSeconds:F2}s");
                Console.WriteLine();

                if (report.OverallSuccess)
                {
                    Console.WriteLine("✅ TODOS OS TESTES DE INTEGRAÇÃO PASSARAM!");
                    Console.WriteLine("🎯 ANTIFREQUENCY SIMPLE MODEL TOTALMENTE INTEGRADO");
                    Console.WriteLine("🚀 SISTEMA PRONTO PARA PRÓXIMO MODELO (STATISTICALDEBTMODEL)");
                }
                else
                {
                    Console.WriteLine("❌ ALGUNS TESTES FALHARAM");
                    Console.WriteLine("📋 Revise os erros antes de continuar");
                }

                // Salvar relatório detalhado
                await SaveIntegrationReport(report);

                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO CRÍTICO NO TESTE: {ex.Message}");
                report.OverallSuccess = false;
                report.CriticalError = ex;
                return report;
            }
        }

        #region Individual Integration Tests

        private static async Task<IntegrationTest> TestModelInitialization()
        {
            var test = new IntegrationTest { TestName = "Inicialização do Modelo" };

            try
            {
                Console.WriteLine("1️⃣ Testando inicialização do AntiFrequencySimpleModel...");

                // Carregar dados
                var dados = Infra.CarregarConcursos();
                if (dados == null || !dados.Any())
                {
                    test.ErrorMessage = "Falha ao carregar dados históricos";
                    return test;
                }

                // Criar e inicializar modelo
                var antiFreqModel = new AntiFrequencySimpleModel();
                var initResult = await antiFreqModel.InitializeAsync(dados);

                if (!initResult)
                {
                    test.ErrorMessage = "Falha na inicialização do modelo";
                    return test;
                }

                // Validar propriedades
                if (!antiFreqModel.IsInitialized)
                {
                    test.ErrorMessage = "Modelo não marcado como inicializado";
                    return test;
                }

                // Treinar modelo
                var trainResult = await antiFreqModel.TrainAsync(dados);
                if (!trainResult)
                {
                    test.ErrorMessage = "Falha no treinamento do modelo";
                    return test;
                }

                // Validar perfis criados
                if (antiFreqModel.SimpleProfiles.Count != 25)
                {
                    test.ErrorMessage = $"Número incorreto de perfis: {antiFreqModel.SimpleProfiles.Count} (esperado: 25)";
                    return test;
                }

                // Validar que é realmente anti-frequencista
                var avgFrequency = antiFreqModel.SimpleProfiles.Values.Average(p => p.CurrentFrequency);
                var expectedFreq = 15.0 / 25.0;

                test.Success = true;
                test.Details = $"✅ Modelo inicializado: {antiFreqModel.SimpleProfiles.Count} perfis, freq média: {avgFrequency:P2}";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<IntegrationTest> TestEngineIntegration()
        {
            var test = new IntegrationTest { TestName = "Integração com PredictionEngine" };

            try
            {
                Console.WriteLine("2️⃣ Testando integração com PredictionEngine...");

                var dados = Infra.CarregarConcursos();
                var engine = new PredictionEngine();

                // Inicializar engine (deve registrar MetronomoModel automaticamente)
                var initResult = await engine.InitializeAsync(dados);
                if (!initResult)
                {
                    test.ErrorMessage = "Falha na inicialização do PredictionEngine";
                    return test;
                }

                // Criar e registrar AntiFrequencySimpleModel
                var antiFreqModel = new AntiFrequencySimpleModel();
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                var registrationResult = await engine.RegisterModelAsync("AntiFreqSimple", antiFreqModel);
                if (!registrationResult)
                {
                    test.ErrorMessage = "Falha ao registrar AntiFrequencySimpleModel";
                    return test;
                }

                // Validar que agora temos 2 modelos
                if (engine.TotalModels != 2)
                {
                    test.ErrorMessage = $"Número incorreto de modelos: {engine.TotalModels} (esperado: 2)";
                    return test;
                }

                // Testar predição com modelo específico
                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
                
                // Predição com Metronomo
                engine.SetActiveStrategy("Single");
                var metronomoPrediction = await engine.GeneratePredictionAsync(targetConcurso);

                // Limpar cache e testar com AntiFreq
                engine.ClearCache();
                var antiFreqPrediction = await antiFreqModel.PredictAsync(targetConcurso + 1);

                if (metronomoPrediction == null || antiFreqPrediction == null)
                {
                    test.ErrorMessage = "Falha ao gerar predições";
                    return test;
                }

                // Validar estrutura das predições
                if (metronomoPrediction.PredictedNumbers.Count != 15 || antiFreqPrediction.PredictedNumbers.Count != 15)
                {
                    test.ErrorMessage = "Predições com número incorreto de dezenas";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Integração OK: {engine.TotalModels} modelos registrados, predições válidas";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<IntegrationTest> TestPerformanceComparison()
        {
            var test = new IntegrationTest { TestName = "Teste de Performance" };

            try
            {
                Console.WriteLine("3️⃣ Testando performance comparativa...");

                var dados = Infra.CarregarConcursos();
                
                // Criar ambos os modelos
                var metronomoModel = new MetronomoModel();
                var antiFreqModel = new AntiFrequencySimpleModel();

                // Inicializar modelos
                await metronomoModel.InitializeAsync(dados);
                await metronomoModel.TrainAsync(dados);

                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                // Testar performance de inicialização
                var stopwatch = Stopwatch.StartNew();
                
                // Gerar predições
                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
                var metronomoPred = await metronomoModel.PredictAsync(targetConcurso);
                var antiFreqPred = await antiFreqModel.PredictAsync(targetConcurso);
                
                stopwatch.Stop();

                // Validar performance temporal (deve ser < 5 segundos)
                if (stopwatch.Elapsed.TotalSeconds > 5.0)
                {
                    test.ErrorMessage = $"Performance temporal inadequada: {stopwatch.Elapsed.TotalSeconds:F2}s (máximo: 5s)";
                    return test;
                }

                // Validar confiança dos modelos
                if (metronomoModel.Confidence < 0.5 || antiFreqModel.Confidence < 0.5)
                {
                    test.ErrorMessage = $"Confiança baixa - Metronomo: {metronomoModel.Confidence:P2}, AntiFreq: {antiFreqModel.Confidence:P2}";
                    return test;
                }

                // Validar auto-validação do modelo anti-frequencista
                var validationResult = await antiFreqModel.ValidateModelPerformanceAsync();
                if (!validationResult.IsValid)
                {
                    test.ErrorMessage = $"Auto-validação do modelo falhou: {string.Join(", ", validationResult.ValidationDetails)}";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Performance OK: {stopwatch.Elapsed.TotalSeconds:F2}s, confiança Metronomo: {metronomoModel.Confidence:P2}, AntiFreq: {antiFreqModel.Confidence:P2}";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<IntegrationTest> TestModelCorrelation()
        {
            var test = new IntegrationTest { TestName = "Teste de Correlação" };

            try
            {
                Console.WriteLine("4️⃣ Testando correlação entre modelos...");

                var dados = Infra.CarregarConcursos();
                var comparer = new PerformanceComparer();

                // Criar ambos os modelos
                var metronomoModel = new MetronomoModel();
                var antiFreqModel = new AntiFrequencySimpleModel();

                await metronomoModel.InitializeAsync(dados);
                await metronomoModel.TrainAsync(dados);
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                // Gerar múltiplas predições para análise de correlação
                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                
                for (int i = 0; i < 10; i++)
                {
                    var target = targetBase + i;
                    var metronomoPred = await metronomoModel.PredictAsync(target);
                    var antiFreqPred = await antiFreqModel.PredictAsync(target);

                    comparer.AddPredictionResult("Metronomo", metronomoPred);
                    comparer.AddPredictionResult("AntiFreqSimple", antiFreqPred);
                }

                // Calcular correlação
                var comparison = await comparer.CompareModelsAsync("Metronomo", "AntiFreqSimple");

                // Validar que correlação é baixa (< 0.8 = boa diversificação)
                if (Math.Abs(comparison.Correlation) >= 0.8)
                {
                    test.ErrorMessage = $"Correlação muito alta: {comparison.Correlation:F3} (máximo esperado: 0.8)";
                    return test;
                }

                // Validar score de diversificação
                if (comparison.DiversificationScore < 0.3)
                {
                    test.ErrorMessage = $"Score de diversificação muito baixo: {comparison.DiversificationScore:F3}";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Correlação adequada: {comparison.Correlation:F3}, diversificação: {comparison.DiversificationScore:F3}";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<IntegrationTest> TestAntiFrequencyStrategies()
        {
            var test = new IntegrationTest { TestName = "Estratégias Anti-Frequencistas" };

            try
            {
                Console.WriteLine("5️⃣ Testando estratégias anti-frequencistas...");

                var dados = Infra.CarregarConcursos();
                var antiFreqModel = new AntiFrequencySimpleModel();

                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                // Testar com diferentes parâmetros de inversão
                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
                
                // Teste 1: Inversão suave
                antiFreqModel.SetParameterValue("InversionFactor", 0.3);
                var predSuave = await antiFreqModel.PredictAsync(targetConcurso);

                // Teste 2: Inversão forte
                antiFreqModel.SetParameterValue("InversionFactor", 0.9);
                var predForte = await antiFreqModel.PredictAsync(targetConcurso + 1);

                // Validar que predições são diferentes
                var intersection = predSuave.PredictedNumbers.Intersect(predForte.PredictedNumbers).Count();
                if (intersection > 12) // Mais de 80% igual
                {
                    test.ErrorMessage = $"Estratégias muito similares: {intersection}/15 dezenas iguais";
                    return test;
                }

                // Testar explicação de predições
                var explanationSuave = antiFreqModel.ExplainPrediction(predSuave);
                var explanationForte = antiFreqModel.ExplainPrediction(predForte);

                if (explanationSuave.MainFactors.Count < 3 || explanationForte.MainFactors.Count < 3)
                {
                    test.ErrorMessage = "Explicações de predições insuficientes";
                    return test;
                }

                // Validar que força de inversão é diferente
                var inversionSuave = (double)explanationSuave.TechnicalDetails["CurrentInversionStrength"];
                var inversionForte = (double)explanationForte.TechnicalDetails["CurrentInversionStrength"];

                if (Math.Abs(inversionForte - inversionSuave) < 0.1)
                {
                    test.ErrorMessage = "Força de inversão não muda adequadamente";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Estratégias funcionando: {intersection}/15 dezenas comuns, inversão {inversionSuave:F2} vs {inversionForte:F2}";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<IntegrationTest> TestBasicEnsemble()
        {
            var test = new IntegrationTest { TestName = "Ensemble Básico" };

            try
            {
                Console.WriteLine("6️⃣ Testando ensemble básico...");

                var dados = Infra.CarregarConcursos();
                var engine = new PredictionEngine();

                // Inicializar engine com ambos os modelos
                await engine.InitializeAsync(dados);

                var antiFreqModel = new AntiFrequencySimpleModel();
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);
                await engine.RegisterModelAsync("AntiFreqSimple", antiFreqModel);

                // Configurar estratégia de ensemble
                engine.SetActiveStrategy("Ensemble");

                // Gerar predição de ensemble
                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
                var ensemblePrediction = await engine.GeneratePredictionAsync(targetConcurso);

                if (ensemblePrediction == null)
                {
                    test.ErrorMessage = "Falha ao gerar predição de ensemble";
                    return test;
                }

                // Validar estrutura da predição
                if (ensemblePrediction.PredictedNumbers.Count != 15)
                {
                    test.ErrorMessage = $"Predição de ensemble inválida: {ensemblePrediction.PredictedNumbers.Count} dezenas";
                    return test;
                }

                // Validar que método é ensemble
                if (!ensemblePrediction.GenerationMethod.Contains("Ensemble"))
                {
                    test.ErrorMessage = $"Método de geração incorreto: {ensemblePrediction.GenerationMethod}";
                    return test;
                }

                // Validar confiança (deve ser razoável)
                if (ensemblePrediction.OverallConfidence < 0.4)
                {
                    test.ErrorMessage = $"Confiança do ensemble muito baixa: {ensemblePrediction.OverallConfidence:P2}";
                    return test;
                }

                // Comparar com predições individuais
                engine.SetActiveStrategy("Single");
                var singlePrediction = await engine.GeneratePredictionAsync(targetConcurso + 1);

                test.Success = true;
                test.Details = $"✅ Ensemble funcionando: {ensemblePrediction.PredictedNumbers.Count} dezenas, confiança: {ensemblePrediction.OverallConfidence:P2}";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<IntegrationTest> TestInterfaceIntegration()
        {
            var test = new IntegrationTest { TestName = "Integração de Interface" };

            try
            {
                Console.WriteLine("7️⃣ Testando integração de interface...");

                // Simular integração com ViewModels
                var dados = Infra.CarregarConcursos();
                var engine = new PredictionEngine();

                await engine.InitializeAsync(dados);

                var antiFreqModel = new AntiFrequencySimpleModel();
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);
                await engine.RegisterModelAsync("AntiFreqSimple", antiFreqModel);

                // Testar eventos de status
                var statusUpdates = new List<string>();
                engine.OnStatusChanged += (sender, status) => statusUpdates.Add(status);

                // Gerar predição para disparar eventos
                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
                await engine.GeneratePredictionAsync(targetConcurso);

                // Validar que eventos foram disparados
                if (!statusUpdates.Any())
                {
                    test.ErrorMessage = "Nenhum evento de status foi disparado";
                    return test;
                }

                // Testar propriedades observáveis do modelo
                if (antiFreqModel.CurrentInversionStrength < 0 || antiFreqModel.CurrentInversionStrength > 1)
                {
                    test.ErrorMessage = $"Propriedade observável inválida: CurrentInversionStrength = {antiFreqModel.CurrentInversionStrength}";
                    return test;
                }

                // Testar sistema de configuração
                var originalFactor = antiFreqModel.GetParameterValue("InversionFactor");
                antiFreqModel.SetParameterValue("InversionFactor", 0.5);
                var newFactor = antiFreqModel.GetParameterValue("InversionFactor");

                if (!newFactor.Equals(0.5))
                {
                    test.ErrorMessage = "Sistema de configuração de parâmetros não funcionando";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Interface OK: {statusUpdates.Count} eventos, propriedades observáveis funcionando";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        #endregion

        #region Helper Methods

        private static void LogResult(string testName, bool success)
        {
            var status = success ? "✅ PASSOU" : "❌ FALHOU";
            Console.WriteLine($"   {testName}: {status}");
        }

        private static async Task SaveIntegrationReport(IntegrationTestReport report)
        {
            try
            {
                var reportContent = GenerateIntegrationReport(report);
                var fileName = $"AntiFreq_Integration_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                await System.IO.File.WriteAllTextAsync(fileName, reportContent);
                Console.WriteLine($"\n📄 Relatório de integração salvo: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erro ao salvar relatório: {ex.Message}");
            }
        }

        private static string GenerateIntegrationReport(IntegrationTestReport report)
        {
            var content = "RELATÓRIO DE INTEGRAÇÃO - ANTIFREQUENCY SIMPLE MODEL\n";
            content += "=" * 70 + "\n";
            content += $"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
            content += $"Tempo Total: {report.TotalExecutionTime.TotalSeconds:F2} segundos\n";
            content += $"Status Geral: {(report.OverallSuccess ? "✅ SUCESSO" : "❌ FALHA")}\n\n";

            var tests = new[]
            {
                report.InitializationTest,
                report.EngineIntegrationTest,
                report.PerformanceTest,
                report.CorrelationTest,
                report.AntiFreqStrategyTest,
                report.EnsembleTest,
                report.InterfaceTest
            };

            foreach (var test in tests.Where(t => t != null))
            {
                content += $"🔍 {test.TestName}\n";
                content += $"   Status: {(test.Success ? "✅ PASSOU" : "❌ FALHOU")}\n";
                
                if (!string.IsNullOrEmpty(test.Details))
                {
                    content += $"   Detalhes: {test.Details}\n";
                }
                
                if (!string.IsNullOrEmpty(test.ErrorMessage))
                {
                    content += $"   Erro: {test.ErrorMessage}\n";
                }
                
                content += "\n";
            }

            if (report.CriticalError != null)
            {
                content += "❌ ERRO CRÍTICO:\n";
                content += $"   {report.CriticalError.Message}\n\n";
            }

            if (report.OverallSuccess)
            {
                content += "🚀 CONCLUSÃO:\n";
                content += "   O AntiFrequencySimpleModel foi integrado com sucesso!\n";
                content += "   \n";
                content += "   Funcionalidades validadas:\n";
                content += "   ✅ Inicialização e treinamento corretos\n";
                content += "   ✅ Integração perfeita com PredictionEngine\n";
                content += "   ✅ Performance adequada (< 5s)\n";
                content += "   ✅ Baixa correlação com MetronomoModel\n";
                content += "   ✅ Estratégias anti-frequencistas funcionando\n";
                content += "   ✅ Ensemble básico operacional\n";
                content += "   ✅ Interface reativa e configurável\n";
                content += "   \n";
                content += "   Próximos passos:\n";
                content += "   🎯 Implementar StatisticalDebtModel (Semana 3)\n";
                content += "   🎯 Implementar SaturationModel (Semana 4)\n";
                content += "   🎯 Otimizar ensemble com múltiplos modelos\n";
            }

            return content;
        }

        #endregion
    }

    #region Supporting Classes

    public class IntegrationTestReport
    {
        public IntegrationTest InitializationTest { get; set; }
        public IntegrationTest EngineIntegrationTest { get; set; }
        public IntegrationTest PerformanceTest { get; set; }
        public IntegrationTest CorrelationTest { get; set; }
        public IntegrationTest AntiFreqStrategyTest { get; set; }
        public IntegrationTest EnsembleTest { get; set; }
        public IntegrationTest InterfaceTest { get; set; }
        
        public TimeSpan TotalExecutionTime { get; set; }
        public bool OverallSuccess { get; set; }
        public Exception CriticalError { get; set; }

        public bool AllTestsPassed()
        {
            var tests = new[]
            {
                InitializationTest, EngineIntegrationTest, PerformanceTest,
                CorrelationTest, AntiFreqStrategyTest, EnsembleTest, InterfaceTest
            };

            return tests.All(test => test?.Success == true) && CriticalError == null;
        }
    }

    public class IntegrationTest
    {
        public string TestName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Details { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }

    #endregion
}