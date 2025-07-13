// D:\PROJETOS\GraphFacil\Library\Interfaces\IExplainableModel.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Utilities;

namespace LotoLibrary.Interfaces;

/// <summary>
/// Interface para modelos que podem explicar suas predições
/// Implementa conceitos de Explainable AI (XAI)
/// </summary>
public interface IExplainableModel
{
    /// <summary>
    /// Gera explicação detalhada de como a predição foi gerada
    /// </summary>
    /// <param name="targetConcurso">Concurso para o qual a explicação se refere</param>
    /// <returns>Explicação estruturada da predição</returns>
    Task<PredictionExplanation> ExplainPredictionAsync(int targetConcurso);

    /// <summary>
    /// Obtém os fatores mais importantes para a decisão do modelo
    /// </summary>
    /// <returns>Lista dos principais fatores de decisão</returns>
    List<string> GetKeyDecisionFactors();

    /// <summary>
    /// Explica por que uma dezena específica foi selecionada ou rejeitada
    /// </summary>
    /// <param name="dezena">Dezena a ser explicada (1-25)</param>
    /// <param name="targetConcurso">Concurso de referência</param>
    /// <returns>Explicação específica para a dezena</returns>
    Task<string> ExplainNumberSelectionAsync(int dezena, int targetConcurso);

    /// <summary>
    /// Indica se o modelo suporta explicações detalhadas
    /// </summary>
    bool SupportsDetailedExplanations { get; }

    /// <summary>
    /// Obtém nível de confiança na explicação fornecida
    /// </summary>
    double ExplanationConfidence { get; }
}
