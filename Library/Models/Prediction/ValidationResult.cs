// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ValidationResult.cs
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

/// <summary>
/// Resultado de valida��o de modelos - VERS�O �NICA PADRONIZADA
/// Esta � a �nica classe ValidationResult que deve existir no projeto
/// </summary>
public class ValidationResult
{
    // ===== PROPRIEDADES B�SICAS =====
    /// <summary>
    /// Indica se a valida��o foi bem-sucedida
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Precis�o/acur�cia do modelo (0.0 a 1.0)
    /// </summary>
    public double Accuracy { get; set; }

    /// <summary>
    /// Mensagem descritiva do resultado
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp da valida��o
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.Now;

    // ===== PROPRIEDADES ESTENDIDAS =====
    /// <summary>
    /// N�mero total de testes executados
    /// </summary>
    public int TotalTests { get; set; }

    /// <summary>
    /// Lista de predi��es corretas (IDs dos concursos)
    /// </summary>
    public List<int> CorrectPredictions { get; set; } = new();

    /// <summary>
    /// Tempo gasto na valida��o
    /// </summary>
    public TimeSpan ValidationTime { get; set; }

    /// <summary>
    /// M�todo de valida��o utilizado
    /// </summary>
    public string ValidationMethod { get; set; } = string.Empty;

    /// <summary>
    /// M�tricas adicionais espec�ficas do modelo
    /// </summary>
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();

    // ===== CONSTRUTORES =====
    /// <summary>
    /// Construtor padr�o
    /// </summary>
    public ValidationResult()
    {
        CorrectPredictions = new List<int>();
        AdditionalMetrics = new Dictionary<string, object>();
    }

    /// <summary>
    /// Construtor com par�metros b�sicos
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

    // ===== M�TODOS UTILIT�RIOS =====
    /// <summary>
    /// Calcula taxa de sucesso baseada nas predi��es corretas
    /// </summary>
    public double CalculateSuccessRate()
    {
        return TotalTests > 0 ? (double)CorrectPredictions.Count / TotalTests : 0.0;
    }

    /// <summary>
    /// Adiciona m�trica personalizada
    /// </summary>
    public void AddMetric(string name, object value)
    {
        AdditionalMetrics[name] = value;
    }

    /// <summary>
    /// Obt�m m�trica espec�fica
    /// </summary>
    public T GetMetric<T>(string name, T defaultValue = default)
    {
        return AdditionalMetrics.TryGetValue(name, out var value) && value is T ? (T)value : defaultValue;
    }

    /// <summary>
    /// Representa��o textual do resultado
    /// </summary>
    public override string ToString()
    {
        return $"ValidationResult: {(IsValid ? "V�LIDO" : "INV�LIDO")} | " +
               $"Accuracy: {Accuracy:P2} | " +
               $"Tests: {TotalTests} | " +
               $"Message: {Message}";
    }
}