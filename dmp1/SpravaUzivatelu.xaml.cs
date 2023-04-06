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
    /// Interakční logika pro SpravaUzivatelu.xaml
    /// </summary>
    public partial class SpravaUzivatelu : Window
    {
        public Window Rodic;

        public ObservableCollection<string> seznamTrid { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Par<string, Par<int, bool>>> seznamUzivatelu { get; set; } = new ObservableCollection<Par<string, Par<int, bool>>>();

        private string VybranaTrida
        {
            get
            {
                return (string)lvTridy.SelectedItem;
            }
        }

        private Par<string, Par<int, bool>> vybranyUzivatel
        {
            get
            {
                return (Par<string, Par<int, bool>>)lvUzivatele.SelectedItem;
            }
        }

        public SpravaUzivatelu()
        {
            InitializeComponent();
            DataContext = this;
            NacteniTrid();
        }

        public SpravaUzivatelu(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
        }

        private void NacteniTrid()
        {
            string[] tridy = (string[])PraceSDB.ZavolejPrikaz("nacti_tridy", true)[0][0];
            seznamTrid.NastavHodnoty(tridy.ToList().Prepend("Učitelé"));
        }

        private void btResetHesla_Click(object sender, RoutedEventArgs e)
        {
            if (vybranyUzivatel.Hodnota.Hodnota)
            {
                LepsiMessageBox.Show("Uživatel nemá heslo!");
                return;
            }

            PraceSDB.ZavolejPrikaz("zmen_uzivatelske_heslo", false, vybranyUzivatel.Hodnota.Klic);
            LepsiMessageBox.Show("Heslo odstraněno");
            NacteniUzivatelu();
        }

        private void lvKategorie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ListView)sender).SelectedIndex = -1;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTridy.SelectedIndex >= 0)
            {
                NacteniUzivatelu();
            }
            else
            {
                seznamUzivatelu.Clear();
            }
        }

        private void NacteniUzivatelu()
        {
            IEnumerable<Par<string, Par<int, bool>>> uzivatele = ((string[])(lvTridy.SelectedIndex > 0 ? PraceSDB.ZavolejPrikaz("nacti_uzivatele_tridy", true, VybranaTrida) : PraceSDB.ZavolejPrikaz("nacti_uzivatele_tridy", true))[0][0]).Select(u => u.RozdelDolary()).Select(u => new Par<string, Par<int, bool>>(u[0], new Par<int, bool>(Convert.ToInt32(u[1]), Convert.ToBoolean(u[2]))));
            seznamUzivatelu.NastavHodnoty(uzivatele);
        }

        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

    }
}
