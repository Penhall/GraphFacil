using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;

namespace LotoLibrary.Models.Base
{
    public abstract class PredictionModelBase : IPredictionModel
    {
        public abstract string Name { get; }

        public abstract Task<ModelValidationResult> ValidateAsync(Lances historico);
    }
}
