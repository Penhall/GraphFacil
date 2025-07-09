// D:\PROJETOS\GraphFacil\Library\Utilities\TestResultXtras.cs - Extensão do TestResult
using System.Collections.Generic;
using LotoLibrary.Suporte;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Extensão do TestResult com informações adicionais
    /// </summary>
    public class TestResultXtras : TestResult
    {
        /// <summary>
        /// Detalhes adicionais sobre o resultado do teste
        /// </summary>
        public string Details { get; set; } = "";

        /// <summary>
        /// Métricas e valores numéricos relacionados ao teste
        /// </summary>
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Dados brutos do teste
        /// </summary>
        public object RawData { get; set; }

        /// <summary>
        /// Cria um TestResultXtras de sucesso com detalhes
        /// </summary>
        public static TestResultXtras CreateSuccess(string testName, string message, string details = "", Dictionary<string, object> metrics = null)
        {
            return new TestResultXtras
            {
                TestName = testName,
                Success = true,
                Message = message,
                Details = details,
                Metrics = metrics ?? new Dictionary<string, object>(),
                StartTime = System.DateTime.Now,
                EndTime = System.DateTime.Now
            };
        }

        /// <summary>
        /// Cria um TestResultXtras de erro com detalhes
        /// </summary>
        public static TestResultXtras CreateError(string testName, string message, string details = "", System.Exception exception = null)
        {
            return new TestResultXtras
            {
                TestName = testName,
                Success = false,
                Message = message,
                Details = details + (exception != null ? $"\nException: {exception.Message}" : ""),
                StartTime = System.DateTime.Now,
                EndTime = System.DateTime.Now
            };
        }

        /// <summary>
        /// Adiciona uma métrica ao resultado
        /// </summary>
        public void AddMetric(string name, object value)
        {
            Metrics[name] = value;
        }

        /// <summary>
        /// Obtém uma métrica por nome
        /// </summary>
        public T GetMetric<T>(string name, T defaultValue = default(T))
        {
            if (Metrics.TryGetValue(name, out var value) && value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }
    }
}
