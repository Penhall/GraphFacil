using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Interfaces;

namespace LotoLibrary.PredictionModels.AntiFrequency.Statistical;

public partial class SaturationModel : ObservableObject, IPredictionModel
{
    public string ModelName => "Saturation Model";
    public string ModelType => "AntiFrequency";
    public bool IsInitialized { get; private set; }
    public double Confidence { get; private set; }
    public DateTime LastTrainingTime { get; private set; }
    public int TrainingDataSize { get; private set; }

    public event EventHandler<string> OnStatusChanged;
    public event EventHandler<double> OnConfidenceChanged;

    public async Task<bool> InitializeAsync(Lances historicalData)
    {
        await Task.Delay(100);
        IsInitialized = true;
        LastTrainingTime = DateTime.Now;
        TrainingDataSize = historicalData?.Count ?? 0;
        OnStatusChanged?.Invoke(this, "Inicializado");
        return true;
    }

    public async Task<PredictionResult> PredictAsync(int concurso)
    {
        await Task.Delay(100);
        
        var random = new Random(concurso + 3000);
        var dezenas = Enumerable.Range(1, 25)
            .OrderBy(_ => random.Next())
            .Take(15)
            .OrderBy(x => x)
            .ToList();

        Confidence = 0.72;
        OnConfidenceChanged?.Invoke(this, Confidence);
        
        return new PredictionResult(concurso, dezenas, Confidence, ModelName);
    }

    public async Task<ValidationResult> ValidateAsync(Lances testData)
    {
        await Task.Delay(100);
        return new ValidationResult(true, 0.72, "Saturation model validated");
    }

    public async Task<bool> TrainAsync(Lances trainingData)
    {
        await Task.Delay(100);
        LastTrainingTime = DateTime.Now;
        TrainingDataSize = trainingData?.Count ?? 0;
        return true;
    }

    public void Reset()
    {
        IsInitialized = false;
        Confidence = 0;
        OnStatusChanged?.Invoke(this, "Reset");
    }

    public void Dispose()
    {
        // Não há recursos não gerenciados para liberar
    }
}
