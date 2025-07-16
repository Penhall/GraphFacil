using LotoLibrary.Models;
using LotoLibrary.Suporte;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para serviços de validação de modelos
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Obtém a suíte de validação padrão
        /// </summary>
        IReadOnlyList<ValidationTestCase> GetValidationSuite();

        /// <summary>
        /// Executa validação completa
        /// </summary>
        Task<ValidationResult> RunFullValidationAsync(Lances historicalData);

        /// <summary>
        /// Executa validação rápida
        /// </summary>
        Task<ValidationResult> RunQuickValidationAsync(Lances historicalData);

        /// <summary>
        /// Valida um modelo específico
        /// </summary>
        Task<ModelValidationResult> ValidateModelAsync(IPredictionModel model, Lances testData);
    }

    /// <summary>
    /// Caso de teste para validação
    /// </summary>
    public class ValidationTestCase
    {
        public string Name { get; set; }
        public bool IsCritical { get; set; }
        public virtual Task<ValidationTestResult> ExecuteAsync(Lances historicalData)
        {
            throw new NotImplementedException("ExecuteAsync deve ser implementado nas classes derivadas");
        }
    }

    /// <summary>
    /// Resultado de um teste de validação
    /// </summary>
    public class ValidationTestResult
    {
        public string TestName { get; set; }
        public bool Success { get; set; }
        public string Details { get; set; }
    }
}
