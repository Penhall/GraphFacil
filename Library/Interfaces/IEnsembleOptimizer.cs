// D:\PROJETOS\GraphFacil\Library\Interfaces\IEnsembleOptimizer.cs - Interface para modelos de meta-aprendizado
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Enums;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para otimizadores de ensemble
    /// </summary>
    public interface IEnsembleOptimizer
    {
        /// <summary>
        /// Otimiza pesos do ensemble baseado em dados históricos
        /// </summary>
        /// <param name="modelPerformances">Performance histórica dos modelos</param>
        /// <param name="correlations">Matriz de correlação entre modelos</param>
        /// <returns>Pesos otimizados</returns>
        Task<Dictionary<string, double>> OptimizeWeightsAsync(
            Dictionary<string, List<double>> modelPerformances,
            Dictionary<string, Dictionary<string, double>> correlations);

        /// <summary>
        /// Calcula diversificação do portfólio de modelos
        /// </summary>
        /// <param name="weights">Pesos atuais</param>
        /// <param name="correlations">Correlações entre modelos</param>
        /// <returns>Score de diversificação (0-1)</returns>
        double CalculateDiversificationScore(
            Dictionary<string, double> weights,
            Dictionary<string, Dictionary<string, double>> correlations);

        /// <summary>
        /// Estratégia de otimização utilizada
        /// </summary>
        EnsembleOptimizationStrategy Strategy { get; set; }

        /// <summary>
        /// Restrições para otimização (peso mín/máx por modelo)
        /// </summary>
        Dictionary<string, (double Min, double Max)> WeightConstraints { get; set; }
    }
}

