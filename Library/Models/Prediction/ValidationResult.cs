using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public double Accuracy { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ValidatedAt { get; set; } = DateTime.Now;
    public int TotalTests { get; set; }
    public List<int> CorrectPredictions { get; set; } = new();
    public TimeSpan ValidationTime { get; set; }

    public ValidationResult() { }

    public ValidationResult(bool isValid, double accuracy, string message)
    {
        IsValid = isValid;
        Accuracy = accuracy;
        Message = message;
    }
}
