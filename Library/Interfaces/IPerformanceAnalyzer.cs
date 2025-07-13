// D:\PROJETOS\GraphFacil\Library\Interfaces\IPerformanceAnalyzer.cs - Interface principal
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para análise de performance
    /// </summary>
    public interface IPerformanceAnalyzer
    {
        Task<PerformanceReport> AnalyzeAsync(IPredictionModel model, Lances testData);
        Task<ComparisonReport> CompareModelsAsync(List<IPredictionModel> models, Lances testData);
        Task<List<PerformanceMetric>> GetDetailedMetricsAsync(IPredictionModel model, Lances testData);
    }
}


