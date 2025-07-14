using System;
using System.Collections.Generic;

namespace LotoLibrary.Models
{
    public class ModelValidationResult
    {
        public string ModelName { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public ModelMetrics Metrics { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
