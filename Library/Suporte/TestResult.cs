// D:\PROJETOS\GraphFacil\LotoLibrary\Services\TestResult2.cs - Teste do primeiro modelo
using System.Collections.Generic;
using System;

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
