// D:\PROJETOS\GraphFacil\LotoLibrary\PredictionModels\AntiFrequency\AntiFrequencySimpleModel.cs
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.AntiFrequency
{
    /// <summary>
    /// Modelo Anti-Frequencista Simples
    /// Implementa estratégia de inversão direta do frequencista
    /// Prioriza dezenas com menor frequência histórica
    /// </summary>
    public class AntiFrequencySimpleModel : IPredictionModel, IConfigurableModel
    {
        #region Properties
        public string ModelName => "Anti-Frequency Simple";
        public string ModelType => "AntiFrequency";
        public bool IsInitialized { get; private set; }
        public double Confidence { get; private set; }
        public DateTime LastTrainingTime { get; private set; }
        public int TrainingDataSize { get; private set; }
        
        public Dictionary<string, object> Parameters { get; set; }
        #endregion

        #region Events
        public event EventHandler<string> OnStatusChanged;
        public event EventHandler<double> OnConfidenceChanged;
        #endregion

        #region Private Fields
        private Lances _historicalData;
        private Dictionary<int, double> _frequencyScores;
        private Dictionary<int, double> _antiFrequencyScores;
        private int _janelaHistorica;
        private double _fatorDecaimento;
        private double _epsilon;
        private double _pesoTemporal;
        #endregion

        #region Constructor
        public AntiFrequencySimpleModel()
        {
            Parameters = GetDefaultParameters();
            LoadParametersFromConfig();
            IsInitialized = false;
            Confidence = 0.0;
        }
        #endregion

        #region IPredictionModel Implementation
        public async Task<bool> InitializeAsync(Lances historicalData)
        {
            try
            {
                OnStatusChanged?.Invoke(this, "Inicializando modelo Anti-Frequency Simple...");
                
                if (historicalData == null || historicalData.Count == 0)
                {
                    OnStatusChanged?.Invoke(this, "❌ Dados históricos inválidos");
                    return false;
                }

                _historicalData = historicalData;
                TrainingDataSize = historicalData.Count;
                
                // Calcular frequências históricas
                await CalculateFrequencyScoresAsync();
                
                // Calcular scores anti-frequencistas
                await CalculateAntiFrequencyScoresAsync();
                
                IsInitialized = true;
                LastTrainingTime = DateTime.Now;
                
                OnStatusChanged?.Invoke(this, "✅ Modelo inicializado com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"❌ Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TrainAsync(Lances trainingData)
        {
            try
            {
                OnStatusChanged?.Invoke(this, "Treinando modelo Anti-Frequency Simple...");
                
                // Para este modelo simples, treinamento = inicialização
                var result = await InitializeAsync(trainingData);
                
                if (result)
                {
                    // Calcular confiança baseada na estabilidade dos scores
                    await CalculateConfidenceAsync();
                    OnConfidenceChanged?.Invoke(this, Confidence);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"❌ Erro no treinamento: {ex.Message}");
                return false;
            }
        }

        public async Task<PredictionResult> PredictAsync(int targetConcurso)
        {
            try
            {
                if (!IsInitialized)
                {
                    OnStatusChanged?.Invoke(this, "❌ Modelo não foi inicializado");
                    return new PredictionResult
                    {
                        Success = false,
                        ErrorMessage = "Modelo não inicializado"
                    };
                }

                OnStatusChanged?.Invoke(this, "Gerando predição anti-frequencista...");

                // Aplicar peso temporal aos scores
                var weightedScores = await ApplyTemporalWeightAsync();
                
                // Ordenar por score anti-frequencista (maiores = menos frequentes)
                var rankedNumbers = weightedScores
                    .OrderByDescending(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .Take(15)
                    .ToList();

                var result = new PredictionResult
                {
                    Success = true,
                    ModelName = ModelName,
                    TargetConcurso = targetConcurso,
                    PredictedNumbers = rankedNumbers,
                    Confidence = Confidence,
                    GeneratedAt = DateTime.Now,
                    Explanation = await GenerateExplanationAsync(rankedNumbers, weightedScores)
                };

                OnStatusChanged?.Invoke(this, "✅ Predição gerada com sucesso");
                return result;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"❌ Erro na predição: {ex.Message}");
                return new PredictionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ValidationResult> ValidateAsync(Lances validationData)
        {
            try
            {
                OnStatusChanged?.Invoke(this, "Validando modelo...");
                
                var validationResult = new ValidationResult
                {
                    ModelName = ModelName,
                    TestStartTime = DateTime.Now,
                    TotalTests = 0,
                    SuccessfulPredictions = 0,
                    Accuracy = 0.0,
                    DetailedResults = new List<ValidationDetail>()
                };

                // Validação com últimos 50 sorteios
                var testCount = Math.Min(50, validationData.Count - 1);
                var trainingSize = validationData.Count - testCount;

                for (int i = 0; i < testCount; i++)
                {
                    var trainingData = new Lances(validationData.Take(trainingSize + i).ToList());
                    var testLance = validationData[trainingSize + i];
                    
                    // Treinar com dados até o ponto atual
                    await TrainAsync(trainingData);
                    
                    // Gerar predição
                    var prediction = await PredictAsync(testLance.Concurso);
                    
                    if (prediction.Success)
                    {
                        var hits = prediction.PredictedNumbers.Intersect(testLance.DezenasSorteadas).Count();
                        var isSuccess = hits >= 11; // Critério de sucesso: 11+ acertos
                        
                        validationResult.TotalTests++;
                        if (isSuccess) validationResult.SuccessfulPredictions++;
                        
                        validationResult.DetailedResults.Add(new ValidationDetail
                        {
                            Concurso = testLance.Concurso,
                            PredictedNumbers = prediction.PredictedNumbers,
                            ActualNumbers = testLance.DezenasSorteadas.ToList(),
                            Hits = hits,
                            Success = isSuccess,
                            Confidence = prediction.Confidence
                        });
                    }
                }

                validationResult.Accuracy = validationResult.TotalTests > 0 ? 
                    (double)validationResult.SuccessfulPredictions / validationResult.TotalTests : 0.0;
                
                validationResult.TestEndTime = DateTime.Now;
                
                OnStatusChanged?.Invoke(this, $"✅ Validação concluída: {validationResult.Accuracy:P2}");
                return validationResult;
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"❌ Erro na validação: {ex.Message}");
                return new ValidationResult
                {
                    ModelName = ModelName,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public void Reset()
        {
            IsInitialized = false;
            Confidence = 0.0;
            _frequencyScores?.Clear();
            _antiFrequencyScores?.Clear();
            _historicalData = null;
            OnStatusChanged?.Invoke(this, "Modelo resetado");
        }
        #endregion

        #region IConfigurableModel Implementation
        public void UpdateParameters(Dictionary<string, object> newParameters)
        {
            if (newParameters == null) return;
            
            foreach (var param in newParameters)
            {
                if (Parameters.ContainsKey(param.Key))
                {
                    Parameters[param.Key] = param.Value;
                }
            }
            
            LoadParametersFromConfig();
            OnStatusChanged?.Invoke(this, "Parâmetros atualizados");
        }

        public Dictionary<string, object> GetDefaultParameters()
        {
            return new Dictionary<string, object>
            {
                { "JanelaHistorica", 100 },
                { "FatorDecaimento", 0.1 },
                { "Epsilon", 0.001 },
                { "PesoTemporal", 0.8 }
            };
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return false;
            
            try
            {
                var janela = Convert.ToInt32(parameters["JanelaHistorica"]);
                var fator = Convert.ToDouble(parameters["FatorDecaimento"]);
                var epsilon = Convert.ToDouble(parameters["Epsilon"]);
                var peso = Convert.ToDouble(parameters["PesoTemporal"]);
                
                return janela > 0 && janela <= 1000 &&
                       fator > 0 && fator <= 1 &&
                       epsilon > 0 && epsilon <= 0.1 &&
                       peso > 0 && peso <= 1;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Private Methods
        private void LoadParametersFromConfig()
        {
            _janelaHistorica = Convert.ToInt32(Parameters["JanelaHistorica"]);
            _fatorDecaimento = Convert.ToDouble(Parameters["FatorDecaimento"]);
            _epsilon = Convert.ToDouble(Parameters["Epsilon"]);
            _pesoTemporal = Convert.ToDouble(Parameters["PesoTemporal"]);
        }

        private async Task CalculateFrequencyScoresAsync()
        {
            await Task.Run(() =>
            {
                _frequencyScores = new Dictionary<int, double>();
                
                // Usar apenas últimos N sorteios se especificado
                var dataToUse = _janelaHistorica > 0 && _janelaHistorica < _historicalData.Count
                    ? _historicalData.Skip(_historicalData.Count - _janelaHistorica).ToList()
                    : _historicalData.ToList();
                
                var totalSorteios = dataToUse.Count;
                
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var aparicoes = dataToUse.Count(lance => lance.DezenasSorteadas.Contains(dezena));
                    _frequencyScores[dezena] = (double)aparicoes / totalSorteios;
                }
            });
        }

        private async Task CalculateAntiFrequencyScoresAsync()
        {
            await Task.Run(() =>
            {
                _antiFrequencyScores = new Dictionary<int, double>();
                
                // Inverter frequências: menos frequente = score maior
                var maxFreq = _frequencyScores.Values.Max();
                var minFreq = _frequencyScores.Values.Min();
                
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var freq = _frequencyScores[dezena];
                    
                    // Fórmula anti-frequencista: score = 1 / (freq + epsilon)
                    // Normalizar para evitar valores extremos
                    var normalizedFreq = (freq - minFreq) / (maxFreq - minFreq + _epsilon);
                    _antiFrequencyScores[dezena] = 1.0 / (normalizedFreq + _epsilon);
                }
            });
        }

        private async Task<Dictionary<int, double>> ApplyTemporalWeightAsync()
        {
            return await Task.Run(() =>
            {
                var weightedScores = new Dictionary<int, double>();
                
                // Calcular peso temporal baseado na última aparição
                var recentData = _historicalData.Skip(Math.Max(0, _historicalData.Count - 20)).ToList();
                
                for (int dezena = 1; dezena <= 25; dezena++)
                {
                    var baseScore = _antiFrequencyScores[dezena];
                    var daysSinceLastAppearance = GetDaysSinceLastAppearance(dezena, recentData);
                    var temporalBonus = Math.Pow(_pesoTemporal, daysSinceLastAppearance);
                    
                    weightedScores[dezena] = baseScore * (1 + temporalBonus);
                }
                
                return weightedScores;
            });
        }

        private int GetDaysSinceLastAppearance(int dezena, List<Lance> recentData)
        {
            for (int i = recentData.Count - 1; i >= 0; i--)
            {
                if (recentData[i].DezenasSorteadas.Contains(dezena))
                {
                    return recentData.Count - 1 - i;
                }
            }
            return recentData.Count; // Não apareceu nos dados recentes
        }

        private async Task CalculateConfidenceAsync()
        {
            await Task.Run(() =>
            {
                // Confiança baseada na estabilidade dos scores
                var scores = _antiFrequencyScores.Values.ToList();
                var mean = scores.Average();
                var variance = scores.Average(x => Math.Pow(x - mean, 2));
                var standardDeviation = Math.Sqrt(variance);
                
                // Confiança inversamente proporcional ao desvio padrão
                var normalizedStdDev = standardDeviation / mean;
                Confidence = Math.Max(0.1, Math.Min(0.9, 1.0 - normalizedStdDev));
            });
        }

        private async Task<string> GenerateExplanationAsync(List<int> selectedNumbers, Dictionary<int, double> scores)
        {
            return await Task.Run(() =>
            {
                var explanation = $"Modelo Anti-Frequencista Simple - Seleção baseada em baixa frequência histórica\n\n";
                explanation += $"Parâmetros utilizados:\n";
                explanation += $"• Janela histórica: {_janelaHistorica} sorteios\n";
                explanation += $"• Fator decaimento: {_fatorDecaimento:F3}\n";
                explanation += $"• Peso temporal: {_pesoTemporal:F3}\n\n";
                
                explanation += "Dezenas selecionadas (score anti-frequencista):\n";
                foreach (var dezena in selectedNumbers.Take(5))
                {
                    var freq = _frequencyScores[dezena];
                    var score = scores[dezena];
                    explanation += $"• {dezena:D2}: freq={freq:P2}, score={score:F3}\n";
                }
                
                explanation += "\nRacional: Dezenas menos frequentes têm maior probabilidade de serem sorteadas (regressão à média)";
                return explanation;
            });
        }
        #endregion
    }
}
