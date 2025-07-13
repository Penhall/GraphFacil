// D:\PROJETOS\GraphFacil\Library\Interfaces\DezenExplanation.cs
using System.Collections.Generic;

namespace LotoLibrary.Utilities;

/// <summary>
/// Explicação específica para uma dezena
/// </summary>
public class DezenExplanation
{
    public int Dezena { get; set; }
    public bool WasSelected { get; set; }
    public double Score { get; set; }
    public string Reason { get; set; }
    public List<string> SupportingFactors { get; set; } = new List<string>();
}
