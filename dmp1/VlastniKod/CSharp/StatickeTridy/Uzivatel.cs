using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    //Přihlášený uživatel
    public static class Uzivatel
    {
        //Event hlídající případné změny kvůli bindingu v XAML
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        //Id účtu
        public static int Id;
        //Herní Id při hře
        public static int HerniId { get; set; }
        //Jméno a příjmeno + přihlašovací jméno v závorce
        public static string Jmeno { get; set; }
        //Aktuální počet bodů
        public static int Body { get; set; } = 101;
        //Kdo to je
        public static UrovenPrav Prava { get; set; } = UrovenPrav.Zak;
        //Adresa profilové fotky
        public static URLAdresa ObrazekProfil { get; set; }
        //Adresa fotky v pozadí
        public static URLAdresa ObrazekPozadi { get; set; }

        //Načte data o uživateli podle přihlašovacího jména
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

        //Nastavý vybraný avatar nebo obrázek v pozadí
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
