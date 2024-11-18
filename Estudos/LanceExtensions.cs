using LotoLibrary.Models;
using System.Collections.Generic;
using System.Linq;


namespace Busisness
{
    /// <summary>
    /// Combina duas listas de objetos "Lance" em uma terceira lista.
    /// </summary>
    /// <param name="arAlvos1">A primeira lista de objetos "Lance".</param>
    /// <param name="arAlvos2">A segunda lista de objetos "Lance".</param>
    /// <param name="arAlvos">A lista resultante que conterá as combinações.</param>
    public static class LanceExtensions
    {
        /// <summary>
        /// Combina os elementos de duas listas de "Lance" em uma terceira lista.
        /// </summary>
        /// <param name="lista1">A primeira lista de "Lance" a ser combinada.</param>
        /// <param name="lista2">A segunda lista de "Lance" a ser combinada.</param>
        /// <returns>Uma lista contendo as combinações dos elementos das listas de entrada.</returns>
        public static void CombinarLances(this Lances arAlvos, Lances lista1, Lances lista2)
        {

            arAlvos.AddRange(
                from o1 in lista1
                from o2 in lista2
                let l = new List<int>(o1.Lista.Concat(o2.Lista))
                select new Lance(arAlvos.Count, l)
            );

        }
    }


}




