using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro VyhledavaciOknoUloh.xaml
    /// </summary>
    public partial class VyhledavaciOknoUloh : Window
    {
        public bool HraciVHledacku
        {
            get
            {
                return htsUlohy.IsChecked;
            }
        }

        public string[] vysledkyHledani
        {
            get
            {
                return sstVysledky.Seznam.ToArray();
            }
        }

        public VyhledavaciOknoUloh(bool hraci = true)
        {
            InitializeComponent();

            if (hraci)
            {

            }

            NactiSeznam();
        }

        private void NactiSeznam(object sender = null, RoutedEventArgs e = null)
        {
            if (tbHledej == null || htsUlohy == null || sstVysledky == null)
            {
                return;
            }

            string hledanyVyraz = tbHledej.Text;
            if (string.IsNullOrWhiteSpace(hledanyVyraz))
            {
                sstVysledky.Seznam.NastavHodnoty(new string[0]);
                return;
            }

            if (htsUlohy.IsChecked)
            {
                string[] ulohy = (string[])PraceSDB.ZavolejPrikaz("vyhledej_ulohy", true, hledanyVyraz, Uzivatel.Id)[0][0];
                sstVysledky.Seznam.NastavHodnoty(ulohy);
            }
            else
            {
                string[] skupiny = (string[])PraceSDB.ZavolejPrikaz("vyhledej_skupiny_uloh", true, hledanyVyraz, Uzivatel.Id)[0][0];
                sstVysledky.Seznam.NastavHodnoty(skupiny);
            }
        }

        public event seznamSTlacitkyKlikHandler kliklNaPrvekVSeznamu;
        private void sstVysledky_KliklNaPrvek(string kliklyPrvek)
        {
            kliklNaPrvekVSeznamu?.Invoke(kliklyPrvek);
        }

        //Místo zavření pouze skrytí okna
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
