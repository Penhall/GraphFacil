// Dashboard/Extensions/ValidationResultExtensions.cs - EXTENSÕES PARA COMPATIBILIDADE
using LotoLibrary.Suporte;

namespace Dashboard.Extensions
{
    /// <summary>
    /// Extensões para adaptar ValidationResult da LotoLibrary para uso na UI
    /// </summary>
    public static class ValidationResultExtensions
    {
        /// <summary>
        /// Obtém o nome do teste para exibição na UI
        /// </summary>
        public static string GetTestName(this ValidationResult result)
        {
            return result.GetMetric<string>("TestName", "Teste sem nome");
        }

        /// <summary>
        /// Obtém o status formatado para exibição na UI
        /// </summary>
        public static string GetStatus(this ValidationResult result)
        {
            return result.GetMetric<string>("Status", result.IsValid ? "✅ Sucesso" : "❌ Falha");
        }

        /// <summary>
        /// Obtém os detalhes para exibição na UI
        /// </summary>
        public static string GetDetails(this ValidationResult result)
        {
            return result.GetMetric<string>("Details", result.Message);
        }

        /// <summary>
        /// Converte para formato de exibição simples (compatibilidade com UI antiga)
        /// </summary>
        public static SimpleValidationResult ToSimpleResult(this ValidationResult result)
        {
            return new SimpleValidationResult
            {
                TestName = result.GetTestName(),
                Status = result.GetStatus(),
                Accuracy = result.Accuracy * 100, // Converter para percentual
                Details = result.GetDetails()
            };
        }
    }

    /// <summary>
    /// Classe simplificada para compatibilidade com binding de UI
    /// (Caso o XAML espere propriedades específicas)
    /// </summary>
    public class SimpleValidationResult
    {
        public string TestName { get; set; } = "";
        public string Status { get; set; } = "";
        public double Accuracy { get; set; }
        public string Details { get; set; } = "";
    }
}