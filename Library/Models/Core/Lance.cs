using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Core;

/// <summary>
/// Representa um lance da Lotofácil
/// </summary>
public class Lance
{
    public int Concurso { get; set; }
    public List<int> Dezenas { get; set; } = new();
    public DateTime DataSorteio { get; set; }
    public decimal ValorPremio { get; set; }
    public int Ganhadores15 { get; set; }
    public int Ganhadores14 { get; set; }
    public int Ganhadores13 { get; set; }
    public int Ganhadores12 { get; set; }
    public int Ganhadores11 { get; set; }

    public Lance()
    {
        Dezenas = new List<int>();
    }

    public Lance(int concurso, List<int> dezenas)
    {
        Concurso = concurso;
        Dezenas = dezenas?.OrderBy(d => d).ToList() ?? new List<int>();
        DataSorteio = DateTime.Now;
    }

    public Lance(int concurso, List<int> dezenas, DateTime dataSorteio)
    {
        Concurso = concurso;
        Dezenas = dezenas?.OrderBy(d => d).ToList() ?? new List<int>();
        DataSorteio = dataSorteio;
    }

    /// <summary>
    /// Verifica se o lance é válido (15 dezenas entre 1 e 25)
    /// </summary>
    public bool IsValid()
    {
        return Dezenas != null && 
               Dezenas.Count == 15 && 
               Dezenas.All(d => d >= 1 && d <= 25) &&
               Dezenas.Distinct().Count() == 15;
    }

    /// <summary>
    /// Calcula quantos números foram acertados comparando com outro lance
    /// </summary>
    public int CalcularAcertos(Lance outroLance)
    {
        if (outroLance?.Dezenas == null || Dezenas == null)
            return 0;

        return Dezenas.Intersect(outroLance.Dezenas).Count();
    }

    public override string ToString()
    {
        var dezenasStr = Dezenas?.Any() == true ? 
            string.Join("-", Dezenas.OrderBy(d => d)) : 
            "Nenhuma dezena";
        return $"Concurso {Concurso}: {dezenasStr}";
    }
}