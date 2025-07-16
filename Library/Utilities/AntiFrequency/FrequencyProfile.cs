using System;
using System.Collections.Generic;

namespace LotoLibrary.Utilities.AntiFrequency
{
    public class FrequencyProfile
    {
        public int Dezena { get; set; }
        public int AppearanceCount { get; set; }
        public int ExpectedAppearances { get; set; }
        public double FrequencyScore { get; set; }
        public double AntiFrequencyScore { get; set; }
        public double StatisticalDebt { get; set; }
        public DateTime LastAppearance { get; set; }
        public TimeSpan AverageInterval { get; set; }
        public double Volatility { get; set; }
        public List<int> RecentAppearances { get; set; } = new();
    }
}
