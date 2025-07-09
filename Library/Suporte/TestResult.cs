// D:\PROJETOS\GraphFacil\LotoLibrary\Services\TestResult.cs - Teste do primeiro modelo
using System;
using System.Collections.Generic;

namespace LotoLibrary.Suporte
{
    public class TestResult
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}
