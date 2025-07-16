// E:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Simple\AntiFrequencySimpleModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Models;
using LotoLibrary.Models.Validation;
using LotoLibrary.Models.Base;
using LotoLibrary.Enums;

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
        #endregion

        #region Constructor
        public AntiFrequencySimpleModel() : base("Anti-Frequency Simple", ModelType.AntiFrequencySimple)
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

        protected override async Task InitializeModelSpecificAsync(Lances historicalData)
        {
            LogInfo("Inicializando modelo Anti-Frequência Simples...");
            
            // Calcular frequências iniciais
            CalculateFrequencies(historicalData);
            
            // Calcular última aparição de cada número
            CalculateLastAppearances(historicalData);
            
            // Métricas iniciais
            AddMetric("TotalConcursos", historicalData.Count);
            AddMetric("JanelaAnalise", _analysisWindow);
            AddMetric("NumeroMenosFrequente", GetLeastFrequentNumber());
            AddMetric("NumeroMaisFrequente", GetMostFrequentNumber());
            
            LogInfo($"Modelo inicializado com {historicalData.Count} concursos");
            
            await Task.CompletedTask;
        }

        protected override async Task<bool> TrainModelSpecificAsync(Lances historicalData)
        {
            LogInfo("Treinando modelo Anti-Frequência Simples...");
            
            try
            {
                // Recalcular frequências com dados de treinamento
                CalculateFrequencies(historicalData);
                
                // Atualizar última aparição
                CalculateLastAppearances(historicalData);
                
                // Calcular métricas de treinamento
                var averageFrequency = _numberFrequencies.Values.Average();
                var standardDeviation = CalculateStandardDeviation(_numberFrequencies.Values);
                
                AddMetric("FrequenciaMedia", averageFrequency);
                AddMetric("DesvioPadrao", standardDeviation);
                AddMetric("CoeficienteVariacao", standardDeviation / averageFrequency);
                
                LogInfo($"Treinamento concluído. Frequência média: {averageFrequency:F2}");
                
                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Erro durante treinamento: {ex.Message}");
                return false;
            }
        }

        protected override async Task<List<int>> PredictModelSpecificAsync(int concurso)
        {
            LogInfo($"Gerando predição para concurso {concurso}...");
            
            try
            {
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
                var finalPrediction = ValidatePrediction(prediction);
                
                LogInfo($"Predição gerada: {string.Join(", ", finalPrediction)}");
                
                await Task.CompletedTask;
                return finalPrediction;
            }
            catch (Exception ex)
            {
                LogError($"Erro durante predição: {ex.Message}");
                return GenerateRandomPrediction();
            }
        }

        protected override string GetModelDescription()
        {
            return "Modelo que prioriza números menos sorteados historicamente, " +
                   "baseado na teoria de que números menos frequentes têm maior " +
                   "probabilidade de serem sorteados em concursos futuros.";
        }

        #endregion

        #region Private Methods

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
            
            LogInfo($"Frequências calculadas para {recentData.Count} concursos");
        }

        private void CalculateLastAppearances(Lances historicalData)
        {
            // Resetar últimas aparições
            for (int i = 1; i <= 25; i++)
            {
                _lastAppearances[i] = DateTime.MinValue;
            }
            
            // Calcular última aparição de cada número
            var sortedData = historicalData.OrderByDescending(l => l.Data).ToList();
            
            foreach (var lance in sortedData)
            {
                foreach (var number in lance.Lista)
                {
                    if (number >= 1 && number <= 25 && _lastAppearances[number] == DateTime.MinValue)
                    {
                        _lastAppearances[number] = lance.Data;
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

        #endregion

        #region Override Methods

        protected override void UpdateConfidence()
        {
            // Confiança baseada na variância das frequências
            var frequencies = _numberFrequencies.Values.ToList();
            var average = frequencies.Average();
            var variance = frequencies.Sum(f => Math.Pow(f - average, 2)) / frequencies.Count;
            
            // Maior variância = maior confiança no modelo anti-frequência
            _confidence = Math.Min(0.95, Math.Max(0.45, variance / (average * average)));
            
            AddMetric("Confianca", _confidence);
            
            LogInfo($"Confiança atualizada para {_confidence:P2}");
        }

        public override bool IsModelType(string modelType)
        {
            return modelType.Equals("AntiFrequencySimple", StringComparison.OrdinalIgnoreCase) ||
                   modelType.Equals("AntiFrequency", StringComparison.OrdinalIgnoreCase) ||
                   base.IsModelType(modelType);
        }

        #endregion

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
            LogInfo($"Janela de análise definida para {_analysisWindow} concursos");
        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            _numberFrequencies?.Clear();
            _lastAppearances?.Clear();
            base.Dispose();
        }

        #endregion
    }
}