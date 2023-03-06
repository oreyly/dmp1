using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interakční logika pro tvorbaHer.xaml
    /// </summary>
    public partial class tvorbaHer : Window
    {
        public DruhSpusteni druhSCasem { get; set; } = DruhSpusteni.Test;

        vyhledavaciOknoHracu voh;
        VyhledavaciOknoUloh vou;
        public tvorbaHer()
        {
            voh = new vyhledavaciOknoHracu();
            voh.kliklNaPrvekVSeznamu += Voh_kliklNaPrvekVSeznamu;
            vou = new VyhledavaciOknoUloh();
            vou.kliklNaPrvekVSeznamu += Vou_kliklNaPrvekVSeznamu;

            InitializeComponent();
            DataContext = this;
            seznamUloh.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            new ListViewDragDropManager<Par<string, Par<bool, bool>>>(seznamUloh);
            PrepocitejHrace();
            ItemContainerGenerator_StatusChanged(seznamUloh.ItemContainerGenerator);
        }

        private void ItemContainerGenerator_StatusChanged(object sender = null, EventArgs e = null)
        {
            ItemContainerGenerator icg = (ItemContainerGenerator)sender;
            if (icg != null && icg.Status == GeneratorStatus.ContainersGenerated)
            {
                foreach (Par<string, Par<bool, bool>> lvi in seznamUloh.ItemContainerGenerator.Items)
                {
                    ListViewItem elf = ((ListViewItem)seznamUloh.ItemContainerGenerator.ContainerFromItem(lvi));
                    if (elf == null) //Nenačítá, pokud není vidět
                    {
                        continue;
                    }
                    elf.Background = SpravnaUloha(lvi) ? Brushes.Transparent : Brushes.Yellow;
                    //((ListViewItem)seznamUloh.ItemContainerGenerator.ContainerFromItem(lvi)).Background = Brushes.Yellow;
                }
            }


            lbPocetUloh.Text = $"Počet úloh: {Ulohy.Count}";
            lbPocetUloh.Background = SpravnyPocetUloh() ? Brushes.Transparent : Brushes.Red;
            PrepocitejHrace();
        }

        private bool SpravnaUloha(Par<string, Par<bool, bool>> lvi)
        {
            return lcbxDruhSpusteni.VybranyItem == null 
                || ((DruhSpusteni)lcbxDruhSpusteni.VybranyItem) == DruhSpusteni.Test 
                || (!lvi.Hodnota.Klic && !lvi.Hodnota.Hodnota) 
                || (!lvi.Hodnota.Klic && ((DruhSpusteni)lcbxDruhSpusteni.VybranyItem) == DruhSpusteni.Uceni);
        }

        private bool SpravnyPocetUloh()
        {
            return
                lcbxDruhSpusteni.VybranyItem == null
                ||
                (Ulohy.Count > 3 && new DruhSpusteni[] { DruhSpusteni.Test, DruhSpusteni.Uceni }.Contains((DruhSpusteni)lcbxDruhSpusteni.VybranyItem))
                ||
                (Ulohy.Count > 6 && new DruhSpusteni[] { DruhSpusteni.Procvicovani, DruhSpusteni.Oboje }.Contains((DruhSpusteni)lcbxDruhSpusteni.VybranyItem));
        }

        public ObservableCollection<Par<string, Par<bool, bool>>> Ulohy { get; set; } = new ObservableCollection<Par<string, Par<bool, bool>>>();

        private void Vou_kliklNaPrvekVSeznamu(string kliklyPrvek)
        {
            if (vou.HraciVHledacku)
            {
                bool napoveda = ((bool[])PraceSDB.ZavolejPrikaz("obsahuji_ulohy_napovedu", true, new string[] { kliklyPrvek.OdeberZavorku() }, Uzivatel.Id)[0][0])[0];
                bool moznost = ((string[])PraceSDB.ZavolejPrikaz("obsah_vysledku_uloh", true, new string[] { kliklyPrvek.OdeberZavorku() }, Uzivatel.Id)[0][0])[0].ToLower()[0] == 'o';
                Ulohy.AddIfNotExists(new Par<string, Par<bool, bool>>(kliklyPrvek, new Par<bool, bool>(napoveda, moznost)));
            }
            else
            {
                string[] ulohy = (string[])PraceSDB.ZavolejPrikaz("nacti_ulohy_skupiny", true, kliklyPrvek, Uzivatel.Id)[0][0];
                bool[] napovedy = (bool[])PraceSDB.ZavolejPrikaz("obsahuji_ulohy_napovedu", true, ulohy.Select(u => u.OdeberZavorku()).ToArray(), Uzivatel.Id)[0][0];
                bool[] moznosti = ((string[])PraceSDB.ZavolejPrikaz("obsah_vysledku_uloh", true, ulohy.Select(u => u.OdeberZavorku()).ToArray(), Uzivatel.Id)[0][0]).Select(moznost => moznost.ToLower()[0] == 'o').ToArray();
                Ulohy.AddIfNotExists(ulohy.Zip(napovedy, (u, n) => new Par<string, bool>(u, n)).Zip(moznosti, (p, m) => new Par<string, Par<bool, bool>>(p.Klic, new Par<bool, bool>(p.Hodnota, m))).ToArray());
            }
        }

        private void Voh_kliklNaPrvekVSeznamu(string kliklyPrvek)
        {
            if(voh.HraciVHledacku)
            {
                sstHraci.Seznam.AddIfNotExists(kliklyPrvek);
            }
            else
            {
                string[] hraci = (string[])PraceSDB.ZavolejPrikaz("nacti_hrace_skupiny", true, kliklyPrvek, Uzivatel.Id)[0][0];
                sstHraci.Seznam.AddIfNotExists(hraci);
            }
            PrepocitejHrace();
        }

        private void sstHraci_KliklNaTlacitko(string kliklyPrvek)
        {
            sstHraci.Seznam.Remove(kliklyPrvek);
            PrepocitejHrace();
        }

        private void PrepocitejHrace()
        {
            lbPocetHracu.Text = $"Počet hráčů: {sstHraci.Seznam.Count}";
            lbPocetHracu.Background = sstHraci.Seznam.Count > 0 ? Brushes.Transparent : Brushes.Red;
        }

        private void sstUlohy_KliklNaTlacitko(object sender, RoutedEventArgs e)
        {
            Par<string, Par<bool, bool>> kliklyPrvek = (Par<string, Par<bool, bool>>)((Button)sender).GetAncestorOfType<ListViewItem>().Content;
            Ulohy.RemoveAt(Ulohy.IndexOf(kliklyPrvek));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (voh.Visibility == Visibility.Visible)
            {
                voh.Hide();
            }
            else
            {
                voh.Show();
                if (voh.Owner == null)
                {
                    voh.Owner = this;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (vou.Visibility == Visibility.Visible)
            {
                vou.Hide();
            }
            else
            {
                vou.Show();
                if (vou.Owner == null)
                {
                    vou.Owner = this;
                }
            }
        }

        
        private void Window_Closed(object sender, EventArgs e)
        {
            voh.Close();
            vou.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //bool[] napovedy = (bool[])PraceSDB.ZavolejPrikaz("obsahuji_ulohy_napovedu", true, sstUlohy.Seznam.Select(u => u.OdeberZavorku()).ToArray(), Uzivatel.Id)[0][0];
            if (!SpravnyPocetUloh())
            {
                MessageBox.Show("Špatný počet úloh!");
                return;
            }

            foreach (Par<string, Par<bool, bool>> par in Ulohy)
            {
                if (!SpravnaUloha(par))
                {
                    MessageBox.Show("Špatný typ úlohy!");
                    return;
                }
            }

            if (sstHraci.Seznam.Count < 1)
            {
                MessageBox.Show("Špatný počet hráčů!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbNazev.Text))
            {
                MessageBox.Show("Název nesmí být prázdný!");
                return;
            }

            if (lcbxDruhSpusteni.VybranyItem == null)
            {
                MessageBox.Show("Není vybrán druh spuštění!");
                return;
            }

            PraceSDB.ZavolejPrikaz("vytvor_hru", false, tbNazev.Text, cbxRaditNahodne.IsChecked, Convert.ToByte((int)lcbxDruhSpusteni.VybranyItem), Convert.ToInt32(iupCas.Text) * 60, (int)lcbxDruhSpusteni.VybranyItem == 4 ? ((DateTime)dpKonec.SelectedDate).AddDays(1).AddTicks(-1) : DBNull.Value, Uzivatel.Id, Ulohy.Select(u => u.Klic.OdeberZavorku()).ToArray(), sstHraci.Seznam.Select(h => h.ZiskejZavorku()).ToArray());
            MessageBox.Show("Vytvořeno");
            new tvorbaHer().Show();
            Close();
        }

        private void lcbxDruhSpusteni_ZmenilVyber(object staryObjekt, object novyObjekt)
        {
            ItemContainerGenerator_StatusChanged(seznamUloh.ItemContainerGenerator);
        }
    }
}
