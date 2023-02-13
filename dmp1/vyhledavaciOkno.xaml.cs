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
using ToggleSwitch;
namespace dmp1
{
    /// <summary>
    /// Interakční logika pro vyhledavaciOkno.xaml
    /// </summary>
    //Okno na vyhledávání hráčů
    public partial class vyhledavaciOkno : Window
    {
        public bool HraciVHledacku
        {
            get
            {
                return htsHraci.IsChecked;
            }
        }

        public string[] vysledkyHledani
        {
            get
            {
                return sstVysledky.Seznam.ToArray();
            }
        }

        public vyhledavaciOkno()
        {
            InitializeComponent();
            NactiSeznam();
        }

        private void NactiSeznam(object sender = null, RoutedEventArgs e = null)
        {
            if (tbHledej == null || htsHraci == null || sstVysledky == null)
            {
                return;
            }

            string hledanyVyraz = tbHledej.Text;
            if (string.IsNullOrWhiteSpace(hledanyVyraz))
            {
                sstVysledky.Seznam.NastavHodnoty(new string[0]);
                return;
            }

            if (htsHraci.IsChecked)
            {
                string[] ucty = (string[])PraceSDB.ZavolejPrikaz("vyhledej_hrace", true, hledanyVyraz)[0][0];
                sstVysledky.Seznam.NastavHodnoty(ucty);
            }
            else
            {
                string[] skupiny = (string[])PraceSDB.ZavolejPrikaz("vyhledej_skupiny", true, hledanyVyraz, Uzivatel.Id)[0][0];
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
