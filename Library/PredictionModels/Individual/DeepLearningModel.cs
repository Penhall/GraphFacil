// E:\PROJETOS\GraphFacil\Library\PredictionModels\Individual\DeepLearningModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Validation;
using LotoLibrary.DeepLearning.Architectures;
using LotoLibrary.DeepLearning.Training;
using LotoLibrary.Enums;

namespace LotoLibrary.PredictionModels.Individual
{
    /// <summary>
    /// Modelo de predição baseado em Deep Learning
    /// NOTA: Versão mock até que as dependências do TensorFlow.NET sejam instaladas
    /// </summary>
    public class DeepLearningModel : IPredictionModel, IConfigurableModel
    {
        #region Fields
        private LstmAttentionNetwork _network;
        private ModelTrainer _trainer;
        private bool _isInitialized;
        private bool _isTrained;
        private Dictionary<string, object> _parameters;
        private Lances _trainingData;
        #endregion

        #region Properties
        public string ModelName => "Deep Learning Model";
        public string ModelVersion => "1.0.0";
        public ModelType ModelType => ModelType.DeepLearning;
        public bool IsInitialized => _isInitialized;
        public bool IsTrained => _isTrained;
        public Dictionary<string, object> Parameters => new Dictionary<string, object>(_parameters);
        public string Description => "Modelo de predição baseado em redes neurais LSTM com mecanismo de atenção";
        public double Confidence { get; private set; } = 0.0;
        #endregion

        #region Constructor
        public DeepLearningModel()
        {
            _parameters = new Dictionary<string, object>
            {
                ["SequenceLength"] = 10,
                ["FeatureSize"] = 25,
                ["Epochs"] = 100,
                ["LearningRate"] = 0.001,
                ["BatchSize"] = 32,
                ["ValidationSplit"] = 0.2,
                ["EarlyStopping"] = true,
                ["Patience"] = 10
            };
            
            _isInitialized = false;
            _isTrained = false;
        }
        #endregion

        #region IPredictionModel Implementation

        public async Task<bool> InitializeAsync(Lances historicalData)
        {
            if (historicalData == null || !historicalData.Any())
            {
                return false;
            }

            try
            {
                _trainingData = historicalData;
                
                // Inicializar componentes
                var sequenceLength = (int)_parameters["SequenceLength"];
                var featureSize = (int)_parameters["FeatureSize"];
                
                _network = new LstmAttentionNetwork(sequenceLength, featureSize);
                _trainer = new ModelTrainer(CreateTrainingConfiguration());
                
                // Configurar o modelo no trainer
                bool setupSuccess = _trainer.SetupModel(sequenceLength, featureSize);
                
                _isInitialized = setupSuccess && _network.IsInitialized;
                
                await Task.Delay(100); // Simular tempo de inicialização
                
                return _isInitialized;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na inicialização do modelo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TrainAsync(Lances historicalData)
        {
            if (!_isInitialized || historicalData == null || !historicalData.Any())
            {
                return false;
            }

            try
            {
                // Preparar dados para treinamento
                var (trainingData, targetData) = PrepareTrainingData(historicalData);
                
                // Treinar o modelo
                var trainingResult = _trainer.TrainModel(trainingData, targetData);
                
                _isTrained = trainingResult.Success;
                
                if (_isTrained)
                {
                    Confidence = trainingResult.FinalAccuracy;
                    Console.WriteLine($"Modelo treinado com sucesso. Acurácia: {Confidence:P2}");
                }
                else
                {
                    Console.WriteLine($"Falha no treinamento: {trainingResult.ErrorMessage}");
                }

                return _isTrained;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante o treinamento: {ex.Message}");
                return false;
            }
        }

        public async Task<List<int>> PredictAsync(int concurso)
        {
            if (!_isInitialized || !_isTrained)
            {
                return new List<int>();
            }

            try
            {
                // Preparar dados de entrada para predição
                var inputData = PrepareInputData(concurso);
                
                // Fazer predição
                var prediction = _network.Predict(inputData);
                
                // Converter predição para números da lotofácil
                var predictedNumbers = ConvertPredictionToNumbers(prediction);
                
                // Validar e ajustar se necessário
                var validNumbers = ValidateAndAdjustPrediction(predictedNumbers);
                
                await Task.Delay(50); // Simular tempo de processamento
                
                return validNumbers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante predição: {ex.Message}");
                return GenerateRandomPrediction(); // Fallback
            }
        }

        public async Task<ValidationResult> ValidateAsync(Lances testData)
        {
            if (!_isInitialized || testData == null || !testData.Any())
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "Modelo não inicializado ou dados de teste inválidos"
                };
            }

            try
            {
                var validationResults = new List<PredictionValidationResult>();
                
                // Validar contra dados de teste
                foreach (var lance in testData.Take(50)) // Limitar a 50 para performance
                {
                    var prediction = await PredictAsync(lance.Id);
                    
                    var validationResult = new PredictionValidationResult(
                        lance.Id, 
                        prediction, 
                        lance.Lista
                    )
                    {
                        TipoEstrategia = ModelName,
                        Confidence = Confidence
                    };
                    
                    validationResults.Add(validationResult);
                }

                // Calcular métricas de validação
                var totalTests = validationResults.Count;
                var successfulTests = validationResults.Count(r => r.Acertos >= 11);
                var averageAccuracy = validationResults.Average(r => r.TaxaAcerto);

                return new ValidationResult
                {
                    IsValid = true,
                    ModelName = ModelName,
                    Accuracy = averageAccuracy,
                    TotalTests = totalTests,
                    PassedTests = successfulTests,
                    ValidationMethod = "Deep Learning Validation",
                    Message = $"Validação concluída - {successfulTests}/{totalTests} testes bem-sucedidos"
                };
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = $"Erro durante validação: {ex.Message}"
                };
            }
        }

