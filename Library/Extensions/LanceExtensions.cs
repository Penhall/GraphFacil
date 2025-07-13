// D:\PROJETOS\GraphFacil\Library\Extensions\LanceExtensions.cs
using System.Collections.Generic;
using System.Linq;
using System;
using LotoLibrary.Models;

namespace LotoLibrary.Extensions;

public static class LanceExtensions
{
    public static List<Lance> Combinar(List<Lance> lista1, List<Lance> lista2)
    {
        return (from o1 in lista1
                from o2 in lista2
                let l = new List<int>(o1.Lista.Concat(o2.Lista))
                select new Lance(lista1.Count, l)).ToList();
    }

    public static List<float> ObterValoresF(this Lances lances, int indicador)
    {
        return lances.Select(lance =>
        {
            return indicador switch
            {
                0 => lance.F0,
                1 => lance.F1,
                2 => lance.F2,
                _ => throw new ArgumentException($"Indicador {indicador} inválido")
            };
        }).ToList();
    }

    public static Lances ToLances(this IEnumerable<Lance> source)
    {
        return new Lances(source);
    }

    public static Lances DistintosPorId(this Lances lances)
    {
        var distinctLances = lances
            .GroupBy(lance => lance.Id)
            .Select(group => group.First())
            .ToList();

        return new Lances(distinctLances);
    }

    public static Lances RemoverDuplicados(this Lances lances)
    {
        var duplicatedIds = lances
            .GroupBy(lance => lance.Id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        return new Lances(lances.Where(lance => !duplicatedIds.Contains(lance.Id)));
    }

    public static int CompararPorF1(this Lance lance, Lance other)
    {
        if (other == null) return 1;

        if (float.IsNaN(lance.F1) || float.IsInfinity(lance.F1)) return -1;
        if (float.IsNaN(other.F1) || float.IsInfinity(other.F1)) return 1;

        return other.F1.CompareTo(lance.F1);
    }



    /// <summary>
    /// Filtra uma coleção de Lances com base nos valores de F0, F1 e F2
    /// </summary>
    /// <param name="lances">Coleção de Lances a ser filtrada</param>
    /// <param name="f0Filter">Condição de filtro para F0 (opcional)</param>
    /// <param name="f1Filter">Condição de filtro para F1 (opcional)</param>
    /// <param name="f2Filter">Condição de filtro para F2 (opcional)</param>
    /// <returns>Nova coleção Lances filtrada</returns>
    public static Lances FiltrarPorValores(
        this Lances lances,
        Func<float, bool> f0Filter = null,
        Func<float, bool> f1Filter = null,
        Func<float, bool> f2Filter = null)
    {
        var lancesFilter = lances.Where(lance =>
            (f0Filter == null || f0Filter(lance.F0)) &&
            (f1Filter == null || f1Filter(lance.F1)) &&
            (f2Filter == null || f2Filter(lance.F2))
        );

        return new Lances(lancesFilter);
    }

    /// <summary>
    /// Filtra uma coleção de Lances dentro de uma faixa de valores para F0, F1 e F2
    /// </summary>
    /// <param name="lances">Coleção de Lances a ser filtrada</param>
    /// <param name="f0Min">Valor mínimo para F0 (opcional)</param>
    /// <param name="f0Max">Valor máximo para F0 (opcional)</param>
    /// <param name="f1Min">Valor mínimo para F1 (opcional)</param>
    /// <param name="f1Max">Valor máximo para F1 (opcional)</param>
    /// <param name="f2Min">Valor mínimo para F2 (opcional)</param>
    /// <param name="f2Max">Valor máximo para F2 (opcional)</param>
    /// <returns>Nova coleção Lances filtrada</returns>
    public static Lances FiltrarPorIntervalo(
        this Lances lances,
        float? f0Min = null, float? f0Max = null,
        float? f1Min = null, float? f1Max = null,
        float? f2Min = null, float? f2Max = null)
    {
        var lancesFilter = lances.Where(lance =>
            (f0Min == null || lance.F0 >= f0Min) &&
            (f0Max == null || lance.F0 <= f0Max) &&
            (f1Min == null || lance.F1 >= f1Min) &&
            (f1Max == null || lance.F1 <= f1Max) &&
            (f2Min == null || lance.F2 >= f2Min) &&
            (f2Max == null || lance.F2 <= f2Max)
        );

        return new Lances(lancesFilter);
    }




    /// <summary>
    /// Ordena a coleção de Lances de forma ascendente baseado no valor de F1
    /// </summary>
    /// <param name="lances">Coleção de Lances a ser ordenada</param>
    /// <param name="descending">Indica se a ordenação deve ser descendente (opcional, padrão é ascendente)</param>
    /// <returns>Nova coleção Lances ordenada</returns>
    public static Lances OrdenarPorF1(
        this Lances lances,
        bool descending = false)
    {
        var orderedLances = descending
            ? lances.OrderByDescending(lance => lance.F1)
            : lances.OrderBy(lance => lance.F1);

        return new Lances(orderedLances);
    }


    /// <summary>
    /// Ordena a coleção de Lances de forma ascendente baseado no valor de F1
    /// </summary>
    /// <param name="lances">Coleção de Lances a ser ordenada</param>
    /// <param name="descending">Indica se a ordenação deve ser descendente (opcional, padrão é descendente)</param>
    /// <returns>Nova coleção Lances ordenada</returns>
    public static Lances OrdenarPorF1Dsc(
        this Lances lances,
        bool descending = true)
    {
        var orderedLances = descending
            ? lances.OrderByDescending(lance => lance.F1)
            : lances.OrderBy(lance => lance.F1);

        return new Lances(orderedLances);
    }


    /// <summary>
    /// Ordena a coleção de Lances de forma ascendente baseado no valor de F1, 
    /// com possibilidade de ordenação secundária
    /// </summary>
    /// <param name="lances">Coleção de Lances a ser ordenada</param>
    /// <param name="thenBySelector">Função para ordenação secundária (opcional)</param>
    /// <param name="descending">Indica se a ordenação deve ser descendente (opcional, padrão é ascendente)</param>
    /// <returns>Nova coleção Lances ordenada</returns>
    public static Lances OrdenarPorF1V2(
        this Lances lances,
        Func<Lance, object> thenBySelector = null,
        bool descending = false)
    {
        IOrderedEnumerable<Lance> orderedLances;

        if (descending)
        {
            orderedLances = thenBySelector == null
                ? lances.OrderByDescending(lance => lance.F1)
                : lances.OrderByDescending(lance => lance.F1)
                        .ThenBy(thenBySelector);
        }
        else
        {
            orderedLances = thenBySelector == null
                ? lances.OrderBy(lance => lance.F1)
                : lances.OrderBy(lance => lance.F1)
                        .ThenBy(thenBySelector);
        }

        return new Lances(orderedLances);
    }


}
