// D:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Base\AntiFrequencyModelBase.cs
using LotoLibrary.Enums;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Suporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.AntiFrequency.Base
{
    /// <summary>
    /// Classe base para modelos anti-frequencistas
    /// Implementa PredictionModelBase + IConfigurableModel completo
    /// </summary>
    public abstract class AntiFrequencyModelBase : PredictionModelBase, IConfigurableModel
    {
        // ===== PROPRIEDADES ABSTRATAS =====
        public abstract string Name { get; }
        public abstract string ModelType { get; }
        public abstract ModelType ModelTypeEnum { get; }
        public abstract AntiFrequencyStrategy Strategy { get; }

        // ===== IMPLEMENTAÇÃO OBRIGATÓRIA IPredictionModel =====
        public override string ModelName => Name;

        // ===== PROPRIEDADES ESPECÍFICAS =====
        public AntiFrequencyStatus Status { get; protected set; } = AntiFrequencyStatus.NotInitialized;
        public ModelComplexity Complexity { get; protected set; } = ModelComplexity.Medium;

        // ===== CAMPOS PARA ICONFIGURABLEMODEL =====
        private Dictionary<string, object> _currentParameters;
        private Dictionary<string, object> _defaultParameters;

        // ===== CAMPOS ANTI-FREQUENCY =====
        protected Dictionary<int, double> _frequencyData;
        protected Dictionary<int, double> _antiFrequencyScores;

        // ===== CONSTRUTOR =====
        protected AntiFrequencyModelBase()
        {
            _frequencyData = new Dictionary<int, double>();
            _antiFrequencyScores = new Dictionary<int, double>();
            InitializeConfigurableParameters();
        }

        // ===== IMPLEMENTAÇÃO ICONFIGURABLEMODEL =====
        public Dictionary<string, object> CurrentParameters => _currentParameters ?? new Dictionary<string, object>();

        public virtual Dictionary<string, object> DefaultParameters => _defaultParameters ?? new Dictionary<string, object>();

        public virtual void UpdateParameters(Dictionary<string, object> parameters)
        {
            if (ValidateParameters(parameters))
            {
                foreach (var param in parameters)
                {
                    _currentParameters[param.Key] = param.Value;
                }
                OnParametersUpdated();
            }
        }

        public virtual bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return false;

            if (parameters.ContainsKey("FatorDecaimento"))
            {
                if (parameters["FatorDecaimento"] is double fator && (fator < 0 || fator > 1))
                    return false;
            }

            if (parameters.ContainsKey("JanelaAnalise"))
            {
                if (parameters["JanelaAnalise"] is int janela && janela < 10)
                    return false;
            }

            return true;
        }

        public virtual string GetParameterDescription(string parameterName)
        {
            return parameterName switch
            {
                "JanelaAnalise" => "Janela de análise histórica (>= 10)",
                "FatorDecaimento" => "Fator de decaimento temporal (0.0 a 1.0)",
                "ThresholdMinimo" => "Threshold mínimo para consideração",
                "PesoTemporal" => "Peso da análise temporal (0.0 a 1.0)",
                "Strategy" => "Estratégia anti-frequência utilizada",
                _ => $"Parâmetro {parameterName}"
            };
        }

        public virtual List<object> GetAllowedValues(string parameterName)
        {
            return parameterName switch
            {
                "Strategy" => new List<object>
                {
                    AntiFrequencyStrategy.Simple,
                    AntiFrequencyStrategy.Gentle,
                    AntiFrequencyStrategy.Moderate,
                    AntiFrequencyStrategy.Strong
                },
                "JanelaAnalise" => new List<object> { 20, 50, 100, 200, 500 },
                _ => null // null = qualquer valor permitido
            };
        }

        public virtual void ResetToDefaults()
        {
            _currentParameters = new Dictionary<string, object>(_defaultParameters);
            OnParametersUpdated();
        }

        // ===== IMPLEMENTAÇÃO DOS MÉTODOS ABSTRATOS =====
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                Status = AntiFrequencyStatus.Initializing;
                await Task.Delay(100);

                CalculateFrequencies(historicalData);
                var initResult = await DoAntiFrequencyInitializeAsync(historicalData);

                Status = initResult ? AntiFrequencyStatus.Ready : AntiFrequencyStatus.Error;
                return initResult;
            }
            catch (Exception)
            {
                Status = AntiFrequencyStatus.Error;
                return false;
            }
        }

        protected override async Task<bool> DoTrainAsync(Lances trainingData)
        {
            try
            {
                Status = AntiFrequencyStatus.Calibrating;
                await Task.Delay(150);

                var trainResult = await DoAntiFrequencyTrainAsync(trainingData);
                Status = trainResult ? AntiFrequencyStatus.Ready : AntiFrequencyStatus.Error;
                return trainResult;
            }
            catch (Exception)
            {
                Status = AntiFrequencyStatus.Error;
                return false;
            }
        }

        protected override async Task<ValidationResult> DoValidateAsync(Lances testData)
        {
            try
            {
                Status = AntiFrequencyStatus.Validating;
                await Task.Delay(75);

                var validationResult = await DoAntiFrequencyValidateAsync(testData);
                Status = AntiFrequencyStatus.Ready;
                return validationResult;
            }
            catch (Exception ex)
            {
                Status = AntiFrequencyStatus.Error;
                return new ValidationResult
                {
                    IsValid = false,
                    Accuracy = 0.0,
                    Message = $"Erro na validação anti-frequência: {ex.Message}"
                };
            }
        }

        public override async Task<PredictionResult> PredictAsync(int concurso)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Modelo anti-frequência não inicializado");

            try
            {
                Status = AntiFrequencyStatus.Predicting;
                await Task.Delay(120);

                var prediction = await DoAntiFrequencyPredictAsync(concurso);
                Status = AntiFrequencyStatus.Ready;
                return prediction;
            }
            catch (Exception ex)
            {
                Status = AntiFrequencyStatus.Error;
                throw new InvalidOperationException($"Erro na predição anti-frequência: {ex.Message}", ex);
            }
        }

        // ===== MÉTODOS ABSTRATOS PARA IMPLEMENTAÇÃO ESPECÍFICA =====
        protected abstract Task<bool> DoAntiFrequencyInitializeAsync(Lances historicalData);
        protected abstract Task<bool> DoAntiFrequencyTrainAsync(Lances trainingData);
        protected abstract Task<ValidationResult> DoAntiFrequencyValidateAsync(Lances testData);
        protected abstract Task<PredictionResult> DoAntiFrequencyPredictAsync(int concurso);

        // ===== MÉTODOS UTILITÁRIOS =====
        protected virtual void CalculateFrequencies(Lances historicalData)
        {
            if (historicalData == null || !historicalData.Any()) return;

            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var frequency = historicalData
                    .SelectMany(lance => lance.Lista)
                    .Count(d => d == dezena);

                _frequencyData[dezena] = (double)frequency / historicalData.Count;
            }

            CalculateAntiFrequencyScores();
        }

        protected virtual void CalculateAntiFrequencyScores()
        {
            var maxFreq = _frequencyData.Values.DefaultIfEmpty(0).Max();

            foreach (var kvp in _frequencyData)
            {
                _antiFrequencyScores[kvp.Key] = maxFreq - kvp.Value;
            }
        }

        protected override double CalculateConfidence()
        {
            return IsInitialized && Status == AntiFrequencyStatus.Ready ? 0.65 : 0.0;
        }

        // ===== MÉTODOS PRIVADOS =====
        private void InitializeConfigurableParameters()
        {
            _defaultParameters = new Dictionary<string, object>
            {
                { "JanelaAnalise", 100 },
                { "FatorDecaimento", 0.1 },
                { "ThresholdMinimo", 0.01 },
                { "PesoTemporal", 0.8 },
                { "Strategy", Strategy }
            };

            _currentParameters = new Dictionary<string, object>(_defaultParameters);
        }

        protected virtual void OnParametersUpdated()
        {
            // Implementação específica pode sobrescrever
        }

        public void SetParameter(string parameterName, object value)
        {
            if (_currentParameters.ContainsKey(parameterName))
            {
                _currentParameters[parameterName] = value;
                OnParametersUpdated();
            }
        }

        protected List<int> SelectTopAntiFrequencyNumbers(int count = 15)
        {
            return _antiFrequencyScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .OrderBy(x => x)
                .ToList();
        }
    }
}
