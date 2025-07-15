// D:\PROJETOS\GraphFacil\Library\Services\TestResult2.cs - Serviço de validação da Fase 1
using System.Collections.Generic;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Resultado base para operações de teste
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Indica se o teste foi bem-sucedido
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem descritiva sobre o resultado
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Detalhes adicionais sobre o resultado do teste
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Métricas e valores numéricos relacionados ao teste
        /// </summary>
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();
    }



}
