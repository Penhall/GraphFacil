// D:\PROJETOS\GraphFacil\Library\Utilities\AntiFrequency\DebtProfile.cs - Cálculos de frequência para modelos anti-frequencistas
namespace LotoLibrary.Utilities.AntiFrequency;


public class DebtProfile
{
    public int Dezena { get; set; }
    public int TotalAppearances { get; set; }
    public double AverageInterval { get; set; }
    public double Volatility { get; set; }
    public int LastAppearance { get; set; }
    public int CurrentGap { get; set; }
    public double DebtTrend { get; set; }
}

