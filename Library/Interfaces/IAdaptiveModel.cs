// D:\PROJETOS\GraphFacil\Library\Interfaces\IAdaptiveModel.cs - Interface para modelos de meta-aprendizado
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Suporte;

namespace LotoLibrary.Interfaces;

/// <summary>
/// Interface para modelos que se adaptam automaticamente a novos dados
/// </summary>
public interface IAdaptiveModel
{
    /// <summary>
    /// Adapta o modelo a novos dados/resultados
    /// </summary>
    /// <param name="newResults">Novos resultados para aprendizado</param>
    /// <returns>Task de adaptação</returns>
    Task AdaptToNewDataAsync(List<ConcursoResult> newResults);

    /// <summary>
    /// Taxa de adaptação do modelo (0.0 = sem adaptação, 1.0 = adaptação total)
    /// </summary>
    double AdaptationRate { get; }

    /// <summary>
    /// Score que indica quão bem o modelo se adapta
    /// </summary>
    double AdaptationScore { get; }

    /// <summary>
    /// Indica se o modelo está atualmente em processo de adaptação
    /// </summary>
    bool IsAdapting { get; }

    /// <summary>
    /// Número de adaptações realizadas
    /// </summary>
    int AdaptationCount { get; }
}


