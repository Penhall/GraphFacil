// D:\PROJETOS\GraphFacil\Library\Interfaces\IMetaModel.cs - Interface para modelos de meta-aprendizado
using LotoLibrary.Suporte;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LotoLibrary.Interfaces;

/// <summary>
/// Interface para modelos de meta-aprendizado que podem recomendar outros modelos
/// </summary>
public interface IMetaModel
{
    /// <summary>
    /// Recomenda o melhor modelo para o contexto atual
    /// </summary>
    /// <param name="context">Análise do contexto atual</param>
    /// <returns>Recomendação de modelo com confiança</returns>
    Task<ModelRecommendation> RecommendBestModelAsync(ContextAnalysis context);

    /// <summary>
    /// Otimiza pesos do ensemble baseado na performance atual
    /// </summary>
    /// <param name="currentPerformances">Performance atual de cada modelo</param>
    /// <returns>Pesos otimizados para cada modelo</returns>
    Task<Dictionary<string, double>> OptimizeEnsembleWeightsAsync(Dictionary<string, double> currentPerformances);

    /// <summary>
    /// Perfis de performance de cada modelo em diferentes contextos
    /// </summary>
    Dictionary<string, ModelPerformanceProfile> ModelProfiles { get; }

    /// <summary>
    /// Regime atual detectado pelo sistema
    /// </summary>
    string CurrentRegime { get; }

    /// <summary>
    /// Estratégia recomendada pelo meta-modelo
    /// </summary>
    string RecommendedStrategy { get; }

    /// <summary>
    /// Confiança do meta-modelo na decisão atual
    /// </summary>
    double MetaConfidence { get; }
}


