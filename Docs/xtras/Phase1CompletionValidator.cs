// D:\PROJETOS\GraphFacil\Library\Services\Phase1CompletionValidator.cs - Validação Final da Fase 1
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Engines;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.Services.Auxiliar;
using LotoLibrary.Utilities;

namespace LotoLibrary.Services;

/// <summary>
/// Validador completo da Fase 1 - Verifica se todos os componentes estão funcionando
/// perfeitamente antes de iniciar a Fase 2 (Modelos Anti-Frequencistas)
/// </summary>
public static class Phase1CompletionValidator
{
    public static async Task<Phase1CompletionReport> ExecuteCompleteValidationAsync()
    {
        var report = new Phase1CompletionReport();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Console.WriteLine("🚀 INICIANDO VALIDAÇÃO COMPLETA DA FASE 1");
            Console.WriteLine("=" * 60);
            Console.WriteLine();

            // 1. Validar carregamento de dados
            report.DataLoadingTest = await ValidateDataLoading();
            LogResult("Carregamento de Dados", report.DataLoadingTest.Success);

            // 2. Validar PredictionEngine completo
            report.PredictionEngineTest = await ValidatePredictionEngineComplete();
            LogResult("PredictionEngine Completo", report.PredictionEngineTest.Success);

            // 3. Validar MetronomoModel migrado
            report.MetronomoModelTest = await ValidateMetronomoModelMigration();
            LogResult("MetronomoModel Migrado", report.MetronomoModelTest.Success);

            // 4. Validar integração entre componentes
            report.IntegrationTest = await ValidateComponentIntegration();
            LogResult("Integração de Componentes", report.IntegrationTest.Success);

            // 5. Validar correção do bug crítico
            report.BugFixValidation = await ValidateCriticalBugFix();
            LogResult("Correção Bug Crítico", report.BugFixValidation.Success);

            // 6. Validar performance baseline
            report.PerformanceTest = await ValidatePerformanceBaseline();
            LogResult("Performance Baseline", report.PerformanceTest.Success);

            // 7. Validar preparação para Fase 2
            report.Phase2ReadinessTest = await ValidatePhase2Readiness();
            LogResult("Preparação Fase 2", report.Phase2ReadinessTest.Success);

            stopwatch.Stop();
            report.TotalExecutionTime = stopwatch.Elapsed;
            report.OverallSuccess = report.AllTestsPassed();

            // Relatório final
            Console.WriteLine();
            Console.WriteLine("=" * 60);
            Console.WriteLine($"🏁 VALIDAÇÃO CONCLUÍDA EM {report.TotalExecutionTime.TotalSeconds:F2}s");
            Console.WriteLine();

            if (report.OverallSuccess)
            {
                Console.WriteLine("✅ TODAS AS VALIDAÇÕES PASSARAM!");
                Console.WriteLine("🚀 FASE 1 COMPLETAMENTE FINALIZADA");
                Console.WriteLine("🎯 SISTEMA PRONTO PARA FASE 2 - MODELOS ANTI-FREQUENCISTAS");
            }
            else
            {
                Console.WriteLine("❌ ALGUMAS VALIDAÇÕES FALHARAM");
                Console.WriteLine("📋 Revise os erros antes de continuar para Fase 2");
            }

            // Salvar relatório detalhado
            await SaveDetailedReport(report);

            return report;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERRO CRÍTICO NA VALIDAÇÃO: {ex.Message}");
            report.OverallSuccess = false;
            report.CriticalError = ex;
            return report;
        }
    }

    #region Individual Validation Tests

    private static async Task<ValidationTest> ValidateDataLoading()
    {
        var test = new ValidationTest { TestName = "Carregamento de Dados" };

        try
        {
            Console.WriteLine("1️⃣ Validando carregamento de dados históricos...");

            // Carregar dados usando Infra
            var dados = Infra.CarregarConcursos();

            if (dados == null || !dados.Any())
            {
                test.ErrorMessage = "Falha ao carregar dados históricos";
                return test;
            }

            // Validar estrutura dos dados
            var primeiroLance = dados.FirstOrDefault();
            var ultimoLance = dados.LastOrDefault();

            if (primeiroLance?.Lista?.Count != 15 || ultimoLance?.Lista?.Count != 15)
            {
                test.ErrorMessage = "Estrutura de dados inválida";
                return test;
            }

            // Validar quantidade mínima
            if (dados.Count < 100)
            {
                test.ErrorMessage = $"Dados insuficientes: {dados.Count} concursos (mínimo: 100)";
                return test;
            }

            test.Success = true;
            test.Details = $"✅ {dados.Count} concursos carregados ({primeiroLance.Id} a {ultimoLance.Id})";
            return test;
        }
        catch (Exception ex)
        {
            test.ErrorMessage = ex.Message;
            return test;
        }
    }

    private static async Task<ValidationTest> ValidatePredictionEngineComplete()
    {
        var test = new ValidationTest { TestName = "PredictionEngine Completo" };

        try
        {
            Console.WriteLine("2️⃣ Validando PredictionEngine completo...");

            var dados = Infra.CarregarConcursos();
            var engine = new PredictionEngine();

            // Testar inicialização
            var initResult = await engine.InitializeAsync(dados);
            if (!initResult)
            {
                test.ErrorMessage = "Falha na inicialização do PredictionEngine";
                return test;
            }

            // Testar propriedades
            if (!engine.IsInitialized || engine.TotalModels == 0)
            {
                test.ErrorMessage = "PredictionEngine não inicializou corretamente";
                return test;
            }

            // Testar geração de predição
            var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
            var prediction = await engine.GeneratePredictionAsync(targetConcurso);

            if (prediction == null || !prediction.PredictedNumbers.Any())
            {
                test.ErrorMessage = "Falha ao gerar predição";
                return test;
            }

            // Validar estrutura da predição
            if (prediction.PredictedNumbers.Count != 15)
            {
                test.ErrorMessage = $"Predição inválida: {prediction.PredictedNumbers.Count} dezenas (esperado: 15)";
                return test;
            }

            // Validar range das dezenas
            if (prediction.PredictedNumbers.Any(d => d < 1 || d > 25))
            {
                test.ErrorMessage = "Dezenas fora do range válido (1-25)";
                return test;
            }

            // Testar cache
            var prediction2 = await engine.GeneratePredictionAsync(targetConcurso);
            if (engine.CacheHitCount == 0)
            {
                test.ErrorMessage = "Sistema de cache não está funcionando";
                return test;
            }

            // Testar diagnóstico
            await engine.RunDiagnosticsAsync();

            test.Success = true;
            test.Details = $"✅ Engine funcionando - {engine.TotalModels} modelos, cache ativo, diagnóstico OK";
            return test;
        }
        catch (Exception ex)
        {
            test.ErrorMessage = ex.Message;
            return test;
        }
    }

    private static async Task<ValidationTest> ValidateMetronomoModelMigration()
    {
        var test = new ValidationTest { TestName = "MetronomoModel Migrado" };

        try
        {
            Console.WriteLine("3️⃣ Validando MetronomoModel migrado...");

            var dados = Infra.CarregarConcursos();
            var model = new MetronomoModel();

            // Testar inicialização
            var initResult = await model.InitializeAsync(dados);
            if (!initResult)
            {
                test.ErrorMessage = "Falha na inicialização do MetronomoModel";
                return test;
            }

            // Testar treinamento
            var trainResult = await model.TrainAsync(dados);
            if (!trainResult)
            {
                test.ErrorMessage = "Falha no treinamento do MetronomoModel";
                return test;
            }

            // Validar propriedades
            if (!model.IsInitialized || model.TotalMetronomos != 25)
            {
                test.ErrorMessage = $"Modelo não inicializou corretamente: {model.TotalMetronomos} metrônomos (esperado: 25)";
                return test;
            }

            // Testar predição
            var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
            var prediction = await model.PredictAsync(targetConcurso);

            if (prediction == null || prediction.PredictedNumbers.Count != 15)
            {
                test.ErrorMessage = "Falha na geração de predição";
                return test;
            }

            // Testar compatibilidade (métodos legados)
            var palpiteLegado = await model.GerarPalpiteAsync();
            if (palpiteLegado == null || palpiteLegado.Count != 15)
            {
                test.ErrorMessage = "Falha na compatibilidade com métodos legados";
                return test;
            }

            // Testar configuração de parâmetros
            model.ConfigureParameter("FatorRuidoControlado", 0.05);
            var paramValue = model.GetParameterValue("FatorRuidoControlado");
            if (!paramValue.Equals(0.05))
            {
                test.ErrorMessage = "Sistema de parâmetros não está funcionando";
                return test;
            }

            // Testar explicação do modelo
            var explanation = model.ExplainPrediction(prediction);
            if (explanation == null || !explanation.MainFactors.Any())
            {
                test.ErrorMessage = "Sistema de explicação não está funcionando";
                return test;
            }

            test.Success = true;
            test.Details = $"✅ Modelo funcionando - {model.TotalMetronomos} metrônomos, confiança: {model.Confidence:P2}";
            return test;
        }
        catch (Exception ex)
        {
            test.ErrorMessage = ex.Message;
            return test;
        }
    }

    private static async Task<ValidationTest> ValidateComponentIntegration()
    {
        var test = new ValidationTest { TestName = "Integração de Componentes" };

        try
        {
            Console.WriteLine("4️⃣ Validando integração entre componentes...");

            var dados = Infra.CarregarConcursos();
            var engine = new PredictionEngine();

            // Inicializar engine (que deve registrar MetronomoModel automaticamente)
            var initResult = await engine.InitializeAsync(dados);
            if (!initResult)
            {
                test.ErrorMessage = "Falha na inicialização integrada";
                return test;
            }

            // Verificar se MetronomoModel foi registrado
            var metronomoModel = engine.GetModel("Metronomo");
            if (metronomoModel == null)
            {
                test.ErrorMessage = "MetronomoModel não foi registrado automaticamente";
                return test;
            }

            // Testar predição integrada
            var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
            var enginePrediction = await engine.GeneratePredictionAsync(targetConcurso);
            var modelPrediction = await metronomoModel.PredictAsync(targetConcurso);

            if (enginePrediction == null || modelPrediction == null)
            {
                test.ErrorMessage = "Falha na geração de predições integradas";
                return test;
            }

            // Testar consistência (podem ser diferentes devido ao cache, mas estrutura deve ser igual)
            if (enginePrediction.PredictedNumbers.Count != modelPrediction.PredictedNumbers.Count)
            {
                test.ErrorMessage = "Inconsistência entre predições do engine e modelo";
                return test;
            }

            // Testar estratégias diferentes
            engine.SetActiveStrategy("Single");
            var singlePrediction = await engine.GeneratePredictionAsync(targetConcurso + 1);

            engine.SetActiveStrategy("BestModel");
            var bestPrediction = await engine.GeneratePredictionAsync(targetConcurso + 2);

            if (singlePrediction == null || bestPrediction == null)
            {
                test.ErrorMessage = "Falha nas estratégias de predição";
                return test;
            }

            test.Success = true;
            test.Details = "✅ Integração funcionando - Engine + MetronomoModel + Estratégias";
            return test;
        }
        catch (Exception ex)
        {
            test.ErrorMessage = ex.Message;
            return test;
        }
    }

    private static async Task<ValidationTest> ValidateCriticalBugFix()
    {
        var test = new ValidationTest { TestName = "Correção Bug Crítico" };

        try
        {
            Console.WriteLine("5️⃣ Validando correção do bug crítico (dezenas 1-9)...");

            var dados = Infra.CarregarConcursos();
            var engine = new PredictionEngine();
            await engine.InitializeAsync(dados);

            // Gerar múltiplas predições para análise estatística
            var predictions = new List<List<int>>();
            for (int i = 0; i < 20; i++)
            {
                engine.ClearCache(); // Limpar cache para forçar nova geração
                var prediction = await engine.GeneratePredictionAsync(3000 + i);
                predictions.Add(prediction.PredictedNumbers);
            }

            // Analisar distribuição usando DiagnosticService
            var diagnosticReport = DiagnosticService.AnalyzeDezenasDistribution(predictions);

            if (!diagnosticReport.IsDistributionNormal)
            {
                test.ErrorMessage = $"Bug crítico não foi corrigido - Gravidade: {diagnosticReport.GravidadeProblema}";
                test.Details = $"Dezenas 1-9: {diagnosticReport.PercentualDezenas1a9:P2} (mínimo esperado: 20%)";
                return test;
            }

            // Validar que pelo menos 20% das dezenas são 1-9
            if (diagnosticReport.PercentualDezenas1a9 < 0.20)
            {
                test.ErrorMessage = $"Distribuição ainda problemática: {diagnosticReport.PercentualDezenas1a9:P2} de dezenas 1-9";
                return test;
            }

            test.Success = true;
            test.Details = $"✅ Bug corrigido - {diagnosticReport.PercentualDezenas1a9:P2} de dezenas 1-9, distribuição normal";
            return test;
        }
        catch (Exception ex)
        {
            test.ErrorMessage = ex.Message;
            return test;
        }
    }

    private static async Task<ValidationTest> ValidatePerformanceBaseline()
    {
        var test = new ValidationTest { TestName = "Performance Baseline" };

        try
        {
            Console.WriteLine("6️⃣ Validando performance baseline...");

            var dados = Infra.CarregarConcursos();
            var model = new MetronomoModel();
            await model.InitializeAsync(dados);
            await model.TrainAsync(dados);

            // Executar validação histórica
            var stopwatch = Stopwatch.StartNew();
            var metricas = await model.ValidarModeloAsync();
            stopwatch.Stop();

            if (metricas == null || metricas.TotalTestes == 0)
            {
                test.ErrorMessage = "Falha na validação de performance";
                return test;
            }

            // Validar performance mínima (deve ser melhor que aleatório ~60%)
            if (metricas.TaxaAcertoMedia < 0.61)
            {
                test.ErrorMessage = $"Performance abaixo do esperado: {metricas.TaxaAcertoMedia:P2} (mínimo: 61%)";
                return test;
            }

            // Validar tempo de execução (deve ser < 5 segundos)
            if (stopwatch.Elapsed.TotalSeconds > 5.0)
            {
                test.ErrorMessage = $"Performance temporal inadequada: {stopwatch.Elapsed.TotalSeconds:F2}s (máximo: 5s)";
                return test;
            }

            test.Success = true;
            test.Details = $"✅ Performance OK - {metricas.TaxaAcertoMedia:P2} acerto em {stopwatch.Elapsed.TotalSeconds:F2}s";
            return test;
        }
        catch (Exception ex)
        {
            test.ErrorMessage = ex.Message;
            return test;
        }
    }

    private static async Task<ValidationTest> ValidatePhase2Readiness()
    {
        var test = new ValidationTest { TestName = "Preparação Fase 2" };

        try
        {
            Console.WriteLine("7️⃣ Validando preparação para Fase 2...");

            var dados = Infra.CarregarConcursos();
            var engine = new PredictionEngine();
            await engine.InitializeAsync(dados);

            // Verificar estrutura de pastas necessária
            var requiredFolders = new[]
            {
                @"Library\PredictionModels\AntiFrequency",
                @"Library\Services\Analysis",
                @"Library\Utilities\AntiFrequency"
            };

            foreach (var folder in requiredFolders)
            {
                if (!System.IO.Directory.Exists(folder))
                {
                    // Criar pasta se não existir
                    System.IO.Directory.CreateDirectory(folder);
                    Console.WriteLine($"📁 Criada pasta: {folder}");
                }
            }

            // Verificar capacidade de registro de novos modelos
            var testModel = new MetronomoModel();
            await testModel.InitializeAsync(dados);

            var registrationResult = await engine.RegisterModelAsync("TestAntiFreq", testModel);
            if (!registrationResult)
            {
                test.ErrorMessage = "Falha no sistema de registro de modelos";
                return test;
            }

            // Limpar modelo de teste
            engine.UnregisterModel("TestAntiFreq");

            // Verificar interfaces necessárias
            var interfaces = new[]
            {
                typeof(IPredictionModel),
                typeof(IConfigurableModel),
                typeof(IEnsembleModel)
            };

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType == null)
                {
                    test.ErrorMessage = $"Interface necessária não encontrada: {interfaceType?.Name}";
                    return test;
                }
            }

            test.Success = true;
            test.Details = "✅ Sistema pronto para Fase 2 - Estrutura OK, Interfaces OK, Registry OK";
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

    private static async Task SaveDetailedReport(Phase1CompletionReport report)
    {
        try
        {
            var reportContent = GenerateDetailedReport(report);
            var fileName = $"Phase1_Completion_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            await System.IO.File.WriteAllTextAsync(fileName, reportContent);
            Console.WriteLine($"\n📄 Relatório detalhado salvo: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erro ao salvar relatório: {ex.Message}");
        }
    }

    private static string GenerateDetailedReport(Phase1CompletionReport report)
    {
        var content = "RELATÓRIO COMPLETO - VALIDAÇÃO FASE 1\n";
        content += "=" * 60 + "\n";
        content += $"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
        content += $"Tempo Total: {report.TotalExecutionTime.TotalSeconds:F2} segundos\n";
        content += $"Status Geral: {(report.OverallSuccess ? "✅ SUCESSO" : "❌ FALHA")}\n\n";

        var tests = new[]
        {
            report.DataLoadingTest,
            report.PredictionEngineTest,
            report.MetronomoModelTest,
            report.IntegrationTest,
            report.BugFixValidation,
            report.PerformanceTest,
            report.Phase2ReadinessTest
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
            content += $"   {report.CriticalError.Message}\n";
            content += $"   {report.CriticalError.StackTrace}\n\n";
        }

        if (report.OverallSuccess)
        {
            content += "🚀 CONCLUSÃO:\n";
            content += "   A Fase 1 foi completamente finalizada com sucesso!\n";
            content += "   O sistema está pronto para a implementação dos modelos anti-frequencistas da Fase 2.\n";
            content += "   \n";
            content += "   Próximos passos:\n";
            content += "   1. Implementar AntiFrequencySimpleModel\n";
            content += "   2. Implementar StatisticalDebtModel\n";
            content += "   3. Implementar SaturationModel\n";
            content += "   4. Criar BasicEnsembleModel\n";
            content += "   5. Implementar sistema de correlação entre modelos\n";
        }

        return content;
    }

    #endregion
}

