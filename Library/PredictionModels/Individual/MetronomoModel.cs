using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Enums;

namespace LotoLibrary.PredictionModels.Individual
{
    public class MetronomoModel : IPredictionModel
    {
        public string Name => "Modelo Metrônomo";
        
        public TipoMetronomo Tipo { get; set; } = TipoMetronomo.Simples;
        public int Intervalo { get; set; } = 5;

        public async Task<ModelValidationResult> ValidateAsync(Lances historico)
        {
            var validation = ValidateInternal(historico);
            return new ModelValidationResult
            {
                IsValid = validation.IsValid,
                ErrorMessage = validation.Message,
                Metrics = new ModelMetrics()
            };
        }

        private LotoLibrary.Models.Prediction.ValidationResult ValidateInternal(Lances historico)
        {
            // Implementação específica do modelo metrônomo
            return new LotoLibrary.Models.Prediction.ValidationResult
            {
                IsValid = true,
                Message = "Validação metrônomo concluída"
            };
        }
    }
}
