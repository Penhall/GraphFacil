// D:\PROJETOS\GraphFacil\Library\Models\Base\PredictionModelBase.cs
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Core;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models.Validation;
using LotoLibrary.Enums;
using LotoLibrary.Suporte;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LotoLibrary.Models.Base;

/// <summary>
/// Classe base abstrata que implementa IPredictionModel
/// CORRIGIDA para usar ValidationResult do namespace correto
/// </summary>
public abstract class PredictionModelBase : IPredictionModel
{
    // ===== PROPRIEDADES BÁSICAS =====
    public abstract string ModelName { get; }
    public virtual string ModelVersion { get; protected set; } = "1.0.0";
    public virtual ModelType ModelType { get; protected set; } = ModelType.Unknown;
    public virtual bool IsInitialized { get; protected set; }
    public virtual bool IsTrained { get; protected set; }
    public virtual double Confidence { get; protected set; }
    public virtual string Description { get; protected set; } = "Modelo de predição básico";

    // ===== CAMPOS PROTEGIDOS =====
    protected Lances _historicalData;
    protected Dictionary<string, object> _parameters;
    protected DateTime _lastTrainingTime;
    protected bool _disposed = false;

    // ===== CONSTRUTOR =====
    protected PredictionModelBase()
    {
        _parameters = new Dictionary<string, object>();
        Confidence = 0.0;
        IsInitialized = false;
    }

    // ===== IMPLEMENTAÇÃO IPredictionModel =====
    public virtual async Task<bool> InitializeAsync(Lances historicalData)
    {
        try
        {
            if (historicalData == null || historicalData.Count == 0)
                return false;

            _historicalData = historicalData;

            // Permitir implementação específica
            var initResult = await DoInitializeAsync(historicalData);

            if (initResult)
            {
                IsInitialized = true;
            }

            return initResult;
        }
        catch (Exception)
        {
            IsInitialized = false;
            return false;
        }
    }

    public virtual async Task<bool> TrainAsync(Lances trainingData)
    {
        try
        {
            if (!IsInitialized || trainingData == null)
                return false;

            var trainResult = await DoTrainAsync(trainingData);

            if (trainResult)
            {
                _lastTrainingTime = DateTime.Now;
                IsTrained = true;
                // Atualizar confiança após treinamento
                Confidence = CalculateConfidence();
            }

            return trainResult;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public abstract Task<PredictionResult> PredictAsync(int concurso);

    // ✅ CORRIGIDO: Usando ValidationResult qualificado do namespace correto
    public virtual async Task<ValidationResult> ValidateAsync(Lances testData)
    {
        try
        {
            if (!IsInitialized || testData == null)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Modelo não inicializado ou dados inválidos"
                };
            }

            return await DoValidateAsync(testData);
        }
        catch (Exception ex)
        {
            return new ValidationResult
            {
                IsValid = false,
                Message = $"Erro na validação: {ex.Message}"
            };
        }
    }

    public virtual void UpdateParameters(Dictionary<string, object> parameters)
    {
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                _parameters[param.Key] = param.Value;
            }

            // Permitir implementação específica
            OnParametersUpdated();
        }
    }

    public virtual void Reset()
    {
        IsInitialized = false;
        IsTrained = false;
        Confidence = 0.0;
        _lastTrainingTime = default;
        _historicalData = null;
        _parameters?.Clear();
    }

    public virtual string GetStatus()
    {
        return $"Modelo: {ModelName} | " +
               $"Inicializado: {IsInitialized} | " +
               $"Confiança: {Confidence:P2} | " +
               $"Último treino: {(_lastTrainingTime == default ? "Nunca" : _lastTrainingTime.ToString("dd/MM/yyyy HH:mm"))}";
    }

    // ===== MÉTODOS ABSTRATOS PARA IMPLEMENTAÇÃO =====
    protected abstract Task<bool> DoInitializeAsync(Lances historicalData);
    protected abstract Task<bool> DoTrainAsync(Lances trainingData);

    // ✅ CORRIGIDO: Retorno usando ValidationResult qualificado
    protected abstract Task<ValidationResult> DoValidateAsync(Lances testData);

    // ===== MÉTODOS VIRTUAIS PARA OVERRIDE =====
    protected virtual double CalculateConfidence()
    {
        // Implementação padrão básica
        return IsInitialized ? 0.5 : 0.0;
    }

    protected virtual void OnParametersUpdated()
    {
        // Implementação específica do modelo pode sobrescrever
    }

    // ===== IMPLEMENTAÇÃO IDisposable =====
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _historicalData = null;
                _parameters?.Clear();
            }
            _disposed = true;
        }
    }
}