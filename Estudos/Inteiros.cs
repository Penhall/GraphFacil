using System.Collections.Generic;

namespace Busisness
{
    public class Inteiros : List<int>
    {
        public Inteiros(IEnumerable<int> collection) : base(collection) { }
        public Inteiros() : base() { }
    }

}




