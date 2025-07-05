using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models;

public static class OscillatorStrategy
{
    // 1. Estratégia de Tendência de Curto Prazo
    public static void AplicarTendenciaCurtoPrazo(List<DezenaOscilante> dezenas, List<Lance> ultimosSorteios)
    {
        var recentes = ultimosSorteios.TakeLast(5).SelectMany(l => l.Lista).ToList();
        foreach (var dezena in dezenas)
        {
            if (recentes.Contains(dezena.Numero))
                dezena.ForcaSincronizacao = Math.Min(1.0, dezena.ForcaSincronizacao + 0.15);
        }
    }

    // 2. Estratégia de Padrões de Grupos
    public static void AplicarPadroesGrupos(List<DezenaOscilante> dezenas, List<int[]> gruposFrequentes)
    {
        foreach (var grupo in gruposFrequentes)
        {
            var membros = dezenas.Where(d => grupo.Contains(d.Numero)).ToList();
            double mediaForca = membros.Average(d => d.ForcaSincronizacao);

            foreach (var dezena in membros)
            {
                dezena.ForcaSincronizacao = (dezena.ForcaSincronizacao + mediaForca) / 2;
            }
        }
    }

    // 3. Estratégia de Ciclos
    public static void AplicarCiclos(List<DezenaOscilante> dezenas, Dictionary<int, int> ciclosMedios)
    {
        foreach (var dezena in dezenas)
        {
            if (ciclosMedios.TryGetValue(dezena.Numero, out var ciclo))
            {
                double ajuste = Math.Sin((dezena.UltimoAtraso / (double)ciclo) * Math.PI);
                dezena.Frequencia *= 1 + (ajuste * 0.2);
            }
        }
    }

    // 4. Estratégia de Validação Cruzada
    public static List<int> GerarPalpiteValidacao(List<DezenaOscilante> dezenas, List<Lance> dadosValidacao)
    {
        // Implemente sua lógica de validação
        return dezenas.OrderByDescending(d => d.Probabilidade)
                     .Take(15)
                     .Select(d => d.Numero)
                     .ToList();
    }
}