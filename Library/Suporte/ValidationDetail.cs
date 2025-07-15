// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ValidationDetail.cs - REESCRITO para nova arquitetura
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Suporte;

/// <summary>
/// Detalhe individual de um teste de validação - REESCRITO
/// </summary>
public class ValidationDetail
{
    /// <summary>
    /// Concurso testado (usando Lance.Id)
    /// </summary>
    public int Concurso { get; set; }

    /// <summary>
    /// Dezenas preditas pelo modelo
    /// </summary>
    public List<int> PredictedNumbers { get; set; } = new List<int>();

    /// <summary>
    /// Dezenas realmente sorteadas (usando Lance.Lista)
    /// </summary>
    public List<int> ActualNumbers { get; set; } = new List<int>();

    /// <summary>
    /// Número de acertos (interseção entre preditas e reais)
    /// </summary>
    public int Hits { get; set; }

    /// <summary>
    /// Confiança da predição (0.0 a 1.0)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Tempo gasto na predição
    /// </summary>
    public TimeSpan PredictionTime { get; set; }

    /// <summary>
    /// Indica se foi um acerto total (15 dezenas) - PROPRIEDADE ADICIONADA
    /// </summary>
    public bool Success => Hits == 15;

    /// <summary>
    /// Alias para compatibilidade - PROPRIEDADE ADICIONADA
    /// </summary>
    public bool IsSuccess => Success;

    /// <summary>
    /// Taxa de acerto para este teste específico
    /// </summary>
    public double HitRate => ActualNumbers.Count > 0 ? (double)Hits / ActualNumbers.Count : 0.0;

    /// <summary>
    /// Dezenas que foram acertadas
    /// </summary>
    public List<int> CorrectNumbers => PredictedNumbers.Intersect(ActualNumbers).ToList();

    /// <summary>
    /// Dezenas que foram erradas (preditas mas não sorteadas)
    /// </summary>
    public List<int> MissedNumbers => PredictedNumbers.Except(ActualNumbers).ToList();

    /// <summary>
    /// Cria ValidationDetail a partir de Lance
    /// </summary>
    public static ValidationDetail FromLance(Lance lance, List<int> predictedNumbers, double confidence, TimeSpan predictionTime)
    {
        var hits = predictedNumbers.Intersect(lance.Lista).Count();

        return new ValidationDetail
        {
            Concurso = lance.Id,  // ✅ Usando lance.Id, não lance.Concurso
            PredictedNumbers = new List<int>(predictedNumbers),
            ActualNumbers = new List<int>(lance.Lista),  // ✅ Usando lance.Lista, não lance.DezenasSorteadas
            Hits = hits,
            Confidence = confidence,
            PredictionTime = predictionTime
        };
    }

    public override string ToString()
    {
        return $"Concurso {Concurso}: {Hits}/15 acertos ({HitRate:P1}) - Confiança: {Confidence:P2}";
    }
}