        #endregion

        #region IConfigurableModel Implementation

        public object GetParameter(string name)
        {
            return _parameters.GetValueOrDefault(name);
        }

        public void SetParameter(string name, object value)
        {
            if (_parameters.ContainsKey(name))
            {
                _parameters[name] = value;
            }
            else
            {
                _parameters.Add(name, value);
            }
        }

        public void UpdateParameters(Dictionary<string, object> newParameters)
        {
            foreach (var param in newParameters)
            {
                SetParameter(param.Key, param.Value);
            }
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            foreach (var param in parameters)
            {
                if (!ValidateParameter(param.Key, param.Value))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetParameterDescription(string name)
        {
            return name switch
            {
                "SequenceLength" => "Comprimento da sequência de entrada para a rede neural",
                "FeatureSize" => "Tamanho das características de entrada",
                "Epochs" => "Número de épocas para treinamento",
                "LearningRate" => "Taxa de aprendizado (0.0001 - 0.1)",
                "BatchSize" => "Tamanho do lote para treinamento",
                "ValidationSplit" => "Porcentagem dos dados para validação (0.0 - 0.5)",
                "EarlyStopping" => "Ativar parada antecipada durante treinamento",
                "Patience" => "Número de épocas sem melhoria antes da parada",
                _ => "Parâmetro desconhecido"
            };
        }

        public List<object> GetAllowedValues(string name)
        {
            return name switch
            {
                "SequenceLength" => new List<object> { 5, 10, 15, 20 },
                "FeatureSize" => new List<object> { 25 }, // Fixo para Lotofácil
                "Epochs" => new List<object> { 50, 100, 200, 500 },
                "LearningRate" => new List<object> { 0.0001, 0.001, 0.01, 0.1 },
                "BatchSize" => new List<object> { 16, 32, 64, 128 },
                "ValidationSplit" => new List<object> { 0.1, 0.2, 0.3 },
                "EarlyStopping" => new List<object> { true, false },
                "Patience" => new List<object> { 5, 10, 15, 20 },
                _ => new List<object>()
            };
        }

        public void ResetToDefaults()
        {
            _parameters.Clear();
            _parameters = new Dictionary<string, object>
            {
                ["SequenceLength"] = 10,
                ["FeatureSize"] = 25,
                ["Epochs"] = 100,
                ["LearningRate"] = 0.001,
                ["BatchSize"] = 32,
                ["ValidationSplit"] = 0.2,
                ["EarlyStopping"] = true,
                ["Patience"] = 10
            };
        }

        #endregion

        #region Private Methods

        private TrainingConfiguration CreateTrainingConfiguration()
        {
            return new TrainingConfiguration
            {
                Epochs = (int)_parameters["Epochs"],
                LearningRate = (double)_parameters["LearningRate"],
                BatchSize = (int)_parameters["BatchSize"],
                ValidationSplit = (double)_parameters["ValidationSplit"],
                EarlyStopping = (bool)_parameters["EarlyStopping"],
                Patience = (int)_parameters["Patience"]
            };
        }

        private (float[,], float[,]) PrepareTrainingData(Lances historicalData)
        {
            var sequenceLength = (int)_parameters["SequenceLength"];
            var featureSize = (int)_parameters["FeatureSize"];
            
            var dataList = historicalData.OrderBy(l => l.Id).ToList();
            var trainingData = new float[dataList.Count - sequenceLength, sequenceLength * featureSize];
            var targetData = new float[dataList.Count - sequenceLength, featureSize];

            for (int i = 0; i < dataList.Count - sequenceLength; i++)
            {
                // Preparar sequência de entrada
                for (int j = 0; j < sequenceLength; j++)
                {
                    var lance = dataList[i + j];
                    var encoded = EncodeNumbers(lance.Lista);
                    
                    for (int k = 0; k < featureSize; k++)
                    {
                        trainingData[i, j * featureSize + k] = encoded[k];
                    }
                }

                // Preparar target
                var targetLance = dataList[i + sequenceLength];
                var targetEncoded = EncodeNumbers(targetLance.Lista);
                for (int k = 0; k < featureSize; k++)
                {
                    targetData[i, k] = targetEncoded[k];
                }
            }

            return (trainingData, targetData);
        }

        private float[,] PrepareInputData(int concurso)
        {
            var sequenceLength = (int)_parameters["SequenceLength"];
            var featureSize = (int)_parameters["FeatureSize"];
            
            // Buscar dados históricos recentes
            var recentData = _trainingData.OrderByDescending(l => l.Id).Take(sequenceLength).Reverse().ToList();
            var inputData = new float[1, sequenceLength * featureSize];

            for (int i = 0; i < recentData.Count; i++)
            {
                var encoded = EncodeNumbers(recentData[i].Lista);
                for (int j = 0; j < featureSize; j++)
                {
                    inputData[0, i * featureSize + j] = encoded[j];
                }
            }

            return inputData;
        }

        private float[] EncodeNumbers(List<int> numbers)
        {
            var encoded = new float[25];
            foreach (var number in numbers)
            {
                if (number >= 1 && number <= 25)
                {
                    encoded[number - 1] = 1.0f;
                }
            }
            return encoded;
        }

        private List<int> ConvertPredictionToNumbers(float[] prediction)
        {
            var numbersWithProbs = new List<(int Number, float Probability)>();
            
            for (int i = 0; i < prediction.Length && i < 25; i++)
            {
                numbersWithProbs.Add((i + 1, prediction[i]));
            }

            return numbersWithProbs
                .OrderByDescending(x => x.Probability)
                .Take(15)
                .Select(x => x.Number)
                .OrderBy(x => x)
                .ToList();
        }

        private List<int> ValidateAndAdjustPrediction(List<int> prediction)
        {
            var validNumbers = prediction.Where(n => n >= 1 && n <= 25).Distinct().ToList();
            
            // Garantir que temos exatamente 15 números
            while (validNumbers.Count < 15)
            {
                var random = new Random();
                int newNumber;
                do
                {
                    newNumber = random.Next(1, 26);
                } while (validNumbers.Contains(newNumber));
                
                validNumbers.Add(newNumber);
            }

            return validNumbers.Take(15).OrderBy(n => n).ToList();
        }

        private List<int> GenerateRandomPrediction()
        {
            var random = new Random();
            var numbers = new List<int>();
            
            while (numbers.Count < 15)
            {
                var number = random.Next(1, 26);
                if (!numbers.Contains(number))
                {
                    numbers.Add(number);
                }
            }
            
            return numbers.OrderBy(n => n).ToList();
        }

        private bool ValidateParameter(string name, object value)
        {
            try
            {
                return name switch
                {
                    "SequenceLength" => value is int seq && seq >= 5 && seq <= 20,
                    "FeatureSize" => value is int feat && feat == 25,
                    "Epochs" => value is int epochs && epochs >= 10 && epochs <= 1000,
                    "LearningRate" => value is double lr && lr >= 0.0001 && lr <= 0.1,
                    "BatchSize" => value is int batch && batch >= 8 && batch <= 256,
                    "ValidationSplit" => value is double split && split >= 0.0 && split <= 0.5,
                    "EarlyStopping" => value is bool,
                    "Patience" => value is int patience && patience >= 1 && patience <= 50,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _network?.Dispose();
            _trainer = null;
            _trainingData = null;
            _parameters?.Clear();
            _isInitialized = false;
            _isTrained = false;
        }

        #endregion
    }
}