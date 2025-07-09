// Dashboard/ViewModels/Services/ViewModelFactory.cs - Factory para ViewModels
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;

namespace Dashboard.ViewModels.Services;

/// <summary>
/// Factory para criação de ViewModels especializados
/// Centraliza criação e garante configuração correta
/// </summary>
public class ViewModelFactory
{
    #region Private Fields
    private readonly Lances _historicalData;
    private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
    #endregion

    #region Constructor
    public ViewModelFactory(Lances historicalData)
    {
        _historicalData = historicalData ?? throw new ArgumentNullException(nameof(historicalData));
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Cria PredictionModelsViewModel (singleton)
    /// </summary>
    public PredictionModelsViewModel CreatePredictionModelsViewModel()
    {
        return GetOrCreateSingleton<PredictionModelsViewModel>(() =>
            new PredictionModelsViewModel(_historicalData));
    }

    /// <summary>
    /// Cria ValidationViewModel (singleton)
    /// </summary>
    public ValidationViewModel CreateValidationViewModel()
    {
        return GetOrCreateSingleton<ValidationViewModel>(() =>
            new ValidationViewModel(_historicalData));
    }

    /// <summary>
    /// Cria ComparisonViewModel (singleton)
    /// </summary>
    public ComparisonViewModel CreateComparisonViewModel()
    {
        return GetOrCreateSingleton<ComparisonViewModel>(() =>
            new ComparisonViewModel(_historicalData));
    }

    /// <summary>
    /// Cria ConfigurationViewModel (singleton)
    /// </summary>
    public ConfigurationViewModel CreateConfigurationViewModel()
    {
        return GetOrCreateSingleton<ConfigurationViewModel>(() =>
            new ConfigurationViewModel(_historicalData));
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Obtém ou cria singleton de um tipo específico
    /// </summary>
    private T GetOrCreateSingleton<T>(Func<T> factory) where T : class
    {
        var type = typeof(T);

        if (_singletons.TryGetValue(type, out var existing))
        {
            return existing as T;
        }

        var instance = factory();
        _singletons[type] = instance;
        return instance;
    }

    /// <summary>
    /// Limpa cache de singletons (útil para testes)
    /// </summary>
    public void ClearCache()
    {
        foreach (var instance in _singletons.Values)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _singletons.Clear();
    }
    #endregion
}
