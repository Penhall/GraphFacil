// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ValidationResult.cs - REESCRITO para nova arquitetura
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Resultado detalhado de validação de um modelo - REESCRITO na nova arquitetura modular
    /// </summary>
    public class ValidationResult
    {
        #region Core Properties
        /// <summary>
        /// Nome do modelo validado
        /// </summary>
        public string ModelName { get; set; } = "";

        /// <summary>
        /// Indica se a validação foi executada com sucesso
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Mensagem de erro caso Success = false
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Momento de início dos testes
        /// </summary>
        public DateTime TestStartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Momento de fim dos testes
        /// </summary>
        public DateTime TestEndTime { get; set; } = DateTime.Now;
        #endregion

        #region Validation Metrics - PROPRIEDADES SETÁVEIS
        /// <summary>
        /// Número de predições que acertaram 15 dezenas
        /// </summary>
        public int SuccessfulPredictions { get; set; } = 0;

        /// <summary>
        /// Total de testes executados
        /// </summary>
        public int TotalTests { get; set; } = 0;

        /// <summary>
        /// Precisão do modelo (0.0 a 1.0) - PROPRIEDADE CALCULADA READONLY
        /// </summary>
        public double Accuracy => TotalTests > 0 ? (double)SuccessfulPredictions / TotalTests : 0.0;

        /// <summary>
        /// Resultados detalhados de cada teste
        /// </summary>
        public List<ValidationDetail> DetailedResults { get; set; } = new List<ValidationDetail>();
        #endregion

        #region Extended Metrics
        /// <summary>
        /// Distribuição de acertos (chave: número de acertos, valor: quantidade de vezes)
        /// </summary>
        public Dictionary<int, int> HitDistribution { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Média de acertos por predição
        /// </summary>
        public double AverageHits => DetailedResults.Count > 0 ? DetailedResults.Average(d => d.Hits) : 0.0;

        /// <summary>
        /// Confiança média das predições
        /// </summary>
        public double AverageConfidence => DetailedResults.Count > 0 ? DetailedResults.Average(d => d.Confidence) : 0.0;

        /// <summary>
        /// Melhor resultado (mais acertos)
        /// </summary>
        public int BestResult => DetailedResults.Count > 0 ? DetailedResults.Max(d => d.Hits) : 0;

        /// <summary>
        /// Pior resultado (menos acertos)
        /// </summary>
        public int WorstResult => DetailedResults.Count > 0 ? DetailedResults.Min(d => d.Hits) : 0;
        #endregion

        #region Constructors
        public ValidationResult()
        {
            // Construtor padrão
        }

        public ValidationResult(string modelName)
        {
            ModelName = modelName;
            TestStartTime = DateTime.Now;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria um resultado de validação bem-sucedida
        /// </summary>
        public static ValidationResult CreateSuccess(string modelName, int successfulPredictions, int totalTests)
        {
            var result = new ValidationResult(modelName)
            {
                Success = true,
                SuccessfulPredictions = successfulPredictions,
                TotalTests = totalTests,
                TestEndTime = DateTime.Now
            };

            return result;
        }

        /// <summary>
        /// Cria um resultado de erro na validação
        /// </summary>
        public static ValidationResult CreateError(string modelName, string errorMessage)
        {
            return new ValidationResult(modelName)
            {
                Success = false,
                ErrorMessage = errorMessage,
                TestEndTime = DateTime.Now
            };
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Adiciona um resultado de teste detalhado
        /// </summary>
        public void AddDetailedResult(ValidationDetail detail)
        {
            DetailedResults.Add(detail);

            // Atualizar métricas
            if (detail.IsSuccess)
            {
                SuccessfulPredictions++;
            }
            TotalTests++;

            // Atualizar distribuição de acertos
            if (HitDistribution.ContainsKey(detail.Hits))
            {
                HitDistribution[detail.Hits]++;
            }
            else
            {
                HitDistribution[detail.Hits] = 1;
            }
        }

        /// <summary>
        /// Duração total da validação
        /// </summary>
        public TimeSpan Duration => TestEndTime - TestStartTime;

        /// <summary>
        /// Obtém a taxa de acerto como porcentagem
        /// </summary>
        public double AccuracyPercentage => Accuracy * 100.0;

        /// <summary>
        /// Finaliza a validação (define TestEndTime)
        /// </summary>
        public void FinishValidation()
        {
            TestEndTime = DateTime.Now;
        }

        /// <summary>
        /// Gera relatório detalhado da validação
        /// </summary>
        public string GenerateReport()
        {
            var report = $"=== RELATÓRIO DE VALIDAÇÃO - {ModelName} ===\n\n";
            report += $"Status: {(Success ? "✅ SUCESSO" : "❌ FALHOU")}\n";
            if (!Success)
            {
                report += $"Erro: {ErrorMessage}\n";
                return report;
            }

            report += $"Período: {TestStartTime:dd/MM/yyyy HH:mm} - {TestEndTime:dd/MM/yyyy HH:mm}\n";
            report += $"Duração: {Duration.TotalMinutes:F1} minutos\n\n";

            report += "MÉTRICAS PRINCIPAIS:\n";
            report += $"• Total de testes: {TotalTests}\n";
            report += $"• Acertos totais (15/15): {SuccessfulPredictions}\n";
            report += $"• Taxa de acerto: {AccuracyPercentage:F2}%\n";
            report += $"• Média de acertos: {AverageHits:F2}\n";
            report += $"• Confiança média: {AverageConfidence:P2}\n\n";

            report += "DISTRIBUIÇÃO DE ACERTOS:\n";
            foreach (var dist in HitDistribution.OrderByDescending(kv => kv.Key))
            {
                var percentage = (dist.Value * 100.0) / TotalTests;
                report += $"• {dist.Key} acertos: {dist.Value} vezes ({percentage:F1}%)\n";
            }

            return report;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            if (!Success)
                return $"Validação FALHOU: {ErrorMessage}";

            return $"{ModelName}: {SuccessfulPredictions}/{TotalTests} acertos ({AccuracyPercentage:F1}%)";
        }
        #endregion
    }

    /// <summary>
    /// Detalhe individual de um teste de validação - REESCRITO
    /// </summary>
    public class ValidationDetail
    {
        /// <summary>
        /// Concurso testado (usando Lance.Id)
        /// </summary>
        public int Concurso { get; set; }

        /// <summary>
        /// Dezenas preditas pelo modelo
        /// </summary>
        public List<int> PredictedNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Dezenas realmente sorteadas (usando Lance.Lista)
        /// </summary>
        public List<int> ActualNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Número de acertos (interseção entre preditas e reais)
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// Confiança da predição (0.0 a 1.0)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Tempo gasto na predição
        /// </summary>
        public TimeSpan PredictionTime { get; set; }

        /// <summary>
        /// Indica se foi um acerto total (15 dezenas) - PROPRIEDADE ADICIONADA
        /// </summary>
        public bool Success => Hits == 15;

        /// <summary>
        /// Alias para compatibilidade - PROPRIEDADE ADICIONADA
        /// </summary>
        public bool IsSuccess => Success;

        /// <summary>
        /// Taxa de acerto para este teste específico
        /// </summary>
        public double HitRate => ActualNumbers.Count > 0 ? (double)Hits / ActualNumbers.Count : 0.0;

        /// <summary>
        /// Dezenas que foram acertadas
        /// </summary>
        public List<int> CorrectNumbers => PredictedNumbers.Intersect(ActualNumbers).ToList();

        /// <summary>
        /// Dezenas que foram erradas (preditas mas não sorteadas)
        /// </summary>
        public List<int> MissedNumbers => PredictedNumbers.Except(ActualNumbers).ToList();

        /// <summary>
        /// Cria ValidationDetail a partir de Lance
        /// </summary>
        public static ValidationDetail FromLance(Lance lance, List<int> predictedNumbers, double confidence, TimeSpan predictionTime)
        {
            var hits = predictedNumbers.Intersect(lance.Lista).Count();

            return new ValidationDetail
            {
                Concurso = lance.Id,  // ✅ Usando lance.Id, não lance.Concurso
                PredictedNumbers = new List<int>(predictedNumbers),
                ActualNumbers = new List<int>(lance.Lista),  // ✅ Usando lance.Lista, não lance.DezenasSorteadas
                Hits = hits,
                Confidence = confidence,
                PredictionTime = predictionTime
            };
        }

        public override string ToString()
        {
            return $"Concurso {Concurso}: {Hits}/15 acertos ({HitRate:P1}) - Confiança: {Confidence:P2}";
        }
    }
}
