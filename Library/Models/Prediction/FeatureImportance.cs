// D:\PROJETOS\GraphFacil\Library\Models\Prediction\FeatureImportance.cs
using LotoLibrary.Enums;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Importância de uma característica no modelo
    /// </summary>
    public class FeatureImportance
    {
        /// <summary>
        /// Nome da característica
        /// </summary>
        public string FeatureName { get; set; } = "";

        /// <summary>
        /// Importância relativa (0.0 a 1.0)
        /// </summary>
        public double Importance { get; set; } = 0.0;

        /// <summary>
        /// Descrição da característica
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Tipo de modelo que usa esta característica
        /// </summary>
        public ModelType Type { get; set; } = ModelType.Statistical;

        /// <summary>
        /// Categoria da característica
        /// </summary>
        public string Category { get; set; } = "";

        /// <summary>
        /// Valor atual da característica
        /// </summary>
        public object Value { get; set; }

        public override string ToString()
        {
            return $"{FeatureName}: {Importance:P2} ({Description})";
        }
    }
}
