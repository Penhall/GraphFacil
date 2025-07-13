// D:\PROJETOS\GraphFacil\Library\Models\Base\PredictionModelBase.cs
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Interfaces;
using LotoLibrary.Models.Prediction;

﻿// D:\PROJETOS\GraphFacil\Library\Models\Base\PredictionModelBase.cs - Classe base
namespace LotoLibrary.Models.Base;

/// <summary>
/// Classe base que implementa funcionalidades comuns a todos os modelos de predição
/// </summary>
public abstract class PredictionModelBase : IPredictionModel
{
    #region Fields
    protected Lances _historicalData;
    protected Lances _trainingData;
    protected DateTime _lastTrainingTime;
    protected double _confidence;
    protected bool _isInitialized;
    #endregion

    #region Properties
    public abstract string ModelName { get; }
    public abstract string ModelType { get; }
    public bool IsInitialized => _isInitialized;
    public double Confidence => _confidence;
    public DateTime LastTrainingTime => _lastTrainingTime;
    public int TrainingDataSize => _trainingData?.Count ?? 0;
    #endregion

    #region Events
    public event EventHandler<string> OnStatusChanged;
    public event EventHandler<double> OnConfidenceChanged;
    #endregion

    #region Public Methods
    public virtual async Task<bool> InitializeAsync(Lances historicalData)
    {
        try
        {
            OnStatusChanged?.Invoke(this, "Inicializando modelo...");

            if (historicalData == null || !historicalData.Any())
            {
                OnStatusChanged?.Invoke(this, "Erro: Dados históricos inválidos");
                return false;
            }

            _historicalData = historicalData;

            var initResult = await DoInitializeAsync(historicalData);

            _isInitialized = initResult;
            OnStatusChanged?.Invoke(this, initResult ? "Modelo inicializado com sucesso" : "Falha na inicialização");

            return initResult;
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke(this, $"Erro na inicialização: {ex.Message}");
            return false;
        }
    }

    public virtual async Task<bool> TrainAsync(Lances trainingData)
    {
        try
        {
            if (!_isInitialized)
            {
                OnStatusChanged?.Invoke(this, "Erro: Modelo não inicializado");
                return false;
            }

            OnStatusChanged?.Invoke(this, "Iniciando treinamento...");

            _trainingData = trainingData;

            var trainResult = await DoTrainAsync(trainingData);

            if (trainResult)
            {
                _lastTrainingTime = DateTime.Now;
                OnStatusChanged?.Invoke(this, "Treinamento concluído com sucesso");
            }
            else
            {
                OnStatusChanged?.Invoke(this, "Falha no treinamento");
            }

            return trainResult;
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke(this, $"Erro no treinamento: {ex.Message}");
            return false;
        }
    }

    public virtual async Task<PredictionResult> PredictAsync(int targetConcurso)
    {
        try
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Modelo não inicializado");
            }

            OnStatusChanged?.Invoke(this, $"Gerando predição para concurso {targetConcurso}...");

            var stopwatch = Stopwatch.StartNew();
            var result = await DoPredictAsync(targetConcurso);
            stopwatch.Stop();

            if (result != null)
            {
                result.ModelUsed = ModelName;
                result.PredictionTime = DateTime.Now;
                result.ProcessingTime = stopwatch.Elapsed;
                result.TargetConcurso = targetConcurso;

                _confidence = result.OverallConfidence;
                OnConfidenceChanged?.Invoke(this, _confidence);
                OnStatusChanged?.Invoke(this, "Predição gerada com sucesso");
            }

            return result;
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke(this, $"Erro na predição: {ex.Message}");
            throw;
        }
    }

    public virtual async Task<ValidationResult> ValidateAsync(Lances validationData)
    {
        try
        {
            OnStatusChanged?.Invoke(this, "Iniciando validação...");

            var result = await DoValidateAsync(validationData);

            OnStatusChanged?.Invoke(this, $"Validação concluída - Accuracy: {result.Accuracy:P2}");

            return result;
        }
        catch (Exception ex)
        {
            OnStatusChanged?.Invoke(this, $"Erro na validação: {ex.Message}");
            throw;
        }
    }

    public virtual void Reset()
    {
        OnStatusChanged?.Invoke(this, "Resetando modelo...");

        _isInitialized = false;
        _confidence = 0;
        _historicalData = null;
        _trainingData = null;
        _lastTrainingTime = default;

        DoReset();

        OnStatusChanged?.Invoke(this, "Modelo resetado");
    }

    public void Dispose()
    {
        // Implementação vazia, pois não há recursos não gerenciados para liberar.
        // Isso satisfaz a interface IDisposable.
    }
    #endregion

    #region Abstract Methods
    protected abstract Task<bool> DoInitializeAsync(Lances historicalData);
    protected abstract Task<bool> DoTrainAsync(Lances trainingData);
    protected abstract Task<PredictionResult> DoPredictAsync(int targetConcurso);
    protected abstract Task<ValidationResult> DoValidateAsync(Lances validationData);
    protected abstract void DoReset();
    #endregion

    #region Protected Helpers
    protected void UpdateStatus(string status)
    {
        OnStatusChanged?.Invoke(this, status);
    }

    protected void UpdateConfidence(double confidence)
    {
        _confidence = Math.Max(0, Math.Min(1, confidence));
        OnConfidenceChanged?.Invoke(this, _confidence);
    }

    protected ValidationResult CreateValidationResult(List<Lance> predictions, List<Lance> actual)
    {
        var correct = 0;
        var total = Math.Min(predictions.Count, actual.Count);

        for (int i = 0; i < total; i++)
        {
            var intersection = predictions[i].Lista.Intersect(actual[i].Lista).Count();
            if (intersection >= 11) // Critério mínimo de acerto
            {
                correct++;
            }
        }

        return new ValidationResult
        {
            Accuracy = total > 0 ? (double)correct / total : 0,
            TotalTests = total,
            CorrectPredictions = new List<int> { correct },
            ValidationTime = TimeSpan.Zero
        };
    }
    #endregion
}
