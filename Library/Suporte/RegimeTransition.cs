// D:\PROJETOS\GraphFacil\Library\Interfaces\RegimeTransition.cs - Interface para modelos de meta-aprendizado
using System;

namespace LotoLibrary.Suporte;

/// <summary>
/// Transição entre regimes
/// </summary>
public class RegimeTransition
{
    public string FromRegime { get; set; }
    public string ToRegime { get; set; }
    public DateTime TransitionDate { get; set; }
    public double TransitionConfidence { get; set; }
    public string TriggerReason { get; set; }
}