using LotoLibrary.Models.Core;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Utilities.AntiFrequency
{
    public class SimpleAntiFreqProfile : IAntiFrequencyProfile
    {
        private readonly Dictionary<int, int> _frequencyCount;

        public SimpleAntiFreqProfile(Lances historicalData)
        {
            _frequencyCount = new Dictionary<int, int>();
            if (historicalData == null || !historicalData.Any())
            {
                return;
            }

            // Calcula a frequência para todos os 25 números
            foreach (var dezena in Enumerable.Range(1, 25))
            {
                _frequencyCount[dezena] = historicalData.Count(l => l.Lista.Contains(dezena));
            }
        }

        public List<int> GetLeastFrequent(int count)
        {
            return _frequencyCount.OrderBy(kv => kv.Value)
                                  .ThenBy(kv => kv.Key) // Desempate estável
                                  .Take(count)
                                  .Select(kv => kv.Key)
                                  .OrderBy(n => n)
                                  .ToList();
        }
    }
}