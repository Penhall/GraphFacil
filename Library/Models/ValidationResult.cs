using System;
using System.Collections.Generic;

namespace LotoLibrary.Models
{
    public class ValidationResult
    {
        public bool Success { get; set; }
        public int SuccessfulPredictions { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestEndTime { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Dictionary<string, object> Metrics { get; set; } = new();
        public TimeSpan Duration => TestEndTime - TestStartTime;

        public ValidationResult()
        {
            TestStartTime = DateTime.Now;
            Success = true; // Default para sucesso
        }

        public void CompleteValidation()
        {
            TestEndTime = DateTime.Now;
        }

        public void RecordError(string error)
        {
            Success = false;
            ErrorMessage = error;
        }
    }
}
