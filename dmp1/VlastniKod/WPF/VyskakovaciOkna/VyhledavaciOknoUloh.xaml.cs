using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    //Okno na vyhledávání úloh
    public partial class VyhledavaciOknoUloh : Window
    {
        //Jestli jsou hledáni samotné úlohy nebo celé skupiny
        public bool UlohyVHledacku
        {
            get
            {
                return htsUlohy.IsChecked;
            }
        }

        //Seznam výsledků
        public ObservableCollection<string> seznam { get; set; } = new ObservableCollection<string>();
        //Seznam do kterého se vybrané výsledky přesouvají
        private ObservableCollection<string> CilovySeznam;
        
        public VyhledavaciOknoUloh(ObservableCollection<string> cilovySeznam)
        {
            InitializeComponent();
            CilovySeznam = cilovySeznam;
            DataContext = this;
            NactiSeznam();
        }

        //Načtení výsledků vyhledávání
        public void NactiSeznam(object sender = null, RoutedEventArgs e = null)
        {
            if (tbHledej == null || htsUlohy == null || lvVysledky == null)
            {
                return;
            }

            string hledanyVyraz = tbHledej.Text;

            if (htsUlohy.IsChecked)
            {
                string[] ulohy = (string[])PraceSDB.ZavolejPrikaz("vyhledej_ulohy", true, hledanyVyraz, Uzivatel.Id)[0][0];
                seznam.NastavHodnoty(ulohy.Except(CilovySeznam));
            }
            else
            {
                string[] skupiny = (string[])PraceSDB.ZavolejPrikaz("vyhledej_skupiny_uloh", true, hledanyVyraz, Uzivatel.Id)[0][0];
                seznam.NastavHodnoty(skupiny);
            }
        }

        //Jestli se má okno zavřít úplně nebo jen schovat
        private bool Konec = false;
        //Úplné zavření okna
        public new void Close()
        {
            Konec = true;
            base.Close();
        }

        //Místo zavření pouze skrytí okna
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Konec)
            {
                return;
            }
            e.Cancel = true;
            Hide();
        }

        //Event oznamující odeslání nových prvků
        public event OdeslaniVybranychPrvkuHandler OdeslaniVybranychPrvku;
        //Zavolá event s novými prvky
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OdeslaniVybranychPrvku?.Invoke(lvVysledky.SelectedItems.Cast<string>().ToArray());
            NactiSeznam();
        }
    }
}
