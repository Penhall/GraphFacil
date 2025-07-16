using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

/// <summary>
/// Métricas de performance para modelos de predição
/// </summary>
public class MetricasPerformance
{
    public double TaxaAcerto11Plus { get; set; }
    public double TaxaAcerto12Plus { get; set; }
    public double TaxaAcerto13Plus { get; set; }
    public double AcertoMedio { get; set; }
    public string Estrategia { get; set; } = string.Empty;
    public DateTime DataAnalise { get; set; } = DateTime.Now;
    public List<string> Resultados { get; set; } = new();
    public int TotalTestes { get; set; }
    public int TotalAcertos { get; set; }
    public double ConfiancaGeral { get; set; }

    public MetricasPerformance()
    {
        Resultados = new List<string>();
    }

    public void AdicionarResultado(string resultado)
    {
        Resultados.Add(resultado);
    }

    public void CalcularMetricas()
    {
        if (TotalTestes > 0)
        {
            AcertoMedio = (double)TotalAcertos / TotalTestes;
            ConfiancaGeral = Math.Min(0.95, AcertoMedio);
        }
    }

    public override string ToString()
    {
        return $"Performance: {AcertoMedio:P2} - Estratégia: {Estrategia}";
    }
}