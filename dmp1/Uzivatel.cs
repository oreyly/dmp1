using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    //Přihlášený uživatel
    static class Uzivatel
    {
        public static int Id = 2;
        public static string Jmeno = "Matyáš Matějka (matema)";
        public static int Body;
        public static int Prava;

        public static void NactiUzivatele(int id, string jmeno, int body, int prava)
        {
            Id = id;
            Jmeno = jmeno;
            Body = body;
            Prava = prava;
        }
    }
}
