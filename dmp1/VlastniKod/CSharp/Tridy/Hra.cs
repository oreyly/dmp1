using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace dmp1
{

    public delegate void ZmenaStavu(object sender, object stary, object novy);
    //Třída obsahující všechna potřebná data pro průběh jedné hry
    [NotifyPropertyChanged]
    public class Hra
    {
        //Id hry
        private int Id { get; set; }
        //Název hry
        public string Nazev { get; set; }

        //Úlohy hry
        public ObservableCollection<Uloha> Ulohy { get; set; }
        //Jestli mají být úlohy náhodně seřazeny
        public bool Nahodne { get; set; }
        //Jestli se má hra sama kontrolovat odpovědi
        public bool Kontrolovat { get; set; }

        //Maximální počet sekund
        private int MaximalniCas 
        {
            set
            {
                CasKonce = DateTime.Now.AddSeconds(value);
            }
        }

        //Čas, kdy se hra sama ukončí
        public DateTime CasKonce { get; set; }

        //V jakém módu byla hru spuštěna
        public DruhSpusteni DruhSpusteni { get; set; }
        //V případě, že aktuální druh spuštění je kontrola, tak co to bylo originálně
        public DruhSpusteni PuvodniDruhSpusteni { get; set; }

        //Aktuálně otevřená úloha
        private Uloha _aktualniUloha;
        public Uloha aktualniUloha
        {
            get
            {
                return _aktualniUloha;
            }

            set
            {
                if (_aktualniUloha != null)
                {
                    _aktualniUloha.Otevrena = false;
                }
                
                if(value != null)
                {
                    value.Otevrena = true;
                }

                Uloha u = _aktualniUloha;
                _aktualniUloha = value;

                ZmenaAktualniUlohy?.Invoke(this, u, value);
            }
        }

        //Event, který se vyvolá při změně úlohy
        public event ZmenaStavu ZmenaAktualniUlohy;


        //Celkový možný počet bodů
        [SafeForDependencyAnalysis]//Atribut NotifyPropertyChanged měl error. bez tohoto atributu
        public int BodyCelkem
        {
            get
            {
                return Ulohy.Sum(u => (int)u.Body);
            }
        }

        //Získaný počet bodů
        [SafeForDependencyAnalysis]//Atribut NotifyPropertyChanged měl error. bez tohoto atributu
        public int BodyZiskal
        {
            get
            {
                return Ulohy.Where(u => u.stavUlohy == StavUlohy.Spravne).Sum(u => (int)u.Body);
            }
        }

        //Vytvoří hru s vlastnostmi pro kontrolu výsledků
        public Hra(int idHry, int[] vysledky, int druh)
        {
            Id = idHry;
            DruhSpusteni = DruhSpusteni.Kontrola;
            PuvodniDruhSpusteni = (DruhSpusteni)druh;

            List<object[]> o = PraceSDB.ZavolejPrikaz("nacti_vysledky_uloh", true, vysledky).Select(radek => (object[])radek[0]).ToList();
            Ulohy = new ObservableCollection<Uloha>(o.Select(u => new Uloha(this, (string)u[0], (string)u[1], (string)u[2], (string)u[3], (int)u[4], (int)u[5], (string)u[6], (int)u[7])));

            return;
        }

        //Nastavení základních hodnot hry
        private Hra(int id, DruhSpusteni druh)
        {
            Id = id;

            Uzivatel.HerniId = (int)PraceSDB.ZavolejPrikaz("nacti_id_hrace", true, Id, Uzivatel.Id)[0][0];

            DruhSpusteni = druh;
        }


        //Nastaví hře potřebné vlastnosti
        public void NastavHru(string nazev, Uloha[] ulohy, bool nahodne, int maximalniCas, bool kontrolovat)
        {
            Nazev = nazev;
            Ulohy = new ObservableCollection<Uloha>(ulohy);

            foreach(Uloha u in Ulohy)
            {
                u.ZamichejMoznosti();
            }

            MaximalniCas = maximalniCas;

            if (DruhSpusteni == DruhSpusteni.Procvicovani)
            {
                Ulohy.NastavHodnoty(Ulohy.OrderBy(u => u.Body).ToArray());
                int krok = (int)Math.Ceiling(Ulohy.Count / 3f);
                Ulohy[krok - 1].ZachytnyBod = true;
                Ulohy[2 * krok - 1].ZachytnyBod = true;
            }
            else if (nahodne)
            {
                Ulohy.ZamichejList();
            }

            Kontrolovat = kontrolovat;

            aktualniUloha = Ulohy[0];
        }

        //Odešle neodeslané odpovědi a ukončí hru
        public void KonecHry()
        {
            foreach(Uloha u in Ulohy)
            {
                u.ZkontrolujOdpoved(false);
            }

            int celkemBodu = 0;
            if(DruhSpusteni != DruhSpusteni.Uceni)
            {
                celkemBodu = Ulohy.Where(uloha => uloha.stavUlohy == StavUlohy.Spravne).Sum(uloha => Convert.ToInt32(uloha.Body));
            }

            PraceSDB.ZavolejPrikaz("konec_hry", false, Uzivatel.HerniId, celkemBodu);
            if(DruhSpusteni != DruhSpusteni.Uceni)
            {
                LepsiMessageBox.Show($"Konec{(DruhSpusteni == DruhSpusteni.Procvicovani ? $", získáno {BodyZiskal} bodů" : "")}!");
            }
        }


        //Vytvoří hru, předá jí potřebné vlastnosti a vrátí nově vytvořenou hru
        public static Hra NactiHru(int id, DruhSpusteni druh)
        {
            string[] data = ((string)PraceSDB.ZavolejPrikaz("nacti_hru", true, id)[0][0]).Split(HlavniStatik.Oddelovac, StringSplitOptions.None);

            Hra h = new Hra(id, druh);

            Uloha[] ulohy = Uloha.VytvorHerniUlohy(data[0].ZiskejPole<int>(), h);

            h.NastavHru(data[1], ulohy, Convert.ToBoolean(data[2]), Convert.ToInt32(data[3]), Convert.ToBoolean(data[4]));

            PraceSDB.ZavolejPrikaz("start_hry", false, Uzivatel.HerniId, (byte)druh);

            return h;
        }
    }
}
