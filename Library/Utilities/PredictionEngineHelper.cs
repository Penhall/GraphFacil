// D:\PROJETOS\GraphFacil\Library\Utilities\PredictionEngineHelper.cs
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Interfaces;

namespace LotoLibrary.Utilities;

/// <summary>
/// Helper para corrigir problemas no PredictionEngine
/// Resolve erros CS0019 relacionados a comparações de "grupo de métodos"
/// </summary>
public static class PredictionEngineHelper
{
    /// <summary>
    /// Verifica se há número mínimo de modelos
    /// Use no lugar de: models.Count == X
    /// </summary>
    /// <param name="models">Coleção de modelos</param>
    /// <param name="minimum">Número mínimo requerido</param>
    /// <returns>True se tem o mínimo de modelos</returns>
    public static bool HasMinimumModels(ICollection<IPredictionModel> models, int minimum)
    {
        return models?.Count >= minimum;
    }

    /// <summary>
    /// Verifica se há exatamente o número especificado de modelos
    /// Use no lugar de: models.Count == X
    /// </summary>
    /// <param name="models">Coleção de modelos</param>
    /// <param name="exact">Número exato esperado</param>
    /// <returns>True se tem exatamente o número especificado</returns>
    public static bool HasExactModels(ICollection<IPredictionModel> models, int exact)
    {
        return models?.Count == exact;
    }

    /// <summary>
    /// Conta modelos inicializados
    /// </summary>
    /// <param name="models">Enumeração de modelos</param>
    /// <returns>Número de modelos inicializados</returns>
    public static int GetInitializedModelCount(IEnumerable<IPredictionModel> models)
    {
        return models?.Count(m => m.IsInitialized) ?? 0;
    }

    /// <summary>
    /// Verifica se pode criar ensemble
    /// </summary>
    /// <param name="models">Coleção de modelos</param>
    /// <returns>True se pode criar ensemble</returns>
    public static bool CanCreateEnsemble(ICollection<IPredictionModel> models)
    {
        return HasMinimumModels(models, 2) && GetInitializedModelCount(models) >= 2;
    }

    /// <summary>
    /// Verifica se há modelos disponíveis
    /// Use no lugar de: models.Count > 0
    /// </summary>
    /// <param name="models">Coleção de modelos</param>
    /// <returns>True se há pelo menos um modelo</returns>
    public static bool HasAnyModels(ICollection<IPredictionModel> models)
    {
        return models?.Count > 0;
    }

    /// <summary>
    /// Verifica se há modelos inicializados disponíveis
    /// </summary>
    /// <param name="models">Enumeração de modelos</param>
    /// <returns>True se há pelo menos um modelo inicializado</returns>
    public static bool HasInitializedModels(IEnumerable<IPredictionModel> models)
    {
        return GetInitializedModelCount(models) > 0;
    }
}