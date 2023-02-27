using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace dmp1
{
    public enum DruhSpusteni {
        [Description("Učení")]
        Uceni=1,
        [Description("Procvičování")]
        Procvicovani,
        Oboje,
        Test 
    }
    //Třída obsahující všechna potřebná data pro průběh jedné hry
    [NotifyPropertyChanged]
    public class Hra
    {
        public string Nazev { get; set; }

        public ObservableCollection<Uloha> Ulohy { get; set; }
        public bool Nahodne; //Jestli mají být úlohy náhodně seřazeny

        public int MaximalniCas { get; set; }

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

                _aktualniUloha = value;
            }
        }

        private DispatcherTimer casovac; //Časovač pro časový limit celé hry

        public bool Kontrolovat { get; set; }

        public void NastavHru(string nazev, Uloha[] ulohy, bool nahodne, int maximalniCas, DruhSpusteni druhSpusteni, bool kontrolovat)
        {
            Nazev = nazev;
            Ulohy = new ObservableCollection<Uloha>(ulohy);

            MaximalniCas = maximalniCas;

            if (nahodne)
            {
                Ulohy.ZamichejList();
            }

            DruhSpusteni = druhSpusteni;
            Kontrolovat = kontrolovat;

            aktualniUloha = Ulohy[0];
        }

        //Nastavení základních hodnot hry
        public Hra()
        {
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

            Hra h = new Hra();

            Uloha[] ulohy = Uloha.VytvorHerniUlohy(data[0].Replace("{", "").Replace("}", "").Split(',').Select(id => Convert.ToInt32(id)).ToArray(), h);

            h.NastavHru(data[1], ulohy, Convert.ToBoolean(data[2]), Convert.ToInt32(data[3]), druh, Convert.ToBoolean(data[4]));

            return h;
        }
    }
}
