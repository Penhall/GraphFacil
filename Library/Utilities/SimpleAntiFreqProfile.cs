// D:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Simple\SimpleAntiFreqProfile.cs - Primeiro modelo anti-frequencista
using System;

namespace LotoLibrary.Utilities;
#region Supporting Data Classes

/// <summary>
/// Perfil de dados simplificado para análise anti-frequencista
/// </summary>
public class AntiFreqDataProfile
{
    public int Dezena { get; set; }
    public double CurrentFrequency { get; set; }
    public double RecentTrend { get; set; }
    public int RecentAppearances { get; set; }
    public int AnalysisWindow { get; set; }
    public DateTime LastUpdate { get; set; }
    public double AntiFrequencyScore { get; set; }
}

#endregion
