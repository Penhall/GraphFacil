using LotoLibrary.Models.Core;

namespace LotoLibrary.Utilities.AntiFrequency
{
    public class SimpleAntiFreqProfileFactory : IAntiFrequencyProfileFactory
    {
        public IAntiFrequencyProfile Create(Lances data)
        {
            return new SimpleAntiFreqProfile(data);
        }
    }
}