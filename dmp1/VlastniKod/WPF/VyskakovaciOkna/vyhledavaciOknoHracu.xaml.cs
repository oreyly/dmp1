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
using ToggleSwitch;
namespace dmp1
{
    public delegate void OdeslaniVybranychPrvkuHandler(string[] Prvky);
    /// <summary>
    /// Interakční logika pro vyhledavaciOkno.xaml
    /// </summary>
    //Okno na vyhledávání hráčů
    public partial class vyhledavaciOknoHracu : Window
    {
        public bool HraciVHledacku
        {
            get
            {
                return htsHraci.IsChecked;
            }
        }

        public ObservableCollection<string> seznam { get; set; } = new ObservableCollection<string>();
        private ObservableCollection<string> CilovySeznam;

        public vyhledavaciOknoHracu(ObservableCollection<string> cilovySeznam)
        {
            InitializeComponent();
            CilovySeznam = cilovySeznam;
            DataContext = this;
            NactiSeznam();
        }

        private void NactiSeznam(object sender = null, RoutedEventArgs e = null)
        {
            if (tbHledej == null || htsHraci == null || lvVysledky == null)
            {
                return;
            }

            string hledanyVyraz = tbHledej.Text;
            /*if (string.IsNullOrWhiteSpace(hledanyVyraz))
            {
                sstVysledky.Seznam.NastavHodnoty(new string[0]);
                return;
            }*/

            if (htsHraci.IsChecked)
            {
                string[] ucty = (string[])PraceSDB.ZavolejPrikaz("vyhledej_hrace", true, hledanyVyraz)[0][0];
                seznam.NastavHodnoty(ucty.Except(CilovySeznam));
            }
            else
            {
                string[] skupiny = (string[])PraceSDB.ZavolejPrikaz("vyhledej_skupiny", true, hledanyVyraz, Uzivatel.Id)[0][0];
                seznam.NastavHodnoty(skupiny);
            }
        }


        private bool Konec = false;
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

        public event OdeslaniVybranychPrvkuHandler OdeslaniVybranychPrvku;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OdeslaniVybranychPrvku?.Invoke(lvVysledky.SelectedItems.Cast<string>().ToArray());
            NactiSeznam();
        }
    }
}
