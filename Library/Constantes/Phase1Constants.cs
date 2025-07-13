// D:\PROJETOS\GraphFacil\Library\Constantes\Phase1Constants.cs
﻿// D:\PROJETOS\GraphFacil\Library\Constants\Phase1Constants.cs
namespace LotoLibrary.Constantes
{
    /// <summary>
    /// Constantes específicas da Fase 1
    /// </summary>
    public static class Phase1Constants
    {
        // Versão da implementação
        public const string PHASE1_VERSION = "1.0.0";
        public const string PHASE1_CODENAME = "Foundation";

        // Configurações de validação
        public const int DEFAULT_VALIDATION_SAMPLE_SIZE = 50;
        public const int MIN_DEZENAS_1_9_PERCENTAGE = 15;
        public const int MAX_NEVER_SELECTED_DEZENAS = 5;

        // Limites de performance
        public const int MAX_PREDICTION_TIME_MS = 2000;
        public const int MAX_INITIALIZATION_TIME_MS = 10000;

        // Arquivos de relatório
        public const string DIAGNOSTIC_REPORT_FILENAME = "DiagnosticReport_Phase1.txt";
        public const string VALIDATION_REPORT_FILENAME = "ValidationReport_Phase1.txt";
        public const string PERFORMANCE_COMPARISON_FILENAME = "PerformanceComparison_Phase1.txt";

        // Mensagens de status
        public const string STATUS_INITIALIZING = "Inicializando Fase 1...";
        public const string STATUS_READY = "✅ Fase 1 pronta";
        public const string STATUS_ERROR = "❌ Erro na Fase 1";
        public const string STATUS_VALIDATING = "Validando implementação...";

        // Configurações do PredictionEngine
        public const string DEFAULT_STRATEGY = "Single";
        public const int DEFAULT_ENSEMBLE_SIZE = 3;
        public const double DEFAULT_CONFIDENCE_THRESHOLD = 0.5;
    }
}

