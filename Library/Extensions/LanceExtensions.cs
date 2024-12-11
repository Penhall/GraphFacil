using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Extensions
{
    /// <summary>
    /// Classe de extensões para manipulação de listas de objetos Lance.
    /// </summary>
    public static class LanceExtensions
    {
        /// <summary>
        /// Combina os elementos de duas listas de "Lance" em uma terceira lista.
        /// </summary>
        /// <param name="lista1">A primeira lista de "Lance" a ser combinada.</param>
        /// <param name="lista2">A segunda lista de "Lance" a ser combinada.</param>
        /// <returns>Uma lista contendo as combinações dos elementos das listas de entrada.</returns>
        public static List<Lance> Combinar(List<Lance> lista1, List<Lance> lista2)
        {
            List<Lance> arAlvos = new List<Lance>();

            arAlvos.AddRange(
                from o1 in lista1
                from o2 in lista2
                let l = new List<int>(o1.Lista.Concat(o2.Lista))
                select new Lance(arAlvos.Count, l)
            );

            return arAlvos;
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


    }
}
