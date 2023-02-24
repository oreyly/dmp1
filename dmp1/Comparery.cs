using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    class ParComparer : IEqualityComparer<Par>
    {
        public bool Equals(Par x, Par y)
        {
            if (x.Otazka == y.Otazka && x.Odpoved == y.Odpoved)
                return true;

            return false;
        }

        public int GetHashCode(Par obj)
        {
            return obj.GetHashCode();
        }
    }
}
