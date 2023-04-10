using EnumsNET;
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
        public ObservableCollection<object> MozneDruhySpusteni { get; set; } = new ObservableCollection<object>() {
            new Par<DruhSpusteni, string>(DruhSpusteni.Uceni, DruhSpusteni.Uceni.AsString(EnumFormat.Description)),
            new Par<DruhSpusteni, string>(DruhSpusteni.Procvicovani, DruhSpusteni.Procvicovani.AsString(EnumFormat.Description)),
            new Par<DruhSpusteni, string>(DruhSpusteni.Oboje, DruhSpusteni.Oboje.AsString(EnumFormat.Description)),
            new Par<DruhSpusteni, string>(DruhSpusteni.Test, DruhSpusteni.Test.AsString(EnumFormat.Description)) 
        };
        public DruhSpusteni druhSCasem { get; set; } = DruhSpusteni.Test;

        public ObservableCollection<Par<string, Par<bool, bool>>> seznamUloh { get; set; } = new ObservableCollection<Par<string, Par<bool, bool>>>();
        private ObservableCollection<string> seznamUlohNazvy = new ObservableCollection<string>();
        public ObservableCollection<string> seznamHracu { get; set; } = new ObservableCollection<string>();
        private string[] vybranyHraci
        {
            get
            {
                return lvHraci.SelectedItems.Cast<string>().ToArray();
            }
        }
        private Par<string, Par<bool, bool>>[] vybraneUlohy
        {
            get
            {
                return lvUlohy.SelectedItems.Cast<Par<string, Par<bool, bool>>>().ToArray();
            }
        }

        vyhledavaciOknoHracu voh;
        VyhledavaciOknoUloh vou;
        private tvorbaHer()
        {
            seznamUloh.CollectionChanged += SeznamUloh_CollectionChanged;
            voh = new vyhledavaciOknoHracu(seznamHracu);
            voh.OdeslaniVybranychPrvku += Voh_OdeslaniVybranychPrvku;
            vou = new VyhledavaciOknoUloh(seznamUlohNazvy);
            vou.OdeslaniVybranychPrvku += Vou_OdeslaniVybranychPrvku;

            InitializeComponent();
            DataContext = this;
            lvUlohy.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            new ListViewDragDropManager<Par<string, Par<bool, bool>>>(lvUlohy);
            ItemContainerGenerator_StatusChanged(lvUlohy.ItemContainerGenerator);
        }

        private void Vou_OdeslaniVybranychPrvku(string[] Prvky)
        {
            if (vou.UlohyVHledacku)
            {
                foreach(string prvek in Prvky)
                {
                    bool napoveda = ((bool[])PraceSDB.ZavolejPrikaz("obsahuji_ulohy_napovedu", true, new string[] { prvek.OdeberZavorku() }, Uzivatel.Id)[0][0])[0];
                    bool moznost = ((string[])PraceSDB.ZavolejPrikaz("obsah_vysledku_uloh", true, new string[] { prvek.OdeberZavorku() }, Uzivatel.Id)[0][0])[0].ToLower()[0] == 'o';
                    seznamUloh.AddIfNotExists(new Par<string, Par<bool, bool>>(prvek, new Par<bool, bool>(napoveda, moznost)));
                }
            }
            else
            {
                foreach (string prvek in Prvky)
                {
                    string[] ulohy = (string[])PraceSDB.ZavolejPrikaz("nacti_ulohy_skupiny", true, prvek, Uzivatel.Id)[0][0];
                    bool[] napovedy = (bool[])PraceSDB.ZavolejPrikaz("obsahuji_ulohy_napovedu", true, ulohy.Select(u => u.OdeberZavorku()).ToArray(), Uzivatel.Id)[0][0];
                    bool[] moznosti = ((string[])PraceSDB.ZavolejPrikaz("obsah_vysledku_uloh", true, ulohy.Select(u => u.OdeberZavorku()).ToArray(), Uzivatel.Id)[0][0]).Select(moznost => moznost.ToLower()[0] == 'o').ToArray();
                    seznamUloh.AddIfNotExists(ulohy.Zip(napovedy, (u, n) => new Par<string, bool>(u, n)).Zip(moznosti, (p, m) => new Par<string, Par<bool, bool>>(p.Klic, new Par<bool, bool>(p.Hodnota, m))).ToArray());
                }
            }
        }

        private void Voh_OdeslaniVybranychPrvku(string[] Prvky)
        {
            if (voh.HraciVHledacku)
            {
                seznamHracu.AddIfNotExists(Prvky);
            }
            else
            {
                foreach(string prvek in Prvky)
                {
                    string[] hraci = (string[])PraceSDB.ZavolejPrikaz("nacti_hrace_skupiny", true, prvek, Uzivatel.Id)[0][0];
                    seznamHracu.AddIfNotExists(hraci);
                }
            }
        }

        private void SeznamUloh_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            seznamUlohNazvy.NastavHodnoty(seznamUloh.Select(u => u.Klic));
        }

        private Window Rodic;
        public tvorbaHer(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic?.Show(); };
        }

        private void ItemContainerGenerator_StatusChanged(object sender = null, EventArgs e = null)
        {
            ItemContainerGenerator icg = (ItemContainerGenerator)sender;
            if (icg != null && icg.Status == GeneratorStatus.ContainersGenerated)
            {
                foreach (Par<string, Par<bool, bool>> lvi in lvUlohy.ItemContainerGenerator.Items)
                {
                    ListViewItem elf = ((ListViewItem)lvUlohy.ItemContainerGenerator.ContainerFromItem(lvi));
                    if (elf == null) //Nenačítá, pokud není vidět
                    {
                        continue;
                    }
                    elf.Background = SpravnaUloha(lvi) ? Brushes.Transparent : Brushes.Yellow;
                    //((ListViewItem)seznamUloh.ItemContainerGenerator.ContainerFromItem(lvi)).Background = Brushes.Yellow;
                }
            }
        }

        private bool SpravnaUloha(Par<string, Par<bool, bool>> lvi)
        {
            return lcbxDruhSpusteni.VybranyItem == null 
                || ((Par<DruhSpusteni, string>)lcbxDruhSpusteni.VybranyItem).Klic == DruhSpusteni.Test 
                || (!lvi.Hodnota.Klic && !lvi.Hodnota.Hodnota) 
                || (!lvi.Hodnota.Klic && ((Par<DruhSpusteni, string>)lcbxDruhSpusteni.VybranyItem).Klic == DruhSpusteni.Uceni);
        }

        private bool SpravnyPocetUloh()
        {
            return
                lcbxDruhSpusteni.VybranyItem == null
                ||
                (seznamUloh.Count > 3 && new DruhSpusteni[] { DruhSpusteni.Test, DruhSpusteni.Uceni }.Contains(((Par<DruhSpusteni, string>)lcbxDruhSpusteni.VybranyItem).Klic))
                ||
                (seznamUloh.Count > 6 && new DruhSpusteni[] { DruhSpusteni.Procvicovani, DruhSpusteni.Oboje }.Contains(((Par<DruhSpusteni, string>)lcbxDruhSpusteni.VybranyItem).Klic));
        }
        
        private void Window_Closed(object sender, EventArgs e)
        {
            voh.Close();
            vou.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!SpravnyPocetUloh())
            {
                LepsiMessageBox.Show("Špatný počet úloh!");
                return;
            }

            foreach (Par<string, Par<bool, bool>> par in seznamUloh)
            {
                if (!SpravnaUloha(par))
                {
                    LepsiMessageBox.Show("Špatný typ úlohy!");
                    return;
                }
            }

            if (seznamHracu.Count < 1)
            {
                LepsiMessageBox.Show("Špatný počet hráčů!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbNazev.Text))
            {
                LepsiMessageBox.Show("Název nesmí být prázdný!");
                return;
            }

            if (lcbxDruhSpusteni.VybranyItem == null)
            {
                LepsiMessageBox.Show("Není vybrán druh spuštění!");
                return;
            }
            bool test = ((Par<DruhSpusteni, string>)lcbxDruhSpusteni.VybranyItem).Klic == druhSCasem;
            PraceSDB.ZavolejPrikaz("vytvor_hru", false, 
                tbNazev.Text, 
                test && cbxRaditNahodne.IsChecked.Value, 
                !test || cbxOpravovat.IsChecked.Value, 
                (byte)(int)((Par<DruhSpusteni, string>)lcbxDruhSpusteni.VybranyItem).Klic,
                test ? Convert.ToInt32(iupCas.Text) * 60 : 0, 
                test ? ((DateTime)dpKonec.SelectedDate).AddDays(1).AddTicks(-1) : DBNull.Value, 
                Uzivatel.Id, 
                seznamUloh.Select(u => u.Klic.OdeberZavorku()).ToArray(), 
                seznamHracu.Select(h => h.ZiskejZavorku()).ToArray()
                );
            LepsiMessageBox.Show("Vytvořeno");
            new tvorbaHer(Rodic).Show();
            Rodic = null;
            Close();
        }

        private void lcbxDruhSpusteni_ZmenilVyber(object staryObjekt, object novyObjekt)
        {
            ItemContainerGenerator_StatusChanged(lvUlohy.ItemContainerGenerator);
            grCBXka.IsEnabled = dpKonec.IsEnabled = iupCas.IsEnabled = ((Par<DruhSpusteni, string>)novyObjekt).Klic == druhSCasem;
        }

        private void btPridatHrace_Click(object sender, RoutedEventArgs e)
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

        private void btOdebratHrace_Click(object sender, RoutedEventArgs e)
        {
            foreach(string hrac in vybranyHraci)
            {
                seznamHracu.Remove(hrac);
            }
        }

        private void btPridatUlohu_Click(object sender, RoutedEventArgs e)
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

        private void btOdebratUlohu_Click(object sender, RoutedEventArgs e)
        {
            //Par<string, Par<bool, bool>> kliklyPrvek = (Par<string, Par<bool, bool>>)((Button)sender).GetAncestorOfType<ListViewItem>().Content;
            foreach (Par<string, Par<bool, bool>> uloha in vybraneUlohy)
            {
                seznamUloh.Remove(uloha);
            }
        }
    }
}
