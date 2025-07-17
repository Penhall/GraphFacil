// Consolidated PerformanceMetrics class combining prediction accuracy and temporal analysis
using System;

namespace LotoLibrary.Models.Prediction;

/// <summary>
/// Comprehensive performance metrics for prediction models
/// Combines basic accuracy metrics with temporal stability analysis
/// </summary>
public class PerformanceMetrics
{
    // Basic Accuracy Metrics
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score => (Precision + Recall) > 0 ? 2 * (Precision * Recall) / (Precision + Recall) : 0;
    
    // Model Information
    public string ModelName { get; set; } = string.Empty;
    public DateTime MeasuredAt { get; set; } = DateTime.Now;
    
    // Temporal Stability Metrics
    public int TotalPredictions { get; set; }
    public double AverageConfidence { get; set; }
    public double ConfidenceStability { get; set; }
    public double PredictionConsistency { get; set; }
    public double TemporalStability { get; set; }
    public DateTime LastUpdateTime { get; set; }
    
    // Performance Trends
    public double AccuracyTrend { get; set; }
    public double ConfidenceTrend { get; set; }
    
    public PerformanceMetrics()
    {
        MeasuredAt = DateTime.Now;
        LastUpdateTime = DateTime.Now;
    }
    
    public PerformanceMetrics(string modelName) : this()
    {
        ModelName = modelName;
    }
    
    /// <summary>
    /// Calculate overall performance score combining all metrics
    /// </summary>
    public double OverallScore => (Accuracy * 0.4) + (F1Score * 0.3) + (TemporalStability * 0.2) + (ConfidenceStability * 0.1);
    
    /// <summary>
    /// Check if metrics indicate stable model performance
    /// </summary>
    public bool IsStable => TemporalStability > 0.7 && ConfidenceStability > 0.7;
}