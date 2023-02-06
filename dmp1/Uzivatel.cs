using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    static class Uzivatel
    {
        public static string Jmeno;
        public static int Body;
        public static int Prava;

        public static void NactiUzivatele(string jmeno, int body, int prava)
        {
            Jmeno = jmeno;
            Body = body;
            Prava = prava;
        }
    }
}
