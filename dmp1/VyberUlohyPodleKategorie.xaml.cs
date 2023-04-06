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
using WPF.JoshSmith.ServiceProviders.UI;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro VyberUlohyPodleKategorie.xaml
    /// </summary>
    public partial class VyberUlohyPodleKategorie : Window
    {
        public ObservableCollection<Par<string, bool>> seznamKategorii { get; set; } = new ObservableCollection<Par<string, bool>>();
        public ObservableCollection<Par<string, int>> seznamUloh { get; set; } = new ObservableCollection<Par<string, int>>();

        Window Rodic;
        private VyberUlohyPodleKategorie()
        {
            InitializeComponent();
            //new ListViewDragDropManager<Par<string, int>>(lvUlohy);
            DataContext = this;
            NacteniKategorii();
        }

        public VyberUlohyPodleKategorie(Window rodic) : this()
        {
            Rodic = rodic;
        }

        private void NacteniKategorii()
        {
            seznamKategorii.NastavHodnoty(((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_kategorie", true, Uzivatel.Id)[0][0]).Select(k => new Par<string, bool>(k.Key, Convert.ToBoolean(k.Value))).OrderBy(k => k.Klic));
        }

        private void NacteniUloh(string kategorie)
        {
            seznamUloh.NastavHodnoty(((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_ulohy_kategorie", true, Uzivatel.Id, kategorie)[0][0]).Select(u => new Par<string, int>(u.Key, Convert.ToInt32(u.Value))));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvKategorie.SelectedIndex >= 0)
            {
                NacteniUloh(((Par<string, bool>)lvKategorie.SelectedItem).Klic);
            }
            else
            {
                seznamUloh.Clear();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Rodic.Show();
        }

        private void lvKategorie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ListView)sender).SelectedIndex = -1;
        }

        private void btPridatKategorii_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                string Jmeno = ib.noveJmeno;
                if (seznamKategorii.Count(par => par.Klic == Jmeno) > 0)
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

        //Přejmenování kategorie
        private void btPrejmenovatKategorii_Click(object sender, RoutedEventArgs e)
        {
            string stareJmeno = ((Par<string, bool>)lvKategorie.SelectedItem).Klic;
            if (!seznamKategorii.First(par => par.Klic == stareJmeno).Hodnota)
            {
                LepsiMessageBox.Show("Nelze upravovat automaticky vytvořené kategorie!");
                return;
            }

            InputBox ib = new InputBox(stareJmeno, "Pojmenování kategorie");
            if (ib.ShowDialog() == true)
            {
                if (seznamKategorii.Count(par => par.Klic == ib.noveJmeno) > 0)
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

        private void btSmazatKategorii_Click(object sender, RoutedEventArgs e)
        {
            Par<string, bool> kategorie = (Par<string, bool>)lvKategorie.SelectedItem;

            if (!kategorie.Hodnota)
            {
                LepsiMessageBox.Show("Nelze odstranit automaticky vytvořené kategorie!");
                return;
            }

            if (MessageBox.Show($"Opravdu si přejete odstranit kategorii '{kategorie.Klic}'?", "Odstranění kategorie", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (!(bool)PraceSDB.ZavolejPrikaz("odstran_kategorii", true, kategorie.Klic, Uzivatel.Id)[0][0])
            {
                LepsiMessageBox.Show("Kategorie nemohla být odstraněna!");
                return;
            }

            NacteniKategorii();
        }

        private void btPridatUlohu_Click(object sender, RoutedEventArgs e)
        {
            new editorUloh(this, -1, ((Par<string, bool>)lvKategorie.SelectedItem).Klic).Show();
            Hide();
        }

        private void btUpravitUlohu_Click(object sender, RoutedEventArgs e)
        {
            new editorUloh(this, ((Par<string, int>)lvUlohy.SelectedItem).Hodnota, ((Par<string, bool>)lvKategorie.SelectedItem).Klic).Show();
            Hide();
        }

        private void btOdstranitUlohu_Click(object sender, RoutedEventArgs e)
        {
            Par<string, int> uloha = (Par<string, int>)lvUlohy.SelectedItem;
            if (MessageBox.Show($"Opravdu si přejetě odstranit úlohu '{uloha.Klic}'?", "Změna úlohy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PraceSDB.ZavolejPrikaz("odstran_ulohu", false, uloha.Hodnota);

                NacteniUloh(((Par<string, bool>)lvKategorie.SelectedItem).Klic);
            }
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && ((Par<string, bool>)lvKategorie.SelectedItem) != null)
            {
                NacteniUloh(((Par<string, bool>)lvKategorie.SelectedItem).Klic);
            }
        }

        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {
            Par<string, int> uloha = (Par<string, int>)e.Data.GetData(typeof(Par<string, int>));
            PraceSDB.ZavolejPrikaz("aktualizuj_kategorii_ulohy", false, Uzivatel.Id, (string)((ListViewItem)sender).Content, uloha.Hodnota);
            NacteniUloh(((Par<string, bool>)lvKategorie.SelectedItem).Klic);
            //MessageBox.Show((string)((ListViewItem)sender).Content + " -> " + ((ListViewItem)e.Data.GetData(typeof(ListViewItem))).Content);
        }

        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ListViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((ListViewItem)sender).IsSelected = true;
                DragDrop.DoDragDrop(lvUlohy, lvUlohy.SelectedItem, DragDropEffects.All);
            }
        }
    }
}
