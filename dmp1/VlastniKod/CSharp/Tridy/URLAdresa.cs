using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    public class URLAdresa
    {
        private static readonly string _Koren;
        public static string Koren
        {
            get
            {
                return _Koren;
            }
        }

        public readonly string Soubor;

        static URLAdresa()
        {
            _Koren = (string)PraceSDB.ZavolejPrikaz("nacti_konstantu", true, "korenova_adresa")[0][0];
        }

        public URLAdresa(string soubor)
        {
            Soubor = soubor;
        }

        public static implicit operator string(URLAdresa urlAdresa)
        {
            if(urlAdresa is null)
            {
                return "";
            }

            return Koren + urlAdresa.Soubor;
        }

        public static implicit operator URLAdresa(string soubor)
        {
            return new URLAdresa(soubor);
        }


        public static bool operator ==(URLAdresa urlAdresa1, URLAdresa urlAdresa2)
        {
            if (urlAdresa1 is null)
            {
                return urlAdresa2 is null;
            }

            if (urlAdresa2 is null)
            {
                return urlAdresa1 is null;
            }

            return urlAdresa1.Equals(urlAdresa2);
        }
        public static bool operator !=(URLAdresa urlAdresa1, URLAdresa urlAdresa2)
        {
            return !urlAdresa1.Equals(urlAdresa2);
        }

        public override string ToString()
        {
            return this;
        }

        public override bool Equals(object obj)
        {
            if(obj is URLAdresa url)
            {
                return url.Soubor == Soubor;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
