// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\FrequencyStats.cs - Cálculos de frequência para modelos anti-frequencistas
namespace LotoLibrary.Utilities.AntiFrequency;



public class FrequencyStats
{
    public int TotalAppearances { get; set; }
    public double AverageInterval { get; set; }
    public double IntervalVariance { get; set; }
    public int LastAppearance { get; set; }
    public double LongestGap { get; set; }
    public double ShortestGap { get; set; }
}

