using System.Collections.Generic;

namespace LotoLibrary.Utilities.AntiFrequency
{
    public interface IAntiFrequencyProfile
    {
        List<int> GetLeastFrequent(int count);
    }
}