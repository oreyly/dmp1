using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    public enum UrovenPrav { Zak, Ucitel, Administrator }
    //Přihlášený uživatel
    static class Uzivatel
    {
        public static int Id = 2;
        public static int HerniId { get; set; }

        public static string Jmeno = "Matyáš Matějka (matema)";
        public static int Body;
        public static UrovenPrav Prava = UrovenPrav.Ucitel;

        public static void NactiUzivatele(int id, string jmeno, int body, UrovenPrav prava)
        {
            Id = id;
            Jmeno = jmeno;
            Body = body;
            Prava = prava;
        }
    }
}
