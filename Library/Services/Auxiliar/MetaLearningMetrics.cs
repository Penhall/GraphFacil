// E:\PROJETOS\GraphFacil\Library\Services\Auxiliar\MetaLearningMetrics.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services.Auxiliar
{
    /// <summary>
    /// Métricas específicas para sistemas de Meta-Learning
    /// </summary>
    public class MetaLearningMetrics
    {
        #region Properties
        /// <summary>
        /// Taxa de adaptação do meta-modelo
        /// </summary>
        public double AdaptationRate { get; set; }

        /// <summary>
        /// Velocidade de convergência
        /// </summary>
        public double ConvergenceSpeed { get; set; }

        /// <summary>
        /// Estabilidade do modelo
        /// </summary>
        public double Stability { get; set; }

        /// <summary>
        /// Diversidade entre modelos base
        /// </summary>
        public double ModelDiversity { get; set; }

        /// <summary>
        /// Eficiência do ensemble
        /// </summary>
        public double EnsembleEfficiency { get; set; }

        /// <summary>
        /// Performance dos modelos base
        /// </summary>
        public Dictionary<string, double> BaseModelPerformance { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Pesos otimizados dos modelos
        /// </summary>
        public Dictionary<string, double> OptimizedWeights { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Histórico de adaptações
        /// </summary>
        public List<AdaptationRecord> AdaptationHistory { get; set; } = new List<AdaptationRecord>();

        /// <summary>
        /// Métricas de contexto
        /// </summary>
        public Dictionary<string, double> ContextMetrics { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        #endregion

        #region Constructor
        public MetaLearningMetrics()
        {
            LastUpdated = DateTime.Now;
        }
        #endregion

        #region Computed Properties
        /// <summary>
        /// Score geral do meta-learning
        /// </summary>
        public double OverallScore
        {
            get
            {
                var scores = new[] { AdaptationRate, ConvergenceSpeed, Stability, ModelDiversity, EnsembleEfficiency };
                return scores.Where(s => s > 0).DefaultIfEmpty(0).Average();
            }
        }

        /// <summary>
        /// Indica se o meta-modelo está funcionando bem
        /// </summary>
        public bool IsHealthy => OverallScore >= 0.7 && Stability >= 0.6;

        /// <summary>
        /// Melhor modelo base
        /// </summary>
        public string BestBaseModel
        {
            get
            {
                return BaseModelPerformance.Any() 
                    ? BaseModelPerformance.OrderByDescending(kvp => kvp.Value).First().Key 
                    : "Nenhum";
            }
        }

        /// <summary>
        /// Pior modelo base
        /// </summary>
        public string WorstBaseModel
        {
            get
            {
                return BaseModelPerformance.Any() 
                    ? BaseModelPerformance.OrderBy(kvp => kvp.Value).First().Key 
                    : "Nenhum";
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Atualiza métricas com novos dados
        /// </summary>
        public void UpdateMetrics(Dictionary<string, double> newMetrics)
        {
            foreach (var metric in newMetrics)
            {
                switch (metric.Key.ToLower())
                {
                    case "adaptationrate":
                        AdaptationRate = metric.Value;
                        break;
                    case "convergencespeed":
                        ConvergenceSpeed = metric.Value;
                        break;
                    case "stability":
                        Stability = metric.Value;
                        break;
                    case "modeldiversity":
                        ModelDiversity = metric.Value;
                        break;
                    case "ensembleefficiency":
                        EnsembleEfficiency = metric.Value;
                        break;
                    default:
                        ContextMetrics[metric.Key] = metric.Value;
                        break;
                }
            }
            
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Registra uma adaptação
        /// </summary>
        public void RecordAdaptation(string trigger, double improvement, TimeSpan duration)
        {
            AdaptationHistory.Add(new AdaptationRecord
            {
                Timestamp = DateTime.Now,
                Trigger = trigger,
                Improvement = improvement,
                Duration = duration
            });

            // Manter apenas últimas 100 adaptações
            if (AdaptationHistory.Count > 100)
            {
                AdaptationHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// Atualiza performance de um modelo base
        /// </summary>
        public void UpdateBaseModelPerformance(string modelName, double performance)
        {
            BaseModelPerformance[modelName] = performance;
        }

        /// <summary>
        /// Atualiza peso otimizado de um modelo
        /// </summary>
        public void UpdateOptimizedWeight(string modelName, double weight)
        {
            OptimizedWeights[modelName] = weight;
        }

        /// <summary>
        /// Calcula diversidade entre modelos
        /// </summary>
        public double CalculateModelDiversity()
        {
            if (BaseModelPerformance.Count < 2)
            {
                return 0.0;
            }

            var performances = BaseModelPerformance.Values.ToList();
            var mean = performances.Average();
            var variance = performances.Sum(p => Math.Pow(p - mean, 2)) / performances.Count;
            var standardDeviation = Math.Sqrt(variance);
            
            // Normalizar para 0-1
            ModelDiversity = Math.Min(1.0, standardDeviation / mean);
            
            return ModelDiversity;
        }

        /// <summary>
        /// Calcula eficiência do ensemble
        /// </summary>
        public double CalculateEnsembleEfficiency()
        {
            if (!BaseModelPerformance.Any() || !OptimizedWeights.Any())
            {
                return 0.0;
            }

            // Eficiência = performance ponderada / performance média
            var weightedPerformance = BaseModelPerformance.Sum(kvp => 
                kvp.Value * OptimizedWeights.GetValueOrDefault(kvp.Key, 1.0));
            var averagePerformance = BaseModelPerformance.Values.Average();
            
            EnsembleEfficiency = weightedPerformance / averagePerformance;
            
            return EnsembleEfficiency;
        }

        /// <summary>
        /// Obtém tendência de adaptação
        /// </summary>
        public string GetAdaptationTrend()
        {
            if (AdaptationHistory.Count < 3)
            {
                return "Insuficiente";
            }

            var recent = AdaptationHistory.TakeLast(5).ToList();
            var improvements = recent.Select(r => r.Improvement).ToList();
            
            var trend = improvements.Last() - improvements.First();
            
            if (trend > 0.05) return "Melhorando";
            if (trend < -0.05) return "Piorando";
            return "Estável";
        }

        /// <summary>
        /// Gera relatório de métricas
        /// </summary>
        public string GenerateReport()
        {
            var report = "RELATÓRIO DE MÉTRICAS META-LEARNING\n";
            report += "=====================================\n\n";
            
            report += $"MÉTRICAS PRINCIPAIS:\n";
            report += $"• Score Geral: {OverallScore:P2}\n";
            report += $"• Status: {(IsHealthy ? "✅ Saudável" : "⚠️ Precisa Atenção")}\n";
            report += $"• Taxa de Adaptação: {AdaptationRate:P2}\n";
            report += $"• Velocidade de Convergência: {ConvergenceSpeed:P2}\n";
            report += $"• Estabilidade: {Stability:P2}\n";
            report += $"• Diversidade de Modelos: {ModelDiversity:P2}\n";
            report += $"• Eficiência do Ensemble: {EnsembleEfficiency:P2}\n\n";
            
            report += $"MODELOS BASE:\n";
            report += $"• Melhor Modelo: {BestBaseModel}\n";
            report += $"• Pior Modelo: {WorstBaseModel}\n";
            report += $"• Total de Modelos: {BaseModelPerformance.Count}\n\n";
            
            if (BaseModelPerformance.Any())
            {
                report += "PERFORMANCE DOS MODELOS:\n";
                foreach (var model in BaseModelPerformance.OrderByDescending(kvp => kvp.Value))
                {
                    var weight = OptimizedWeights.GetValueOrDefault(model.Key, 1.0);
                    report += $"• {model.Key}: {model.Value:P2} (peso: {weight:F2})\n";
                }
                report += "\n";
            }
            
            report += $"ADAPTAÇÃO:\n";
            report += $"• Tendência: {GetAdaptationTrend()}\n";
            report += $"• Adaptações Recentes: {AdaptationHistory.Count(r => r.Timestamp > DateTime.Now.AddDays(-7))}\n";
            report += $"• Última Atualização: {LastUpdated:dd/MM/yyyy HH:mm:ss}\n\n";

            if (ContextMetrics.Any())
            {
                report += "MÉTRICAS DE CONTEXTO:\n";
                foreach (var metric in ContextMetrics)
                {
                    report += $"• {metric.Key}: {metric.Value:F4}\n";
                }
            }

            return report;
        }

        /// <summary>
        /// Reseta todas as métricas
        /// </summary>
        public void Reset()
        {
            AdaptationRate = 0.0;
            ConvergenceSpeed = 0.0;
            Stability = 0.0;
            ModelDiversity = 0.0;
            EnsembleEfficiency = 0.0;
            BaseModelPerformance.Clear();
            OptimizedWeights.Clear();
            AdaptationHistory.Clear();
            ContextMetrics.Clear();
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Clona as métricas
        /// </summary>
        public MetaLearningMetrics Clone()
        {
            return new MetaLearningMetrics
            {
                AdaptationRate = AdaptationRate,
                ConvergenceSpeed = ConvergenceSpeed,
                Stability = Stability,
                ModelDiversity = ModelDiversity,
                EnsembleEfficiency = EnsembleEfficiency,
                BaseModelPerformance = new Dictionary<string, double>(BaseModelPerformance),
                OptimizedWeights = new Dictionary<string, double>(OptimizedWeights),
                AdaptationHistory = new List<AdaptationRecord>(AdaptationHistory),
                ContextMetrics = new Dictionary<string, double>(ContextMetrics),
                LastUpdated = LastUpdated
            };
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Registro de uma adaptação do meta-modelo
    /// </summary>
    public class AdaptationRecord
    {
        public DateTime Timestamp { get; set; }
        public string Trigger { get; set; } = string.Empty;
        public double Improvement { get; set; }
        public TimeSpan Duration { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Configuração de meta-learning
    /// </summary>
    public class MetaLearningConfiguration
    {
        public double AdaptationThreshold { get; set; } = 0.05;
        public int MaxAdaptationHistory { get; set; } = 100;
        public double StabilityWindow { get; set; } = 0.1;
        public bool EnableAutoOptimization { get; set; } = true;
        public double MinModelWeight { get; set; } = 0.01;
        public double MaxModelWeight { get; set; } = 2.0;
    }

    #endregion
}