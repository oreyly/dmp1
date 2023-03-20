using PostSharp.Patterns.Model;
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
        public static int Body = 101;
        public static UrovenPrav Prava = UrovenPrav.Zak;
        public static string ObrazekProfil { get; set; } = "https://home.spsostrov.cz/~matema/dlouhodobka/avatari/profil02.jpg";
        public static string ObrazekPozadi { get; set; } = "http://home.spsostrov.cz/~matema/dlouhodobka/temata/tema01.jpg";

        public static void NactiUzivatele(int id, string jmeno, int body, UrovenPrav prava)
        {
            Id = id;
            Jmeno = jmeno;
            Body = body;
            Prava = prava;
        }
    }
}
