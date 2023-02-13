using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace dmp1
{
    //Třída obsahující všechna potřebná data pro průběh jedné hry
    [NotifyPropertyChanged]
    public class Hra
    {
        public string Nazev { get; set; }
        public string Popis { get; set; }

        public Uloha[] Ulohy { get; set; }
        public bool Nahodne; //Jestli mají být úlohy náhodně seřazeny

        public int maximalniCas;

        private int _DruhSpusteni; //V jakém módu může být hra spuštěna (učení, procvičování, oboje, test)

        public int DruhSpusteniI
        {
            get
            {
                return _DruhSpusteni;
            }

            set
            {
                _DruhSpusteni = value;
            }
        }
        public string DruhSpusteniS
        {
            get
            {
                switch(_DruhSpusteni)
                {
                    case 1:
                        return "Učení";
                    case 2:
                        return "Procvičování";
                    case 3:
                        return "Oboje";
                    case 4:
                        return "Test";
                    default:
                        throw new Exception("Neexistující způsob spuštění");
                }
            }
        }

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

        //Nastavení základních hodnot hry
        public Hra(string nazev, string popis, Uloha[] ulohy, bool nahodne, int maximalnicas, int druhSpusteni)
        {
            int pocet = 41;
            Ulohy = new Uloha[pocet];
            DruhSpusteniI = 2;

            for (int i = 0; i < pocet; ++i)
            {
                //Ulohy[i] = new Uloha(i * 5);
            }

            aktualniUloha = Ulohy[2];
        }
    }
}
