using LotoLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace LotoLibrary.Infrastructure.Logging;

public class MLLogger : IMLLogger
{
    private readonly ILogger _logger;
    private readonly string _logFilePath;

    public MLLogger(ILogger logger, string logFilePath = "ml_log.txt")
    {
        _logger = logger;
        _logFilePath = logFilePath;
    }

    public void LogInformation(string message)
    {
        var logMessage = $"{DateTime.Now}: INFO - {message}";
        _logger.LogInformation(message);
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }

    public void LogWarning(string message)
    {
        var logMessage = $"{DateTime.Now}: WARN - {message}";
        _logger.LogWarning(message);
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }

    public void LogError(string message, Exception ex = null)
    {
        var logMessage = $"{DateTime.Now}: ERROR - {message}";
        if (ex != null)
        {
            logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
        _logger.LogError(ex, message);
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }

    public void LogMetrics(string modelName, IDictionary<string, double> metrics)
    {
        var logMessage = $"{DateTime.Now}: METRICS - Model: {modelName}\n";
        foreach (var metric in metrics)
        {
            logMessage += $"{metric.Key}: {metric.Value:F6}\n";
        }
        _logger.LogInformation(logMessage);
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }
}
