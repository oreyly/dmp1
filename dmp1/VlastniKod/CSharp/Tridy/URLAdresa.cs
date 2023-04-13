using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    [NotifyPropertyChanged]
    //Třída pro oddělení adresy uložiště a adresy souboru
    public class URLAdresa
    {
        // Adresa uložiště
        private static readonly string _Koren;
        public static string Koren
        {
            get
            {
                return _Koren;
            }
        }

        //Adresa souboru v uložišti
        public readonly string Soubor;

        //Statický konstruktor pro nastavení adresy uložiště
        static URLAdresa()
        {
            _Koren = (string)PraceSDB.ZavolejPrikaz("nacti_konstantu", true, "korenova_adresa")[0][0];
        }

        //Normální kontruktor pro vytvoření adresy
        public URLAdresa(string soubor)
        {
            Soubor = soubor;
        }

        //Implicitní převod adresy na string
        public static implicit operator string(URLAdresa urlAdresa)
        {
            if(urlAdresa is null)
            {
                return "";
            }

            return Koren + urlAdresa.Soubor;
        }

        //Implicitní převod stringu na adresu
        public static implicit operator URLAdresa(string soubor)
        {
            return new URLAdresa(soubor);
        }

        //Přepsání porovnávacích operátorů
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


        //Přepsání porovnávacích operátorů
        public static bool operator !=(URLAdresa urlAdresa1, URLAdresa urlAdresa2)
        {
            return !urlAdresa1.Equals(urlAdresa2);
        }

        //ToString vrátí sebe implicitně převedeného na string
        public override string ToString()
        {
            return this;
        }

        //Equals nyní porovnává pouze lokace souborů namísto celých objektů
        public override bool Equals(object obj)
        {
            if(obj is URLAdresa url)
            {
                return url.Soubor == Soubor;
            }

            return base.Equals(obj);
        }

        //Overridnuto pouze aby se Visual Studio nerozčilovalo
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
