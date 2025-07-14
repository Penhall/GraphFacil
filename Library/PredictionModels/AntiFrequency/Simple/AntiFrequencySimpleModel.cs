using LotoLibrary.Enums;
using LotoLibrary.Models;
using LotoLibrary.PredictionModels.AntiFrequency.Base;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.AntiFrequency.Simple
{
    public class AntiFrequencySimpleModel : AntiFrequencyModelBase
    {
        public override string Name => "Modelo Anti-Frequência Simples";
        public override AntiFrequencyStrategy Strategy => AntiFrequencyStrategy.Gentle;

        public override async Task<ModelValidationResult> ValidateAsync(Lances historico)
        {
            var validation = ValidateInternal(historico);
            return new ModelValidationResult
            {
                IsValid = validation.IsValid,
                ErrorMessage = validation.Message,
                Metrics = new ModelMetrics()
            };
        }

        protected override LotoLibrary.Models.Prediction.ValidationResult ValidateInternal(Lances historico)
        {
            // Implementação específica do modelo simples
            return new LotoLibrary.Models.Prediction.ValidationResult
            {
                IsValid = true,
                Message = "Validação concluída com sucesso"
            };
        }
    }
}
