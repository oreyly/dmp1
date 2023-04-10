using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    public enum UrovenPrav { Zak, Ucitel, Administrator }
    //Přihlášený uživatel
    public static class Uzivatel
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public static int Id = 2;
        public static int HerniId { get; set; }
        public static string Jmeno { get; set; }
        public static int Body { get; set; } = 101;
        public static UrovenPrav Prava { get; set; } = UrovenPrav.Zak;
        public static URLAdresa ObrazekProfil { get; set; }// = "https://home.spsostrov.cz/~matema/dlouhodobka/avatari/profil02.jpg";
        public static URLAdresa ObrazekPozadi { get; set; }// = "http://home.spsostrov.cz/~matema/dlouhodobka/temata/tema01.jpg";

        public static void NactiUzivatele(string nazev)
        {
            PraceSDB.ZavolejPrikaz("prihlasit", false, nazev);

            string[] uzivatel = ((string)PraceSDB.ZavolejPrikaz("nacti_uzivatele", true, nazev)[0][0]).RozdelDolary();

            Id = Convert.ToInt32(uzivatel[0]);
            Jmeno = uzivatel[1];
            Body = Convert.ToInt32(uzivatel[2]);
            Prava = (UrovenPrav)Convert.ToInt32(uzivatel[3]);
            ObrazekPozadi = uzivatel[4];
            ObrazekProfil = uzivatel[5];

        }

        public static void NastavProdukt(DruhProduktu dp, URLAdresa url)
        {
            switch (dp)
            {
                case DruhProduktu.ProfilovaFotka:
                    PraceSDB.ZavolejPrikaz("nastav_avatar", false, Id, url.Soubor);
                    ObrazekProfil = url;
                    StaticPropertyChanged(null, new PropertyChangedEventArgs("ObrazekProfil"));
                    break;
                case DruhProduktu.Pozadi:
                    ObrazekPozadi = url;
                    StaticPropertyChanged(null, new PropertyChangedEventArgs("ObrazekPozadi"));
                    break;
            }
        }
    }
}
