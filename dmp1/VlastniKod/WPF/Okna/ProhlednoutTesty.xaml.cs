using dmp1.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interakční logika pro ProhlednoutTesty.xaml
    /// </summary>
    public partial class ProhlednoutTesty : Window
    {
        public ProhlednoutTesty()
        {
            InitializeComponent();
            DataContext = this;
            rb1.IsChecked = true;
        }

        public ObservableCollection<object[]> seznamHer { get; set; } = new ObservableCollection<object[]>();

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rb1 == null || rb2 == null)
            {
                return;
            }

            string[] data;
            if (Uzivatel.Prava == UrovenPrav.Zak)
            {
                data = (string[])PraceSDB.ZavolejPrikaz("nacti_seznam_vysledku_zak", true, Uzivatel.Id, (byte)((bool)rb1.IsChecked ? 2 : 4))[0][0];
            }
            else
            {
                data = (string[])PraceSDB.ZavolejPrikaz("nacti_seznam_vysledku_ucitel", true, Uzivatel.Id, (byte)((bool)rb1.IsChecked ? 2 : 4))[0][0];
            }

            seznamHer.NastavHodnoty(data.Select(dato => dato.Split(HlavniStatik.Oddelovac, StringSplitOptions.None)).Select(skupinaDat => new object[] { skupinaDat[0], skupinaDat[1], skupinaDat[2], skupinaDat[3], skupinaDat[4], Convert.ToInt32(skupinaDat[5]) }));
        }

        private void btSpustit_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((object[])((Button)sender).GetAncestorOfType<ListViewItem>().Content)[5];

            if(Uzivatel.Prava == UrovenPrav.Ucitel)
            {
                new ProhlednoutTestyLidi(id, this).Show();
            }
            else if(Uzivatel.Prava == UrovenPrav.Zak)
            {
                new VysledkoveOkno(id, Uzivatel.Jmeno.ZiskejZavorku(), this).Show();
            }

            Hide();
        }
    }
}
