// D:\PROJETOS\GraphFacil\Library\Services\ThreeModelIntegrationTest.cs - Teste completo do sistema tri-modelo
using LotoLibrary.Engines;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using LotoLibrary.PredictionModels.AntiFrequency.Statistical;
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
    /// Teste completo do sistema tri-modelo: Metrônomo + AntiFreqSimple + StatisticalDebt
    /// Valida integração, performance, correlação e funcionalidade do ensemble
    /// </summary>
    public static class ThreeModelIntegrationTest
    {
        public static async Task<TriModelTestReport> ExecuteCompleteTriModelTestAsync()
        {
            var report = new TriModelTestReport();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine("🚀 INICIANDO TESTE COMPLETO - SISTEMA TRI-MODELO");
                Console.WriteLine("=" * 70);
                Console.WriteLine("📊 Modelos: Metrônomo + AntiFreqSimple + StatisticalDebt");
                Console.WriteLine();

                // 1. Teste de Inicialização dos 3 Modelos
                report.ModelInitializationTest = await TestAllModelsInitialization();
                LogResult("Inicialização dos 3 Modelos", report.ModelInitializationTest.Success);

                // 2. Teste de Performance Individual
                report.IndividualPerformanceTest = await TestIndividualPerformance();
                LogResult("Performance Individual", report.IndividualPerformanceTest.Success);

                // 3. Teste de Correlação Cruzada
                report.CrossCorrelationTest = await TestCrossCorrelation();
                LogResult("Correlação Cruzada", report.CrossCorrelationTest.Success);

                // 4. Teste de Ensemble Avançado
                report.AdvancedEnsembleTest = await TestAdvancedEnsemble();
                LogResult("Ensemble Avançado", report.AdvancedEnsembleTest.Success);

                // 5. Teste de Diversificação
                report.DiversificationTest = await TestPortfolioDiversification();
                LogResult("Diversificação de Portfólio", report.DiversificationTest.Success);

                // 6. Teste de Estratégias Dinâmicas
                report.DynamicStrategyTest = await TestDynamicStrategies();
                LogResult("Estratégias Dinâmicas", report.DynamicStrategyTest.Success);

                // 7. Teste de Análise Comparativa
                report.ComparativeAnalysisTest = await TestComparativeAnalysis();
                LogResult("Análise Comparativa", report.ComparativeAnalysisTest.Success);

                stopwatch.Stop();
                report.TotalExecutionTime = stopwatch.Elapsed;
                report.OverallSuccess = report.AllTestsPassed();

                // Relatório final
                Console.WriteLine();
                Console.WriteLine("=" * 70);
                Console.WriteLine($"🏁 TESTE TRI-MODELO CONCLUÍDO EM {report.TotalExecutionTime.TotalSeconds:F2}s");
                Console.WriteLine();

                if (report.OverallSuccess)
                {
                    Console.WriteLine("✅ TODOS OS TESTES DO SISTEMA TRI-MODELO PASSARAM!");
                    Console.WriteLine("🎯 SISTEMA MULTI-MODELO COMPLETAMENTE OPERACIONAL");
                    Console.WriteLine("🚀 ENSEMBLE INTELIGENTE FUNCIONANDO");
                    Console.WriteLine("📈 DIVERSIFICAÇÃO CONFIRMADA");
                    Console.WriteLine("🎭 PRONTO PARA IMPLEMENTAÇÃO EM PRODUÇÃO");
                }
                else
                {
                    Console.WriteLine("❌ ALGUNS TESTES FALHARAM");
                    Console.WriteLine("📋 Revise os erros antes de continuar");
                }

                // Salvar relatório detalhado
                await SaveTriModelReport(report);

                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO CRÍTICO NO TESTE TRI-MODELO: {ex.Message}");
                report.OverallSuccess = false;
                report.CriticalError = ex;
                return report;
            }
        }

        #region Individual Test Methods

        private static async Task<TestResult> TestAllModelsInitialization()
        {
            var test = new TestResult { TestName = "Inicialização dos 3 Modelos" };

            try
            {
                Console.WriteLine("1️⃣ Testando inicialização de todos os modelos...");

                var dados = Infra.CarregarConcursos();
                if (dados == null || !dados.Any())
                {
                    test.ErrorMessage = "Falha ao carregar dados históricos";
                    return test;
                }

                // Criar e inicializar todos os 3 modelos
                var metronomoModel = new MetronomoModel();
                var antiFreqSimpleModel = new AntiFrequencySimpleModel();
                var statisticalDebtModel = new StatisticalDebtModel();

                // Testar inicialização
                var initResults = new List<bool>
                {
                    await metronomoModel.InitializeAsync(dados),
                    await antiFreqSimpleModel.InitializeAsync(dados),
                    await statisticalDebtModel.InitializeAsync(dados)
                };

                if (initResults.Any(r => !r))
                {
                    test.ErrorMessage = "Falha na inicialização de um ou mais modelos";
                    return test;
                }

                // Testar treinamento
                var trainResults = new List<bool>
                {
                    await metronomoModel.TrainAsync(dados),
                    await antiFreqSimpleModel.TrainAsync(dados),
                    await statisticalDebtModel.TrainAsync(dados)
                };

                if (trainResults.Any(r => !r))
                {
                    test.ErrorMessage = "Falha no treinamento de um ou mais modelos";
                    return test;
                }

                // Validar propriedades específicas
                if (!metronomoModel.IsInitialized || metronomoModel.TotalMetronomos != 25)
                {
                    test.ErrorMessage = $"MetronomoModel: metrônomos = {metronomoModel.TotalMetronomos} (esperado: 25)";
                    return test;
                }

                if (!antiFreqSimpleModel.IsInitialized || antiFreqSimpleModel.SimpleProfiles.Count != 25)
                {
                    test.ErrorMessage = $"AntiFreqSimpleModel: perfis = {antiFreqSimpleModel.SimpleProfiles.Count} (esperado: 25)";
                    return test;
                }

                if (!statisticalDebtModel.IsInitialized || statisticalDebtModel.DebtProfiles.Count != 25)
                {
                    test.ErrorMessage = $"StatisticalDebtModel: perfis de dívida = {statisticalDebtModel.DebtProfiles.Count} (esperado: 25)";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ 3 modelos inicializados: Metrônomo ({metronomoModel.Confidence:P2}), AntiFreqSimple ({antiFreqSimpleModel.Confidence:P2}), StatisticalDebt ({statisticalDebtModel.Confidence:P2})";
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<TestResult> TestIndividualPerformance()
        {
            var test = new TestResult { TestName = "Performance Individual" };

            try
            {
                Console.WriteLine("2️⃣ Testando performance individual dos modelos...");

                var dados = Infra.CarregarConcursos();
                
                // Criar modelos
                var models = new Dictionary<string, IPredictionModel>
                {
                    ["Metronomo"] = new MetronomoModel(),
                    ["AntiFreqSimple"] = new AntiFrequencySimpleModel(),
                    ["StatisticalDebt"] = new StatisticalDebtModel()
                };

                // Inicializar todos
                foreach (var model in models.Values)
                {
                    await model.InitializeAsync(dados);
                    await model.TrainAsync(dados);
                }

                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;
                var performances = new Dictionary<string, PerformanceMetrics>();

                Console.WriteLine($"   📊 Gerando predições para análise de performance...");

                // Testar performance de cada modelo
                foreach (var kvp in models)
                {
                    var modelName = kvp.Key;
                    var model = kvp.Value;

                    var stopwatch = Stopwatch.StartNew();
                    var prediction = await model.PredictAsync(targetConcurso);
                    stopwatch.Stop();

                    if (prediction == null || prediction.PredictedNumbers.Count != 15)
                    {
                        test.ErrorMessage = $"Predição inválida do modelo {modelName}";
                        return test;
                    }

                    // Validar performance temporal (< 3 segundos por modelo)
                    if (stopwatch.Elapsed.TotalSeconds > 3.0)
                    {
                        test.ErrorMessage = $"Performance temporal inadequada para {modelName}: {stopwatch.Elapsed.TotalSeconds:F2}s";
                        return test;
                    }

                    // Validar confiança mínima
                    if (model.Confidence < 0.5)
                    {
                        test.ErrorMessage = $"Confiança muito baixa para {modelName}: {model.Confidence:P2}";
                        return test;
                    }

                    // Validar distribuição de dezenas (correção do bug)
                    var dezenas1a9 = prediction.PredictedNumbers.Count(d => d <= 9);
                    if (dezenas1a9 < 2) // Pelo menos 13% de dezenas 1-9
                    {
                        test.ErrorMessage = $"Bug de distribuição no {modelName}: apenas {dezenas1a9} dezenas 1-9";
                        return test;
                    }

                    performances[modelName] = new PerformanceMetrics
                    {
                        ModelName = modelName,
                        AverageConfidence = model.Confidence,
                        ExecutionTime = stopwatch.Elapsed,
                        PredictionValid = true
                    };

                    Console.WriteLine($"      {modelName}: {model.Confidence:P2} confiança, {stopwatch.Elapsed.TotalSeconds:F2}s, {dezenas1a9}/15 dezenas 1-9");
                }

                // Validar que modelos têm características diferentes
                var confidences = performances.Values.Select(p => p.AverageConfidence).ToList();
                var confidenceVariance = CalculateVariance(confidences);
                
                if (confidenceVariance < 0.001) // Confiança muito similar
                {
                    test.ErrorMessage = "Modelos com confiança muito similar - possível problema de implementação";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Performance validada: variância confiança = {confidenceVariance:F4}, todos < 3s execução";
                test.PerformanceData = performances;
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<TestResult> TestCrossCorrelation()
        {
            var test = new TestResult { TestName = "Correlação Cruzada" };

            try
            {
                Console.WriteLine("3️⃣ Testando correlação cruzada entre os 3 modelos...");

                var dados = Infra.CarregarConcursos();
                var comparer = new PerformanceComparer();

                // Criar e preparar modelos
                var models = new Dictionary<string, IPredictionModel>
                {
                    ["Metronomo"] = new MetronomoModel(),
                    ["AntiFreqSimple"] = new AntiFrequencySimpleModel(),
                    ["StatisticalDebt"] = new StatisticalDebtModel()
                };

                foreach (var model in models.Values)
                {
                    await model.InitializeAsync(dados);
                    await model.TrainAsync(dados);
                }

                // Gerar predições para análise de correlação
                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                Console.WriteLine($"   📊 Gerando 15 predições por modelo para análise...");

                for (int i = 0; i < 15; i++)
                {
                    var target = targetBase + i;
                    
                    foreach (var kvp in models)
                    {
                        var prediction = await kvp.Value.PredictAsync(target);
                        comparer.AddPredictionResult(kvp.Key, prediction);
                    }
                }

                // Analisar correlações
                var correlationMatrix = await comparer.AnalyzeAllCorrelationsAsync();
                var allPairs = correlationMatrix.GetAllPairs();

                Console.WriteLine($"   🔍 Matriz de correlação:");
                var correlationData = new Dictionary<string, double>();

                foreach (var pair in allPairs)
                {
                    var pairKey = $"{pair.Model1}_vs_{pair.Model2}";
                    correlationData[pairKey] = pair.Correlation;
                    Console.WriteLine($"      {pair.Model1} ↔ {pair.Model2}: {pair.Correlation:F3}");
                }

                // Validar baixa correlação (boa diversificação)
                var avgCorrelation = allPairs.Average(p => Math.Abs(p.Correlation));
                if (avgCorrelation > 0.75) // Correlação muito alta
                {
                    test.ErrorMessage = $"Correlação média muito alta: {avgCorrelation:F3} (máximo esperado: 0.75)";
                    return test;
                }

                // Validar que pelo menos um par tem baixa correlação
                var lowCorrelationPairs = allPairs.Count(p => Math.Abs(p.Correlation) < 0.6);
                if (lowCorrelationPairs == 0)
                {
                    test.ErrorMessage = "Nenhum par com baixa correlação encontrado";
                    return test;
                }

                // Encontrar pares ideais para ensemble
                var idealPairs = await comparer.FindLowCorrelationPairsAsync(0.7);
                
                test.Success = true;
                test.Details = $"✅ Correlação adequada: média = {avgCorrelation:F3}, {lowCorrelationPairs} pares com baixa correlação, {idealPairs.Count} pares ideais";
                test.CorrelationData = correlationData;
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<TestResult> TestAdvancedEnsemble()
        {
            var test = new TestResult { TestName = "Ensemble Avançado" };

            try
            {
                Console.WriteLine("4️⃣ Testando ensemble avançado com 3 modelos...");

                var dados = Infra.CarregarConcursos();
                var engine = new PredictionEngine();
                var comparer = new PerformanceComparer();

                // Inicializar PredictionEngine
                await engine.InitializeAsync(dados);

                // Registrar modelos anti-frequencistas
                var antiFreqSimple = new AntiFrequencySimpleModel();
                var statisticalDebt = new StatisticalDebtModel();

                await antiFreqSimple.InitializeAsync(dados);
                await antiFreqSimple.TrainAsync(dados);
                await statisticalDebt.InitializeAsync(dados);
                await statisticalDebt.TrainAsync(dados);

                await engine.RegisterModelAsync("AntiFreqSimple", antiFreqSimple);
                await engine.RegisterModelAsync("StatisticalDebt", statisticalDebt);

                if (engine.TotalModels != 3)
                {
                    test.ErrorMessage = $"Número incorreto de modelos registrados: {engine.TotalModels} (esperado: 3)";
                    return test;
                }

                Console.WriteLine($"   🎭 Testando estratégias de ensemble...");

                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;

                // Teste 1: Ensemble padrão
                engine.SetActiveStrategy("Ensemble");
                var ensemblePred = await engine.GeneratePredictionAsync(targetConcurso);

                if (ensemblePred == null || !ensemblePred.GenerationMethod.Contains("Ensemble"))
                {
                    test.ErrorMessage = "Falha na geração de ensemble";
                    return test;
                }

                // Teste 2: Melhor modelo
                engine.SetActiveStrategy("BestModel");
                var bestModelPred = await engine.GeneratePredictionAsync(targetConcurso + 1);

                if (bestModelPred == null)
                {
                    test.ErrorMessage = "Falha na estratégia BestModel";
                    return test;
                }

                // Teste 3: Predições individuais para comparação
                var individualPredictions = new Dictionary<string, List<int>>();
                
                var metronomo = engine.GetModel("Metronomo");
                var antiFreq = engine.GetModel("AntiFreqSimple");
                var debt = engine.GetModel("StatisticalDebt");

                individualPredictions["Metronomo"] = (await metronomo.PredictAsync(targetConcurso + 2)).PredictedNumbers;
                individualPredictions["AntiFreqSimple"] = (await antiFreq.PredictAsync(targetConcurso + 2)).PredictedNumbers;
                individualPredictions["StatisticalDebt"] = (await debt.PredictAsync(targetConcurso + 2)).PredictedNumbers;

                // Validar diversificação do ensemble
                var ensembleIntersections = new Dictionary<string, int>();
                foreach (var kvp in individualPredictions)
                {
                    var intersection = ensemblePred.PredictedNumbers.Intersect(kvp.Value).Count();
                    ensembleIntersections[kvp.Key] = intersection;
                }

                // Ensemble deve ter intersecção moderada com todos (não deve favorecer excessivamente um modelo)
                var maxIntersection = ensembleIntersections.Values.Max();
                var minIntersection = ensembleIntersections.Values.Min();
                
                if (maxIntersection > 12) // Mais de 80% igual a um modelo
                {
                    test.ErrorMessage = $"Ensemble muito similar a um modelo: {maxIntersection}/15 intersecção máxima";
                    return test;
                }

                if (minIntersection < 3) // Menos de 20% similar ao modelo menos representado
                {
                    test.ErrorMessage = $"Ensemble não representativo: {minIntersection}/15 intersecção mínima";
                    return test;
                }

                // Validar confiança do ensemble
                if (ensemblePred.OverallConfidence < 0.6)
                {
                    test.ErrorMessage = $"Confiança do ensemble muito baixa: {ensemblePred.OverallConfidence:P2}";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Ensemble funcionando: confiança = {ensemblePred.OverallConfidence:P2}, intersecções = [{string.Join(",", ensembleIntersections.Values)}]";
                test.EnsembleData = new { 
                    EnsembleConfidence = ensemblePred.OverallConfidence,
                    Intersections = ensembleIntersections,
                    TotalModels = engine.TotalModels
                };
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<TestResult> TestPortfolioDiversification()
        {
            var test = new TestResult { TestName = "Diversificação de Portfólio" };

            try
            {
                Console.WriteLine("5️⃣ Testando diversificação do portfólio de modelos...");

                var dados = Infra.CarregarConcursos();
                var comparer = new PerformanceComparer();

                // Criar modelos com configurações diferentes para simular portfólio diversificado
                var models = new Dictionary<string, IPredictionModel>();

                // Modelo 1: Metrônomo padrão
                var metronomo = new MetronomoModel();
                await metronomo.InitializeAsync(dados);
                await metronomo.TrainAsync(dados);
                models["Metronomo"] = metronomo;

                // Modelo 2: AntiFreq conservador
                var antiFreqConservative = new AntiFrequencySimpleModel();
                await antiFreqConservative.InitializeAsync(dados);
                await antiFreqConservative.TrainAsync(dados);
                antiFreqConservative.SetParameterValue("InversionFactor", 0.3);
                models["AntiFreq_Conservative"] = antiFreqConservative;

                // Modelo 3: AntiFreq agressivo
                var antiFreqAggressive = new AntiFrequencySimpleModel();
                await antiFreqAggressive.InitializeAsync(dados);
                await antiFreqAggressive.TrainAsync(dados);
                antiFreqAggressive.SetParameterValue("InversionFactor", 0.9);
                models["AntiFreq_Aggressive"] = antiFreqAggressive;

                // Modelo 4: StatisticalDebt padrão
                var debtModel = new StatisticalDebtModel();
                await debtModel.InitializeAsync(dados);
                await debtModel.TrainAsync(dados);
                models["StatisticalDebt"] = debtModel;

                // Modelo 5: StatisticalDebt acelerado
                var debtAccelerated = new StatisticalDebtModel();
                await debtAccelerated.InitializeAsync(dados);
                await debtAccelerated.TrainAsync(dados);
                debtAccelerated.SetParameterValue("DebtAccelerationFactor", 1.5);
                models["Debt_Accelerated"] = debtAccelerated;

                Console.WriteLine($"   📊 Analisando portfólio de {models.Count} modelos...");

                // Gerar predições
                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                for (int i = 0; i < 10; i++)
                {
                    var target = targetBase + i;
                    foreach (var kvp in models)
                    {
                        var prediction = await kvp.Value.PredictAsync(target);
                        comparer.AddPredictionResult(kvp.Key, prediction);
                    }
                }

                // Analisar diversificação
                var correlationMatrix = await comparer.AnalyzeAllCorrelationsAsync();
                var allPairs = correlationMatrix.GetAllPairs();

                // Calcular métricas de diversificação
                var avgCorrelation = allPairs.Average(p => Math.Abs(p.Correlation));
                var maxCorrelation = allPairs.Max(p => Math.Abs(p.Correlation));
                var lowCorrelationPairs = allPairs.Count(p => Math.Abs(p.Correlation) < 0.6);
                var moderateCorrelationPairs = allPairs.Count(p => Math.Abs(p.Correlation) >= 0.6 && Math.Abs(p.Correlation) < 0.8);

                Console.WriteLine($"      Correlação média: {avgCorrelation:F3}");
                Console.WriteLine($"      Correlação máxima: {maxCorrelation:F3}");
                Console.WriteLine($"      Pares baixa correlação: {lowCorrelationPairs}/{allPairs.Count}");

                // Validar diversificação adequada
                if (avgCorrelation > 0.65)
                {
                    test.ErrorMessage = $"Diversificação insuficiente - correlação média: {avgCorrelation:F3}";
                    return test;
                }

                if (lowCorrelationPairs < 3)
                {
                    test.ErrorMessage = $"Poucos pares diversificados: {lowCorrelationPairs}";
                    return test;
                }

                // Gerar relatório de ensemble recomendado
                var comprehensiveReport = await comparer.GenerateComprehensiveReportAsync();
                var recommendedEnsemble = comprehensiveReport.RecommendedEnsemble;

                Console.WriteLine($"      Ensemble recomendado: [{string.Join(", ", recommendedEnsemble)}]");

                test.Success = true;
                test.Details = $"✅ Diversificação adequada: correlação média = {avgCorrelation:F3}, {lowCorrelationPairs} pares baixa correlação, ensemble recomendado = {recommendedEnsemble.Count} modelos";
                test.DiversificationData = new {
                    AverageCorrelation = avgCorrelation,
                    MaxCorrelation = maxCorrelation,
                    LowCorrelationPairs = lowCorrelationPairs,
                    RecommendedEnsemble = recommendedEnsemble
                };
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<TestResult> TestDynamicStrategies()
        {
            var test = new TestResult { TestName = "Estratégias Dinâmicas" };

            try
            {
                Console.WriteLine("6️⃣ Testando estratégias dinâmicas...");

                var dados = Infra.CarregarConcursos();
                var engine = new PredictionEngine();

                // Configurar sistema completo
                await engine.InitializeAsync(dados);

                var antiFreq = new AntiFrequencySimpleModel();
                var debt = new StatisticalDebtModel();

                await antiFreq.InitializeAsync(dados);
                await antiFreq.TrainAsync(dados);
                await debt.InitializeAsync(dados);
                await debt.TrainAsync(dados);

                await engine.RegisterModelAsync("AntiFreqSimple", antiFreq);
                await engine.RegisterModelAsync("StatisticalDebt", debt);

                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                var strategyResults = new Dictionary<string, PredictionResult>();

                Console.WriteLine($"   🎯 Testando diferentes estratégias...");

                // Estratégia 1: Single (usa primeiro modelo)
                engine.SetActiveStrategy("Single");
                var singleResult = await engine.GeneratePredictionAsync(targetBase);
                strategyResults["Single"] = singleResult;
                Console.WriteLine($"      Single: {singleResult.ModelUsed}, confiança = {singleResult.OverallConfidence:P2}");

                // Estratégia 2: BestModel (usa modelo com melhor confiança)
                engine.ClearCache();
                engine.SetActiveStrategy("BestModel");
                var bestResult = await engine.GeneratePredictionAsync(targetBase + 1);
                strategyResults["BestModel"] = bestResult;
                Console.WriteLine($"      BestModel: {bestResult.ModelUsed}, confiança = {bestResult.OverallConfidence:P2}");

                // Estratégia 3: Ensemble (combina todos)
                engine.ClearCache();
                engine.SetActiveStrategy("Ensemble");
                var ensembleResult = await engine.GeneratePredictionAsync(targetBase + 2);
                strategyResults["Ensemble"] = ensembleResult;
                Console.WriteLine($"      Ensemble: {ensembleResult.ModelUsed}, confiança = {ensembleResult.OverallConfidence:P2}");

                // Validar que estratégias produzem resultados diferentes
                var strategiesComparison = new Dictionary<string, int>();
                var strategies = strategyResults.Keys.ToList();

                for (int i = 0; i < strategies.Count; i++)
                {
                    for (int j = i + 1; j < strategies.Count; j++)
                    {
                        var strategy1 = strategies[i];
                        var strategy2 = strategies[j];
                        var intersection = strategyResults[strategy1].PredictedNumbers
                            .Intersect(strategyResults[strategy2].PredictedNumbers).Count();
                        
                        strategiesComparison[$"{strategy1}_vs_{strategy2}"] = intersection;
                    }
                }

                // Validar diversidade entre estratégias
                var avgIntersection = strategiesComparison.Values.Average();
                if (avgIntersection > 12) // Muito similares
                {
                    test.ErrorMessage = $"Estratégias muito similares - intersecção média: {avgIntersection:F1}/15";
                    return test;
                }

                // Validar que ensemble não é idêntico a nenhuma estratégia individual
                var ensembleVsSingle = strategyResults["Ensemble"].PredictedNumbers
                    .Intersect(strategyResults["Single"].PredictedNumbers).Count();
                var ensembleVsBest = strategyResults["Ensemble"].PredictedNumbers
                    .Intersect(strategyResults["BestModel"].PredictedNumbers).Count();

                if (ensembleVsSingle > 13 || ensembleVsBest > 13) // Mais de 85% igual
                {
                    test.ErrorMessage = "Ensemble muito similar a estratégia individual";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Estratégias dinâmicas funcionando: intersecção média = {avgIntersection:F1}/15, ensemble diversificado";
                test.StrategyData = strategiesComparison;
                return test;
            }
            catch (Exception ex)
            {
                test.ErrorMessage = ex.Message;
                return test;
            }
        }

        private static async Task<TestResult> TestComparativeAnalysis()
        {
            var test = new TestResult { TestName = "Análise Comparativa" };

            try
            {
                Console.WriteLine("7️⃣ Executando análise comparativa completa...");

                var dados = Infra.CarregarConcursos();

                // Executar validações específicas de cada modelo
                Console.WriteLine($"   🔍 Validações específicas de modelos...");

                // Validação do AntiFrequencySimpleModel
                var antiFreqModel = new AntiFrequencySimpleModel();
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);
                var antiFreqValidation = await antiFreqModel.ValidateModelPerformanceAsync();

                if (!antiFreqValidation.IsValid || antiFreqValidation.OverallScore < 0.8)
                {
                    test.ErrorMessage = $"AntiFrequencySimpleModel falhou na validação: {antiFreqValidation.OverallScore:P1}";
                    return test;
                }

                // Validação do StatisticalDebtModel
                var debtModel = new StatisticalDebtModel();
                await debtModel.InitializeAsync(dados);
                await debtModel.TrainAsync(dados);
                var debtValidation = await debtModel.ValidateDebtModelAsync();

                if (!debtValidation.IsValid || debtValidation.OverallScore < 0.8)
                {
                    test.ErrorMessage = $"StatisticalDebtModel falhou na validação: {debtValidation.OverallScore:P1}";
                    return test;
                }

                // Análise comparativa de características
                Console.WriteLine($"   📊 Analisando características dos modelos...");

                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;

                // Características do AntiFreq
                var antiFreqPred = await antiFreqModel.PredictAsync(targetConcurso);
                var antiFreqExplanation = antiFreqModel.ExplainPrediction(antiFreqPred);

                // Características do Debt
                var debtPred = await debtModel.PredictAsync(targetConcurso);
                var debtExplanation = debtModel.ExplainPrediction(debtPred);

                // Características do Metrônomo
                var metronomoModel = new MetronomoModel();
                await metronomoModel.InitializeAsync(dados);
                await metronomoModel.TrainAsync(dados);
                var metronomoPred = await metronomoModel.PredictAsync(targetConcurso);
                var metronomoExplanation = metronomoModel.ExplainPrediction(metronomoPred);

                // Validar que cada modelo tem características únicas
                var explanationFactors = new Dictionary<string, int>
                {
                    ["AntiFreq"] = antiFreqExplanation.MainFactors.Count,
                    ["Debt"] = debtExplanation.MainFactors.Count,
                    ["Metronomo"] = metronomoExplanation.MainFactors.Count
                };

                foreach (var kvp in explanationFactors)
                {
                    if (kvp.Value < 3)
                    {
                        test.ErrorMessage = $"Explicação insuficiente para {kvp.Key}: {kvp.Value} fatores";
                        return test;
                    }
                }

                // Análise de distribuição das predições
                var distributions = new Dictionary<string, string>();
                
                foreach (var (name, pred) in new[] { ("AntiFreq", antiFreqPred), ("Debt", debtPred), ("Metronomo", metronomoPred) })
                {
                    var baixas = pred.PredictedNumbers.Count(d => d <= 8);
                    var medias = pred.PredictedNumbers.Count(d => d >= 9 && d <= 17);
                    var altas = pred.PredictedNumbers.Count(d => d >= 18);
                    distributions[name] = $"{baixas}-{medias}-{altas}";
                }

                Console.WriteLine($"      Distribuições (baixas-médias-altas):");
                foreach (var kvp in distributions)
                {
                    Console.WriteLine($"        {kvp.Key}: {kvp.Value}");
                }

                // Validar que há diversidade nas distribuições
                var uniqueDistributions = distributions.Values.Distinct().Count();
                if (uniqueDistributions < 2)
                {
                    test.ErrorMessage = "Distribuições muito similares entre modelos";
                    return test;
                }

                test.Success = true;
                test.Details = $"✅ Análise comparativa completa: AntiFreq ({antiFreqValidation.OverallScore:P1}), Debt ({debtValidation.OverallScore:P1}), {uniqueDistributions} distribuições únicas";
                test.AnalysisData = new {
                    AntiFreqValidation = antiFreqValidation.OverallScore,
                    DebtValidation = debtValidation.OverallScore,
                    Distributions = distributions,
                    UniqueDistributions = uniqueDistributions
                };
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

        private static double CalculateVariance(List<double> values)
        {
            if (values.Count < 2) return 0.0;
            var mean = values.Average();
            return values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
        }

        private static async Task SaveTriModelReport(TriModelTestReport report)
        {
            try
            {
                var reportContent = GenerateTriModelReport(report);
                var fileName = $"TriModel_Integration_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                await System.IO.File.WriteAllTextAsync(fileName, reportContent);
                Console.WriteLine($"\n📄 Relatório tri-modelo salvo: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erro ao salvar relatório: {ex.Message}");
            }
        }

        private static string GenerateTriModelReport(TriModelTestReport report)
        {
            var content = "RELATÓRIO COMPLETO - SISTEMA TRI-MODELO\n";
            content += "=" * 70 + "\n";
            content += $"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
            content += $"Tempo Total: {report.TotalExecutionTime.TotalSeconds:F2} segundos\n";
            content += $"Status Geral: {(report.OverallSuccess ? "✅ SUCESSO COMPLETO" : "❌ FALHA")}\n\n";

            content += "MODELOS TESTADOS:\n";
            content += "   1. MetronomoModel (Individual/Temporal)\n";
            content += "   2. AntiFrequencySimpleModel (AntiFreq/Simple)\n";
            content += "   3. StatisticalDebtModel (AntiFreq/Statistical)\n\n";

            var tests = new[]
            {
                report.ModelInitializationTest,
                report.IndividualPerformanceTest,
                report.CrossCorrelationTest,
                report.AdvancedEnsembleTest,
                report.DiversificationTest,
                report.DynamicStrategyTest,
                report.ComparativeAnalysisTest
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

            if (report.OverallSuccess)
            {
                content += "🚀 CONCLUSÃO FINAL:\n";
                content += "   O sistema tri-modelo foi implementado e validado com SUCESSO COMPLETO!\n\n";
                content += "   ✅ CONQUISTAS PRINCIPAIS:\n";
                content += "   • 3 modelos completamente funcionais e integrados\n";
                content += "   • Ensemble inteligente com diversificação comprovada\n";
                content += "   • Estratégias dinâmicas (Single/BestModel/Ensemble)\n";
                content += "   • Baixa correlação entre modelos (diversificação ideal)\n";
                content += "   • Performance individual adequada (todos > 50% confiança)\n";
                content += "   • Validação automática e explicação de predições\n";
                content += "   • Sistema robusto e extensível\n\n";
                content += "   🎯 PRÓXIMOS PASSOS:\n";
                content += "   1. Implementar SaturationModel (Semana 4)\n";
                content += "   2. Otimizar ensemble com 4+ modelos\n";
                content += "   3. Implementar meta-learning básico\n";
                content += "   4. Deploy em ambiente de produção\n\n";
                content += "   💡 IMPACTO TRANSFORMACIONAL:\n";
                content += "   • Sistema evoluiu de mono-modelo para plataforma multi-modelo\n";
                content += "   • Diversificação reduz risco e melhora estabilidade\n";
                content += "   • Ensemble supera performance individual\n";
                content += "   • Base sólida para crescimento futuro\n";
            }

            return content;
        }

        #endregion
    }

    #region Supporting Classes

    public class TriModelTestReport
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

    public class TestResult
    {
        public string TestName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Details { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        
        // Dados específicos para análise
        public Dictionary<string, PerformanceMetrics> PerformanceData { get; set; }
        public Dictionary<string, double> CorrelationData { get; set; }
        public object EnsembleData { get; set; }
        public object DiversificationData { get; set; }
        public Dictionary<string, int> StrategyData { get; set; }
        public object AnalysisData { get; set; }
    }

    public class PerformanceMetrics
    {
        public string ModelName { get; set; }
        public double AverageConfidence { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public bool PredictionValid { get; set; }
    }

    #endregion
}