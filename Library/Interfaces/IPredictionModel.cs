using System.Threading.Tasks;
using LotoLibrary.Models;

namespace LotoLibrary.Interfaces
{
    public interface IPredictionModel
    {
        string Name { get; }
        Task<ModelValidationResult> ValidateAsync(Lances historico);
    }
}
