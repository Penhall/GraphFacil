using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Enums;

namespace LotoLibrary.PredictionModels.AntiFrequency.Base
{
    public abstract class AntiFrequencyModelBase : IPredictionModel
    {
        public abstract string Name { get; }
        public abstract AntiFrequencyStrategy Strategy { get; }

        public abstract Task<ModelValidationResult> ValidateAsync(Lances historico);

        protected virtual LotoLibrary.Models.Prediction.ValidationResult ValidateInternal(Lances historico)
        {
            // Implementação da validação interna
            return new LotoLibrary.Models.Prediction.ValidationResult();
        }
    }
}
