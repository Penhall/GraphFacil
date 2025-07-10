// D:\PROJETOS\GraphFacil\Library\PredictionModels\Individual\MetronomoModel.cs
// IMPLEMENTAÇÃO COMPLETA E CORRIGIDA - MetronomoModel
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.Individual
{
    /// <summary>
    /// Modelo Metronomo - IMPLEMENTAÇÃO COMPLETA
    /// Modelo original baseado em oscilação e sincronização de dezenas
    /// Herda de PredictionModelBase para implementação completa da IPredictionModel
    /// </summary>
    public class MetronomoModel : PredictionModelBase, IConfigurableModel
    {
        #region IPredictionModel Properties Implementation
        public override string ModelName => "Metronomo Model";
        public override string ModelType => "Individual-Oscillation";
        #endregion

        #region IConfigurableModel Properties
        public Dictionary<string, object> CurrentParameters { get; private set; }
        public Dictionary<string, object> DefaultParameters { get; private set; }
        #endregion

        #region Private Fields
        private Dictionary<int, MetronomoIndividual> _metronomos;
        private Dictionary<int, double> _numberWeights;
        private Dictionary<int, int> _numberLastAppearance;
        private Dictionary<string, string> _parameterDescriptions;
        private Dictionary<string, List<object>> _allowedValues;
        private Random _random;
        private int _concursoAlvo;
        #endregion

        #region Constructor
        public MetronomoModel()
        {
            InitializeParameters();
            ResetToDefaults();
            _metronomos = new Dictionary<int, MetronomoIndividual>();
            _numberWeights = new Dictionary<int, double>();
            _numberLastAppearance = new Dictionary<int, int>();
            _random = new Random(GetParameter<int>("RandomSeed"));
        }

        /// <summary>
        /// Construtor para compatibilidade com código legado
        /// </summary>
        public MetronomoModel(Lances historico) : this()
        {
            if (historico != null && historico.Any())
            {
                // Inicialização síncrona para compatibilidade
                var task = Task.Run(async () => await InitializeAsync(historico));
                task.Wait();
            }
        }
        #endregion

        #region PredictionModelBase Implementation
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                UpdateStatus("Inicializando modelo Metronomo...");

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

                // Determinar concurso alvo
                _concursoAlvo = (historicalData.LastOrDefault()?.Id ?? 0) + 1;

                // Inicializar estruturas de dados
                InitializeDataStructures();

                // Configurar metrônomos para todas as dezenas
                await InitializeMetronomos(historicalData);

                UpdateStatus("Modelo Metronomo inicializado com sucesso");
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
                UpdateStatus("Iniciando treinamento do modelo Metronomo...");

                if (trainingData == null || trainingData.Count == 0)
                {
                    UpdateStatus("Erro: Dados de treino inválidos");
                    return false;
                }

                // Analisar padrões históricos
                await AnalyzeHistoricalPatterns(trainingData);

                // Calcular pesos baseados no algoritmo Metronomo
                await CalculateMetronomoWeights(trainingData);

                // Configurar metrônomos individuais
                await ConfigureIndividualMetronomos(trainingData);

                // Calcular confiança baseada na consistência
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
                UpdateStatus($"Gerando predição Metronomo para concurso {targetConcurso}...");

                // Aplicar algoritmo Metronomo para seleção de dezenas
                var selectedNumbers = ApplyMetronomoAlgorithm(targetConcurso);

                if (selectedNumbers.Count != 15)
                {
                    var errorMsg = $"Erro na seleção: {selectedNumbers.Count} dezenas selecionadas em vez de 15";
                    UpdateStatus(errorMsg);
                    return PredictionResult.CreateError(ModelName, errorMsg);
                }

                // Calcular confiança da predição
                var confidence = CalculatePredictionConfidence(selectedNumbers);

                // Gerar explicação
                var explanation = GenerateExplanation(selectedNumbers, targetConcurso);

                var result = PredictionResult.CreateSuccess(ModelName, selectedNumbers, confidence, explanation);
                
                // Adicionar metadados
                result.TargetConcurso = targetConcurso;
                result.ProcessingTime = DateTime.Now - startTime;
                result.ModelVersion = "2.0";
                result.AddMetadata("TrainingDataSize", TrainingDataSize);
                result.AddMetadata("LastTrainingTime", LastTrainingTime);
                result.AddMetadata("BaseConfidence", GetParameter<double>("BaseConfidence"));
                result.AddMetadata("OscillationFactor", GetParameter<double>("OscillationFactor"));

                // Adicionar probabilidades individuais
                foreach (var number in selectedNumbers)
                {
                    result.NumberProbabilities[number] = _numberWeights.GetValueOrDefault(number, 0.0);
                }

                UpdateStatus($"Predição Metronomo gerada com sucesso. Confiança: {confidence:P2}");
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
                UpdateStatus("Iniciando validação do modelo Metronomo...");

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
            _metronomos?.Clear();
            _numberWeights?.Clear();
            _numberLastAppearance?.Clear();
            _concursoAlvo = 0;
            
            // Recriar gerador aleatório
            _random = new Random(GetParameter<int>("RandomSeed"));
            
            ResetToDefaults();
            UpdateStatus("Modelo Metronomo resetado para configurações padrão");
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

            // Recriar gerador aleatório se a semente mudou
            if (parameters.ContainsKey("RandomSeed"))
            {
                _random = new Random(GetParameter<int>("RandomSeed"));
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

        #region Private Methods - Core Algorithm
        private void InitializeParameters()
        {
            DefaultParameters = new Dictionary<string, object>
            {
                { "MinimumDataSize", 50 },
                { "BaseConfidence", 0.605 },
                { "MaxTestCount", 100 },
                { "RandomSeed", 42 },
                { "OscillationFactor", 0.5 },
                { "SyncThreshold", 0.7 },
                { "TemporalWeight", 0.6 },
                { "FrequencyWeight", 0.4 },
                { "ValidationSize", 100 }
            };

            _parameterDescriptions = new Dictionary<string, string>
            {
                { "MinimumDataSize", "Tamanho mínimo dos dados de treino necessários" },
                { "BaseConfidence", "Confiança base do modelo original Metronomo" },
                { "MaxTestCount", "Número máximo de testes na validação" },
                { "RandomSeed", "Semente para geração aleatória e reprodutibilidade" },
                { "OscillationFactor", "Fator de oscilação das dezenas (0.0 a 1.0)" },
                { "SyncThreshold", "Limite de sincronização entre metrônomos" },
                { "TemporalWeight", "Peso da componente temporal na decisão" },
                { "FrequencyWeight", "Peso da componente de frequência na decisão" },
                { "ValidationSize", "Tamanho do conjunto de validação" }
            };

            _allowedValues = new Dictionary<string, List<object>>();
        }

        private bool ValidateSpecificParameter(string parameterName, object value)
        {
            switch (parameterName)
            {
                case "MinimumDataSize":
                    return value is int size && size > 0 && size <= 1000;
                    
                case "BaseConfidence":
                    return value is double conf && conf >= 0.0 && conf <= 1.0;
                    
                case "MaxTestCount":
                    return value is int count && count > 0 && count <= 500;
                    
                case "RandomSeed":
                    return value is int;
                    
                case "OscillationFactor":
                    return value is double factor && factor >= 0.0 && factor <= 1.0;
                    
                case "SyncThreshold":
                    return value is double threshold && threshold >= 0.0 && threshold <= 1.0;
                    
                case "TemporalWeight":
                    return value is double weight && weight >= 0.0 && weight <= 1.0;
                    
                case "FrequencyWeight":
                    return value is double weight && weight >= 0.0 && weight <= 1.0;
                    
                case "ValidationSize":
                    return value is int valSize && valSize > 0 && valSize <= 500;
                    
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

        private void InitializeDataStructures()
        {
            _metronomos = new Dictionary<int, MetronomoIndividual>();
            _numberWeights = new Dictionary<int, double>();
            _numberLastAppearance = new Dictionary<int, int>();

            // Inicializar para todas as dezenas (1-25)
            for (int i = 1; i <= 25; i++)
            {
                _numberWeights[i] = 1.0;
                _numberLastAppearance[i] = 0;
                _metronomos[i] = new MetronomoIndividual(i);
            }
        }

        private async Task InitializeMetronomos(Lances historicalData)
        {
            foreach (var numero in Enumerable.Range(1, 25))
            {
                if (!_metronomos.ContainsKey(numero))
                {
                    _metronomos[numero] = new MetronomoIndividual(numero);
                }

                // Configurar metronomo com dados históricos
                _metronomos[numero].Initialize(historicalData);
            }
        }

        private async Task AnalyzeHistoricalPatterns(Lances trainingData)
        {
            // Analisar padrões de aparição das dezenas
            foreach (var numero in Enumerable.Range(1, 25))
            {
                var appearances = trainingData.Where(lance => lance.Lista.Contains(numero)).ToList();
                var frequency = (double)appearances.Count / trainingData.Count;
                
                // Calcular última aparição
                var lastAppearance = 0;
                for (int i = trainingData.Count - 1; i >= 0; i--)
                {
                    if (trainingData[i].Lista.Contains(numero))
                    {
                        lastAppearance = trainingData.Count - i;
                        break;
                    }
                }

                _numberLastAppearance[numero] = lastAppearance;
                
                // Atualizar metronomo individual
                if (_metronomos.ContainsKey(numero))
                {
                    _metronomos[numero].UpdateFrequency(frequency);
                    _metronomos[numero].UpdateLastAppearance(lastAppearance);
                }
            }
        }

        private async Task CalculateMetronomoWeights(Lances trainingData)
        {
            var oscillationFactor = GetParameter<double>("OscillationFactor");
            var temporalWeight = GetParameter<double>("TemporalWeight");
            var frequencyWeight = GetParameter<double>("FrequencyWeight");

            foreach (var numero in Enumerable.Range(1, 25))
            {
                // Componente de frequência
                var frequency = CalculateNumberFrequency(numero, trainingData);
                
                // Componente temporal (baseada na última aparição)
                var temporal = CalculateTemporalComponent(numero);
                
                // Componente de oscilação (algoritmo original)
                var oscillation = CalculateOscillationComponent(numero, oscillationFactor);
                
                // Peso final combinado
                var finalWeight = (frequency * frequencyWeight) + 
                                (temporal * temporalWeight) + 
                                (oscillation * (1.0 - temporalWeight - frequencyWeight));

                _numberWeights[numero] = Math.Max(0.01, finalWeight); // Mínimo para evitar zero
            }
        }

        private async Task ConfigureIndividualMetronomos(Lances trainingData)
        {
            var syncThreshold = GetParameter<double>("SyncThreshold");

            foreach (var metronomo in _metronomos.Values)
            {
                // Configurar sincronização com outros metrônomos
                metronomo.ConfigureSynchronization(_metronomos.Values.ToList(), syncThreshold);
                
                // Treinar com dados históricos
                metronomo.Train(trainingData);
            }
        }

        private double CalculateNumberFrequency(int numero, Lances trainingData)
        {
            var appearances = trainingData.Count(lance => lance.Lista.Contains(numero));
            return trainingData.Count > 0 ? (double)appearances / trainingData.Count : 0.0;
        }

        private double CalculateTemporalComponent(int numero)
        {
            var atraso = _numberLastAppearance[numero];
            // Converter atraso em peso (mais atraso = maior peso)
            return Math.Min(1.0, atraso / 50.0);
        }

        private double CalculateOscillationComponent(int numero, double factor)
        {
            // Simular oscilação baseada na posição do número e fator
            var phase = (numero * Math.PI) / 25.0;
            var oscillation = (Math.Sin(phase) + 1.0) / 2.0;
            
            return oscillation * factor;
        }

        private List<int> ApplyMetronomoAlgorithm(int targetConcurso)
        {
            // Usar algoritmo de seleção baseado em pesos e sincronização
            var candidateNumbers = new List<(int Number, double Weight)>();

            foreach (var numero in Enumerable.Range(1, 25))
            {
                var baseWeight = _numberWeights[numero];
                var metronomoWeight = _metronomos[numero].GetCurrentWeight();
                var syncBonus = _metronomos[numero].GetSynchronizationBonus();
                
                var finalWeight = baseWeight * metronomoWeight * (1.0 + syncBonus);
                candidateNumbers.Add((numero, finalWeight));
            }

            // Selecionar top 15 com alguma aleatoriedade
            var selectedNumbers = candidateNumbers
                .OrderByDescending(x => x.Weight)
                .ThenBy(x => _random.NextDouble()) // Adicionar aleatoriedade para empates
                .Take(15)
                .Select(x => x.Number)
                .OrderBy(n => n)
                .ToList();

            return selectedNumbers;
        }

        private double CalculateModelConfidence()
        {
            var baseConfidence = GetParameter<double>("BaseConfidence");
            
            if (_numberWeights == null || !_numberWeights.Any())
                return baseConfidence;

            // Calcular confiança baseada na variância dos pesos
            var weights = _numberWeights.Values.ToList();
            var avgWeight = weights.Average();
            var variance = weights.Select(w => Math.Pow(w - avgWeight, 2)).Average();
            
            // Menor variância = maior confiança
            var confidenceAdjustment = Math.Max(-0.1, Math.Min(0.1, (0.1 - variance) * 0.5));
            
            return Math.Max(0.1, Math.Min(0.9, baseConfidence + confidenceAdjustment));
        }

        private double CalculatePredictionConfidence(List<int> selectedNumbers)
        {
            if (selectedNumbers == null || selectedNumbers.Count == 0)
                return 0.0;

            // Confiança baseada na consistência dos pesos das dezenas selecionadas
            var weights = selectedNumbers.Select(n => _numberWeights.GetValueOrDefault(n, 0.0)).ToList();
            var avgWeight = weights.Average();
            var variance = weights.Select(w => Math.Pow(w - avgWeight, 2)).Average();
            
            // Converter variância em confiança
            var confidence = Math.Max(0.1, Math.Min(0.9, 1.0 - Math.Sqrt(variance) * 1.5));
            
            return confidence;
        }

        private string GenerateExplanation(List<int> selectedNumbers, int targetConcurso)
        {
            var explanation = $"Predição Metronomo para concurso {targetConcurso}: ";
            explanation += "Dezenas selecionadas com base em oscilação, frequência histórica e sincronização temporal. ";
            
            if (selectedNumbers.Any())
            {
                var avgWeight = selectedNumbers.Select(n => _numberWeights.GetValueOrDefault(n, 0.0)).Average();
                explanation += $"Peso médio das selecionadas: {avgWeight:F3}. ";
                
                var atrasoMedio = selectedNumbers.Select(n => _numberLastAppearance.GetValueOrDefault(n, 0)).Average();
                explanation += $"Atraso médio: {atrasoMedio:F1} sorteios.";
            }
            
            return explanation;
        }
        #endregion

        #region Public Methods - Compatibility and Statistics
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
                { "LastTrainingTime", LastTrainingTime },
                { "ConcursoAlvo", _concursoAlvo }
            };

            if (_numberWeights != null && _numberWeights.Any())
            {
                stats.Add("MinWeight", _numberWeights.Values.Min());
                stats.Add("MaxWeight", _numberWeights.Values.Max());
                stats.Add("AvgWeight", _numberWeights.Values.Average());
            }

            if (_metronomos != null && _metronomos.Any())
            {
                stats.Add("ActiveMetronomos", _metronomos.Count);
                stats.Add("SynchronizedMetronomos", _metronomos.Values.Count(m => m.IsSynchronized));
            }

            return stats;
        }

        /// <summary>
        /// Obtém os pesos atuais das dezenas
        /// </summary>
        public Dictionary<int, double> GetNumberWeights()
        {
            return _numberWeights != null 
                ? new Dictionary<int, double>(_numberWeights) 
                : new Dictionary<int, double>();
        }

        /// <summary>
        /// Obtém informações dos metrônomos individuais
        /// </summary>
        public Dictionary<int, MetronomoIndividual> GetMetronomos()
        {
            return _metronomos != null 
                ? new Dictionary<int, MetronomoIndividual>(_metronomos) 
                : new Dictionary<int, MetronomoIndividual>();
        }

        /// <summary>
        /// Método de compatibilidade com código legado
        /// </summary>
        public async Task<List<int>> GerarPalpite(int concursoAlvo)
        {
            var result = await DoPredictAsync(concursoAlvo);
            return result.Success ? result.PredictedNumbers : new List<int>();
        }
        #endregion
    }

    #region Supporting Classes
    /// <summary>
    /// Metronomo individual para cada dezena
    /// </summary>
    public class MetronomoIndividual
    {
        public int Numero { get; }
        public double Frequency { get; private set; }
        public int LastAppearance { get; private set; }
        public double Phase { get; private set; }
        public double SynchronizationLevel { get; private set; }
        public bool IsSynchronized { get; private set; }

        private List<MetronomoIndividual> _synchronizedWith;
        private double _syncThreshold;

        public MetronomoIndividual(int numero)
        {
            Numero = numero;
            Frequency = 0.0;
            LastAppearance = 0;
            Phase = 0.0;
            SynchronizationLevel = 0.0;
            IsSynchronized = false;
            _synchronizedWith = new List<MetronomoIndividual>();
        }

        public void Initialize(Lances historicalData)
        {
            // Calcular frequência inicial
            var appearances = historicalData.Count(lance => lance.Lista.Contains(Numero));
            Frequency = historicalData.Count > 0 ? (double)appearances / historicalData.Count : 0.0;

            // Calcular última aparição
            for (int i = historicalData.Count - 1; i >= 0; i--)
            {
                if (historicalData[i].Lista.Contains(Numero))
                {
                    LastAppearance = historicalData.Count - i;
                    break;
                }
            }

            // Calcular fase inicial baseada na posição e frequência
            Phase = (Numero * Math.PI / 25.0) + (Frequency * Math.PI);
        }

        public void UpdateFrequency(double frequency)
        {
            Frequency = frequency;
        }

        public void UpdateLastAppearance(int lastAppearance)
        {
            LastAppearance = lastAppearance;
        }

        public void ConfigureSynchronization(List<MetronomoIndividual> otherMetronomos, double syncThreshold)
        {
            _syncThreshold = syncThreshold;
            _synchronizedWith.Clear();

            foreach (var other in otherMetronomos)
            {
                if (other.Numero != this.Numero)
                {
                    var phaseDiff = Math.Abs(this.Phase - other.Phase);
                    var normalizedDiff = Math.Min(phaseDiff, 2 * Math.PI - phaseDiff) / Math.PI;
                    
                    if (normalizedDiff < (1.0 - _syncThreshold))
                    {
                        _synchronizedWith.Add(other);
                    }
                }
            }

            SynchronizationLevel = _synchronizedWith.Count / 24.0; // 24 outras dezenas
            IsSynchronized = SynchronizationLevel >= _syncThreshold;
        }

        public void Train(Lances trainingData)
        {
            // Ajustar fase baseada no padrão histórico
            var recentAppearances = trainingData
                .TakeLast(50)
                .Where(lance => lance.Lista.Contains(Numero))
                .Count();

            var recentFrequency = recentAppearances / 50.0;
            var frequencyDiff = recentFrequency - Frequency;

            // Ajustar fase baseada na mudança de frequência
            Phase += frequencyDiff * Math.PI;
            Phase = Phase % (2 * Math.PI);
        }

        public double GetCurrentWeight()
        {
            // Peso baseado em fase, frequência e última aparição
            var phaseWeight = (Math.Sin(Phase) + 1.0) / 2.0;
            var frequencyWeight = 1.0 - Frequency; // Anti-frequência
            var delayWeight = Math.Min(1.0, LastAppearance / 30.0);

            return (phaseWeight + frequencyWeight + delayWeight) / 3.0;
        }

        public double GetSynchronizationBonus()
        {
            return IsSynchronized ? SynchronizationLevel * 0.2 : 0.0; // Até 20% de bônus
        }

        public override string ToString()
        {
            return $"Metronomo {Numero}: Freq={Frequency:F3}, Delay={LastAppearance}, Sync={IsSynchronized}";
        }
    }
    #endregion
}