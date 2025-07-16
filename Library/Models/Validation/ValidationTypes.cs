// D:\PROJETOS\GraphFacil\Library\Models\Validation\ValidationTypes.cs - Refatoração consolidada das classes de validação
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Validation
{
    #region Base Classes
    public abstract class BaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public TimeSpan ExecutionTime { get; set; } = TimeSpan.Zero;
        public Dictionary<string, object> AdditionalData { get; set; } = new();

        protected BaseResult() { }
        protected BaseResult(bool success, string message, string details = "")
        {
            Success = success;
            Message = message;
            Details = details;
        }
    }

    public class TestResult : BaseResult
    {
        public string TestName { get; set; } = string.Empty;
        public double Score { get; set; }
        public Dictionary<string, object> Metrics { get; set; } = new();

        public TestResult() { }
        public TestResult(string testName, bool success, string message = "", string details = "")
            : base(success, message, details)
        {
            TestName = testName;
        }
    }

    public class ValidationResult : BaseResult
    {
        public bool IsValid { get => Success; set => Success = value; }
        public string ModelName { get; set; } = string.Empty;
        public double Accuracy { get; set; }
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public List<int> CorrectPredictions { get; set; } = new();
        public string ValidationMethod { get; set; } = string.Empty;
        public int TestSamplesCount { get; set; }
        public ModelMetrics Metrics { get; set; } = new();

        public ValidationResult() { }
        public ValidationResult(bool isValid, double accuracy, string message, string modelName = "")
            : base(isValid, message)
        {
            Accuracy = accuracy;
            ModelName = modelName;
        }
    }

    public class ModelMetrics
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> CustomMetrics { get; set; } = new();
    }

    public class ValidationReport : BaseResult
    {
        public string ReportName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public double OverallScore { get; set; }
        public List<TestResult> TestResults { get; set; } = new();
        public string TestLog { get; set; } = string.Empty;
        public object MetaLearningMetrics { get; set; }
    }
    #endregion

    #region Legacy Compatibility
    [Obsolete("Use TestResult instead")]
    public class TestResult2 : TestResult
    {
        public bool Passed { get => Success; set => Success = value; }
        public TestResult2() { }
        public TestResult2(string testName, bool passed, double score, string details)
            : base(testName, passed, "", details) { Score = score; }
    }

    [Obsolete("Use TestResult instead")]
    public class TestResult : TestResult
    {
        public string ErrorMessage { get => Success ? string.Empty : Message; set { if (!Success) Message = value; } }
        public TestResult() { }
        public TestResult(string testName, bool success, string message = "") : base(testName, success, message) { }
    }
    #endregion
}
