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
        //Nadřazené okno
        public Window Rodic;

        //Seznam tříd
        public ObservableCollection<string> seznamTrid { get; set; } = new ObservableCollection<string>();
        //Seznam uživatelů
        public ObservableCollection<Par<string, Par<int, bool>>> seznamUzivatelu { get; set; } = new ObservableCollection<Par<string, Par<int, bool>>>();

        //Vybraná třída
        private string VybranaTrida
        {
            get
            {
                return (string)lvTridy.SelectedItem;
            }
        }

        //Vybraný uživatel
        private Par<string, Par<int, bool>> VybranyUzivatel
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

        //Konstruktor umožňující návrat k rodičovi
        public SpravaUzivatelu(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
        }

        //Načtení tříd
        private void NacteniTrid()
        {
            string[] tridy = (string[])PraceSDB.ZavolejPrikaz("nacti_tridy", true)[0][0];
            seznamTrid.NastavHodnoty(tridy.ToList().Prepend("Učitelé"));
        }

        //Reset hesla vybranému uživateli
        private void btResetHesla_Click(object sender, RoutedEventArgs e)
        {
            if (VybranyUzivatel.Hodnota.Hodnota)
            {
                LepsiMessageBox.Show("Uživatel nemá heslo!");
                return;
            }

            PraceSDB.ZavolejPrikaz("zmen_uzivatelske_heslo", false, VybranyUzivatel.Hodnota.Klic);
            LepsiMessageBox.Show("Heslo odstraněno");
            NacteniUzivatelu();
        }

        //Odoznačení prvku v seznamu
        private void lvKategorie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ListView)sender).SelectedIndex = -1;
        }

        //Změna vybrané třídy
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

        //Načtení uživatelů
        private void NacteniUzivatelu()
        {
            IEnumerable<Par<string, Par<int, bool>>> uzivatele = ((string[])(lvTridy.SelectedIndex > 0 ? PraceSDB.ZavolejPrikaz("nacti_uzivatele_tridy", true, VybranaTrida) : PraceSDB.ZavolejPrikaz("nacti_uzivatele_tridy", true))[0][0]).Select(u => u.RozdelDolary()).Select(u => new Par<string, Par<int, bool>>(u[0], new Par<int, bool>(Convert.ToInt32(u[1]), Convert.ToBoolean(u[2]))));
            seznamUzivatelu.NastavHodnoty(uzivatele);
        }

        //Zastavení MouseUp eventu při kliku na prvek v seznamu
        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        //Odhlášení vybraného uživatele
        private void btOdhlasit_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)PraceSDB.ZavolejPrikaz("zkus_odhlasit", true, VybranyUzivatel.Klic.ZiskejZavorku())[0][0])
            {
                LepsiMessageBox.Show("Uživatel odhlášen!");
                return;
            }
            else
            {
                LepsiMessageBox.Show("Uživatel není přihlášen!");
            }
        }
    }
}
