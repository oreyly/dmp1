using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interakční logika pro EditorObchodu.xaml
    /// </summary>
    public partial class EditorObchodu : Window
    {
        //Nadřazené okno
        Window Rodic;

        //Okno pro výběr souboru
        OpenFileDialog ofd;

        //Seznam produktů
        public ObservableCollection<Produkt> seznamProduktu { get; set; } = new ObservableCollection<Produkt>();
       
        //Vybraný produkt
        private Produkt VybranyProdukt
        {
            get
            {
                return (Produkt)lvProdukty.SelectedItem;
            }
        }

        private EditorObchodu()
        {
            InitializeComponent();
            DataContext = this;
            VytvorOFD();
            NacteniVsechProduktu();
        }

        //Konstruktor umožňující návrat k rodičovi
        public EditorObchodu(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
        }

        //Inicializace ofd
        private void VytvorOFD()
        {
            ofd = new OpenFileDialog();

            ofd.FileOk += delegate (object s, CancelEventArgs ev) {
                var size = new FileInfo(ofd.FileName).Length;
                if (size > 4_000_000)
                {
                    LepsiMessageBox.Show("Soubor je moc velký, zkus něco jiného!");
                    ev.Cancel = true;
                }
            };

            ofd.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            foreach (ImageCodecInfo c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            ofd.Filter = string.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "All Files", "*.*");

            ofd.DefaultExt = ".png";
        }

        //Načtení produktů daného typu
        private void NacteniVsechProduktu()
        {
            IEnumerable<Produkt> produkty;
            if (htsDruh.IsChecked)
            {
                produkty = ((string[])PraceSDB.ZavolejPrikaz("nacti_temata_k_editaci", true)[0][0]).Select(p => p.RozdelDolary()).Select((p, i) => new Produkt(Convert.ToInt32(p[0]), p[1], p[2], Convert.ToInt32(p[3]), DruhProduktu.Pozadi));
            }
            else
            {
                produkty = ((string[])PraceSDB.ZavolejPrikaz("nacti_avatary_k_editaci", true)[0][0]).Select(p => p.RozdelDolary()).Select((p, i) => new Produkt(Convert.ToInt32(p[0]), p[1], p[2], Convert.ToInt32(p[3]), DruhProduktu.ProfilovaFotka));
            }

            seznamProduktu.NastavHodnoty(produkty);
        }

        //Změna druhu produktu
        private void HorizontalToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            NacteniVsechProduktu();
        }

        //Odoznačení prvku v seznamu
        private void lvKategorie_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ListView)sender).SelectedIndex = -1;
        }

        //Zastavení MouseUp eventu při kliku na prvek v seznamu
        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        //Změna vybraného produktu
        private void lvProdukty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Produkt stary = (Produkt)e.RemovedItems[0];
                if (!stary.Novy && stary.ZmenilSe)
                {
                    lvProdukty.SelectionChanged -= lvProdukty_SelectionChanged;
                    lvProdukty.SelectedItem = stary;

                    MessageBoxResult mbr = LepsiMessageBox.Show("Přejete si uložit provedené změny?", DruhTlacitekLMB.AnoNeZrusit);

                    switch(mbr)
                    {
                        case MessageBoxResult.Yes:
                            if(!stary.Ulozit())
                            {
                                lvProdukty.SelectedItem = stary;
                                if (((Produkt)e.AddedItems[0]).Novy)
                                {
                                    seznamProduktu.RemoveAt(seznamProduktu.Count - 1);
                                }
                            }
                            else
                            {
                                lvProdukty.SelectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
                            }
                            break;

                        case MessageBoxResult.No:
                            stary.ObnovVse();
                            lvProdukty.SelectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
                            break;
                    }

                    lvProdukty.SelectionChanged += lvProdukty_SelectionChanged;
                }
            }
        }

        //Přidání nového produktu
        private void btPridat_Click(object sender, RoutedEventArgs e)
        {
            seznamProduktu.Add(new Produkt(htsDruh.IsChecked ? DruhProduktu.Pozadi : DruhProduktu.ProfilovaFotka));
            lvProdukty.SelectedIndex = seznamProduktu.Count - 1;
        }

        //Odstranění vybraného produktu
        private void btOdstranit_Click(object sender, RoutedEventArgs e)
        {
            if (LepsiMessageBox.Show("Opravdu si přejete odstranit vybraný produkt?", DruhTlacitekLMB.AnoNe) != MessageBoxResult.Yes)
            {
                return;
            }

            if(!VybranyProdukt.Novy)
            {
                PraceSDB.ZavolejPrikaz(htsDruh.IsChecked ? "odstran_tema" : "odstran_avatar", false, VybranyProdukt.Id);
            }

            seznamProduktu.Remove(VybranyProdukt);
        }

        //Uložení produktu
        private void btUlozit_Click(object sender, RoutedEventArgs e)
        {
            VybranyProdukt.Ulozit();
        }

        //Obnovení produktu do původního stavu
        private void btObnovit_Click(object sender, RoutedEventArgs e)
        {
            VybranyProdukt.ObnovVse();
        }

        //Obnovení obrázku produktu
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
            {
                VybranyProdukt.ObnovURL();
                return;
            }

            if (ofd.ShowDialog() == true)
            {
                VybranyProdukt.URL = HlavniStatik.NahrajObrazek(ofd.FileName);
            }
        }
    }
}
