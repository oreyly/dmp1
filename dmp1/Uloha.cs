using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PostSharp.Patterns.Model;
using NC = NCalc;

namespace dmp1
{
    //Třída obsahující potřebná data o jednotlivých úlohách
    [NotifyPropertyChanged]
    public class Uloha
    {
        private int Id;
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                string[] data = value.Split(new string[] { "$$$" }, StringSplitOptions.None);
                if (OtevrenyVysledek = data[0] == "O")
                {
                    List<Par> datka = new List<Par>();
                    for (int i = 1; i < data.Length; i += 2)
                    {
                        datka.Add(new Par(data[i], data[i + 1]));
                    }
                    otevreneVysledky.NastavHodnoty(datka);
                }
                else
                {
                    SpravnyVysledek = Convert.ToInt32(data[5]);
                    CastiVysledku4.NastavHodnoty(data.Skip(1).Take(4));
                }
            }
        }

        public bool OtevrenyVysledek { get; set; } //Jestli má úloha otevřené odpovědi
        public ObservableCollection<Par> otevreneVysledky { get; set; }
        public ObservableCollection<string> CastiVysledku4 { get; set; } //Případné ABCD odpovědi
        public int SpravnyVysledek { get; set; } //Kolikátá odpověď je správná
        public string Napoveda { get; set; }
        public bool obsahujeObrazek { get; set; }
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
                    obsahujeObrazek = true;
                    Obrazek = data[1];
                }
                else if (data[0] == "F")
                {
                    obsahujeObrazek = false;
                    Predpis = data[1];
                }
                else
                {
                    obsahujeObrazek = true;
                }
            }
        }
        public string Obrazek { get; set; } //URL obrázku
        public string Predpis { get; set; } //Případný předpis grafu apod.
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

        private Uloha Zaklad;
        [SafeForDependencyAnalysis]
        public bool ZmenilSe
        {
            get
            {
                //Par[] p1 = Zaklad.otevreneVysledky.ToArray();
                //Par[] a = otevreneVysledky.ToArray();
                //bool a = Enumerable.SequenceEqual(Zaklad.otevreneVysledky.ToArray(), otevreneVysledky.ToArray());
                if(Nova)
                {
                    return true;
                }

                return !(Zaklad.Nazev == Nazev 
                    && Zaklad.Popis == Popis 
                    && Zaklad.OtevrenyVysledek == OtevrenyVysledek 
                    && Enumerable.SequenceEqual(Zaklad.otevreneVysledky, otevreneVysledky, new ParComparer()) 
                    && Enumerable.SequenceEqual(Zaklad.CastiVysledku4, CastiVysledku4) 
                    && Zaklad.SpravnyVysledek == SpravnyVysledek 
                    && Zaklad.Napoveda == Napoveda 
                    && Zaklad.obsahujeObrazek == obsahujeObrazek 
                    && Zaklad.Obrazek == Obrazek 
                    && Zaklad.Predpis == Predpis 
                    && Zaklad.Body == Body 
                    && Zaklad.Kategorie == Kategorie);
            }
        }
        private void Naklonuj()
        {
            Zaklad = new Uloha()
            {
                Nazev = Nazev,
                Popis = Popis,
                OtevrenyVysledek = OtevrenyVysledek,
                otevreneVysledky = new ObservableCollection<Par>(otevreneVysledky.Select(p => new Par(p.Otazka, p.Odpoved))),
                CastiVysledku4 = new ObservableCollection<string>(CastiVysledku4),
                SpravnyVysledek = SpravnyVysledek,
                Napoveda = Napoveda,
                obsahujeObrazek = obsahujeObrazek,
                Obrazek = Obrazek,
                Predpis = Predpis,
                Body = Body,
                Kategorie = Kategorie
            };
        }

        //Nastavení základních hodnot
        public Uloha(string nazev, string popis, string vysledek, string napoveda, string obrPred, int body, string kategorie, int id)
        {
            CastiVysledku4 = new ObservableCollection<string>() { "", "", "", "" };
            otevreneVysledky = new ObservableCollection<Par>() { new Par("", "") };
            Nazev = nazev;
            Popis = popis;
            Vysledek = vysledek;
            Napoveda = napoveda;
            ObrazekPredpis = obrPred;
            Body = body;
            Kategorie = kategorie;
            Nova = false;
            Id = id;

            Naklonuj();
        }

        public Uloha()
        {
            CastiVysledku4 = new ObservableCollection<string>() { "", "", "", "" };
            SpravnyVysledek = 1;
            otevreneVysledky = new ObservableCollection<Par>() { new Par("", "") };
            Nazev = "";
            Popis = "";
            Vysledek = "";
            Napoveda = "";
            ObrazekPredpis = "";
            Body = 0;
            Kategorie = "";
            Nova = true;
        }

        public override string ToString()
        {
            return Nazev + 10;
        }

        public readonly bool Nova;
        public bool UlozSe(IEnumerable<Uloha> seznam)
        {
            if (string.IsNullOrWhiteSpace(Nazev))
            {
                MessageBox.Show("Úloha nelze uložit! - Neplatný nadpis");
                return false;
            }

            if (seznam.Count(ul => ul.Nazev == Nazev && ul != this) > 0)
            {
                MessageBox.Show("Úloha nelze uložit! - Nadpis je již využíván");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Popis))
            {
                MessageBox.Show("Úloha nelze uložit! - Neplatný popis");
                return false;
            }

            string obrPredpis;
            if (obsahujeObrazek)
            {
                if (string.IsNullOrWhiteSpace(ObrazekPredpis))
                {
                    obrPredpis = "N";
                }
                else
                {
                    obrPredpis = ObrazekPredpis;
                }
            }
            else
            {
                if (HlavniStatik.VytvorFunkci(Predpis) == null)
                {
                    MessageBox.Show("Úloha nelze uložit! - Neplatný předpis funkce");
                    return false;
                }
                else
                {
                    obrPredpis = "F$$$" + Predpis;
                }
            }

            if (string.IsNullOrWhiteSpace(Kategorie))
            {
                MessageBox.Show("Úloha nelze uložit! - Neplatná kategorie");
                return false;
            }

            string vysledek;
            if (OtevrenyVysledek)
            {
                vysledek = "O";
                foreach (Par p in otevreneVysledky)
                {
                    if (string.IsNullOrWhiteSpace(p.Otazka) || string.IsNullOrWhiteSpace(p.Odpoved))
                    {
                        MessageBox.Show("Úloha nelze uložit! - Některé otevřené otázce chybí otázka nabo odpověď!");
                        return false;
                    }
                    vysledek += $"$$${p.Otazka}$$${p.Odpoved}";
                }
            }
            else
            {
                vysledek = "M";
                foreach (string odpoved in CastiVysledku4)
                {
                    if (string.IsNullOrWhiteSpace(odpoved))
                    {
                        MessageBox.Show("Úloha nelze uložit! - Některé z možností výsledků chybí odpověď!");
                        return false;
                    }
                    vysledek += "$$$" + odpoved;
                }
                vysledek += "$$$" + SpravnyVysledek;
            }

            if (Nova)
            {
                PraceSDB.ZavolejPrikaz("vytvor_ulohu", false, Nazev, Popis, vysledek, Napoveda, obrPredpis, Body, Uzivatel.Id, Kategorie);
            }
            else
            {
                PraceSDB.ZavolejPrikaz("aktualizuj_ulohu", false, Nazev, Popis, vysledek, Napoveda, obrPredpis, Body, Uzivatel.Id, Kategorie, Id);
            }
            return true;
        }

        public void ObnovSe()
        {
            Nazev = Zaklad.Nazev;
            Popis = Zaklad.Popis;
            OtevrenyVysledek = Zaklad.OtevrenyVysledek;
            otevreneVysledky = new ObservableCollection<Par>(Zaklad.otevreneVysledky.Select(p => new Par(p.Otazka, p.Odpoved)));
            CastiVysledku4 = new ObservableCollection<string>(Zaklad.CastiVysledku4);
            SpravnyVysledek = Zaklad.SpravnyVysledek;
            Napoveda = Zaklad.Napoveda;
            obsahujeObrazek = Zaklad.obsahujeObrazek;
            Obrazek = Zaklad.Obrazek;
            Predpis = Zaklad.Predpis;
            Body = Zaklad.Body;
            Kategorie = Zaklad.Kategorie;
        }

        internal void OdstranSe()
        {
            PraceSDB.ZavolejPrikaz("odstran_ulohu", false, Id);
        }
    }
}
