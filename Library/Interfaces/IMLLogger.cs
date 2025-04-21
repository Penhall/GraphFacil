using System.Collections.Generic;
using System;

namespace LotoLibrary.Interfaces;

public interface IMLLogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception ex = null);
    void LogMetrics(string modelName, IDictionary<string, double> metrics);
    void LogDebug(string message);
}
