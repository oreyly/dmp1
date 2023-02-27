using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    class ParComparer<T,Q> : IEqualityComparer<Par<T, Q>>
    {
        public bool Equals(Par<T, Q> x, Par<T, Q> y)
        {
            if (x.Klic.Equals(y.Klic) && x.Hodnota.Equals(y.Hodnota))
                return true;

            return false;
        }

        public int GetHashCode(Par<T, Q> obj)
        {
            return obj.GetHashCode();
        }
    }
}
