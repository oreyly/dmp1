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
    /// Interakční logika pro editorSkupinUloh.xaml
    /// </summary>
    public partial class editorSkupinUloh : Window
    {
        public ObservableCollection<string> seznamSkupin { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> seznamUloh { get; set; } = new ObservableCollection<string>();
        private string vybranaSkupina
        {
            get
            {
                return (string)lvSkupiny.SelectedItem;
            }
        }
        private string[] vybraneUlohy
        {
            get
            {
                return lvUlohy.SelectedItems.Cast<string>().ToArray();
            }
        }
        VyhledavaciOknoUloh vo;
        private editorSkupinUloh()
        {
            vo = new VyhledavaciOknoUloh();
            vo.kliklNaPrvekVSeznamu += Vo_kliklNaPrvekVSeznamu;
            InitializeComponent();
            DataContext = this;
            NacteniSkupin();
        }

        private Window Rodic;
        public editorSkupinUloh(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
        }

        private void NacteniSkupin()
        {
            string[] skupiny = (string[])PraceSDB.ZavolejPrikaz("nacti_skupiny_uloh", true, Uzivatel.Id)[0][0];
            seznamSkupin.NastavHodnoty(skupiny.OrderBy(s => s));
        }

        private void Vo_kliklNaPrvekVSeznamu(string kliklyPrvek)
        {
            if (vo.HraciVHledacku)
            {
                PraceSDB.ZavolejPrikaz("pridej_ulohu_do_skupiny", false, kliklyPrvek.OdeberZavorku(), vybranaSkupina, Uzivatel.Id);
            }
            else
            {
                PraceSDB.ZavolejPrikaz("pridej_celou_skupinu_uloh", false, kliklyPrvek, vybranaSkupina, Uzivatel.Id);
            }

            NacteniUloh();
        }

        private void NacteniUloh()
        {
            string[] ulohy = (string[])PraceSDB.ZavolejPrikaz("nacti_ulohy_skupiny", true, vybranaSkupina, Uzivatel.Id)[0][0];
            seznamUloh.NastavHodnoty(ulohy);
        }

        private void btSkupinaPridat_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (seznamSkupin.Contains(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("vytvor_skupinu_uloh", false, ib.noveJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void btSkupinaPrejmenovat_Click(object sender, RoutedEventArgs e)
        {
            string stareJmeno = vybranaSkupina;

            InputBox ib = new InputBox(stareJmeno, "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (seznamSkupin.Contains(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("aktualizuj_skupinu_uloh", false, ib.noveJmeno, stareJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void btSkupinaSmazat_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Opravdu si přejete odstranit skupinu úloh '{vybranaSkupina}'?", "Odstranění skupiny", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PraceSDB.ZavolejPrikaz("odstran_skupinu_her", false, vybranaSkupina, Uzivatel.Id);
                NacteniSkupin();
            }
        }

        private void btUlohyPridat_Click(object sender, RoutedEventArgs e)
        {
            if (vo.Visibility == Visibility.Visible)
            {
                vo.Hide();
            }
            else
            {
                vo.Show();
                if (vo.Owner == null)
                {
                    vo.Owner = this;
                }
            }
        }

        private void btUlohySmazat_Click(object sender, RoutedEventArgs e)
        {
            foreach(string uloha in vybraneUlohy)
            {
                PraceSDB.ZavolejPrikaz("odeber_ulohu_ze_skupiny", false, uloha.OdeberZavorku(), vybranaSkupina, Uzivatel.Id);
            }
            NacteniUloh();
        }

        private void lvKategorie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ListView)sender).SelectedIndex = -1;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSkupiny.SelectedIndex >= 0)
            {
                NacteniUloh();
            }
            else
            {
                seznamUloh.Clear();
            }
        }

        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void btUlohyPridat_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                vo.Hide();
            }
        }
    }
}
