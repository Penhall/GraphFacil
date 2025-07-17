// E:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Simple\AntiFrequencySimpleModel.cs
using LotoLibrary.Enums;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Core;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.AntiFrequency.Simple
{
    /// <summary>
    /// Modelo Anti-Frequência Simples - prioriza números menos sorteados
    /// </summary>
    public class AntiFrequencySimpleModel : PredictionModelBase
    {
        #region Fields
        private Dictionary<int, int> _numberFrequencies;
        private Dictionary<int, DateTime> _lastAppearances;
        private int _analysisWindow;
        #endregion

        #region Properties
        public Dictionary<int, int> NumberFrequencies => new Dictionary<int, int>(_numberFrequencies);
        public int AnalysisWindow => _analysisWindow;

        public override string ModelName => "Anti-Frequency Simple Model";
        #endregion

        #region Constructor
        public AntiFrequencySimpleModel()
        {
            _numberFrequencies = new Dictionary<int, int>();
            _lastAppearances = new Dictionary<int, DateTime>();
            _analysisWindow = 50; // Analisar últimos 50 concursos por padrão

            // Inicializar contadores
            for (int i = 1; i <= 25; i++)
            {
                _numberFrequencies[i] = 0;
                _lastAppearances[i] = DateTime.MinValue;
            }
        }
        #endregion

        #region Abstract Methods Implementation

        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                // Calcular frequências iniciais
                CalculateFrequencies(historicalData);

                // Calcular última aparição de cada número
                CalculateLastAppearances(historicalData);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override async Task<bool> DoTrainAsync(Lances historicalData)
        {
            try
            {
                // Recalcular frequências com dados de treinamento
                CalculateFrequencies(historicalData);

                // Atualizar última aparição
                CalculateLastAppearances(historicalData);

                // Calcular métricas de treinamento
                var averageFrequency = _numberFrequencies.Values.Average();
                var standardDeviation = CalculateStandardDeviation(_numberFrequencies.Values);

                IsTrained = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override async Task<ValidationResult> DoValidateAsync(Lances testData)
        {
            try
            {
                if (!IsInitialized || testData == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Accuracy = 0.0,
                        Message = "Modelo não inicializado ou dados inválidos",
                        TotalTests = 0
                    };
                }

                // REAL VALIDATION: Test anti-frequency strategy against historical data
                var validationResults = await ValidateAntiFrequencyStrategy(testData);
                
                return new ValidationResult
                {
                    IsValid = true,
                    Accuracy = validationResults.accuracy,
                    Message = $"Validação real concluída: {validationResults.hitRate:P2} de acerto, {validationResults.averageHits:F1} acertos médios",
                    TotalTests = validationResults.totalTests
                };
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Accuracy = 0.0,
                    Message = $"Erro na validação: {ex.Message}",
                    TotalTests = 0
                };
            }
        }

        public override async Task<PredictionResult> PredictAsync(int concurso)
        {
            try
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("Modelo não inicializado");

                var prediction = new List<int>();

                // Obter números ordenados por frequência (menor para maior)
                var numbersByFrequency = _numberFrequencies
                    .OrderBy(kvp => kvp.Value)
                    .ThenBy(kvp => _lastAppearances[kvp.Key])
                    .ToList();

                // Estratégia 1: Pegar os 10 números menos frequentes
                for (int i = 0; i < Math.Min(10, numbersByFrequency.Count); i++)
                {
                    prediction.Add(numbersByFrequency[i].Key);
                }

                // Estratégia 2: Pegar números que não aparecem há mais tempo
                var numbersNotSeen = GetNumbersNotSeenRecently(5);
                foreach (var number in numbersNotSeen)
                {
                    if (!prediction.Contains(number) && prediction.Count < 15)
                    {
                        prediction.Add(number);
                    }
                }

                // Estratégia 3: Completar com números de frequência média-baixa
                if (prediction.Count < 15)
                {
                    var mediumFrequencyNumbers = numbersByFrequency
                        .Skip(10)
                        .Take(10)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (var number in mediumFrequencyNumbers)
                    {
                        if (!prediction.Contains(number) && prediction.Count < 15)
                        {
                            prediction.Add(number);
                        }
                    }
                }

                // Garantir que temos exatamente 15 números
                var finalPrediction = ValidatePredictionNumbers(prediction);

                // Calcular confiança baseada na variância das frequências
                var confidence = CalculateConfidence();

                return new PredictionResult
                {
                    ModelName = ModelName,
                    TargetConcurso = concurso,
                    PredictedNumbers = finalPrediction,
                    Confidence = confidence,
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro na predição: {ex.Message}", ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// REAL VALIDATION: Test anti-frequency strategy against historical data
        /// </summary>
        private async Task<(double accuracy, double hitRate, double averageHits, int totalTests)> ValidateAntiFrequencyStrategy(Lances testData)
        {
            int totalTests = 0;
            int totalHits = 0;
            int successfulPredictions = 0;
            
            var sortedData = testData.OrderBy(l => l.Id).ToList();
            
            // Use first 70% for frequency calculation, test on remaining 30%
            int trainingSize = (int)(sortedData.Count * 0.7);
            var trainingData = sortedData.Take(trainingSize).ToList();
            var validationData = sortedData.Skip(trainingSize).ToList();
            
            if (validationData.Count < 10) // Need minimum validation data
            {
                return (0.0, 0.0, 0.0, 0);
            }
            
            foreach (var actualDraw in validationData)
            {
                // Recalculate frequencies with data up to this point
                var historicalUpToNow = sortedData.Where(l => l.Id < actualDraw.Id).ToList();
                if (historicalUpToNow.Count < _analysisWindow) continue;
                
                // Calculate frequencies for recent window
                var tempFrequencies = new Dictionary<int, int>();
                for (int i = 1; i <= 25; i++) tempFrequencies[i] = 0;
                
                var recentData = historicalUpToNow.TakeLast(_analysisWindow);
                foreach (var lance in recentData)
                {
                    foreach (var number in lance.Lista)
                    {
                        if (number >= 1 && number <= 25)
                            tempFrequencies[number]++;
                    }
                }
                
                // Get least frequent numbers (anti-frequency strategy)
                var leastFrequent = tempFrequencies
                    .OrderBy(kvp => kvp.Value)
                    .ThenBy(kvp => kvp.Key) // Consistent ordering
                    .Take(15)
                    .Select(kvp => kvp.Key)
                    .ToList();
                
                // Count hits
                var hits = leastFrequent.Intersect(actualDraw.Lista).Count();
                totalHits += hits;
                totalTests++;
                
                // Consider 11+ hits as successful prediction
                if (hits >= 11)
                    successfulPredictions++;
            }
            
            if (totalTests == 0) return (0.0, 0.0, 0.0, 0);
            
            double averageHits = (double)totalHits / totalTests;
            double hitRate = (double)successfulPredictions / totalTests;
            
            // Calculate accuracy based on how close we get to actual results
            // 15 perfect hits = 100%, linear scale down
            double accuracy = Math.Min(1.0, averageHits / 15.0);
            
            return (accuracy, hitRate, averageHits, totalTests);
        }

        private void CalculateFrequencies(Lances historicalData)
        {
            // Resetar contadores
            for (int i = 1; i <= 25; i++)
            {
                _numberFrequencies[i] = 0;
            }

            // Considerar apenas a janela de análise
            var recentData = historicalData
                .OrderByDescending(l => l.Id)
                .Take(_analysisWindow)
                .ToList();

            // Contar frequências
            foreach (var lance in recentData)
            {
                foreach (var number in lance.Lista)
                {
                    if (number >= 1 && number <= 25)
                    {
                        _numberFrequencies[number]++;
                    }
                }
            }

            // Frequências calculadas para os concursos recentes
        }

        private void CalculateLastAppearances(Lances historicalData)
        {
            // Resetar últimas aparições
            for (int i = 1; i <= 25; i++)
            {
                _lastAppearances[i] = DateTime.MinValue;
            }

            // Calcular última aparição de cada número (usando Id como referência temporal)
            var sortedData = historicalData.OrderByDescending(l => l.Id).ToList();

            foreach (var lance in sortedData)
            {
                foreach (var number in lance.Lista)
                {
                    if (number >= 1 && number <= 25 && _lastAppearances[number] == DateTime.MinValue)
                    {
                        // Usar o Id como referência temporal (convertido para DateTime)
                        _lastAppearances[number] = DateTime.Now.AddDays(-lance.Id);
                    }
                }
            }
        }

        private int GetLeastFrequentNumber()
        {
            return _numberFrequencies.OrderBy(kvp => kvp.Value).First().Key;
        }

        private int GetMostFrequentNumber()
        {
            return _numberFrequencies.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        private double CalculateStandardDeviation(IEnumerable<int> values)
        {
            var average = values.Average();
            var sumOfSquares = values.Sum(v => Math.Pow(v - average, 2));
            return Math.Sqrt(sumOfSquares / values.Count());
        }

        private List<int> GetNumbersNotSeenRecently(int count)
        {
            var cutoffDate = DateTime.Now.AddDays(-30); // Números não vistos em 30 dias

            return _lastAppearances
                .Where(kvp => kvp.Value < cutoffDate)
                .OrderBy(kvp => kvp.Value)
                .Take(count)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        private List<int> ValidatePredictionNumbers(List<int> prediction)
        {
            // Garantir que temos exatamente 15 números únicos
            var uniqueNumbers = prediction.Distinct().ToList();

            // Se temos menos de 15, completar com números aleatórios
            if (uniqueNumbers.Count < 15)
            {
                var random = new Random();
                var availableNumbers = Enumerable.Range(1, 25)
                    .Where(n => !uniqueNumbers.Contains(n))
                    .OrderBy(x => random.Next())
                    .Take(15 - uniqueNumbers.Count);

                uniqueNumbers.AddRange(availableNumbers);
            }

            // Se temos mais de 15, pegar apenas os primeiros 15
            if (uniqueNumbers.Count > 15)
            {
                uniqueNumbers = uniqueNumbers.Take(15).ToList();
            }

            return uniqueNumbers.OrderBy(x => x).ToList();
        }

        #endregion

        protected override double CalculateConfidence()
        {
            if (!IsInitialized) return 0.0;

            // Confiança baseada na variância das frequências
            var frequencies = _numberFrequencies.Values.ToList();
            if (frequencies.Count == 0) return 0.5;

            var average = frequencies.Average();
            var variance = frequencies.Sum(f => Math.Pow(f - average, 2)) / frequencies.Count;

            // Maior variância = maior confiança no modelo anti-frequência
            return Math.Min(0.66, Math.Max(0.63, 0.64 + (variance / (average * average) * 0.02)));
        }


        public override void Reset()
        {
            base.Reset();
            _numberFrequencies.Clear();
            _lastAppearances.Clear();
            
            // Reinicializar contadores
            for (int i = 1; i <= 25; i++)
            {
                _numberFrequencies[i] = 0;
                _lastAppearances[i] = DateTime.MinValue;
            }
        }

        #region Public Methods

        /// <summary>
        /// Obtém relatório de frequências
        /// </summary>
        public string GetFrequencyReport()
        {
            var report = "RELATÓRIO DE FREQUÊNCIAS\n";
            report += "========================\n\n";

            var sortedFrequencies = _numberFrequencies.OrderBy(kvp => kvp.Value).ToList();

            report += "NÚMEROS MENOS FREQUENTES:\n";
            for (int i = 0; i < Math.Min(5, sortedFrequencies.Count); i++)
            {
                var item = sortedFrequencies[i];
                report += $"{item.Key:D2}: {item.Value} vezes\n";
            }

            report += "\nNÚMEROS MAIS FREQUENTES:\n";
            for (int i = sortedFrequencies.Count - 5; i < sortedFrequencies.Count; i++)
            {
                if (i >= 0)
                {
                    var item = sortedFrequencies[i];
                    report += $"{item.Key:D2}: {item.Value} vezes\n";
                }
            }

            return report;
        }

        /// <summary>
        /// Define a janela de análise
        /// </summary>
        public void SetAnalysisWindow(int window)
        {
            _analysisWindow = Math.Max(10, Math.Min(200, window));
            // Janela de análise definida
        }

        #endregion

    }
}