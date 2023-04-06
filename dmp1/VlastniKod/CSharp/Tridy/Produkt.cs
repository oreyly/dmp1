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
    public enum DruhProduktu
    {
        Pozadi,
        ProfilovaFotka
    }

    [NotifyPropertyChanged]
    public class Produkt
    {
        public int Id;
        private string PuvodniNazev;
        public string Nazev { get; set; }
        private URLAdresa PuvodniURL;
        public URLAdresa URL { get; set; }
        private int PuvodniCena;
        public int Cena { get; set; }
        public DruhProduktu druhProduktu { get; set; }
        public bool Koupeno { get; set; }
        public bool ZmenilSe
        {
            get
            {
                return !(PuvodniNazev == Nazev && PuvodniCena == Cena && PuvodniURL == URL);
            }
        }

        private Produkt()
        {

        }

        public Produkt(DruhProduktu druh)
        {
            Novy = true;
            Nazev = " ";
            druhProduktu = druh;
        }

        public Produkt(int id, string url, int cena, DruhProduktu druhProduktu, bool koupeno)
        {
            Id = id;
            URL = url;
            Cena = cena;
            this.druhProduktu = druhProduktu;
            Koupeno = koupeno;
        }

        public Produkt(int id, string nazev, string url, int cena, DruhProduktu druhProduktu)
        {
            Id = id;
            Nazev = PuvodniNazev = nazev;
            URL = PuvodniURL = url;
            Cena = PuvodniCena = cena;
            this.druhProduktu = druhProduktu;
        }

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

        public bool Novy { get; set; }
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

                PraceSDB.ZavolejPrikaz(druhProduktu == DruhProduktu.ProfilovaFotka ? "uloz_avatar" : "uloz_tema", true, Id, Nazev, URL, Cena);
                PuvodniNazev = Nazev;
                PuvodniURL = URL;
                PuvodniCena = Cena;
            }

            return true;
        }

        public void ObnovURL()
        {
            URL = PuvodniURL;
        }

        public void ObnovVse()
        {
            Nazev = PuvodniNazev;
            URL = PuvodniURL;
            Cena = PuvodniCena;
        }
    }
}
