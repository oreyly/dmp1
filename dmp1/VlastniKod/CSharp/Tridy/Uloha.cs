using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using EnumsNET;
using Newtonsoft.Json.Linq;
using PostSharp.Patterns.Model;
using NC = NCalc;

namespace dmp1
{
    //Třída obsahující potřebná data o jednotlivých úlohách
    [NotifyPropertyChanged]
    public class Uloha
    {
        //Id úlohy
        private int Id;
        //Název/nadpis úlohy
        public string Nazev { get; set; }
        //Název kategorie úlohy
        public string Kategorie { get; set; }
        //Zadání úlohy
        public string Popis { get; set; }

        //Výsledek úlohy
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

                string[] data = value.RozdelDolary();
                if (OtevrenyVysledek = data[0] == "O") //O značí otevřené odpovědi
                {
                    List<Par<string, string>> datka = new List<Par<string, string>>();
                    for (int i = 1; i < data.Length; i += 2)
                    {
                        datka.Add(new Par<string, string>(data[i], data[i + 1]));
                    }
                    otevreneVysledky.NastavHodnoty(datka);
                    if (hra != null)
                    {
                        otevreneVysledkyOdpovedi.NastavHodnoty(datka.Select((dato, i) => new Par<string, Par<string, string>>(dato.Klic, new Par<string, string>(otevreneVysledky[i].Hodnota,hra.DruhSpusteni == DruhSpusteni.Uceni ? dato.Hodnota : ""))));
                    }
                }
                else //Jinak jsou odpovědi ve formě možností
                {
                    SpravnyVysledek = Convert.ToInt32(data[5]);
                    if(hra != null && hra.DruhSpusteni == DruhSpusteni.Uceni)
                    {
                        SpravnyVysledekOdpoved = SpravnyVysledek;
                    }
                    CastiVysledku4.NastavHodnoty(data.Skip(1).Take(4));
                }
            }
        }

        //Jestli má úloha otevřené odpovědi
        public bool OtevrenyVysledek { get; set; }
        //Pole otevřených odpovědí -> otázka-odpověď
        public ObservableCollection<Par<string, string>> otevreneVysledky { get; set; }
        //Pole otevřených odpovědí zadaných hráčem -> otázka-odpověď
        public ObservableCollection<Par<string, Par<string, string>>> otevreneVysledkyOdpovedi { get; set; }
        //ABCD možnosti odpovědí
        public ObservableCollection<string> CastiVysledku4 { get; set; }
        //Kolikátá odpověď je správná (od 1)
        public int SpravnyVysledek { get; set; }
        //Kolikátou odpověď hráč označil (od 1)
        public int SpravnyVysledekOdpoved { get; set; }
        //Nápověda k výsledku
        public string Napoveda { get; set; } 

        //Předpis obrázku -> URL$$$cesta nebo N bez obrázku
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
                string[] data = value.RozdelDolary();
                if (data[0] == "URL")
                {
                    Obrazek = data[1];
                }
            }
        }

        //URL obrázku
        public URLAdresa Obrazek { get; set; } 

        //Počet bodů za správnou odpověď
        private int _Body;
        public object Body { 
            get
            {
                //Pokud je úloha ve hře k učení, zobrazuje místo hodnot pouze pomlčky
                if (hra != null && hra.DruhSpusteni == DruhSpusteni.Uceni)
                {
                    return "---";
                }
                else
                {
                    return _Body;
                }
            }
            set
            {
                if (int.TryParse(value.ToString(), out int b))
                {
                    _Body = b;
                }
            }
        }

        //Jestli je aktuálně úloha otevřená ve hře
        public bool Otevrena { get; set; } 
        //Stav úlohy
        public StavUlohy stavUlohy { get; set; } = StavUlohy.Prazdna;
        //Jestli je úloha záchytným bodem
        public bool ZachytnyBod { get; set; } = false;
        //Hra do které úloha patří
        private Hra hra { get; set; }
        //Hranice, která se má vykreslovat na herní ploše kolem dané úlohy
        public float TloustkaOhraniceni
        {
            get
            {
                //Obrys pouze pokud je otevřená
                if(Otevrena)
                {
                    return 2f;
                }
                else
                {
                    return 0f;
                }
            }
        }

        //Nastavení barvy v seznamu úloh ve hře
        public Brush Barva 
        {
            get
            {
                if (ZachytnyBod)
                {
                    return Brushes.BlanchedAlmond;
                }

                if (hra.DruhSpusteni == DruhSpusteni.Uceni)
                {
                    return Brushes.Gray;
                }

                switch (stavUlohy)
                {
                    case StavUlohy.Spravne:
                        return Brushes.Green;
                    case StavUlohy.Spatne:
                        return Brushes.Red;
                    case StavUlohy.Prazdna:
                        return Brushes.Gray;
                    case StavUlohy.Zodpovezena:
                        return Brushes.Yellow;
                    default:
                        throw new Exception("Neznámý stav úlohy");
                }
            }
        }

        //Úloha před editací
        private Uloha Zaklad;

        //Porovná aktuální vlastnosti s vlastnostmi před editací a zjistí, jestli se něco změnilo
        [SafeForDependencyAnalysis]
        public bool ZmenilSe
        {
            get
            {
                if(Nova)
                {
                    return true;
                }

                return !(Zaklad.Nazev == Nazev 
                    && Zaklad.Popis == Popis 
                    && Zaklad.OtevrenyVysledek == OtevrenyVysledek 
                    && Enumerable.SequenceEqual(Zaklad.otevreneVysledky, otevreneVysledky, new ParComparer<string,string>()) 
                    && Enumerable.SequenceEqual(Zaklad.CastiVysledku4, CastiVysledku4) 
                    && Zaklad.SpravnyVysledek == SpravnyVysledek 
                    && Zaklad.Napoveda == Napoveda 
                    && Zaklad.Obrazek == Obrazek
                    && Zaklad.Body.Equals(Body));
            }
        }

        //Uloží svou kopii před editací
        private void Naklonuj()
        {
            Zaklad = new Uloha()
            {
                Nazev = Nazev,
                Popis = Popis,
                OtevrenyVysledek = OtevrenyVysledek,
                otevreneVysledky = new ObservableCollection<Par<string, string>>(otevreneVysledky.Select(p => new Par<string, string>(p.Klic, p.Hodnota))),
                CastiVysledku4 = new ObservableCollection<string>(CastiVysledku4),
                SpravnyVysledek = SpravnyVysledek,
                Napoveda = Napoveda,
                Obrazek = Obrazek,
                Body = Body,
            };
        }

        //Konstruktor využívaný při kontrole úloh
        public Uloha(Hra h, string nazev, string popis, string vysledek, string obrPred, int body, int id, string vysledekOdpoved, int spravne)
        {
            hra = h;

            Id = id;
            Nazev = nazev;
            Popis = popis;
            otevreneVysledky = new ObservableCollection<Par<string, string>>();
            otevreneVysledkyOdpovedi = new ObservableCollection<Par<string, Par<string, string>>>();
            CastiVysledku4 = new ObservableCollection<string>() { "", "", "", "" };
            Vysledek = vysledek;
            ObrazekPredpis = obrPred;
            Body = body;

            if (OtevrenyVysledek)
            {
                string[] data = vysledekOdpoved.RozdelDolary();
                List<Par<string, Par<string, string>>> datka = new List<Par<string, Par<string, string>>>();
                for (int i = 0; i < data.Length; ++i)
                {
                    datka.Add(new Par<string, Par<string, string>>(otevreneVysledky[i].Klic, new Par<string, string>(otevreneVysledky[i].Hodnota, data[i])));
                }
                otevreneVysledkyOdpovedi.NastavHodnoty(datka);
            }
            else
            {
                SpravnyVysledekOdpoved = CastiVysledku4.IndexOf(vysledekOdpoved) + 1;
            }

            stavUlohy = spravne switch
            {
                1 => StavUlohy.Spravne,
                2 => StavUlohy.Spatne,
                3 => StavUlohy.Zodpovezena,
                _ => throw new Exception("Neznámý stav úlohy")
            };
        }


        //Konstruktor pro vytvoření normální úlohy
        public Uloha(string nazev, string popis, string vysledek, string napoveda, string obrPred, int body, int id, Hra h = null, string kategorie = "")
        {
            hra = h;
            Kategorie = kategorie;
            CastiVysledku4 = new ObservableCollection<string>() { "", "", "", "" };
            otevreneVysledky = new ObservableCollection<Par<string, string>>() { new Par<string, string>("", "") };
            otevreneVysledkyOdpovedi = new ObservableCollection<Par<string, Par<string, string>>>() { new Par<string, Par<string, string>>("", new Par<string, string>("", "")) };
            Nazev = nazev;
            Popis = popis;
            Vysledek = vysledek;
            Napoveda = napoveda;
            ObrazekPredpis = obrPred;
            Body = body;
            Nova = false;
            Id = id;

            if (h == null)
            {
                Naklonuj();
            }
        }

        //Konstruktor pro vytvoření prázdné úlohy, maximálně s kategorií
        public Uloha(string kategorie = "")
        {
            Kategorie = kategorie;
            CastiVysledku4 = new ObservableCollection<string>() { "", "", "", "" };
            SpravnyVysledek = 1;
            otevreneVysledky = new ObservableCollection<Par<string, string>>() { new Par<string, string>("", "") };
            Nazev = "";
            Popis = "";
            Vysledek = "";
            Napoveda = "";
            ObrazekPredpis = "N";
            Body = 0;
            Nova = true;
        }

        //Zamíchá ABCD možnosti a taky správně nastaví nový správný výsledek
        public void ZamichejMoznosti()
        {
            if(!OtevrenyVysledek)
            {
                string spravny = CastiVysledku4[SpravnyVysledek - 1];
                CastiVysledku4.ZamichejList();
                if (SpravnyVysledek == SpravnyVysledekOdpoved)
                {
                    SpravnyVysledek = CastiVysledku4.IndexOf(spravny) + 1;
                    SpravnyVysledekOdpoved = SpravnyVysledek;
                }
                else
                {
                    SpravnyVysledek = CastiVysledku4.IndexOf(spravny) + 1;
                }
            }
        }

        //Značí jestli je úloha nová a měla by se teprve zapsat do databáze
        public bool Nova { get; set; }
        //Zapíše úlohu do databáze pokud splňuje všechny
        public bool UlozSe()
        {
            if (!Nova && !ZmenilSe)
            {
                return true;
            }

            if (LepsiMessageBox.Show("Uložit změny?", DruhTlacitekLMB.AnoNe) != MessageBoxResult.Yes)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(Nazev))
            {
                LepsiMessageBox.Show("Úloha nelze uložit! - Neplatný nadpis");
                return false;
            }

            if ((Zaklad is null || Nazev != Zaklad.Nazev) && (bool)PraceSDB.ZavolejPrikaz("existuje_nazev_ulohy", true, Uzivatel.Id, Nazev)[0][0])
            {
                LepsiMessageBox.Show("Úloha nelze uložit! - Nadpis je již využíván");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Popis) && Obrazek is null)
            {
                LepsiMessageBox.Show("Úloha nelze uložit! - Neplatný popis");
                return false;
            }

            if ((int)Body <= 0)
            {
                LepsiMessageBox.Show("Úloha nelze uložit! - Neplatný počet bodů");
                return false;
            }

            string obrPredpis;
            if (string.IsNullOrWhiteSpace(Obrazek?.Soubor))
            {
                obrPredpis = "N";
            }
            else
            {
                obrPredpis = "URL$$$" + Obrazek.Soubor;
            }

            string vysledek;
            if (OtevrenyVysledek)
            {
                vysledek = "O";
                foreach (Par<string, string> p in otevreneVysledky)
                {
                    if (string.IsNullOrWhiteSpace(p.Klic) || string.IsNullOrWhiteSpace(p.Hodnota))
                    {
                        LepsiMessageBox.Show("Úloha nelze uložit! - Některé otevřené otázce chybí otázka nabo odpověď!");
                        return false;
                    }
                    vysledek += $"$$${p.Klic}$$${p.Hodnota}";
                }
                object o = otevreneVysledky.Select(p => p.Klic).Distinct().Count();
                if (otevreneVysledky.Select(p => p.Klic).Distinct().Count() != otevreneVysledky.Count)
                {
                    LepsiMessageBox.Show("Úloha nelze uložit! - Některá z otázek je duplicitní!");
                    return false;
                }
            }
            else
            {
                vysledek = "M";
                foreach (string odpoved in CastiVysledku4)
                {
                    if (string.IsNullOrWhiteSpace(odpoved))
                    {
                        LepsiMessageBox.Show("Úloha nelze uložit! - Některé z možností výsledků chybí odpověď!");
                        return false;
                    }
                    vysledek += "$$$" + odpoved;
                }
                vysledek += "$$$" + SpravnyVysledek;

                if (CastiVysledku4.Distinct().Count() != CastiVysledku4.Count)
                {
                    LepsiMessageBox.Show("Úloha nelze uložit! - Některé z možností výsledků je duplicitní!");
                    return false;
                }
            }

            if (Nova)
            {
                Id = (int)PraceSDB.ZavolejPrikaz("vytvor_ulohu", true, Nazev, Popis, vysledek, Napoveda, obrPredpis, Body, Uzivatel.Id, Kategorie)[0][0];
                Nova = false;
            }
            else
            {
                PraceSDB.ZavolejPrikaz("aktualizuj_ulohu", false, Nazev, Popis, vysledek, Napoveda, obrPredpis, Body, Uzivatel.Id, Id);
            }

            Naklonuj();
            return true;
        }

        //Zkontroluje jestli je zadaná odpověď správná
        public void ZkontrolujOdpoved(bool ukazovatMB)
        {
            if (stavUlohy is StavUlohy.Spravne or StavUlohy.Spatne)
            {
                _ = ukazovatMB ? LepsiMessageBox.Show("Úloha již byla zodpovězena") : default;
                return;
            }

            bool spravne;
            string celkovyVysledek = "";
            if (OtevrenyVysledek)
            {
                spravne = true;

                for (int i = 0; i < otevreneVysledky.Count; ++i)
                {
                    string vysledek = otevreneVysledkyOdpovedi[i].Hodnota.Hodnota;

                    celkovyVysledek += $"{vysledek}$$$";
                    if (otevreneVysledky[i].Hodnota != vysledek)
                    {
                        spravne = false;
                    }

                    if (ukazovatMB && string.IsNullOrWhiteSpace(vysledek))
                    {
                        LepsiMessageBox.Show("Chybí jedna z odpovědí!");
                        return;
                    }
                }

                celkovyVysledek = celkovyVysledek.Substring(0, celkovyVysledek.Length - 3);
            }
            else
            {
                if (SpravnyVysledekOdpoved == 0)
                {
                    if(ukazovatMB)
                    {
                        LepsiMessageBox.Show("Není vybrána odpověď!");
                        return;
                    }
                    spravne = false;
                }
                else
                {
                    if (SpravnyVysledekOdpoved== SpravnyVysledek)
                    {
                        spravne = true;
                    }
                    else
                    {
                        spravne = false;
                    }
                    celkovyVysledek = CastiVysledku4[SpravnyVysledekOdpoved - 1];
                }
            }

            PraceSDB.ZavolejPrikaz("uloz_vysledek", false, Id, Uzivatel.HerniId, celkovyVysledek, hra.Kontrolovat ? spravne ? 1 : 2 : 3);

            if (!hra.Kontrolovat)
            {
                stavUlohy = StavUlohy.Zodpovezena;
            }
            else
            {
                stavUlohy = spravne ? StavUlohy.Spravne : StavUlohy.Spatne;
            }
        }

        //Vrátí pole nových úloh, jež reprezentují herní úlohy zadaných idéček
        public static Uloha[] VytvorHerniUlohy(int[] idecka, Hra h)
        {
            object[][] dataUloh = PraceSDB.ZavolejPrikaz("nacti_herni_ulohy", true, idecka).Select(o => (object[])o[0]).ToArray();
            return dataUloh.Select(dato => new Uloha((string)dato[0], (string)dato[1], (string)dato[2], (string)dato[3], (string)dato[4], (int)dato[5], (int)dato[7], h)).ToArray();
        }

        //Změní stav úlohy
        public void PrehodnotStav(int novyStav)
        {
            PraceSDB.ZavolejPrikaz("prehodnotit_vysledek", false, Id, novyStav);

            stavUlohy = novyStav == 1 ? StavUlohy.Spravne : StavUlohy.Spatne;
        }
    }
}
