// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PerformanceReport.cs - Modelos de dados
using LotoLibrary.Enums;
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

/// <summary>
/// Relatório de desempenho de um modelo
/// </summary>
public class PerformanceReport
{
    /// <summary>
    /// Nome do modelo
    /// </summary>
    public string ModelName { get; set; } = "";

    /// <summary>
    /// Tempo do relatório
    /// </summary>
    public DateTime ReportTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Resultados de validação
    /// </summary>
    public ValidationResult ValidationResults { get; set; }

    /// <summary>
    /// Nota de desempenho
    /// </summary>
    public PerformanceGrade Grade { get; set; } = PerformanceGrade.C;

    /// <summary>
    /// Métricas detalhadas
    /// </summary>
    public Dictionary<string, double> Metrics { get; set; } = new Dictionary<string, double>();

    /// <summary>
    /// Observações sobre o desempenho
    /// </summary>
    public List<string> Observations { get; set; } = new List<string>();

    /// <summary>
    /// Recomendações para melhoria
    /// </summary>
    public List<string> Recommendations { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"Relatório {ModelName} ({Grade}): {ReportTime:dd/MM/yyyy HH:mm}";
    }
}
