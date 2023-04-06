using PostSharp.Patterns.Model;
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
    /// Interakční logika pro spravaSkupin.xaml
    /// </summary>
    //[NotifyPropertyChanged]
    public partial class spravaSkupin : Window
    {
        public ObservableCollection<string> seznamSkupin { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> seznamHracu { get; set; } = new ObservableCollection<string>();

        private string vybranaSkupina
        {
            get
            {
                return (string)lvSkupiny.SelectedItem;
            }
        }
        private string[] vybranyHraci
        {
            get
            {
                return lvHraci.SelectedItems.Cast<string>().ToArray();
            }
        }

        private spravaSkupin()
        {
            vo = new vyhledavaciOknoHracu();
            vo.kliklNaPrvekVSeznamu += Vo_kliklNaPrvekVSeznamu;
            InitializeComponent();
            DataContext = this;
            NacteniSkupin();
        }

        private Window Rodic;
        public spravaSkupin(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
        }

        private void NacteniSkupin()
        {
            seznamSkupin.NastavHodnoty((string[])PraceSDB.ZavolejPrikaz("nacti_skupiny_hracu", true, Uzivatel.Id)[0][0]);
        }

        private void NacteniHracu()
        {
            string[] ucty = (string[])PraceSDB.ZavolejPrikaz("nacti_hrace_skupiny", true, vybranaSkupina, Uzivatel.Id)[0][0];
            seznamHracu.NastavHodnoty(ucty);
        }

        vyhledavaciOknoHracu vo;

        private void Vo_kliklNaPrvekVSeznamu(string kliklyPrvek)
        {
            if(vo.HraciVHledacku)
            {
                PraceSDB.ZavolejPrikaz("pridej_hrace", false, kliklyPrvek.ZiskejZavorku(), vybranaSkupina, Uzivatel.Id);
                NacteniHracu();
            }
            else
            {
                PraceSDB.ZavolejPrikaz("pridej_celou_skupinu", false, kliklyPrvek, vybranaSkupina, Uzivatel.Id);
                NacteniHracu();
            }
        }

        private void btSkupinaVytvorit_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (seznamSkupin.Contains(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if ((bool)PraceSDB.ZavolejPrikaz("existuje_skupina_hracu", true, Uzivatel.Id, ib.noveJmeno)[0][0])
                {
                    LepsiMessageBox.Show("Nelze pojmenovat skupinu stejně jako třídu!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("vytvor_skupinu", false, ib.noveJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void btSkupinaPrejmenovat_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox(vybranaSkupina, "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (seznamSkupin.Contains(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if ((bool)PraceSDB.ZavolejPrikaz("existuje_skupina_hracu", true, Uzivatel.Id, ib.noveJmeno)[0][0])
                {
                    LepsiMessageBox.Show("Nelze pojmenovat skupinu stejně jako třídu!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    LepsiMessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("aktualizuj_skupinu", false, ib.noveJmeno, vybranaSkupina, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void btSkupinaSmazat_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Opravdu si přejete odstranit skupinu '{vybranaSkupina}'?", "Odstranění skupiny", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PraceSDB.ZavolejPrikaz("odstran_skupinu", false, Uzivatel.Id, vybranaSkupina);
                NacteniSkupin();
            }
        }

        private void btHracePridat_Click(object sender, RoutedEventArgs e)
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

        private void btHraceOdstranit_Click(object sender, RoutedEventArgs e)
        {
            foreach(string hrac in vybranyHraci)
            {
                PraceSDB.ZavolejPrikaz("odeber_hrace", false, hrac.ZiskejZavorku(), vybranaSkupina, Uzivatel.Id);
            }
            NacteniHracu();
        }

        private void lvKategorie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ListView)sender).SelectedIndex = -1;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSkupiny.SelectedIndex >= 0)
            {
                NacteniHracu();
            }
            else
            {
                seznamHracu.Clear();
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
