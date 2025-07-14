// D:\PROJETOS\GraphFacil\Library\Interfaces\IEnsembleModel.cs
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LotoLibrary.Interfaces;

/// <summary>
/// Interface para modelos ensemble
/// Resolve erros CS1061 relacionados a IEnsembleModel
/// </summary>
public interface IEnsembleModel : IPredictionModel
{
    // ===== PROPRIEDADES ESPECÍFICAS =====
    /// <summary>
    /// Lista de modelos componentes do ensemble
    /// </summary>
    List<IPredictionModel> ComponentModels { get; }

    /// <summary>
    /// Pesos atribuídos a cada modelo componente
    /// </summary>
    Dictionary<string, double> ModelWeights { get; }

    /// <summary>
    /// Estratégia de combinação utilizada
    /// </summary>
    string CombinationStrategy { get; }

    // ===== MÉTODOS ESPECÍFICOS =====
    /// <summary>
    /// Adiciona modelo ao ensemble
    /// </summary>
    /// <param name="model">Modelo a ser adicionado</param>
    /// <param name="weight">Peso do modelo (opcional)</param>
    /// <returns>True se adicionado com sucesso</returns>
    Task<bool> AddModelAsync(IPredictionModel model, double weight = 1.0);

    /// <summary>
    /// Remove modelo do ensemble
    /// </summary>
    /// <param name="modelName">Nome do modelo a ser removido</param>
    /// <returns>True se removido com sucesso</returns>
    bool RemoveModel(string modelName);

    /// <summary>
    /// Otimiza pesos dos modelos componentes
    /// </summary>
    /// <param name="validationData">Dados para otimização</param>
    /// <returns>True se otimização foi bem-sucedida</returns>
    Task<bool> OptimizeWeightsAsync(Lances validationData);

    /// <summary>
    /// Obtém predições individuais de cada modelo componente
    /// </summary>
    /// <param name="concurso">Concurso alvo</param>
    /// <returns>Dicionário com predições por modelo</returns>
    Task<Dictionary<string, PredictionResult>> GetIndividualPredictionsAsync(int concurso);

    /// <summary>
    /// Indica se o ensemble está inicializado (override da interface pai)
    /// </summary>
    new bool IsInitialized { get; }

    /// <summary>
    /// Gera predição usando o ensemble (override da interface pai)
    /// </summary>
    /// <param name="concurso">Concurso alvo</param>
    /// <returns>Resultado da predição ensemble</returns>
    new Task<PredictionResult> PredictAsync(int concurso);
}