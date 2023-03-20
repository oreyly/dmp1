using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
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
        private int Id;
        public string URL { get; set; }
        public int Cena { get; set; }
        public DruhProduktu druhProduktu { get; set; }
        public bool Koupeno { get; set; }

        private Produkt()
        {

        }

        public Produkt(int id, string url, int cena, DruhProduktu druhProduktu, bool koupeno)
        {
            Id = id;
            URL = url;
            Cena = cena;
            this.druhProduktu = druhProduktu;
            Koupeno = koupeno;
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
                MessageBox.Show("Zakoupeno");
            }
            else
            {
                MessageBox.Show("Příliš drahé");
            }
        }
    }
}
