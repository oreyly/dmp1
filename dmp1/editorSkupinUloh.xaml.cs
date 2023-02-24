using System;
using System.Collections.Generic;
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
        VyhledavaciOknoUloh vo;
        public editorSkupinUloh()
        {
            vo = new VyhledavaciOknoUloh();
            vo.kliklNaPrvekVSeznamu += Vo_kliklNaPrvekVSeznamu;
            InitializeComponent();
            DataContext = this;
            NacteniSkupin();
        }

        private void NacteniSkupin()
        {
            string[] skupiny = (string[])PraceSDB.ZavolejPrikaz("nacti_skupiny_uloh", true, Uzivatel.Id)[0][0];
            sstSkupiny.Seznam.NastavHodnoty(skupiny);
            //sstSkupiny.druhTlacitka = druhTlacitkaVSeznamu.Smazat;
        }

        private void sstUlohy_KliklNaPrvek(string kliklyPrvek)
        {
            PraceSDB.ZavolejPrikaz("odeber_ulohu_ze_skupiny", false, kliklyPrvek.OdeberZavorku(), sstSkupiny.VybranyRadek, Uzivatel.Id);
            NactiSkupinu(sstSkupiny.VybranyRadek);
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btSkupina_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (sstSkupiny.Seznam.Contains(ib.noveJmeno))
                {
                    MessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    MessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("vytvor_skupinu_uloh", false, ib.noveJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void btHrace_Click(object sender, RoutedEventArgs e)
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

        private void Vo_kliklNaPrvekVSeznamu(string kliklyPrvek)
        {
            if (sstSkupiny.VybranyRadek == null)
            {
                return;
            }

            if (vo.HraciVHledacku)
            {
                PraceSDB.ZavolejPrikaz("pridej_ulohu_do_skupiny", false, kliklyPrvek.OdeberZavorku(), sstSkupiny.VybranyRadek, Uzivatel.Id);
            }
            else
            {
                PraceSDB.ZavolejPrikaz("pridej_celou_skupinu_uloh", false, kliklyPrvek, sstSkupiny.VybranyRadek, Uzivatel.Id);
            }

            NactiSkupinu(sstSkupiny.VybranyRadek);
        }

        private void sstSkupiny_KliklNaPrvek(string kliklyPrvek)
        {
            if (MessageBox.Show($"Opravdu si přejete odstranit skupinu úloh '{kliklyPrvek}'?", "Odstranění skupiny", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PraceSDB.ZavolejPrikaz("odstran_skupinu_her", false, kliklyPrvek, Uzivatel.Id);
                NacteniSkupin();
            }
        }

        private void sstSkupiny_DoubleKliklNaPrvek(string kliklyPrvek)
        {
            string stareJmeno = kliklyPrvek;

            InputBox ib = new InputBox(stareJmeno, "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (sstSkupiny.Seznam.Contains(ib.noveJmeno))
                {
                    MessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    MessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("aktualizuj_skupinu_uloh", false, ib.noveJmeno, stareJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void sstSkupiny_KliklNaPrvek_1(string kliklyPrvek)
        {
            NactiSkupinu(kliklyPrvek);
        }

        private void NactiSkupinu(string skupina)
        {
            string[] ucty = (string[])PraceSDB.ZavolejPrikaz("nacti_ulohy_skupiny", true, skupina, Uzivatel.Id)[0][0];
            sstUlohy.Seznam.NastavHodnoty(ucty);
        }
    }
}
