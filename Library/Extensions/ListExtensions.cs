using System.Collections.Generic;
using LotoLibrary.Models.Core;

namespace LotoLibrary.Extensions
{
    /// <summary>
    /// Extensões para List<Lance>
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Converte uma List<Lance> para Lances
        /// </summary>
        /// <param name="list">Lista de Lance</param>
        /// <returns>Objeto Lances</returns>
        public static Lances ToLances(this List<Lance> list)
        {
            if (list == null)
                return new Lances();

            return new Lances(list);
        }
    }
}