// D:\PROJETOS\GraphFacil\Library\Interfaces\RegimeDetectionResult.cs - Interface para modelos de meta-aprendizado
using System.Collections.Generic;

namespace LotoLibrary.Suporte;

/// <summary>
/// Resultado da detecção de regime
/// </summary>
public class RegimeDetectionResult
{
    public string RegimeName { get; set; }
    public double Confidence { get; set; }
    public string Description { get; set; }
    public Dictionary<string, double> RegimeScores { get; set; }
    public string OptimalModel { get; set; }
}
