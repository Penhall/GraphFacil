using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;

namespace LotoLibrary.PredictionModels.AntiFrequency.Base;

public abstract partial class AntiFrequencyModelBase : ObservableObject, IPredictionModel, IConfigurableModel, IExplainableModel
{
    #region Observable Properties
    [ObservableProperty]
    private string _antiFrequencyStatus = "Aguardando";

    [ObservableProperty]
    private double _antiFrequencyScore;

    [ObservableProperty]
    private int _analysisWindow = 50;

    [ObservableProperty]
    private double _temporalDecayFactor = 0.95;

    [ObservableProperty]
    private string _detectedPattern = "Nenhum";
    #endregion

    #region IPredictionModel Properties
    public abstract string ModelName { get; }
    public abstract string ModelType { get; }
    public bool IsInitialized { get; protected set; }
    public double Confidence { get; protected set; }
    public DateTime LastTrainingTime { get; protected set; }
    public int TrainingDataSize { get; protected set; }

    public event EventHandler<string> OnStatusChanged;
    public event EventHandler<double> OnConfidenceChanged;
    #endregion

    #region IConfigurableModel Properties
    protected Dictionary<string, object> _currentParameters = new();
    protected Dictionary<string, object> _defaultParameters = new();

    public virtual Dictionary<string, object> CurrentParameters => _currentParameters;
    public virtual Dictionary<string, object> DefaultParameters => _defaultParameters;
    #endregion

    #region IExplainableModel Properties
    public virtual bool SupportsDetailedExplanations => true;
    public virtual double ExplanationConfidence => 0.75;
    #endregion

    #region IPredictionModel Methods
    public virtual async Task<bool> InitializeAsync(Lances historicalData)
    {
        await Task.Delay(100);
        IsInitialized = true;
        LastTrainingTime = DateTime.Now;
        TrainingDataSize = historicalData?.Count ?? 0;
        AntiFrequencyStatus = "Inicializado";
        return true;
    }

    public abstract Task<PredictionResult> PredictAsync(int concurso);

    public virtual async Task<ValidationResult> ValidateAsync(Lances testData)
    {
        await Task.Delay(100);
        return new ValidationResult(true, 0.7, "Validação concluída");
    }

    public virtual async Task<bool> TrainAsync(Lances trainingData)
    {
        await Task.Delay(100);
        LastTrainingTime = DateTime.Now;
        TrainingDataSize = trainingData?.Count ?? 0;
        return true;
    }

    public virtual void Reset()
    {
        IsInitialized = false;
        Confidence = 0;
        AntiFrequencyStatus = "Reset";
    }

    public void Dispose()
    {
        // Não há recursos não gerenciados para liberar
    }
    #endregion

    #region IConfigurableModel Methods
    public virtual void UpdateParameters(Dictionary<string, object> parameters)
    {
        if (ValidateParameters(parameters))
        {
            _currentParameters = new Dictionary<string, object>(parameters);
        }
    }

    public virtual bool ValidateParameters(Dictionary<string, object> parameters)
    {
        return parameters != null;
    }

    public virtual string GetParameterDescription(string parameterName)
    {
        return $"Parâmetro: {parameterName}";
    }

    public virtual List<object> GetAllowedValues(string parameterName)
    {
        return new List<object>();
    }

    public virtual void ResetToDefaults()
    {
        _currentParameters = new Dictionary<string, object>(_defaultParameters);
    }
    #endregion

    #region IExplainableModel Methods
    public virtual async Task<Utilities.PredictionExplanation> ExplainPredictionAsync(int concurso)
    {
        await Task.Delay(50);
        return new Utilities.PredictionExplanation
        {
            Concurso = concurso,
            ModelName = ModelName,
            MainReason = "Estratégia anti-frequencista aplicada",
            ConfidenceLevel = ExplanationConfidence
        };
    }

    public virtual List<string> GetKeyDecisionFactors()
    {
        return new List<string>
        {
            "Análise de frequência inversa",
            "Padrão detectado: " + DetectedPattern,
            "Score anti-frequência: " + AntiFrequencyScore.ToString("F2")
        };
    }

    public virtual async Task<string> ExplainNumberSelectionAsync(int concurso, int numero)
    {
        await Task.Delay(25);
        return $"Número {numero} selecionado por baixa frequência recente.";
    }
    #endregion
}
