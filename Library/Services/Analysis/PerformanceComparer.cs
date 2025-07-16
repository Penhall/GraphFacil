// E:\PROJETOS\GraphFacil\Library\Services\Analysis\PerformanceComparer.cs
using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models.Validation;

namespace LotoLibrary.Services.Analysis
{
    /// <summary>
    /// Comparador de performance entre modelos
    /// </summary>
    public class PerformanceComparer
    {
        #region Public Methods

        /// <summary>
        /// Compara a performance de múltiplos modelos
        /// </summary>
        public ModelComparisonResult CompareModels(List<ValidationResult> modelResults)
        {
            if (modelResults == null || !modelResults.Any())
            {
                return new ModelComparisonResult
                {
                    Success = false,
                    ErrorMessage = "Nenhum resultado de modelo fornecido para comparação"
                };
            }

            try
            {
                var comparison = new ModelComparisonResult
                {
                    Success = true,
                    TotalModelsCompared = modelResults.Count,
                    ComparisonDate = DateTime.Now,
                    ModelComparisons = new List<ModelComparison>()
                };

                // Criar comparações individuais
                foreach (var result in modelResults)
                {
                    var modelComparison = new ModelComparison
                    {
                        ModelName = result.ModelName,
                        Accuracy = result.Accuracy,
                        TotalTests = result.TotalTests,
                        PassedTests = result.PassedTests,
                        ValidationMethod = result.ValidationMethod,
                        ExecutionTime = result.ExecutionTime,
                        SuccessRate = result.CalculateSuccessRate(),
                        HitRate = result.CalculateHitRate()
                    };

                    comparison.ModelComparisons.Add(modelComparison);
                }

                // Determinar melhor modelo
                var bestModel = comparison.ModelComparisons.OrderByDescending(m => m.Accuracy).First();
                comparison.BestModel = bestModel.ModelName;
                comparison.BestAccuracy = bestModel.Accuracy;

                // Determinar pior modelo
                var worstModel = comparison.ModelComparisons.OrderBy(m => m.Accuracy).First();
                comparison.WorstModel = worstModel.ModelName;
                comparison.WorstAccuracy = worstModel.Accuracy;

                // Calcular médias
                comparison.AverageAccuracy = comparison.ModelComparisons.Average(m => m.Accuracy);
                comparison.AverageSuccessRate = comparison.ModelComparisons.Average(m => m.SuccessRate);
                comparison.AverageExecutionTime = TimeSpan.FromMilliseconds(
                    comparison.ModelComparisons.Average(m => m.ExecutionTime.TotalMilliseconds));

                // Gerar relatório
                comparison.Summary = GenerateComparisonSummary(comparison);

                return comparison;
            }
            catch (Exception ex)
            {
                return new ModelComparisonResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Compara dois modelos específicos
        /// </summary>
        public PairwiseComparison CompareTwoModels(ValidationResult model1, ValidationResult model2)
        {
            if (model1 == null || model2 == null)
            {
                return new PairwiseComparison
                {
                    Success = false,
                    ErrorMessage = "Um ou ambos os modelos não foram fornecidos"
                };
            }

            var comparison = new PairwiseComparison
            {
                Success = true,
                Model1Name = model1.ModelName,
                Model2Name = model2.ModelName,
                Model1Accuracy = model1.Accuracy,
                Model2Accuracy = model2.Accuracy,
                AccuracyDifference = Math.Abs(model1.Accuracy - model2.Accuracy),
                WinnerModel = model1.Accuracy > model2.Accuracy ? model1.ModelName : model2.ModelName,
                ComparisonDate = DateTime.Now
            };

            // Comparações detalhadas
            comparison.Model1SuccessRate = model1.CalculateSuccessRate();
            comparison.Model2SuccessRate = model2.CalculateSuccessRate();
            comparison.Model1ExecutionTime = model1.ExecutionTime;
            comparison.Model2ExecutionTime = model2.ExecutionTime;

            // Determinar vencedor por critério
            comparison.AccuracyWinner = model1.Accuracy > model2.Accuracy ? model1.ModelName : model2.ModelName;
            comparison.SpeedWinner = model1.ExecutionTime < model2.ExecutionTime ? model1.ModelName : model2.ModelName;
            comparison.ConsistencyWinner = model1.CalculateSuccessRate() > model2.CalculateSuccessRate() ? model1.ModelName : model2.ModelName;

            // Gerar análise
            comparison.Analysis = GeneratePairwiseAnalysis(comparison);

            return comparison;
        }

        /// <summary>
        /// Rankeia modelos por performance
        /// </summary>
        public ModelRanking RankModels(List<ValidationResult> modelResults, RankingCriteria criteria = RankingCriteria.Accuracy)
        {
            if (modelResults == null || !modelResults.Any())
            {
                return new ModelRanking
                {
                    Success = false,
                    ErrorMessage = "Nenhum resultado fornecido para ranking"
                };
            }

            var ranking = new ModelRanking
            {
                Success = true,
                RankingCriteria = criteria,
                RankingDate = DateTime.Now,
                Rankings = new List<ModelRankItem>()
            };

            // Ordenar baseado no critério
            var orderedResults = criteria switch
            {
                RankingCriteria.Accuracy => modelResults.OrderByDescending(m => m.Accuracy),
                RankingCriteria.Speed => modelResults.OrderBy(m => m.ExecutionTime.TotalMilliseconds),
                RankingCriteria.Consistency => modelResults.OrderByDescending(m => m.CalculateSuccessRate()),
                RankingCriteria.HitRate => modelResults.OrderByDescending(m => m.CalculateHitRate()),
                _ => modelResults.OrderByDescending(m => m.Accuracy)
            };

            int position = 1;
            foreach (var result in orderedResults)
            {
                ranking.Rankings.Add(new ModelRankItem
                {
                    Position = position++,
                    ModelName = result.ModelName,
                    Score = GetScoreForCriteria(result, criteria),
                    Accuracy = result.Accuracy,
                    SuccessRate = result.CalculateSuccessRate(),
                    ExecutionTime = result.ExecutionTime
                });
            }

            ranking.Summary = GenerateRankingSummary(ranking);
            return ranking;
        }

        #endregion

        #region Private Methods

        private string GenerateComparisonSummary(ModelComparisonResult comparison)
        {
            return $"""
                📊 COMPARAÇÃO DE MODELOS - {comparison.ComparisonDate:dd/MM/yyyy HH:mm}
                ================================================
                
                🏆 MELHOR MODELO: {comparison.BestModel} ({comparison.BestAccuracy:P2})
                🔻 PIOR MODELO: {comparison.WorstModel} ({comparison.WorstAccuracy:P2})
                
                📈 ESTATÍSTICAS GERAIS:
                • Modelos Comparados: {comparison.TotalModelsCompared}
                • Acurácia Média: {comparison.AverageAccuracy:P2}
                • Taxa de Sucesso Média: {comparison.AverageSuccessRate:P2}
                • Tempo Médio de Execução: {comparison.AverageExecutionTime.TotalMilliseconds:F0}ms
                
                📋 DETALHES POR MODELO:
                {string.Join("\n", comparison.ModelComparisons.Select(m => 
                    $"• {m.ModelName}: {m.Accuracy:P2} acurácia, {m.SuccessRate:P2} sucesso ({m.ExecutionTime.TotalMilliseconds:F0}ms)"))}
                """;
        }

        private string GeneratePairwiseAnalysis(PairwiseComparison comparison)
        {
            var winner = comparison.WinnerModel;
            var loser = comparison.WinnerModel == comparison.Model1Name ? comparison.Model2Name : comparison.Model1Name;
            
            return $"""
                🔍 ANÁLISE COMPARATIVA - {comparison.ComparisonDate:dd/MM/yyyy HH:mm}
                ================================================
                
                🏆 VENCEDOR GERAL: {winner}
                
                📊 COMPARAÇÃO DETALHADA:
                • Acurácia: {comparison.AccuracyWinner} vence por {comparison.AccuracyDifference:P2}
                • Velocidade: {comparison.SpeedWinner} é mais rápido
                • Consistência: {comparison.ConsistencyWinner} é mais consistente
                
                📈 MÉTRICAS:
                {comparison.Model1Name}: {comparison.Model1Accuracy:P2} acurácia, {comparison.Model1SuccessRate:P2} sucesso
                {comparison.Model2Name}: {comparison.Model2Accuracy:P2} acurácia, {comparison.Model2SuccessRate:P2} sucesso
                
                💡 RECOMENDAÇÃO: Use {winner} para melhor {(comparison.AccuracyWinner == winner ? "acurácia" : "performance geral")}
                """;
        }

        private string GenerateRankingSummary(ModelRanking ranking)
        {
            return $"""
                🏅 RANKING DE MODELOS - {ranking.RankingDate:dd/MM/yyyy HH:mm}
                Critério: {ranking.RankingCriteria}
                ================================================
                
                {string.Join("\n", ranking.Rankings.Select(r => 
                    $"{r.Position}º. {r.ModelName} - Score: {r.Score:F3} ({r.Accuracy:P2} acurácia)"))}
                """;
        }

        private double GetScoreForCriteria(ValidationResult result, RankingCriteria criteria)
        {
            return criteria switch
            {
                RankingCriteria.Accuracy => result.Accuracy,
                RankingCriteria.Speed => 1.0 / (result.ExecutionTime.TotalSeconds + 0.001), // Inverso do tempo
                RankingCriteria.Consistency => result.CalculateSuccessRate(),
                RankingCriteria.HitRate => result.CalculateHitRate(),
                _ => result.Accuracy
            };
        }

        #endregion
    }

    #region Supporting Classes and Enums

    public enum RankingCriteria
    {
        Accuracy,
        Speed,
        Consistency,
        HitRate
    }

    public class ModelComparisonResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int TotalModelsCompared { get; set; }
        public DateTime ComparisonDate { get; set; }
        public List<ModelComparison> ModelComparisons { get; set; } = new();
        public string BestModel { get; set; } = string.Empty;
        public double BestAccuracy { get; set; }
        public string WorstModel { get; set; } = string.Empty;
        public double WorstAccuracy { get; set; }
        public double AverageAccuracy { get; set; }
        public double AverageSuccessRate { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    public class ModelComparison
    {
        public string ModelName { get; set; } = string.Empty;
        public double Accuracy { get; set; }
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public string ValidationMethod { get; set; } = string.Empty;
        public TimeSpan ExecutionTime { get; set; }
        public double SuccessRate { get; set; }
        public double HitRate { get; set; }
    }

    public class PairwiseComparison
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string Model1Name { get; set; } = string.Empty;
        public string Model2Name { get; set; } = string.Empty;
        public double Model1Accuracy { get; set; }
        public double Model2Accuracy { get; set; }
        public double AccuracyDifference { get; set; }
        public string WinnerModel { get; set; } = string.Empty;
        public DateTime ComparisonDate { get; set; }
        public double Model1SuccessRate { get; set; }
        public double Model2SuccessRate { get; set; }
        public TimeSpan Model1ExecutionTime { get; set; }
        public TimeSpan Model2ExecutionTime { get; set; }
        public string AccuracyWinner { get; set; } = string.Empty;
        public string SpeedWinner { get; set; } = string.Empty;
        public string ConsistencyWinner { get; set; } = string.Empty;
        public string Analysis { get; set; } = string.Empty;
    }

    public class ModelRanking
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public RankingCriteria RankingCriteria { get; set; }
        public DateTime RankingDate { get; set; }
        public List<ModelRankItem> Rankings { get; set; } = new();
        public string Summary { get; set; } = string.Empty;
    }

    public class ModelRankItem
    {
        public int Position { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public double Score { get; set; }
        public double Accuracy { get; set; }
        public double SuccessRate { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }

    #endregion
}