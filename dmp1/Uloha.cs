using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PostSharp.Patterns.Model;

namespace dmp1
{
    //Třída obsahující potřebná data o jednotlivých úlohách
    [NotifyPropertyChanged]
    public class Uloha
    {
        public string Nazev { get; set; }
        public string Popis { get; set; }

        private string _Vysledek;
        public string Vysledek
        {
            get
            {
                return _Vysledek;
            }
            set
            {
                _Vysledek = value;
                string[] data = value.Split(new string[] { "$$$" }, StringSplitOptions.None);
                if (OtevrenyVysledek = data[0] == "O")
                {

                }
                else
                {
                    SpravnyVysledek = Convert.ToInt32(data[5]);
                    CastiVysledku4.NastavHodnoty(data.Skip(1).Take(4));
                }
            }
        }

        public bool OtevrenyVysledek { get; set; } //Jestli má úloha otevřené odpovědi
        public ObservableCollection<string> CastiVysledku4 { get; set; } //Případné ABCD odpovědi
        public int SpravnyVysledek { get; set; } //Kolikátá odpověď je správná
        public string Napoveda { get; set; }
        private string _ObrazekPredpis;
        public string ObrazekPredpis
        {
            get
            {
                return _ObrazekPredpis;
            }
            set
            {
                _ObrazekPredpis = value;
                string[] data = value.Split(new string[] { "$$$" }, StringSplitOptions.None);
                if (data[0] == "URL")
                {
                    Obrazek = data[1];
                }
            }
        }
        public string Obrazek { get; set; } //URL obrázku
        public string Predpis { get; set; } //Případný předpis grafu apod,
        public int Body { get; set; }
        public string Kategorie { get; set; }
        public bool Otevrena; //Jestli je aktuálně úloha otevřená ve hře
        public Brush Barva //Nastavení barvy v seznamu úloh ve hře
        {
            get
            {
                if (Body < 50)
                {
                    return new SolidColorBrush(Colors.Red);
                }

                return new SolidColorBrush(Colors.Green);
            }
        }

        //Nastavení základních hodnot
        public Uloha(string nazev, string popis, string vysledek, string napoveda, string obrPred, int body, string kategorie)
        {
            CastiVysledku4 = new ObservableCollection<string>();
            Nazev = nazev;
            Popis = popis;
            Vysledek = vysledek;
            Napoveda = napoveda;
            ObrazekPredpis = obrPred;
            Body = body;
            Kategorie = kategorie;
        }

        public override string ToString()
        {
            return Nazev;
        }
    }
}
