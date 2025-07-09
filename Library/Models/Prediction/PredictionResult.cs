// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionResult.cs - Resultado de predição
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Resultado completo de uma predição
    /// </summary>
    public class PredictionResult
    {
        #region Core Properties
        /// <summary>
        /// Indica se a predição foi gerada com sucesso
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem de erro caso Success = false
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Nome do modelo que gerou a predição
        /// </summary>
        public string ModelName { get; set; } = "";

        /// <summary>
        /// Nível de confiança da predição (0.0 a 1.0)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Momento em que a predição foi gerada
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Explicação sobre como a predição foi gerada
        /// </summary>
        public string Explanation { get; set; } = "";
        #endregion

        #region Prediction Data
        /// <summary>
        /// Dezenas preditas
        /// </summary>
        public List<int> PredictedNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Concurso alvo da predição
        /// </summary>
        public int TargetConcurso { get; set; }

        /// <summary>
        /// Probabilidades individuais de cada dezena
        /// </summary>
        public Dictionary<int, double> NumberProbabilities { get; set; } = new Dictionary<int, double>();

        /// <summary>
        /// Metadados adicionais do modelo
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria um resultado de sucesso
        /// </summary>
        public static PredictionResult CreateSuccess(string modelName, List<int> predictedNumbers, double confidence, string explanation = "")
        {
            return new PredictionResult
            {
                Success = true,
                ModelName = modelName,
                PredictedNumbers = predictedNumbers,
                Confidence = confidence,
                Explanation = explanation,
                GeneratedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Cria um resultado de erro
        /// </summary>
        public static PredictionResult CreateError(string modelName, string errorMessage)
        {
            return new PredictionResult
            {
                Success = false,
                ModelName = modelName,
                ErrorMessage = errorMessage,
                GeneratedAt = DateTime.Now
            };
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Verifica se a predição contém um número específico
        /// </summary>
        public bool ContainsNumber(int number)
        {
            return PredictedNumbers.Contains(number);
        }

        /// <summary>
        /// Obtém a probabilidade de um número específico
        /// </summary>
        public double GetNumberProbability(int number)
        {
            return NumberProbabilities.TryGetValue(number, out var prob) ? prob : 0.0;
        }

        /// <summary>
        /// Adiciona metadados ao resultado
        /// </summary>
        public void AddMetadata(string key, object value)
        {
            Metadata[key] = value;
        }
        #endregion
    }
}
