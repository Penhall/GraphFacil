// E:\PROJETOS\GraphFacil\Library\Interfaces\IValidationService.cs
using LotoLibrary.Models;
using LotoLibrary.Models.Validation;
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
        IReadOnlyList<TestResultCase> GetValidationSuite();

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
        Task<ValidationResult> ValidateModelAsync(IPredictionModel model, Lances testData);
    }

    /// <summary>
    /// Caso de teste para validação
    /// </summary>
    public class TestResultCase
    {
        public string Name { get; set; }
        public bool IsCritical { get; set; }
        
        public virtual Task<TestResult> ExecuteAsync(Lances historicalData)
        {
            throw new NotImplementedException("ExecuteAsync deve ser implementado nas classes derivadas");
        }
    }
}