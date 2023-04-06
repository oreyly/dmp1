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
using ObservableDictionary;


namespace dmp1
{
    /// <summary>
    /// Interakční logika pro UpravaKategorie.xaml
    /// </summary>
    public partial class UpravaKategorie : Window
    {
        //Seznam všech kategorií a jestli jsou editovatelné
        public ObservableCollection<KeyValuePair<string, bool>> Kategorie { get; set; } = new ObservableCollection<KeyValuePair<string, bool>>();

        public UpravaKategorie()
        {
            InitializeComponent();
            DataContext = this;
            NacteniKategorii();
        }

        //Přejmenování kategorie
        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string stareJmeno = ((ListBoxItem)sender).Content.ToString();
            if (Kategorie.First(par => par.Key == stareJmeno).Value)
            {
                LepsiMessageBox.Show("Nelze upravovat automaticky vytvořené kategorie!");
                return;
            }

            InputBox ib = new InputBox(stareJmeno, "Pojmenování kategorie");
            if (ib.ShowDialog() == true)
            {
                if (Kategorie.Count(par => par.Key == ib.noveJmeno) > 0)
                {
                    LepsiMessageBox.Show("Kategorie s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Název kategorie nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("aktualizuj_kategorii", false, ib.noveJmeno, stareJmeno, Uzivatel.Id);
                    NacteniKategorii();
                }
            }
        }

        private void NacteniKategorii()
        {
            Kategorie.NastavHodnoty(((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_kategorie", true, Uzivatel.Id)[0][0]).ToDictionary(x => x.Key, x => Convert.ToBoolean(x.Value)));
        }

        //Odstranění kategorie
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, bool> lvi = (KeyValuePair<string, bool>)((Button)sender).GetAncestorOfType<ListViewItem>().Content;
            if (!(bool)PraceSDB.ZavolejPrikaz("odstran_kategorii", true, lvi.Key, Uzivatel.Id)[0][0])
            {
                LepsiMessageBox.Show("Kategorie nemohla být odstraněna!");
            }
            NacteniKategorii();
        }

        //Vytvoření nové kategorie
        private void novaKategorie_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                string Jmeno = ib.noveJmeno;
                if (Kategorie.Count(par => par.Key == Jmeno) > 0)
                {
                    LepsiMessageBox.Show("Kategorie s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Název kategorie nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("vytvor_kategorii", false, ib.noveJmeno, Uzivatel.Id);
                    NacteniKategorii();
                }
            }
        }
    }
}
