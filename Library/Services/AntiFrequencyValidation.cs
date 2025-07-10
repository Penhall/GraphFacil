using System.Linq;
using LotoLibrary.Interfaces;
// D:\PROJETOS\GraphFacil\LotoLibrary\Services\AntiFrequencyValidation.cs - Teste do primeiro modelo
using LotoLibrary.Engines;
using LotoLibrary.Models;
using LotoLibrary.Suporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Enums;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Serviço para validação e teste do AntiFrequencySimpleModel
    /// </summary>
    public class AntiFrequencyValidation
    {
        private readonly PredictionEngine _predictionEngine;
        private readonly ModelFactory _modelFactory;

        public AntiFrequencyValidation()
        {
            _predictionEngine = new PredictionEngine();
            _modelFactory = new ModelFactory();
        }

        #region Validation Methods
        public async Task<ValidationReport> ExecuteFullValidationAsync(Lances historicalData)
        {
            var report = new ValidationReport
            {
                TestName = "Anti-Frequency Simple Model - Validação Completa",
                StartTime = DateTime.Now,
                TestResults = new List<TestResult>()
            };

            try
            {
                Console.WriteLine("🚀 INICIANDO VALIDAÇÃO DO ANTIFREQUENCYSIMPLEMODEL");
                Console.WriteLine("".PadLeft(60, '='));

                // Teste 1: Criação do modelo
                var test1 = await TestModelCreation();
                report.TestResults.Add(test1);

                // Teste 2: Inicialização com dados históricos
                var test2 = await TestModelInitialization(historicalData);
                report.TestResults.Add(test2);

                // Teste 3: Treinamento
                var test3 = await TestModelTraining(historicalData);
                report.TestResults.Add(test3);

                // Teste 4: Geração de predições
                var test4 = await TestPredictionGeneration(historicalData);
                report.TestResults.Add(test4);

                // Teste 5: Validação com dados históricos
                var test5 = await TestHistoricalValidation(historicalData);
                report.TestResults.Add(test5);

                // Teste 6: Configuração de parâmetros
                var test6 = await TestParameterConfiguration();
                report.TestResults.Add(test6);

                // Teste 7: Comparação com MetronomoModel
                var test7 = await TestComparisonWithMetronomo(historicalData);
                report.TestResults.Add(test7);

                report.EndTime = DateTime.Now;
                report.Duration = report.EndTime - report.StartTime;
                report.TotalTests = report.TestResults.Count;
                report.PassedTests = report.TestResults.Count(r => r.Success);
                report.OverallSuccess = report.PassedTests == report.TotalTests;

                await GenerateDetailedReport(report);

                Console.WriteLine("\n✅ VALIDAÇÃO CONCLUÍDA");
                Console.WriteLine($"📊 Resultado: {report.PassedTests}/{report.TotalTests} testes passaram");
                Console.WriteLine($"⏱️ Tempo total: {report.Duration.TotalSeconds:F2}s");

                return report;
            }
            catch (Exception ex)
            {
                report.EndTime = DateTime.Now;
                report.Duration = report.EndTime - report.StartTime;
                report.ErrorMessage = ex.Message;
                report.OverallSuccess = false;

                Console.WriteLine($"❌ ERRO NA VALIDAÇÃO: {ex.Message}");
                return report;
            }
        }

        private async Task<TestResult> TestModelCreation()
        {
            var result = new TestResult
            {
                TestName = "Criação do Modelo via Factory",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n1️⃣ Testando criação do modelo...");

                // Testar criação via factory
                var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple);

                if (model == null)
                {
                    throw new Exception("Factory retornou null");
                }

                if (model.ModelName != "Anti-Frequency Simple")
                {
                    throw new Exception($"Nome do modelo incorreto: {model.ModelName}");
                }

                // Testar se implementa IConfigurableModel
                if (!(model is IConfigurableModel))
                {
                    throw new Exception("Modelo não implementa IConfigurableModel");
                }

                result.Success = true;
                result.Message = "✅ Modelo criado com sucesso";
                Console.WriteLine("   ✅ Modelo criado via factory");
                Console.WriteLine($"   📝 Nome: {model.ModelName}");
                Console.WriteLine($"   🔧 Configurável: {model is IConfigurableModel}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro na criação: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        private async Task<TestResult> TestModelInitialization(Lances historicalData)
        {
            var result = new TestResult
            {
                TestName = "Inicialização do Modelo",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n2️⃣ Testando inicialização...");

                var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple);

                if (model.IsInitialized)
                {
                    throw new Exception("Modelo deveria começar não inicializado");
                }

                var success = await model.InitializeAsync(historicalData);

                if (!success)
                {
                    throw new Exception("Inicialização falhou");
                }

                if (!model.IsInitialized)
                {
                    throw new Exception("Modelo não marcou como inicializado");
                }

                if (model.TrainingDataSize != historicalData.Count)
                {
                    throw new Exception($"Tamanho dos dados incorreto: {model.TrainingDataSize} vs {historicalData.Count}");
                }

                result.Success = true;
                result.Message = "✅ Inicialização bem-sucedida";
                Console.WriteLine("   ✅ Modelo inicializado");
                Console.WriteLine($"   📊 Dados de treino: {model.TrainingDataSize}");
                Console.WriteLine($"   📅 Última atualização: {model.LastTrainingTime}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro na inicialização: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        private async Task<TestResult> TestModelTraining(Lances historicalData)
        {
            var result = new TestResult
            {
                TestName = "Treinamento do Modelo",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n3️⃣ Testando treinamento...");

                var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple);
                var success = await model.TrainAsync(historicalData);

                if (!success)
                {
                    throw new Exception("Treinamento falhou");
                }

                if (model.Confidence <= 0 || model.Confidence > 1)
                {
                    throw new Exception($"Confiança inválida: {model.Confidence}");
                }

                result.Success = true;
                result.Message = "✅ Treinamento concluído";
                Console.WriteLine("   ✅ Modelo treinado");
                Console.WriteLine($"   🎯 Confiança: {model.Confidence:P2}");
                Console.WriteLine($"   📊 Dados utilizados: {model.TrainingDataSize}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro no treinamento: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        private async Task<TestResult> TestPredictionGeneration(Lances historicalData)
        {
            var result = new TestResult
            {
                TestName = "Geração de Predições",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n4️⃣ Testando geração de predições...");

                var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple);
                await model.TrainAsync(historicalData);

                var targetConcurso = historicalData.Count + 1;
                var prediction = await model.PredictAsync(targetConcurso);

                if (!prediction.Success)
                {
                    throw new Exception($"Predição falhou: {prediction.ErrorMessage}");
                }

                if (prediction.PredictedNumbers.Count != 15)
                {
                    throw new Exception($"Número incorreto de predições: {prediction.PredictedNumbers.Count}");
                }

                var invalidNumbers = prediction.PredictedNumbers.Where(n => n < 1 || n > 25).ToList();
                if (invalidNumbers.Any())
                {
                    throw new Exception($"Números inválidos na predição: {string.Join(", ", invalidNumbers)}");
                }

                // Verificar se prioriza dezenas menos frequentes
                var numberFrequencies = CalculateNumberFrequencies(historicalData);
                var selectedFrequencies = prediction.PredictedNumbers.Select(n => numberFrequencies[n]).ToList();
                var averageSelectedFreq = selectedFrequencies.Average();
                var averageOverallFreq = numberFrequencies.Values.Average();

                if (averageSelectedFreq >= averageOverallFreq)
                {
                    Console.WriteLine("   ⚠️ Aviso: Seleção não favorece dezenas menos frequentes");
                }

                result.Success = true;
                result.Message = "✅ Predição gerada com sucesso";
                Console.WriteLine("   ✅ Predição gerada");
                Console.WriteLine($"   🎯 Números: {string.Join(", ", prediction.PredictedNumbers.Take(10))}...");
                Console.WriteLine($"   📊 Freq. média selecionada: {averageSelectedFreq:P2}");
                Console.WriteLine($"   📊 Freq. média geral: {averageOverallFreq:P2}");
                Console.WriteLine($"   🎯 Confiança: {prediction.Confidence:P2}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro na predição: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        private async Task<TestResult> TestHistoricalValidation(Lances historicalData)
        {
            var result = new TestResult
            {
                TestName = "Validação Histórica",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n5️⃣ Testando validação histórica...");

                var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple);
                await model.TrainAsync(historicalData);

                // Usar últimos 20 sorteios para validação
                var validationData = new Lances(historicalData.Skip(historicalData.Count - 20).ToList());
                var validationResult = await model.ValidateAsync(validationData);

                if (!validationResult.Success)
                {
                    throw new Exception($"Validação falhou: {validationResult.ErrorMessage}");
                }

                if (validationResult.Accuracy < 0 || validationResult.Accuracy > 1)
                {
                    throw new Exception($"Accuracy inválida: {validationResult.Accuracy}");
                }

                // Critério de sucesso: accuracy >= 50% (melhor que aleatório)
                var isAcceptable = validationResult.Accuracy >= 0.5;

                result.Success = true;
                result.Message = $"✅ Validação concluída - Accuracy: {validationResult.Accuracy:P2}";
                result.AdditionalData = new Dictionary<string, object>
                {
                    ["Accuracy"] = validationResult.Accuracy,
                    ["TotalTests"] = validationResult.TotalTests,
                    ["SuccessfulPredictions"] = validationResult.SuccessfulPredictions,
                    ["IsAcceptable"] = isAcceptable
                };

                Console.WriteLine("   ✅ Validação histórica concluída");
                Console.WriteLine($"   📊 Accuracy: {validationResult.Accuracy:P2}");
                Console.WriteLine($"   🎯 Testes: {validationResult.SuccessfulPredictions}/{validationResult.TotalTests}");
                Console.WriteLine($"   {(isAcceptable ? "✅" : "⚠️")} Critério: {(isAcceptable ? "PASSOU" : "ATENÇÃO")}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro na validação: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        private async Task<TestResult> TestParameterConfiguration()
        {
            var result = new TestResult
            {
                TestName = "Configuração de Parâmetros",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n6️⃣ Testando configuração de parâmetros...");

                var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple) as IConfigurableModel;

                if (model == null)
                {
                    throw new Exception("Modelo não implementa IConfigurableModel");
                }

                var defaultParams = model.GetDefaultParameters();
                if (defaultParams == null || defaultParams.Count == 0)
                {
                    throw new Exception("Parâmetros padrão não encontrados");
                }

                // Testar parâmetros válidos
                var validParams = new Dictionary<string, object>
                {
                    ["JanelaHistorica"] = 50,
                    ["FatorDecaimento"] = 0.05,
                    ["Epsilon"] = 0.01,
                    ["PesoTemporal"] = 0.9
                };

                if (!model.ValidateParameters(validParams))
                {
                    throw new Exception("Parâmetros válidos foram rejeitados");
                }

                model.UpdateParameters(validParams);

                // Testar parâmetros inválidos
                var invalidParams = new Dictionary<string, object>
                {
                    ["JanelaHistorica"] = -1,
                    ["FatorDecaimento"] = 2.0,
                    ["Epsilon"] = 0.0,
                    ["PesoTemporal"] = 1.5
                };

                if (model.ValidateParameters(invalidParams))
                {
                    throw new Exception("Parâmetros inválidos foram aceitos");
                }

                result.Success = true;
                result.Message = "✅ Configuração de parâmetros funcionando";
                Console.WriteLine("   ✅ Parâmetros configurados");
                Console.WriteLine($"   🔧 Parâmetros padrão: {defaultParams.Count}");
                Console.WriteLine("   ✅ Validação funcionando");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro na configuração: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }

        private async Task<TestResult> TestComparisonWithMetronomo(Lances historicalData)
        {
            var result = new TestResult
            {
                TestName = "Comparação com MetronomoModel",
                StartTime = DateTime.Now
            };

            try
            {
                Console.WriteLine("\n7️⃣ Testando comparação com MetronomoModel...");

                // Criar ambos os modelos
                var antiFreqModel = _modelFactory.CreateModel(ModelType.AntiFrequencySimple);
                var metronomoModel = _modelFactory.CreateModel(ModelType.Metronomo);

                // Treinar ambos
                await antiFreqModel.TrainAsync(historicalData);
                await metronomoModel.TrainAsync(historicalData);

                // Gerar predições
                var targetConcurso = historicalData.Count + 1;
                var antiFreqPrediction = await antiFreqModel.PredictAsync(targetConcurso);
                var metronomoPrediction = await metronomoModel.PredictAsync(targetConcurso);

                if (!antiFreqPrediction.Success || !metronomoPrediction.Success)
                {
                    throw new Exception("Falha na geração de predições");
                }

                // Calcular diversificação (números diferentes)
                var commonNumbers = antiFreqPrediction.PredictedNumbers.Intersect(metronomoPrediction.PredictedNumbers).Count();
                var diversificationRate = 1.0 - (double)commonNumbers / 15;

                // Verificar se o anti-frequency realmente favorece dezenas menos frequentes
                var frequencies = CalculateNumberFrequencies(historicalData);
                var antiFreqAvgFreq = antiFreqPrediction.PredictedNumbers.Average(n => frequencies[n]);
                var metronomoAvgFreq = metronomoPrediction.PredictedNumbers.Average(n => frequencies[n]);

                var favorsLessFrequent = antiFreqAvgFreq < metronomoAvgFreq;

                result.Success = true;
                result.Message = $"✅ Comparação concluída - Diversificação: {diversificationRate:P2}";
                result.AdditionalData = new Dictionary<string, object>
                {
                    ["DiversificationRate"] = diversificationRate,
                    ["AntiFreqAvgFreq"] = antiFreqAvgFreq,
                    ["MetronomoAvgFreq"] = metronomoAvgFreq,
                    ["FavorsLessFrequent"] = favorsLessFrequent,
                    ["CommonNumbers"] = commonNumbers
                };

                Console.WriteLine("   ✅ Comparação concluída");
                Console.WriteLine($"   🎯 Diversificação: {diversificationRate:P2}");
                Console.WriteLine($"   📊 Anti-Freq freq. média: {antiFreqAvgFreq:P2}");
                Console.WriteLine($"   📊 Metronomo freq. média: {metronomoAvgFreq:P2}");
                Console.WriteLine($"   {(favorsLessFrequent ? "✅" : "⚠️")} Favorece menos frequentes: {favorsLessFrequent}");
                Console.WriteLine($"   🔄 Números em comum: {commonNumbers}/15");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"❌ Erro na comparação: {ex.Message}";
                Console.WriteLine($"   ❌ {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            result.Duration = result.EndTime - result.StartTime;
            return result;
        }
        #endregion

        #region Helper Methods
        private Dictionary<int, double> CalculateNumberFrequencies(Lances historicalData)
        {
            var frequencies = new Dictionary<int, double>();
            var totalDraws = historicalData.Count;

            for (int n = 1; n <= 25; n++)
            {
                var appearances = historicalData.Count(lance => lance.Lista.Any(x => x == n));
                frequencies[n] = (double)appearances / totalDraws;
            }

            return frequencies;
        }

        private async Task GenerateDetailedReport(ValidationReport report)
        {
            await Task.Run(() =>
            {
                Console.WriteLine("\n" + "".PadLeft(60, '='));
                Console.WriteLine("📋 RELATÓRIO DETALHADO");
                Console.WriteLine("".PadLeft(60, '='));
                Console.WriteLine($"🎯 Teste: {report.TestName}");
                Console.WriteLine($"⏱️ Duração: {report.Duration.TotalSeconds:F2}s");
                Console.WriteLine($"📊 Resultado: {report.PassedTests}/{report.TotalTests} testes passaram");
                Console.WriteLine($"✅ Sucesso geral: {(report.OverallSuccess ? "SIM" : "NÃO")}");

                if (!string.IsNullOrEmpty(report.ErrorMessage))
                {
                    Console.WriteLine($"❌ Erro: {report.ErrorMessage}");
                }

                Console.WriteLine("\n📋 Detalhes por teste:");
                foreach (var test in report.TestResults)
                {
                    Console.WriteLine($"   {(test.Success ? "✅" : "❌")} {test.TestName}");
                    Console.WriteLine($"      📝 {test.Message}");
                    Console.WriteLine($"      ⏱️ {test.Duration.TotalMilliseconds:F0}ms");
                }

                Console.WriteLine("\n" + "".PadLeft(60, '='));
            });
        }
        #endregion
    }

}
