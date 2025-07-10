using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotoLibrary.Utilities
{
    public class Uva:IComparable
    {
        public Uva(int id, int pt)
        {
            Id = id;
            Pt = pt;
        }

        public int Id {  get; set; }
        public int Pt {  get; set; }

        public int CompareTo(object obj)
        {
            Uva uva= obj as Uva;    
            return -Pt.CompareTo(uva.Pt);
        }
    }

    public class Uvas : List<Uva> { }
}
