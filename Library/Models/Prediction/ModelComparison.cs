// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelComparison.cs - Modelos de dados
using LotoLibrary.Enums;
using System;

namespace LotoLibrary.Models.Prediction
{
   /// <summary>
/// Comparação entre modelos
/// </summary>
public class ModelComparison
{
    /// <summary>
    /// Nome do modelo
    /// </summary>
    public string ModelName { get; set; } = "";

    /// <summary>
    /// Precisão do modelo (0.0 a 1.0)
    /// </summary>
    public double Accuracy { get; set; } = 0.0;

    /// <summary>
    /// Confiança média do modelo (0.0 a 1.0)
    /// </summary>
    public double Confidence { get; set; } = 0.0;

    /// <summary>
    /// Tempo médio de processamento
    /// </summary>
    public TimeSpan AverageProcessingTime { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Nota de desempenho
    /// </summary>
    public PerformanceGrade Grade { get; set; } = PerformanceGrade.C;

    /// <summary>
    /// Posição no ranking
    /// </summary>
    public int Rank { get; set; } = 0;

    /// <summary>
    /// Número total de predições feitas
    /// </summary>
    public int TotalPredictions { get; set; } = 0;

    /// <summary>
    /// Número de predições bem-sucedidas
    /// </summary>
    public int SuccessfulPredictions { get; set; } = 0;

    /// <summary>
    /// Taxa de sucesso
    /// </summary>
    public double SuccessRate => TotalPredictions > 0 ? (double)SuccessfulPredictions / TotalPredictions : 0.0;

    /// <summary>
    /// Última atualização dos dados
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"{ModelName}: {Accuracy:P2} ({Grade}) - Rank #{Rank}";
    }
}
}
