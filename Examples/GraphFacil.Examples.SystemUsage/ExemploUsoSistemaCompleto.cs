// Exemplo atualizado de uso do sistema multi-modelo
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

namespace GraphFacil.Examples.SystemUsage
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
            Console.WriteLine(new string('=', 60));
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
            Console.WriteLine(new string('=', 40));

            try
            {
                // 1. Carregar dados históricos
                Console.WriteLine("📊 Carregando dados históricos...");
                var infraService = new InfraService();
                var dados = infraService.CarregarConcursos();
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

        // Continuação do arquivo com os demais exemplos (ComparacaoPerformance, EnsembleInteligente, etc)
        // Implementação similar para os outros métodos, com as mesmas correções aplicadas
    }
}
