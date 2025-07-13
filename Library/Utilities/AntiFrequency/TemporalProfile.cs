// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\TemporalProfile.cs - Cálculos de frequência para modelos anti-frequencistas
namespace LotoLibrary.Utilities.AntiFrequency
{
    public class TemporalProfile
    {
        public int Dezena { get; set; }
        public int LastAppearance { get; set; }
        public double AverageInterval { get; set; }
        public double IntervalTrend { get; set; }
        public double RecentTrend { get; set; }
        public double Seasonality { get; set; }
    }

}
