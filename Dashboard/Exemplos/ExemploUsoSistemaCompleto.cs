// D:\PROJETOS\GraphFacil\Dashboard\Exemplos\ExemploUsoSistemaCompleto.cs - Demonstra uso do sistema multi-modelo
using LotoLibrary.Engines;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using LotoLibrary.Services;
using LotoLibrary.Services.Analysis;
using LotoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Exemplos
{
    /// <summary>
    /// Exemplo completo demonstrando como usar o sistema multi-modelo
    /// com Metrônomo + AntiFrequência + Ensemble + Análise de Correlação
    /// </summary>
    public static class ExemploUsoSistemaCompleto
    {
        public static async Task ExecutarExemploCompletoAsync()
        {
            Console.WriteLine("🎯 EXEMPLO COMPLETO - SISTEMA MULTI-MODELO LOTOFÁCIL");
            Console.WriteLine("=" * 60);
            Console.WriteLine();

            await ExemploBasicoMultiModelo();
            await ExemploComparacaoPerformance();
            await ExemploEnsembleInteligente();
            await ExemploAnaliseCorrelacao();
            await ExemploConfiguracao();
            await ExemploValidacaoCompleta();

            Console.WriteLine("🎊 EXEMPLO COMPLETO FINALIZADO!");
            Console.WriteLine("💡 Agora você pode adaptar estes exemplos para sua aplicação.");
        }

        #region Exemplo 1: Uso Básico Multi-Modelo
        
        /// <summary>
        /// Demonstra o uso básico com múltiplos modelos
        /// </summary>
        public static async Task ExemploBasicoMultiModelo()
        {
            Console.WriteLine("1️⃣ EXEMPLO BÁSICO - MÚLTIPLOS MODELOS");
            Console.WriteLine("=" * 40);

            try
            {
                // 1. Carregar dados históricos
                Console.WriteLine("📊 Carregando dados históricos...");
                var dados = Infra.CarregarConcursos();
                Console.WriteLine($"   ✅ {dados.Count} concursos carregados");

                // 2. Inicializar PredictionEngine
                Console.WriteLine("\n🚀 Inicializando PredictionEngine...");
                var engine = new PredictionEngine();
                var initResult = await engine.InitializeAsync(dados);
                Console.WriteLine($"   ✅ Engine inicializado: {engine.TotalModels} modelo(s)");

                // 3. Criar e registrar modelo anti-frequencista
                Console.WriteLine("\n🔄 Criando modelo anti-frequencista...");
                var antiFreqModel = new AntiFrequencySimpleModel();
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                var registrationResult = await engine.RegisterModelAsync("AntiFreqSimple", antiFreqModel);
                Console.WriteLine($"   ✅ AntiFrequencySimpleModel registrado: {engine.TotalModels} modelos totais");

                // 4. Gerar predições com diferentes estratégias
                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;

                Console.WriteLine($"\n🎯 Gerando predições para concurso {targetConcurso}:");

                // Predição com Metrônomo
                engine.SetActiveStrategy("Single");
                var metronomoPred = await engine.GeneratePredictionAsync(targetConcurso);
                Console.WriteLine($"   🎵 Metrônomo: [{string.Join(", ", metronomoPred.PredictedNumbers.Select(n => n.ToString("D2")))}]");
                Console.WriteLine($"      Confiança: {metronomoPred.OverallConfidence:P2}");

                // Predição com AntiFrequência
                engine.ClearCache();
                var antiFreqPred = await antiFreqModel.PredictAsync(targetConcurso);
                Console.WriteLine($"   🔄 AntiFreq: [{string.Join(", ", antiFreqPred.PredictedNumbers.Select(n => n.ToString("D2")))}]");
                Console.WriteLine($"      Confiança: {antiFreqPred.OverallConfidence:P2}");

                // Predição com Ensemble
                engine.SetActiveStrategy("Ensemble");
                var ensemblePred = await engine.GeneratePredictionAsync(targetConcurso);
                Console.WriteLine($"   🎭 Ensemble: [{string.Join(", ", ensemblePred.PredictedNumbers.Select(n => n.ToString("D2")))}]");
                Console.WriteLine($"      Confiança: {ensemblePred.OverallConfidence:P2}");

                // 5. Comparar intersecções
                var metronomoAntiFreq = metronomoPred.PredictedNumbers.Intersect(antiFreqPred.PredictedNumbers).Count();
                var ensembleMetronomo = ensemblePred.PredictedNumbers.Intersect(metronomoPred.PredictedNumbers).Count();
                var ensembleAntiFreq = ensemblePred.PredictedNumbers.Intersect(antiFreqPred.PredictedNumbers).Count();

                Console.WriteLine($"\n📊 Análise de intersecções:");
                Console.WriteLine($"   Metrônomo ∩ AntiFreq: {metronomoAntiFreq}/15 dezenas comuns");
                Console.WriteLine($"   Ensemble ∩ Metrônomo: {ensembleMetronomo}/15 dezenas comuns");
                Console.WriteLine($"   Ensemble ∩ AntiFreq: {ensembleAntiFreq}/15 dezenas comuns");

                Console.WriteLine("\n✅ Exemplo básico concluído!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no exemplo básico: {ex.Message}");
            }
        }

        #endregion

        #region Exemplo 2: Comparação de Performance

        /// <summary>
        /// Demonstra como comparar performance entre modelos
        /// </summary>
        public static async Task ExemploComparacaoPerformance()
        {
            Console.WriteLine("2️⃣ EXEMPLO - COMPARAÇÃO DE PERFORMANCE");
            Console.WriteLine("=" * 40);

            try
            {
                var dados = Infra.CarregarConcursos();
                var comparer = new PerformanceComparer();

                // Criar modelos
                var metronomoModel = new MetronomoModel();
                var antiFreqModel = new AntiFrequencySimpleModel();

                await metronomoModel.InitializeAsync(dados);
                await metronomoModel.TrainAsync(dados);
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                // Gerar predições para comparação
                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                Console.WriteLine($"📈 Gerando 10 predições para análise...");

                for (int i = 0; i < 10; i++)
                {
                    var target = targetBase + i;
                    var metronomoPred = await metronomoModel.PredictAsync(target);
                    var antiFreqPred = await antiFreqModel.PredictAsync(target);

                    comparer.AddPredictionResult("Metronomo", metronomoPred);
                    comparer.AddPredictionResult("AntiFreqSimple", antiFreqPred);
                }

                // Comparar modelos
                var comparison = await comparer.CompareModelsAsync("Metronomo", "AntiFreqSimple");

                Console.WriteLine($"\n🔍 ANÁLISE COMPARATIVA:");
                Console.WriteLine($"📊 Modelo 1 (Metrônomo):");
                Console.WriteLine($"   - Confiança média: {comparison.Model1Metrics.AverageConfidence:P2}");
                Console.WriteLine($"   - Estabilidade: {comparison.Model1Metrics.ConfidenceStability:P2}");
                Console.WriteLine($"   - Consistência: {comparison.Model1Metrics.PredictionConsistency:P2}");

                Console.WriteLine($"\n📊 Modelo 2 (AntiFrequência):");
                Console.WriteLine($"   - Confiança média: {comparison.Model2Metrics.AverageConfidence:P2}");
                Console.WriteLine($"   - Estabilidade: {comparison.Model2Metrics.ConfidenceStability:P2}");
                Console.WriteLine($"   - Consistência: {comparison.Model2Metrics.PredictionConsistency:P2}");

                Console.WriteLine($"\n🔗 CORRELAÇÃO E DIVERSIFICAÇÃO:");
                Console.WriteLine($"   - Correlação: {comparison.Correlation:F3}");
                Console.WriteLine($"   - Score de diversificação: {comparison.DiversificationScore:P2}");
                Console.WriteLine($"   - Peso recomendado Metrônomo: {comparison.RecommendedWeight1:P1}");
                Console.WriteLine($"   - Peso recomendado AntiFreq: {comparison.RecommendedWeight2:P1}");

                // Interpretação
                if (Math.Abs(comparison.Correlation) < 0.5)
                {
                    Console.WriteLine($"   💡 EXCELENTE diversificação - modelos complementares!");
                }
                else if (Math.Abs(comparison.Correlation) < 0.7)
                {
                    Console.WriteLine($"   👍 BOA diversificação - útil para ensemble");
                }
                else
                {
                    Console.WriteLine($"   ⚠️ Alta correlação - modelos similares");
                }

                Console.WriteLine("\n✅ Análise de performance concluída!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na comparação: {ex.Message}");
            }
        }

        #endregion

        #region Exemplo 3: Ensemble Inteligente

        /// <summary>
        /// Demonstra ensemble com otimização de pesos
        /// </summary>
        public static async Task ExemploEnsembleInteligente()
        {
            Console.WriteLine("3️⃣ EXEMPLO - ENSEMBLE INTELIGENTE");
            Console.WriteLine("=" * 40);

            try
            {
                var dados = Infra.CarregarConcursos();
                var engine = new PredictionEngine();
                var comparer = new PerformanceComparer();

                // Inicializar sistema
                await engine.InitializeAsync(dados);

                var antiFreqModel = new AntiFrequencySimpleModel();
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);
                await engine.RegisterModelAsync("AntiFreqSimple", antiFreqModel);

                Console.WriteLine($"🎭 Sistema ensemble inicializado com {engine.TotalModels} modelos");

                // Gerar dados para otimização
                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                for (int i = 0; i < 15; i++)
                {
                    var target = targetBase + i;
                    
                    // Predições individuais
                    var metronomoModel = engine.GetModel("Metronomo");
                    var metronomoPred = await metronomoModel.PredictAsync(target);
                    var antiFreqPred = await antiFreqModel.PredictAsync(target);

                    comparer.AddPredictionResult("Metronomo", metronomoPred);
                    comparer.AddPredictionResult("AntiFreqSimple", antiFreqPred);
                }

                // Calcular pesos ótimos
                var optimalWeights = await comparer.CalculateEnsembleWeightsAsync(new[] { "Metronomo", "AntiFreqSimple" }.ToList());

                Console.WriteLine($"\n⚖️ PESOS ÓTIMOS CALCULADOS:");
                foreach (var weight in optimalWeights)
                {
                    Console.WriteLine($"   {weight.Key}: {weight.Value:P1}");
                }

                // Testar ensemble com diferentes estratégias
                Console.WriteLine($"\n🎯 Testando estratégias de ensemble:");

                // Ensemble padrão (votação simples)
                engine.SetActiveStrategy("Ensemble");
                var ensembleDefault = await engine.GeneratePredictionAsync(targetBase + 20);
                Console.WriteLine($"   📊 Ensemble Padrão: [{string.Join(", ", ensembleDefault.PredictedNumbers.Take(8).Select(n => n.ToString("D2")))}...]");
                Console.WriteLine($"      Confiança: {ensembleDefault.OverallConfidence:P2}");

                // Melhor modelo apenas
                engine.SetActiveStrategy("BestModel");
                var bestModel = await engine.GeneratePredictionAsync(targetBase + 21);
                Console.WriteLine($"   🏆 Melhor Modelo: [{string.Join(", ", bestModel.PredictedNumbers.Take(8).Select(n => n.ToString("D2")))}...]");
                Console.WriteLine($"      Confiança: {bestModel.OverallConfidence:P2}");

                // Análise de diversificação
                var intersection = ensembleDefault.PredictedNumbers.Intersect(bestModel.PredictedNumbers).Count();
                Console.WriteLine($"\n📈 Análise de diversificação:");
                Console.WriteLine($"   Ensemble ∩ BestModel: {intersection}/15 dezenas comuns");
                
                if (intersection < 10)
                {
                    Console.WriteLine($"   💡 BOA diversificação entre estratégias");
                }
                else
                {
                    Console.WriteLine($"   ⚠️ Estratégias convergindo - considerar mais modelos");
                }

                Console.WriteLine("\n✅ Ensemble inteligente demonstrado!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no ensemble: {ex.Message}");
            }
        }

        #endregion

        #region Exemplo 4: Análise de Correlação

        /// <summary>
        /// Demonstra análise avançada de correlação
        /// </summary>
        public static async Task ExemploAnaliseCorrelacao()
        {
            Console.WriteLine("4️⃣ EXEMPLO - ANÁLISE DE CORRELAÇÃO");
            Console.WriteLine("=" * 40);

            try
            {
                var dados = Infra.CarregarConcursos();
                var comparer = new PerformanceComparer();

                // Preparar modelos
                var metronomoModel = new MetronomoModel();
                var antiFreqModel = new AntiFrequencySimpleModel();

                await metronomoModel.InitializeAsync(dados);
                await metronomoModel.TrainAsync(dados);
                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                // Configurar AntiFreq com diferentes parâmetros para simular múltiplos modelos
                var antiFreqConservative = new AntiFrequencySimpleModel();
                await antiFreqConservative.InitializeAsync(dados);
                await antiFreqConservative.TrainAsync(dados);
                antiFreqConservative.SetParameterValue("InversionFactor", 0.3); // Mais conservador

                var antiFreqAggressive = new AntiFrequencySimpleModel();
                await antiFreqAggressive.InitializeAsync(dados);
                await antiFreqAggressive.TrainAsync(dados);
                antiFreqAggressive.SetParameterValue("InversionFactor", 0.9); // Mais agressivo

                Console.WriteLine($"🔍 Gerando predições para análise de correlação...");

                // Gerar predições
                var targetBase = (dados.LastOrDefault()?.Id ?? 0) + 1;
                for (int i = 0; i < 20; i++)
                {
                    var target = targetBase + i;

                    var pred1 = await metronomoModel.PredictAsync(target);
                    var pred2 = await antiFreqModel.PredictAsync(target);
                    var pred3 = await antiFreqConservative.PredictAsync(target);
                    var pred4 = await antiFreqAggressive.PredictAsync(target);

                    comparer.AddPredictionResult("Metronomo", pred1);
                    comparer.AddPredictionResult("AntiFreqNormal", pred2);
                    comparer.AddPredictionResult("AntiFreqConservative", pred3);
                    comparer.AddPredictionResult("AntiFreqAggressive", pred4);
                }

                // Analisar todas as correlações
                var correlationMatrix = await comparer.AnalyzeAllCorrelationsAsync();

                Console.WriteLine($"\n📊 MATRIZ DE CORRELAÇÃO:");
                var modelPairs = correlationMatrix.GetAllPairs();
                foreach (var pair in modelPairs.OrderBy(p => Math.Abs(p.Correlation)))
                {
                    var correlationLevel = GetCorrelationLevel(Math.Abs(pair.Correlation));
                    Console.WriteLine($"   {pair.Model1} ↔ {pair.Model2}: {pair.Correlation:F3} ({correlationLevel})");
                }

                // Encontrar pares com baixa correlação
                var lowCorrelationPairs = await comparer.FindLowCorrelationPairsAsync(0.6);
                Console.WriteLine($"\n🎯 PARES IDEAIS PARA ENSEMBLE (correlação < 0.6):");
                foreach (var pair in lowCorrelationPairs.Take(3))
                {
                    Console.WriteLine($"   👍 {pair.Model1} + {pair.Model2}: correlação {pair.Correlation:F3}");
                }

                // Gerar relatório abrangente
                var comprehensiveReport = await comparer.GenerateComprehensiveReportAsync();
                Console.WriteLine($"\n📋 RELATÓRIO EXECUTIVO:");
                Console.WriteLine($"   - Modelos analisados: {comprehensiveReport.ModelsAnalyzed.Count}");
                Console.WriteLine($"   - Pares de baixa correlação: {comprehensiveReport.LowCorrelationPairs.Count}");
                Console.WriteLine($"   - Ensemble recomendado: [{string.Join(", ", comprehensiveReport.RecommendedEnsemble)}]");

                Console.WriteLine("\n✅ Análise de correlação concluída!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na análise: {ex.Message}");
            }
        }

        #endregion

        #region Exemplo 5: Configuração Avançada

        /// <summary>
        /// Demonstra configuração avançada de parâmetros
        /// </summary>
        public static async Task ExemploConfiguracao()
        {
            Console.WriteLine("5️⃣ EXEMPLO - CONFIGURAÇÃO AVANÇADA");
            Console.WriteLine("=" * 40);

            try
            {
                var dados = Infra.CarregarConcursos();
                var antiFreqModel = new AntiFrequencySimpleModel();

                await antiFreqModel.InitializeAsync(dados);
                await antiFreqModel.TrainAsync(dados);

                Console.WriteLine($"⚙️ Configurações disponíveis do AntiFrequencySimpleModel:");

                // Listar todos os parâmetros
                var allParams = antiFreqModel.GetAllParameters();
                foreach (var param in allParams)
                {
                    Console.WriteLine($"   📋 {param.Key}: {param.Value} ({param.Value.GetType().Name})");
                }

                Console.WriteLine($"\n🔧 Testando diferentes configurações:");

                var targetConcurso = (dados.LastOrDefault()?.Id ?? 0) + 1;

                // Configuração 1: Conservadora
                Console.WriteLine($"\n   🟢 CONFIGURAÇÃO CONSERVADORA:");
                antiFreqModel.SetParameterValue("InversionFactor", 0.3);
                antiFreqModel.SetParameterValue("MinimumThreshold", 0.1);
                var predConservative = await antiFreqModel.PredictAsync(targetConcurso);
                
                Console.WriteLine($"      Predição: [{string.Join(", ", predConservative.PredictedNumbers.Take(8).Select(n => n.ToString("D2")))}...]");
                Console.WriteLine($"      Força inversão: {antiFreqModel.CurrentInversionStrength:P1}");
                Console.WriteLine($"      Sub-representadas: {antiFreqModel.UnderRepresentedCount}");

                // Configuração 2: Agressiva
                Console.WriteLine($"\n   🔴 CONFIGURAÇÃO AGRESSIVA:");
                antiFreqModel.SetParameterValue("InversionFactor", 0.9);
                antiFreqModel.SetParameterValue("MinimumThreshold", 0.02);
                var predAggressive = await antiFreqModel.PredictAsync(targetConcurso + 1);
                
                Console.WriteLine($"      Predição: [{string.Join(", ", predAggressive.PredictedNumbers.Take(8).Select(n => n.ToString("D2")))}...]");
                Console.WriteLine($"      Força inversão: {antiFreqModel.CurrentInversionStrength:P1}");
                Console.WriteLine($"      Sub-representadas: {antiFreqModel.UnderRepresentedCount}");

                // Configuração 3: Balanceada
                Console.WriteLine($"\n   🟡 CONFIGURAÇÃO BALANCEADA:");
                antiFreqModel.SetParameterValue("InversionFactor", 0.6);
                antiFreqModel.SetParameterValue("MinimumThreshold", 0.05);
                antiFreqModel.SetParameterValue("DiversificationWeight", 0.2);
                var predBalanced = await antiFreqModel.PredictAsync(targetConcurso + 2);
                
                Console.WriteLine($"      Predição: [{string.Join(", ", predBalanced.PredictedNumbers.Take(8).Select(n => n.ToString("D2")))}...]");
                Console.WriteLine($"      Força inversão: {antiFreqModel.CurrentInversionStrength:P1}");
                Console.WriteLine($"      Score diversificação: {antiFreqModel.DiversificationScore:P1}");

                // Analisar diferenças
                var intersectionCA = predConservative.PredictedNumbers.Intersect(predAggressive.PredictedNumbers).Count();
                var intersectionCB = predConservative.PredictedNumbers.Intersect(predBalanced.PredictedNumbers).Count();
                var intersectionAB = predAggressive.PredictedNumbers.Intersect(predBalanced.PredictedNumbers).Count();

                Console.WriteLine($"\n📊 Impacto das configurações:");
                Console.WriteLine($"   Conservadora ∩ Agressiva: {intersectionCA}/15 ({(15-intersectionCA)} dezenas diferentes)");
                Console.WriteLine($"   Conservadora ∩ Balanceada: {intersectionCB}/15 ({(15-intersectionCB)} dezenas diferentes)");
                Console.WriteLine($"   Agressiva ∩ Balanceada: {intersectionAB}/15 ({(15-intersectionAB)} dezenas diferentes)");

                Console.WriteLine("\n✅ Configuração avançada demonstrada!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na configuração: {ex.Message}");
            }
        }

        #endregion

        #region Exemplo 6: Validação Completa

        /// <summary>
        /// Demonstra sistema completo de validação
        /// </summary>
        public static async Task ExemploValidacaoCompleta()
        {
            Console.WriteLine("6️⃣ EXEMPLO - VALIDAÇÃO COMPLETA");
            Console.WriteLine("=" * 40);

            try
            {
                Console.WriteLine($"🧪 Executando validação completa da Fase 1...");
                var phase1Report = await Phase1CompletionValidator.ExecuteCompleteValidationAsync();

                Console.WriteLine($"\n📊 RESULTADOS DA VALIDAÇÃO FASE 1:");
                Console.WriteLine($"   Status Geral: {(phase1Report.OverallSuccess ? "✅ PASSOU" : "❌ FALHOU")}");
                Console.WriteLine($"   Tempo Execução: {phase1Report.TotalExecutionTime.TotalSeconds:F2}s");

                if (phase1Report.OverallSuccess)
                {
                    Console.WriteLine($"\n🚀 Sistema validado - executando teste de integração...");
                    var integrationReport = await AntiFreqIntegrationTester.ExecuteCompleteIntegrationTestAsync();

                    Console.WriteLine($"\n📊 RESULTADOS DA INTEGRAÇÃO:");
                    Console.WriteLine($"   Status Geral: {(integrationReport.OverallSuccess ? "✅ PASSOU" : "❌ FALHOU")}");
                    Console.WriteLine($"   Tempo Execução: {integrationReport.TotalExecutionTime.TotalSeconds:F2}s");

                    if (integrationReport.OverallSuccess)
                    {
                        Console.WriteLine($"\n🎯 SISTEMA COMPLETAMENTE VALIDADO!");
                        Console.WriteLine($"   ✅ Fase 1 completamente funcional");
                        Console.WriteLine($"   ✅ AntiFrequencySimpleModel integrado");
                        Console.WriteLine($"   ✅ Multi-modelo operacional");
                        Console.WriteLine($"   ✅ Ensemble funcionando");
                        Console.WriteLine($"   ✅ Análise de correlação ativa");
                        Console.WriteLine($"   ✅ Sistema pronto para próximos modelos");

                        // Demonstrar status final
                        await DemonstrarStatusFinal();
                    }
                }

                Console.WriteLine("\n✅ Validação completa finalizada!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro na validação: {ex.Message}");
            }
        }

        private static async Task DemonstrarStatusFinal()
        {
            var dados = Infra.CarregarConcursos();
            var engine = new PredictionEngine();
            await engine.InitializeAsync(dados);

            var antiFreqModel = new AntiFrequencySimpleModel();
            await antiFreqModel.InitializeAsync(dados);
            await antiFreqModel.TrainAsync(dados);
            await engine.RegisterModelAsync("AntiFreqSimple", antiFreqModel);

            Console.WriteLine($"\n🎭 STATUS FINAL DO SISTEMA:");
            Console.WriteLine(engine.GetSystemStatus());
        }

        #endregion

        #region Helper Methods

        private static string GetCorrelationLevel(double correlation)
        {
            if (correlation < 0.3) return "Baixa";
            if (correlation < 0.6) return "Moderada";
            if (correlation < 0.8) return "Alta";
            return "Muito Alta";
        }

        #endregion
    }
}