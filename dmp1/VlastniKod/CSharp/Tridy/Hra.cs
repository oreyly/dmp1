using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace dmp1
{
    public enum DruhSpusteni {
        [Description("Učení")]
        Uceni=1,
        [Description("Procvičování")]
        Procvicovani,
        Oboje,
        Test,
        Kontrola
    }

    public delegate void ZmenaStavu(object sender, object stary, object novy);
    //Třída obsahující všechna potřebná data pro průběh jedné hry
    [NotifyPropertyChanged]
    public class Hra
    {
        private int Id;
        public string Nazev { get; set; }

        public ObservableCollection<Uloha> Ulohy { get; set; }
        public bool Nahodne; //Jestli mají být úlohy náhodně seřazeny

        private int MaximalniCas 
        {
            set
            {
                CasKonce = DateTime.Now.AddSeconds(value);
            }
        }

        public DateTime CasKonce { get; set; }

        public DruhSpusteni DruhSpusteni { get; set; } //V jakém módu může být hra spuštěna (učení, procvičování, oboje, test)

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

        public event ZmenaStavu ZmenaAktualniUlohy;

        public bool Kontrolovat { get; set; }

        [SafeForDependencyAnalysis]
        public int BodyCelkem
        {
            get
            {
                return Ulohy.Sum(u => (int)u.Body);
            }
        }

        [SafeForDependencyAnalysis]
        public int BodyZiskal
        {
            get
            {
                return Ulohy.Where(u => u.stavUlohy == StavUlohy.Spravne).Sum(u => (int)u.Body);
            }
        }

        public void NastavHru(string nazev, Uloha[] ulohy, bool nahodne, int maximalniCas, bool kontrolovat)
        {
            Nazev = nazev;
            Ulohy = new ObservableCollection<Uloha>(ulohy);

            foreach(Uloha u in Ulohy)
            {
                u.ZamichejMoznosti();
            }

            MaximalniCas = maximalniCas;

            if (nahodne)
            {
                Ulohy.ZamichejList();
            }
            else if (DruhSpusteni == DruhSpusteni.Procvicovani)
            {
                Ulohy.NastavHodnoty(Ulohy.OrderBy(u => u.Body).ToArray());
                int krok = (int)Math.Ceiling(Ulohy.Count / 3f);
                Ulohy[krok - 1].ZachytnyBod = true;
                Ulohy[2 * krok - 1].ZachytnyBod = true;
            }

            Kontrolovat = kontrolovat;

            aktualniUloha = Ulohy[0];
        }

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
            MessageBox.Show("Konec!");
            Environment.Exit(0);
        }

        public Hra(int idHry, int[] vysledky)
        {
            Id = idHry;
            DruhSpusteni = DruhSpusteni.Kontrola;

            List<object[]> o = PraceSDB.ZavolejPrikaz("nacti_vysledky_uloh", true, vysledky).Select(radek => (object[])radek[0]).ToList();
            Ulohy = new ObservableCollection<Uloha>(o.Select(u => new Uloha(this, (string)u[0], (string)u[1], (string)u[2], (string)u[3], (int)u[4], (int)u[5], (string)u[6], (int)u[7])));
            aktualniUloha = Ulohy[0];

            //foreach()
            return;
        }

        //Nastavení základních hodnot hry
        private Hra(int id, DruhSpusteni druh)
        {
            Id = id;

            Uzivatel.HerniId = (int)PraceSDB.ZavolejPrikaz("nacti_id_hrace", true, Id, Uzivatel.Id)[0][0];

            DruhSpusteni = druh;
            /*int pocet = 41;
            Ulohy = new Uloha[pocet];
            DruhSpusteniI = 2;

            for (int i = 0; i < pocet; ++i)
            {
                //Ulohy[i] = new Uloha(i * 5);
            }

            aktualniUloha = Ulohy[2];*/
            //Pokračovat
        }

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
