// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ValidationResult.cs
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

/// <summary>
/// Resultado de validação de modelos - VERSÃO ÚNICA PADRONIZADA
/// Esta é a única classe ValidationResult que deve existir no projeto
/// </summary>
public class ValidationResult
{
    // ===== PROPRIEDADES BÁSICAS =====
    /// <summary>
    /// Indica se a validação foi bem-sucedida
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Precisão/acurácia do modelo (0.0 a 1.0)
    /// </summary>
    public double Accuracy { get; set; }

    /// <summary>
    /// Mensagem descritiva do resultado
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp da validação
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.Now;

    // ===== PROPRIEDADES ESTENDIDAS =====
    /// <summary>
    /// Número total de testes executados
    /// </summary>
    public int TotalTests { get; set; }

    /// <summary>
    /// Lista de predições corretas (IDs dos concursos)
    /// </summary>
    public List<int> CorrectPredictions { get; set; } = new();

    /// <summary>
    /// Tempo gasto na validação
    /// </summary>
    public TimeSpan ValidationTime { get; set; }

    /// <summary>
    /// Método de validação utilizado
    /// </summary>
    public string ValidationMethod { get; set; } = string.Empty;

    /// <summary>
    /// Métricas adicionais específicas do modelo
    /// </summary>
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();

    // ===== CONSTRUTORES =====
    /// <summary>
    /// Construtor padrão
    /// </summary>
    public ValidationResult()
    {
        CorrectPredictions = new List<int>();
        AdditionalMetrics = new Dictionary<string, object>();
    }

    /// <summary>
    /// Construtor com parâmetros básicos
    /// </summary>
    public ValidationResult(bool isValid, double accuracy, string message)
    {
        IsValid = isValid;
        Accuracy = accuracy;
        Message = message;
        CorrectPredictions = new List<int>();
        AdditionalMetrics = new Dictionary<string, object>();
    }

    /// <summary>
    /// Construtor completo
    /// </summary>
    public ValidationResult(bool isValid, double accuracy, string message, int totalTests)
    {
        IsValid = isValid;
        Accuracy = accuracy;
        Message = message;
        TotalTests = totalTests;
        CorrectPredictions = new List<int>();
        AdditionalMetrics = new Dictionary<string, object>();
    }

    // ===== MÉTODOS UTILITÁRIOS =====
    /// <summary>
    /// Calcula taxa de sucesso baseada nas predições corretas
    /// </summary>
    public double CalculateSuccessRate()
    {
        return TotalTests > 0 ? (double)CorrectPredictions.Count / TotalTests : 0.0;
    }

    /// <summary>
    /// Adiciona métrica personalizada
    /// </summary>
    public void AddMetric(string name, object value)
    {
        AdditionalMetrics[name] = value;
    }

    /// <summary>
    /// Obtém métrica específica
    /// </summary>
    public T GetMetric<T>(string name, T defaultValue = default)
    {
        return AdditionalMetrics.TryGetValue(name, out var value) && value is T ? (T)value : defaultValue;
    }

    /// <summary>
    /// Representação textual do resultado
    /// </summary>
    public override string ToString()
    {
        return $"ValidationResult: {(IsValid ? "VÁLIDO" : "INVÁLIDO")} | " +
               $"Accuracy: {Accuracy:P2} | " +
               $"Tests: {TotalTests} | " +
               $"Message: {Message}";
    }
}