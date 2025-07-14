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
    // ===== PROPRIEDADES ESPEC�FICAS =====
    /// <summary>
    /// Lista de modelos componentes do ensemble
    /// </summary>
    List<IPredictionModel> ComponentModels { get; }

    /// <summary>
    /// Pesos atribu�dos a cada modelo componente
    /// </summary>
    Dictionary<string, double> ModelWeights { get; }

    /// <summary>
    /// Estrat�gia de combina��o utilizada
    /// </summary>
    string CombinationStrategy { get; }

    // ===== M�TODOS ESPEC�FICOS =====
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
    /// <param name="validationData">Dados para otimiza��o</param>
    /// <returns>True se otimiza��o foi bem-sucedida</returns>
    Task<bool> OptimizeWeightsAsync(Lances validationData);

    /// <summary>
    /// Obt�m predi��es individuais de cada modelo componente
    /// </summary>
    /// <param name="concurso">Concurso alvo</param>
    /// <returns>Dicion�rio com predi��es por modelo</returns>
    Task<Dictionary<string, PredictionResult>> GetIndividualPredictionsAsync(int concurso);

    /// <summary>
    /// Indica se o ensemble est� inicializado (override da interface pai)
    /// </summary>
    new bool IsInitialized { get; }

    /// <summary>
    /// Gera predi��o usando o ensemble (override da interface pai)
    /// </summary>
    /// <param name="concurso">Concurso alvo</param>
    /// <returns>Resultado da predi��o ensemble</returns>
    new Task<PredictionResult> PredictAsync(int concurso);
}