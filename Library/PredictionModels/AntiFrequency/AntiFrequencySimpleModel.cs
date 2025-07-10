// D:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\AntiFrequencySimpleModel.cs
// IMPLEMENTAÇÃO COMPLETA E CORRIGIDA - AntiFrequencySimpleModel
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.AntiFrequency
{
    /// <summary>
    /// Modelo Anti-Frequência Simples - IMPLEMENTAÇÃO COMPLETA
    /// Prioriza dezenas com menor frequência histórica
    /// Herda de PredictionModelBase para implementação completa da IPredictionModel
    /// </summary>
    public class AntiFrequencySimpleModel : PredictionModelBase, IConfigurableModel
    {
        #region IPredictionModel Properties Implementation
        public override string ModelName => "Anti-Frequency Simple Model";
        public override string ModelType => "AntiFrequency";
        #endregion

        #region IConfigurableModel Properties
        public Dictionary<string, object> CurrentParameters { get; private set; }
        public Dictionary<string, object> DefaultParameters { get; private set; }
        #endregion

        #region Private Fields
        private Dictionary<int, int> _numberFrequencies;
        private Dictionary<int, double> _antiFrequencyScores;
        private Dictionary<string, string> _parameterDescriptions;
        private Dictionary<string, List<object>> _allowedValues;
        #endregion

        #region Constructor
        public AntiFrequencySimpleModel()
        {
            InitializeParameters();
            ResetToDefaults();
        }
        #endregion

        #region PredictionModelBase Implementation
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                UpdateStatus("Inicializando modelo Anti-Frequency...");

                if (historicalData == null || historicalData.Count == 0)
                {
                    UpdateStatus("Erro: Dados históricos inválidos");
                    return false;
                }

                var minimumDataSize = GetParameter<int>("MinimumDataSize");
                if (historicalData.Count < minimumDataSize)
                {
                    UpdateStatus($"Erro: Dados insuficientes. Mínimo: {minimumDataSize}, Atual: {historicalData.Count}");
                    return false;
                }

                // Inicializar estruturas de dados
                _numberFrequencies = new Dictionary<int, int>();
                _antiFrequencyScores = new Dictionary<int, double>();

                // Inicializar contadores para todas as dezenas (1-25)
                for (int i = 1; i <= 25; i++)
                {
                    _numberFrequencies[i] = 0;
                }

                UpdateStatus("Modelo Anti-Frequency inicializado com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        protected override async Task<bool> DoTrainAsync(Lances trainingData)
        {
            try
            {
                UpdateStatus("Iniciando treinamento do modelo Anti-Frequency...");

                if (trainingData == null || trainingData.Count == 0)
                {
                    UpdateStatus("Erro: Dados de treino inválidos");
                    return false;
                }

                // Resetar contadores
                for (int i = 1; i <= 25; i++)
                {
                    _numberFrequencies[i] = 0;
                }

                // Calcular frequências considerando janela histórica
                await CalculateFrequencies(trainingData);

                // Calcular scores anti-frequência
                await CalculateAntiFrequencyScores();

                // Calcular confiança baseada na distribuição
                var confidence = CalculateModelConfidence();
                UpdateConfidence(confidence);

                UpdateStatus($"Treinamento concluído. Dados processados: {trainingData.Count}");
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro no treinamento: {ex.Message}");
                return false;
            }
        }

        protected override async Task<PredictionResult> DoPredictAsync(int targetConcurso)
        {
            var startTime = DateTime.Now;

            try
            {
                UpdateStatus($"Gerando predição para concurso {targetConcurso}...");

                // Selecionar as 15 dezenas com menores frequências
                var selectedNumbers = SelectAntiFrequencyNumbers();

                if (selectedNumbers.Count != 15)
                {
                    var errorMsg = $"Erro na seleção: {selectedNumbers.Count} dezenas selecionadas em vez de 15";
                    UpdateStatus(errorMsg);
                    return PredictionResult.CreateError(ModelName, errorMsg);
                }

                // Calcular confiança da predição
                var confidence = CalculatePredictionConfidence(selectedNumbers);

                // Gerar explicação
                var explanation = GenerateExplanation(selectedNumbers);

                var result = PredictionResult.CreateSuccess(ModelName, selectedNumbers, confidence, explanation);
                
                // Adicionar metadados
                result.TargetConcurso = targetConcurso;
                result.ProcessingTime = DateTime.Now - startTime;
                result.ModelVersion = "2.0";
                result.AddMetadata("TrainingDataSize", TrainingDataSize);
                result.AddMetadata("LastTrainingTime", LastTrainingTime);
                result.AddMetadata("JanelaHistorica", GetParameter<int>("JanelaHistorica"));

                // Adicionar probabilidades individuais
                foreach (var number in selectedNumbers)
                {
                    result.NumberProbabilities[number] = _antiFrequencyScores.GetValueOrDefault(number, 0.0);
                }

                UpdateStatus($"Predição gerada com sucesso. Confiança: {confidence:P2}");
                return result;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Erro na predição: {ex.Message}";
                UpdateStatus(errorMsg);
                return PredictionResult.CreateError(ModelName, errorMsg);
            }
        }

        protected override async Task<ValidationResult> DoValidateAsync(Lances validationData)
        {
            try
            {
                UpdateStatus("Iniciando validação do modelo...");

                if (validationData == null || validationData.Count == 0)
                {
                    var errorMsg = "Dados de validação vazios";
                    UpdateStatus(errorMsg);
                    return ValidationResult.CreateError(ModelName, errorMsg);
                }

                var validationResult = new ValidationResult(ModelName);
                var maxTests = Math.Min(validationData.Count, GetParameter<int>("MaxTestCount"));

                for (int i = 0; i < maxTests; i++)
                {
                    var lance = validationData[i];
                    var predictionStart = DateTime.Now;
                    
                    var prediction = await DoPredictAsync(lance.Id);
                    var predictionTime = DateTime.Now - predictionStart;

                    if (prediction.Success)
                    {
                        var detail = ValidationDetail.FromLance(lance, prediction.PredictedNumbers, prediction.Confidence, predictionTime);
                        validationResult.AddDetailedResult(detail);
                    }

                    // Atualizar status periodicamente
                    if (i % 10 == 0)
                    {
                        UpdateStatus($"Validação: {i + 1}/{maxTests} testes processados");
                    }
                }

                validationResult.FinishValidation();
                
                var accuracyMsg = $"Validação concluída. Precisão: {validationResult.AccuracyPercentage:F2}%";
                UpdateStatus(accuracyMsg);
                
                return validationResult;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Erro na validação: {ex.Message}";
                UpdateStatus(errorMsg);
                return ValidationResult.CreateError(ModelName, errorMsg);
            }
        }

        protected override void DoReset()
        {
            _numberFrequencies?.Clear();
            _antiFrequencyScores?.Clear();
            ResetToDefaults();
            UpdateStatus("Modelo resetado para configurações padrão");
        }
        #endregion

        #region IConfigurableModel Implementation
        public void UpdateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                if (DefaultParameters.ContainsKey(param.Key))
                {
                    CurrentParameters[param.Key] = param.Value;
                }
                else
                {
                    throw new ArgumentException($"Parâmetro '{param.Key}' não é válido para este modelo");
                }
            }

            UpdateStatus("Parâmetros atualizados");
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return false;

            foreach (var param in parameters)
            {
                // Verificar se o parâmetro existe
                if (!DefaultParameters.ContainsKey(param.Key))
                {
                    return false;
                }

                // Verificar valores permitidos se definidos
                if (_allowedValues.ContainsKey(param.Key))
                {
                    var allowedValues = _allowedValues[param.Key];
                    if (allowedValues != null && !allowedValues.Contains(param.Value))
                    {
                        return false;
                    }
                }

                // Validação específica por parâmetro
                if (!ValidateSpecificParameter(param.Key, param.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public string GetParameterDescription(string parameterName)
        {
            return _parameterDescriptions.TryGetValue(parameterName, out var description) 
                ? description 
                : $"Parâmetro {parameterName}";
        }

        public List<object> GetAllowedValues(string parameterName)
        {
            return _allowedValues.TryGetValue(parameterName, out var values) 
                ? new List<object>(values) 
                : null;
        }

        public void ResetToDefaults()
        {
            CurrentParameters = new Dictionary<string, object>(DefaultParameters);
        }
        #endregion

        #region Private Methods
        private void InitializeParameters()
        {
            DefaultParameters = new Dictionary<string, object>
            {
                { "JanelaHistorica", 100 },
                { "FatorDecaimento", 0.1 },
                { "Epsilon", 0.001 },
                { "PesoTemporal", 0.8 },
                { "MinimumDataSize", 20 },
                { "MaxTestCount", 100 }
            };

            _parameterDescriptions = new Dictionary<string, string>
            {
                { "JanelaHistorica", "Número de sorteios recentes a considerar na análise de frequência" },
                { "FatorDecaimento", "Fator de decaimento temporal para ponderar frequências antigas" },
                { "Epsilon", "Valor mínimo para evitar divisão por zero nos cálculos" },
                { "PesoTemporal", "Peso da componente temporal na pontuação final" },
                { "MinimumDataSize", "Tamanho mínimo dos dados de treino necessários" },
                { "MaxTestCount", "Número máximo de testes na validação" }
            };

            _allowedValues = new Dictionary<string, List<object>>();
        }

        private bool ValidateSpecificParameter(string parameterName, object value)
        {
            switch (parameterName)
            {
                case "JanelaHistorica":
                    return value is int janela && janela > 0 && janela <= 1000;
                    
                case "FatorDecaimento":
                    return value is double fator && fator >= 0.0 && fator <= 1.0;
                    
                case "Epsilon":
                    return value is double eps && eps > 0.0 && eps <= 0.1;
                    
                case "PesoTemporal":
                    return value is double peso && peso >= 0.0 && peso <= 1.0;
                    
                case "MinimumDataSize":
                    return value is int size && size > 0 && size <= 500;
                    
                case "MaxTestCount":
                    return value is int count && count > 0 && count <= 1000;
                    
                default:
                    return true;
            }
        }

        private T GetParameter<T>(string name)
        {
            if (CurrentParameters.TryGetValue(name, out var value) && value is T)
            {
                return (T)value;
            }
            
            if (DefaultParameters.TryGetValue(name, out var defaultValue) && defaultValue is T)
            {
                return (T)defaultValue;
            }
            
            return default(T);
        }

        private async Task CalculateFrequencies(Lances trainingData)
        {
            var janelaHistorica = GetParameter<int>("JanelaHistorica");
            var dataToAnalyze = trainingData.TakeLast(janelaHistorica).ToList();

            foreach (var lance in dataToAnalyze)
            {
                foreach (var numero in lance.Lista)
                {
                    _numberFrequencies[numero]++;
                }
            }
        }

        private async Task CalculateAntiFrequencyScores()
        {
            var epsilon = GetParameter<double>("Epsilon");
            var pesoTemporal = GetParameter<double>("PesoTemporal");
            
            // Encontrar frequência máxima para normalização
            var maxFreq = _numberFrequencies.Values.Max();
            
            for (int numero = 1; numero <= 25; numero++)
            {
                var freq = _numberFrequencies[numero];
                var normalizedFreq = maxFreq > 0 ? (double)freq / maxFreq : 0.0;
                
                // Score anti-frequência: quanto menor a frequência, maior o score
                var antiFreqScore = 1.0 - normalizedFreq;
                
                // Aplicar peso temporal e epsilon
                _antiFrequencyScores[numero] = (antiFreqScore * pesoTemporal) + epsilon;
            }
        }

        private List<int> SelectAntiFrequencyNumbers()
        {
            // Ordenar por score anti-frequência (maior score = menor frequência)
            var sortedNumbers = _antiFrequencyScores
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key) // Desempate por número menor
                .Take(15)
                .Select(kv => kv.Key)
                .OrderBy(n => n)
                .ToList();

            return sortedNumbers;
        }

        private double CalculateModelConfidence()
        {
            if (_antiFrequencyScores == null || _antiFrequencyScores.Count == 0)
                return 0.0;

            // Confiança baseada na variância dos scores
            var scores = _antiFrequencyScores.Values.ToList();
            var avgScore = scores.Average();
            var variance = scores.Select(s => Math.Pow(s - avgScore, 2)).Average();
            
            // Converter variância em confiança (menor variância = maior confiança)
            var confidence = Math.Max(0.1, Math.Min(0.9, 1.0 - Math.Sqrt(variance)));
            
            return confidence;
        }

        private double CalculatePredictionConfidence(List<int> selectedNumbers)
        {
            if (selectedNumbers == null || selectedNumbers.Count == 0)
                return 0.0;

            // Confiança baseada na consistência dos scores das dezenas selecionadas
            var scores = selectedNumbers.Select(n => _antiFrequencyScores.GetValueOrDefault(n, 0.0)).ToList();
            var avgScore = scores.Average();
            var variance = scores.Select(s => Math.Pow(s - avgScore, 2)).Average();
            
            // Converter variância em confiança
            var confidence = Math.Max(0.1, Math.Min(0.9, 1.0 - Math.Sqrt(variance) * 2));
            
            return confidence;
        }

        private string GenerateExplanation(List<int> selectedNumbers)
        {
            var janelaHistorica = GetParameter<int>("JanelaHistorica");
            var explanation = $"Selecionadas as 15 dezenas com menores frequências nos últimos {janelaHistorica} sorteios. ";
            
            if (selectedNumbers.Any())
            {
                var minFreq = selectedNumbers.Min(n => _numberFrequencies.GetValueOrDefault(n, 0));
                var maxFreq = selectedNumbers.Max(n => _numberFrequencies.GetValueOrDefault(n, 0));
                
                explanation += $"Frequências das selecionadas: {minFreq} a {maxFreq} aparições.";
            }
            
            return explanation;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtém estatísticas detalhadas do modelo
        /// </summary>
        public Dictionary<string, object> GetModelStatistics()
        {
            var stats = new Dictionary<string, object>
            {
                { "ModelName", ModelName },
                { "ModelType", ModelType },
                { "IsInitialized", IsInitialized },
                { "Confidence", Confidence },
                { "TrainingDataSize", TrainingDataSize },
                { "LastTrainingTime", LastTrainingTime }
            };

            if (_numberFrequencies != null && _numberFrequencies.Any())
            {
                stats.Add("MinFrequency", _numberFrequencies.Values.Min());
                stats.Add("MaxFrequency", _numberFrequencies.Values.Max());
                stats.Add("AvgFrequency", _numberFrequencies.Values.Average());
            }

            if (_antiFrequencyScores != null && _antiFrequencyScores.Any())
            {
                stats.Add("MinAntiScore", _antiFrequencyScores.Values.Min());
                stats.Add("MaxAntiScore", _antiFrequencyScores.Values.Max());
                stats.Add("AvgAntiScore", _antiFrequencyScores.Values.Average());
            }

            return stats;
        }

        /// <summary>
        /// Obtém a distribuição de frequências atual
        /// </summary>
        public Dictionary<int, int> GetFrequencyDistribution()
        {
            return _numberFrequencies != null 
                ? new Dictionary<int, int>(_numberFrequencies) 
                : new Dictionary<int, int>();
        }

        /// <summary>
        /// Obtém os scores anti-frequência atuais
        /// </summary>
        public Dictionary<int, double> GetAntiFrequencyScores()
        {
            return _antiFrequencyScores != null 
                ? new Dictionary<int, double>(_antiFrequencyScores) 
                : new Dictionary<int, double>();
        }
        #endregion
    }
}