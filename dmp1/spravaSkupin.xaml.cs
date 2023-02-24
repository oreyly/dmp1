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
    [NotifyPropertyChanged]
    public partial class spravaSkupin : Window
    {
        public Dictionary<string, bool> skupiny { get; set; }
        public spravaSkupin()
        {
            vo = new vyhledavaciOknoHracu();
            vo.kliklNaPrvekVSeznamu += Vo_kliklNaPrvekVSeznamu;
            InitializeComponent();
            DataContext = this;
            NacteniSkupin();
            //MessageBox.Show("More");
            /*for (int i = 0; i < 50; ++i)
            {
                if (!(bool)PraceSDB.ZavolejPrikaz("zzz_registruj", true, $"uživ{i}")[0][0])
                {
                    
                }
            }*/
        }

        private void NacteniSkupin()
        {
            skupiny = ((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_skupiny", true, Uzivatel.Id)[0][0]).ToDictionary(x => x.Key, x => Convert.ToBoolean(x.Value));
        }

        private void btSkupina_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Pojmenování skupiny");
            if (ib.ShowDialog() == true)
            {
                if (skupiny.ContainsKey(ib.noveJmeno))
                {
                    MessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    MessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("vytvor_skupinu", false, ib.noveJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        vyhledavaciOknoHracu vo;
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
            if(lbSkupiny.SelectedItem == null)
            {
                return;
            }

            KeyValuePair<string, bool> skupina = (KeyValuePair<string, bool>)lbSkupiny?.SelectedItem;
            if (skupina.Value)
            {
                return;
            }

            if(vo.HraciVHledacku)
            {
                PraceSDB.ZavolejPrikaz("pridej_hrace", false, kliklyPrvek.ZiskejZavorku(), skupina.Key, Uzivatel.Id);
                NactiSkupinu(skupina.Key);
            }
            else
            {
                PraceSDB.ZavolejPrikaz("pridej_celou_skupinu", false, kliklyPrvek, skupina.Key, Uzivatel.Id);
                NactiSkupinu(skupina.Key);
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string stareJmeno = ((ListBoxItem)sender).Content.ToString();
            if(skupiny[stareJmeno])
            {
                MessageBox.Show("Nelze upravovat automaticky vytvořené skupiny!");
                return;
            }

            InputBox ib = new InputBox(stareJmeno, "Pojmenování skupiny"); 
            if (ib.ShowDialog() == true)
            {
                if (skupiny.ContainsKey(ib.noveJmeno))
                {
                    MessageBox.Show("Skupina s tímto názvem již existuje!");
                }
                else if (string.IsNullOrWhiteSpace(ib.noveJmeno))
                {
                    MessageBox.Show("Název skupiny nesmí být prázdný!");
                }
                else
                {
                    PraceSDB.ZavolejPrikaz("aktualizuj_skupinu", false, ib.noveJmeno, stareJmeno, Uzivatel.Id);
                    NacteniSkupin();
                }
            }
        }

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e = null)
        {
            string Jmeno = ((ListBoxItem)sender).Content.ToString();

            NactiSkupinu(Jmeno);
        }

        private void NactiSkupinu(string skupina)
        {
            string[] ucty = (string[])PraceSDB.ZavolejPrikaz("nacti_hrace_skupiny", true, skupina, Uzivatel.Id)[0][0];
            sstHraci.Seznam.NastavHodnoty(ucty);
            sstHraci.Zapnuto = !skupiny[skupina];
        }

        private void sstHraci_KliklNaPrvek(string kliklyPrvek)
        {
            KeyValuePair<string, bool> skupina = (KeyValuePair<string, bool>)lbSkupiny?.SelectedItem;
            PraceSDB.ZavolejPrikaz("odeber_hrace", false, kliklyPrvek.ZiskejZavorku(), skupina.Key, Uzivatel.Id);
            NactiSkupinu(skupina.Key);
        }
    }
}
