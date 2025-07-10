// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Explicação de um modelo
    /// </summary>
    public class ModelExplanation
    {
        /// <summary>
        /// Nome do modelo
        /// </summary>
        public string ModelName { get; set; } = "";

        /// <summary>
        /// Descrição da estratégia usada
        /// </summary>
        public string Strategy { get; set; } = "";

        /// <summary>
        /// Fatores que influenciaram a decisão
        /// </summary>
        public List<DecisionFactor> DecisionFactors { get; set; } = new List<DecisionFactor>();

        /// <summary>
        /// Importância das características
        /// </summary>
        public List<FeatureImportance> FeatureImportances { get; set; } = new List<FeatureImportance>();

        /// <summary>
        /// Explicação textual detalhada
        /// </summary>
        public string DetailedExplanation { get; set; } = "";

        /// <summary>
        /// Nível de confiança na explicação
        /// </summary>
        public double ExplanationConfidence { get; set; } = 0.0;

        /// <summary>
        /// Timestamp da explicação
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{ModelName}: {Strategy} (Confiança: {ExplanationConfidence:P2})";
        }
    }

}
