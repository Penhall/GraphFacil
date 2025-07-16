using LotoLibrary.Models.Core;

namespace LotoLibrary.Utilities.AntiFrequency
{
    public interface IAntiFrequencyProfileFactory
    {
        IAntiFrequencyProfile Create(Lances data);
    }
}