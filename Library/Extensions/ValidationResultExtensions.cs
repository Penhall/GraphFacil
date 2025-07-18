using System;
using System.Linq;
using LotoLibrary.Models.Validation;

namespace LotoLibrary.Extensions
{
    /// <summary>
    /// Extensões para ValidationResult
    /// </summary>
    public static class ValidationResultExtensions
    {
        /// <summary>
        /// Calcula a taxa de sucesso da validação
        /// </summary>
        public static double CalculateSuccessRate(this ValidationResult result)
        {
            if (result == null || result.TotalTests == 0)
                return 0.0;

            return (double)result.PassedTests / result.TotalTests;
        }

        /// <summary>
        /// Calcula a taxa de acerto da validação
        /// </summary>
        public static double CalculateHitRate(this ValidationResult result)
        {
            if (result == null)
                return 0.0;

            return result.Accuracy;
        }

        /// <summary>
        /// Propriedade para compatibilidade com código existente
        /// </summary>
        public static DateTime ValidationTime(this ValidationResult result)
        {
            return result?.Timestamp ?? DateTime.Now;
        }
    }
}