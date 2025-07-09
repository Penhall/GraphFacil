// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ValidationResult.cs - Resultado de validação
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Resultado detalhado de validação de um modelo
    /// </summary>
    public class ValidationResult
    {
        #region Core Properties
        /// <summary>
        /// Nome do modelo validado
        /// </summary>
        public string ModelName { get; set; } = "";

        /// <summary>
        /// Indica se a validação foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

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

        #region Validation Metrics
        /// <summary>
        /// Número de predições bem-sucedidas
        /// </summary>
        public int SuccessfulPredictions { get; set; }

        /// <summary>
        /// Total de testes executados
        /// </summary>
        public int TotalTests { get; set; }

        /// <summary>
        /// Precisão do modelo (0.0 a 1.0)
        /// </summary>
        public double Accuracy => TotalTests > 0 ? (double)SuccessfulPredictions / TotalTests : 0.0;

        /// <summary>
        /// Resultados detalhados de cada teste
        /// </summary>
        public List<ValidationDetail> DetailedResults { get; set; } = new List<ValidationDetail>();
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria um resultado de validação bem-sucedida
        /// </summary>
        public static ValidationResult CreateSuccess(string modelName, int successfulPredictions, int totalTests)
        {
            return new ValidationResult
            {
                ModelName = modelName,
                Success = true,
                SuccessfulPredictions = successfulPredictions,
                TotalTests = totalTests,
                TestEndTime = DateTime.Now
            };
        }

        /// <summary>
        /// Cria um resultado de erro na validação
        /// </summary>
        public static ValidationResult CreateError(string modelName, string errorMessage)
        {
            return new ValidationResult
            {
                ModelName = modelName,
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
        }

        /// <summary>
        /// Duração total da validação
        /// </summary>
        public TimeSpan Duration => TestEndTime - TestStartTime;

        /// <summary>
        /// Obtém a taxa de acerto como porcentagem
        /// </summary>
        public double AccuracyPercentage => Accuracy * 100.0;
        #endregion
    }

    /// <summary>
    /// Detalhe individual de um teste de validação
    /// </summary>
    public class ValidationDetail
    {
        /// <summary>
        /// Concurso testado
        /// </summary>
        public int Concurso { get; set; }

        /// <summary>
        /// Dezenas preditas pelo modelo
        /// </summary>
        public List<int> PredictedNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Dezenas realmente sorteadas
        /// </summary>
        public List<int> ActualNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Número de acertos
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// Confiança da predição
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Tempo gasto na predição
        /// </summary>
        public TimeSpan PredictionTime { get; set; }

        /// <summary>
        /// Indica se foi um acerto (15 dezenas)
        /// </summary>
        public bool IsSuccess => Hits == 15;

        /// <summary>
        /// Taxa de acerto para este teste específico
        /// </summary>
        public double HitRate => ActualNumbers.Count > 0 ? (double)Hits / ActualNumbers.Count : 0.0;
    }
}
