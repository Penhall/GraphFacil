using System.Collections.Generic;

namespace Busisness
{
    /// <summary>
    /// Extensão da lista de inteiros com funcionalidades adicionais.
    /// </summary>
    public class Inteiros : List<int>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe Inteiros com uma coleção.
        /// </summary>
        /// <param name="collection">A coleção inicial de inteiros.</param>
        public Inteiros(IEnumerable<int> collection) : base(collection) { }

        /// <summary>
        /// Inicializa uma nova instância vazia da classe Inteiros.
        /// </summary>
        public Inteiros() : base() { }
    }
}
