// D:\PROJETOS\GraphFacil\LotoLibrary\Services\ValidationReport.cs - Teste do primeiro modelo
using System;
using System.Collections.Generic;

namespace LotoLibrary.Suporte
{
    #region Supporting Classes
    public class ValidationReport
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public bool OverallSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
    #endregion
}
