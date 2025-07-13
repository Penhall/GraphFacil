// D:\PROJETOS\GraphFacil\Library\Services\Analysis\PerformanceComparer.cs - Comparador de performance entre modelos
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LotoLibrary.Services.Analysis
{
    /// <summary>
    /// Serviço para comparar performance entre diferentes modelos
    /// Especializado em análise de correlação e diversificação
    /// </summary>
    public class PerformanceComparer
    {
        #region Fields
        private Dictionary<string, List<PredictionResult>> _modelHistory;
        private Dictionary<string, PerformanceMetrics> _cachedMetrics;
        private CorrelationMatrix _correlationMatrix;
        #endregion

        #region Constructor
        public PerformanceComparer()
        {
            _modelHistory = new Dictionary<string, List<PredictionResult>>();
            _cachedMetrics = new Dictionary<string, PerformanceMetrics>();
            _correlationMatrix = new CorrelationMatrix();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Adiciona resultado de predição ao histórico do modelo
        /// </summary>
        public void AddPredictionResult(string modelName, PredictionResult result)
        {
            if (!_modelHistory.ContainsKey(modelName))
            {
                _modelHistory[modelName] = new List<PredictionResult>();
            }

            _modelHistory[modelName].Add(result);

            // Manter apenas últimas 100 predições por modelo
            if (_modelHistory[modelName].Count > 100)
            {
                _modelHistory[modelName].RemoveAt(0);
            }

            // Invalidar cache de métricas
            _cachedMetrics.Remove(modelName);
        }

        /// <summary>
        /// Compara performance entre dois modelos específicos
        /// </summary>
        public async Task<ModelComparisonResult> CompareModelsAsync(string model1, string model2)
        {
            var metrics1 = await CalculateMetricsAsync(model1);
            var metrics2 = await CalculateMetricsAsync(model2);
            var correlation = await CalculateCorrelationAsync(model1, model2);

            return new ModelComparisonResult
            {
                Model1Name = model1,
                Model2Name = model2,
                Model1Metrics = metrics1,
                Model2Metrics = metrics2,
                Correlation = correlation,
                DiversificationScore = CalculateDiversificationScore(correlation),
                RecommendedWeight1 = CalculateOptimalWeight(metrics1, metrics2, correlation),
                ComparisonTimestamp = DateTime.Now
            };
        }

        /// <summary>
        /// Analisa correlação entre todos os modelos disponíveis
        /// </summary>
        public async Task<CorrelationMatrix> AnalyzeAllCorrelationsAsync()
        {
            var modelNames = _modelHistory.Keys.ToList();
            await _correlationMatrix.BuildMatrixAsync(modelNames, _modelHistory);
            return _correlationMatrix;
        }

        /// <summary>
        /// Identifica modelos com baixa correlação (bons para ensemble)
        /// </summary>
        public async Task<List<ModelPair>> FindLowCorrelationPairsAsync(double maxCorrelation = 0.7)
        {
            var matrix = await AnalyzeAllCorrelationsAsync();
            var lowCorrelationPairs = new List<ModelPair>();

            foreach (var pair in matrix.GetAllPairs())
            {
                if (Math.Abs(pair.Correlation) < maxCorrelation)
                {
                    lowCorrelationPairs.Add(pair);
                }
            }

            return lowCorrelationPairs.OrderBy(p => Math.Abs(p.Correlation)).ToList();
        }

        /// <summary>
        /// Calcula pesos ótimos para ensemble baseado em performance e correlação
        /// </summary>
        public async Task<Dictionary<string, double>> CalculateEnsembleWeightsAsync(List<string> modelNames)
        {
            var weights = new Dictionary<string, double>();
            var metrics = new Dictionary<string, PerformanceMetrics>();

            // Calcular métricas de todos os modelos
            foreach (var model in modelNames)
            {
                metrics[model] = await CalculateMetricsAsync(model);
            }

            // Calcular matriz de correlação
            var correlationMatrix = await AnalyzeAllCorrelationsAsync();

            // Algoritmo de otimização de pesos
            weights = OptimizeWeights(metrics, correlationMatrix, modelNames);

            return weights;
        }

        /// <summary>
        /// Gera relatório completo de comparação
        /// </summary>
        public async Task<ComprehensiveReport> GenerateComprehensiveReportAsync()
        {
            var report = new ComprehensiveReport
            {
                GenerationTime = DateTime.Now,
                ModelsAnalyzed = _modelHistory.Keys.ToList(),
                IndividualMetrics = new Dictionary<string, PerformanceMetrics>(),
                CorrelationMatrix = await AnalyzeAllCorrelationsAsync(),
                LowCorrelationPairs = await FindLowCorrelationPairsAsync(),
                RecommendedEnsemble = new List<string>()
            };

            // Calcular métricas individuais
            foreach (var model in _modelHistory.Keys)
            {
                report.IndividualMetrics[model] = await CalculateMetricsAsync(model);
            }

            // Recomendar ensemble baseado em performance e diversificação
            report.RecommendedEnsemble = SelectOptimalEnsemble(report.IndividualMetrics, report.CorrelationMatrix);

            return report;
        }
        #endregion

        #region Private Methods

        private async Task<PerformanceMetrics> CalculateMetricsAsync(string modelName)
        {
            if (_cachedMetrics.ContainsKey(modelName))
            {
                return _cachedMetrics[modelName];
            }

            if (!_modelHistory.ContainsKey(modelName) || !_modelHistory[modelName].Any())
            {
                return new PerformanceMetrics { ModelName = modelName };
            }

            var predictions = _modelHistory[modelName];

            var metrics = new PerformanceMetrics
            {
                ModelName = modelName,
                TotalPredictions = predictions.Count,
                AverageConfidence = predictions.Average(p => p.OverallConfidence),
                ConfidenceStability = CalculateStability(predictions.Select(p => p.OverallConfidence).ToList()),
                PredictionConsistency = await CalculatePredictionConsistency(predictions),
                TemporalStability = CalculateTemporalStability(predictions),
                LastUpdateTime = predictions.LastOrDefault()?.Timestamp ?? DateTime.MinValue
            };

            _cachedMetrics[modelName] = metrics;
            return metrics;
        }

        private async Task<double> CalculateCorrelationAsync(string model1, string model2)
        {
            if (!_modelHistory.ContainsKey(model1) || !_modelHistory.ContainsKey(model2))
                return 0.0;

            var predictions1 = _modelHistory[model1];
            var predictions2 = _modelHistory[model2];

            // Encontrar predições para os mesmos concursos
            var commonPredictions = FindCommonPredictions(predictions1, predictions2);

            if (commonPredictions.Count < 10) // Dados insuficientes
                return 0.0;

            return await CalculatePredictionCorrelation(commonPredictions);
        }

        private List<(PredictionResult pred1, PredictionResult pred2)> FindCommonPredictions(
            List<PredictionResult> predictions1, List<PredictionResult> predictions2)
        {
            var common = new List<(PredictionResult, PredictionResult)>();

            foreach (var pred1 in predictions1)
            {
                var pred2 = predictions2.FirstOrDefault(p => p.TargetConcurso == pred1.TargetConcurso);
                if (pred2 != null)
                {
                    common.Add((pred1, pred2));
                }
            }

            return common;
        }

        private async Task<double> CalculatePredictionCorrelation(
            List<(PredictionResult pred1, PredictionResult pred2)> commonPredictions)
        {
            await Task.Delay(1); // Placeholder for async operation

            var similarities = new List<double>();

            foreach (var (pred1, pred2) in commonPredictions)
            {
                var intersection = pred1.PredictedNumbers.Intersect(pred2.PredictedNumbers).Count();
                var union = pred1.PredictedNumbers.Union(pred2.PredictedNumbers).Count();

                // Jaccard similarity
                var similarity = (double)intersection / union;
                similarities.Add(similarity);
            }

            return similarities.Average();
        }

        private double CalculateStability(List<double> values)
        {
            if (values.Count < 2) return 1.0;

            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / (values.Count - 1);
            var stdDev = Math.Sqrt(variance);

            // Normalizar estabilidade (menor desvio = maior estabilidade)
            return Math.Max(0.0, 1.0 - stdDev / mean);
        }

        private async Task<double> CalculatePredictionConsistency(List<PredictionResult> predictions)
        {
            await Task.Delay(1); // Placeholder for async operation

            if (predictions.Count < 3) return 0.5;

            var consistencyScores = new List<double>();

            // Analisar consistência na distribuição de dezenas
            foreach (var pred in predictions)
            {
                var baixas = pred.PredictedNumbers.Count(n => n <= 8);
                var medias = pred.PredictedNumbers.Count(n => n >= 9 && n <= 17);
                var altas = pred.PredictedNumbers.Count(n => n >= 18);

                var distribution = new[] { baixas, medias, altas };
                var consistency = CalculateDistributionConsistency(distribution);
                consistencyScores.Add(consistency);
            }

            return consistencyScores.Average();
        }

        private double CalculateTemporalStability(List<PredictionResult> predictions)
        {
            if (predictions.Count < 5) return 0.5;

            // Analisar estabilidade temporal das predições
            var recentPredictions = predictions.TakeLast(10).ToList();
            var olderPredictions = predictions.Skip(Math.Max(0, predictions.Count - 20)).Take(10).ToList();

            var recentAvgConfidence = recentPredictions.Average(p => p.OverallConfidence);
            var olderAvgConfidence = olderPredictions.Average(p => p.OverallConfidence);

            var stability = 1.0 - Math.Abs(recentAvgConfidence - olderAvgConfidence);
            return Math.Max(0.0, stability);
        }

        private double CalculateDistributionConsistency(int[] distribution)
        {
            var ideal = 5.0; // 15/3 = 5 por grupo
            var totalDeviation = distribution.Sum(d => Math.Abs(d - ideal));
            return Math.Max(0.0, 1.0 - totalDeviation / 15.0);
        }

        private double CalculateDiversificationScore(double correlation)
        {
            // Quanto menor a correlação, maior a diversificação
            return 1.0 - Math.Abs(correlation);
        }

        private double CalculateOptimalWeight(PerformanceMetrics metrics1, PerformanceMetrics metrics2, double correlation)
        {
            // Algoritmo simplificado de otimização de peso
            var performance1 = metrics1.AverageConfidence * metrics1.ConfidenceStability;
            var performance2 = metrics2.AverageConfidence * metrics2.ConfidenceStability;

            var rawWeight = performance1 / (performance1 + performance2);

            // Ajustar baseado na correlação (favorece diversificação)
            var diversificationBonus = (1.0 - Math.Abs(correlation)) * 0.1;

            return Math.Max(0.2, Math.Min(0.8, rawWeight + diversificationBonus));
        }

        private Dictionary<string, double> OptimizeWeights(
            Dictionary<string, PerformanceMetrics> metrics,
            CorrelationMatrix correlationMatrix,
            List<string> modelNames)
        {
            var weights = new Dictionary<string, double>();
            var totalPerformance = 0.0;

            // Calcular performance score para cada modelo
            var performanceScores = new Dictionary<string, double>();
            foreach (var model in modelNames)
            {
                var score = metrics[model].AverageConfidence * metrics[model].ConfidenceStability;
                performanceScores[model] = score;
                totalPerformance += score;
            }

            // Calcular pesos iniciais baseados em performance
            foreach (var model in modelNames)
            {
                weights[model] = performanceScores[model] / totalPerformance;
            }

            // Ajustar pesos para reduzir correlação
            weights = AdjustWeightsForDiversification(weights, correlationMatrix);

            return weights;
        }

        private Dictionary<string, double> AdjustWeightsForDiversification(
            Dictionary<string, double> initialWeights,
            CorrelationMatrix correlationMatrix)
        {
            var adjustedWeights = new Dictionary<string, double>(initialWeights);

            // Implementação simplificada - pode ser expandida com algoritmos mais sofisticados
            foreach (var model in adjustedWeights.Keys.ToList())
            {
                var avgCorrelation = correlationMatrix.GetAverageCorrelation(model);
                var diversificationFactor = 1.0 - Math.Abs(avgCorrelation);

                adjustedWeights[model] *= 1.0 + diversificationFactor * 0.1;
            }

            // Normalizar pesos
            var totalWeight = adjustedWeights.Values.Sum();
            foreach (var model in adjustedWeights.Keys.ToList())
            {
                adjustedWeights[model] /= totalWeight;
            }

            return adjustedWeights;
        }

        private List<string> SelectOptimalEnsemble(
            Dictionary<string, PerformanceMetrics> metrics,
            CorrelationMatrix correlationMatrix)
        {
            var candidates = metrics
                .Where(kvp => kvp.Value.AverageConfidence > 0.6) // Performance mínima
                .OrderByDescending(kvp => kvp.Value.AverageConfidence * kvp.Value.ConfidenceStability)
                .Take(5) // Top 5 candidatos
                .Select(kvp => kvp.Key)
                .ToList();

            var ensemble = new List<string>();

            // Adicionar modelo com melhor performance
            if (candidates.Any())
            {
                ensemble.Add(candidates.First());
            }

            // Adicionar modelos com baixa correlação
            foreach (var candidate in candidates.Skip(1))
            {
                var maxCorrelation = ensemble.Max(m => Math.Abs(correlationMatrix.GetCorrelation(m, candidate)));
                if (maxCorrelation < 0.7) // Baixa correlação
                {
                    ensemble.Add(candidate);
                }

                if (ensemble.Count >= 3) break; // Máximo 3 modelos no ensemble
            }

            return ensemble;
        }
        #endregion
    }

}
