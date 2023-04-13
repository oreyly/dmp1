using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace dmp1
{
    [NotifyPropertyChanged]
    //Třída představující avatar nebo téma
    public class Produkt
    {
        //Id v databázi
        public int Id;
        //Název před editací
        private string PuvodniNazev;
        //Název
        public string Nazev { get; set; }
        //Adresa obrázku před editací
        private URLAdresa PuvodniURL;
        //Adresa obrázku
        public URLAdresa URL { get; set; }
        //Cena před editací
        private int PuvodniCena;
        //Cena
        public int Cena { get; set; }
        //Co to je za produkt
        public DruhProduktu druhProduktu { get; set; }
        //Jestli je již zakoupen
        public bool Koupeno { get; set; }
        //Porovná aktuální a původní hodnoty
        public bool ZmenilSe
        {
            get
            {
                return !(PuvodniNazev == Nazev && PuvodniCena == Cena && PuvodniURL == URL);
            }
        }

        //Konstruktor pro prázdný produkt
        public Produkt(DruhProduktu druh)
        {
            Novy = true;
            Nazev = " ";
            druhProduktu = druh;
        }

        //Konstruktor pro obchod
        public Produkt(int id, string url, int cena, DruhProduktu druhProduktu, bool koupeno)
        {
            Id = id;
            URL = url;
            Cena = cena;
            this.druhProduktu = druhProduktu;
            Koupeno = koupeno;
        }

        //Konstruktor pro editaci
        public Produkt(int id, string nazev, string url, int cena, DruhProduktu druhProduktu)
        {
            Id = id;
            Nazev = PuvodniNazev = nazev;
            URL = PuvodniURL = url;
            Cena = PuvodniCena = cena;
            this.druhProduktu = druhProduktu;
        }

        //Pokusí se zakoupit produkt
        public void Koupit()
        {
            if (Cena <= Uzivatel.Body)
            {
                if (druhProduktu == DruhProduktu.ProfilovaFotka)
                {
                    PraceSDB.ZavolejPrikaz("koupit_avatar", false, Uzivatel.Id, Id);
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("koupit_tema", false, Uzivatel.Id, Id);
                }
                Koupeno = true;
                Uzivatel.Body -= Cena;
                LepsiMessageBox.Show("Zakoupeno");
            }
            else
            {
                LepsiMessageBox.Show("Příliš drahé");
            }
        }

        //Jestli je produkt nově vytvořen
        public bool Novy { get; set; }

        //Zkusí uložit změny produktu do databáze
        public bool Ulozit()
        {
            if (Novy)
            {
                if (string.IsNullOrWhiteSpace(Nazev))
                {
                    LepsiMessageBox.Show("Název nesmí být prázdný!");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(URL))
                {
                    LepsiMessageBox.Show("Není vybrán žádný obrázek!");
                    return false;
                }

                if (Nazev != PuvodniNazev && (bool)PraceSDB.ZavolejPrikaz(druhProduktu == DruhProduktu.ProfilovaFotka ? "existuje_nazev_avatara" : "existuje_nazev_tematu", true, Nazev)[0][0])
                {
                    LepsiMessageBox.Show("Název již existuje!");
                    return false;
                }

                Id = (int)PraceSDB.ZavolejPrikaz(druhProduktu == DruhProduktu.ProfilovaFotka ? "vytvor_avatar" : "vytvor_tema", true, Nazev, URL.Soubor, Cena)[0][0];
                PuvodniNazev = Nazev;
                PuvodniURL = URL;
                PuvodniCena = Cena;
                Novy = false;
            }
            else if(ZmenilSe)
            {
                if (string.IsNullOrWhiteSpace(Nazev))
                {
                    LepsiMessageBox.Show("Název nesmí být prázdný!");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(URL))
                {
                    LepsiMessageBox.Show("Není vybrán žádný obrázek!");
                    return false;
                }

                if (Nazev != PuvodniNazev && (bool)PraceSDB.ZavolejPrikaz(druhProduktu == DruhProduktu.ProfilovaFotka ? "existuje_nazev_avatara" : "existuje_nazev_tematu", true, Nazev)[0][0])
                {
                    LepsiMessageBox.Show("Název již existuje!");
                    return false;
                }

                PraceSDB.ZavolejPrikaz(druhProduktu == DruhProduktu.ProfilovaFotka ? "uloz_avatar" : "uloz_tema", true, Id, Nazev, URL.Soubor, Cena);
                PuvodniNazev = Nazev;
                PuvodniURL = URL;
                PuvodniCena = Cena;
            }

            return true;
        }

        //Obnoví adresu obrázku do původního stavu
        public void ObnovURL()
        {
            URL = PuvodniURL;
        }

        //Obnoví produkt do původního stavu
        public void ObnovVse()
        {
            Nazev = PuvodniNazev;
            URL = PuvodniURL;
            Cena = PuvodniCena;
        }
    }
}
