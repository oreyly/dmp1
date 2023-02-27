using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro editorUloh.xaml
    /// </summary>
    //Okno pro editaci a přidávání úloh
    public partial class editorUloh : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> Kategorie = new ObservableCollection<string>(); //Seznam dostupných kategorií
        private OpenFileDialog ofd; //Okno pro výběr souboru

        //Pomocná proměnná pro přepínání ToggleSwitchů
        public bool ABCDMoznosti
        {
            get
            {
                return htsTypVysledku.IsChecked;
            }
        }

        //public ObservableCollection<Uloha> seznamUloh { get; set; } = new ObservableCollection<Uloha>();

        private Uloha _VybranaUloha;
        public Uloha VybranaUloha { 
            get
            {
                return _VybranaUloha;
            }
            set
            {
                //Nastavení DataContextu na nově vybranou úlohu
                _VybranaUloha = value;
                DataContext = _VybranaUloha;
                OnPropertyChanged();
            }
        }

        HttpClient client = new HttpClient();
        //Výběr nového obrázku
        private void imgNahled_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                VybranaUloha.Obrazek = null;
                return;
            }

            if (ofd.ShowDialog() == true)
            {
                //MessageBox.Show(ofd.FileName);
                //imgNahled.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
                BitmapImage bi = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
                using (MemoryStream ms = new MemoryStream())
                {
                    JpegBitmapEncoder enkoder = new JpegBitmapEncoder();
                    enkoder.Frames.Add(BitmapFrame.Create(bi));
                    enkoder.Save(ms);
                    string byty = Convert.ToBase64String(ms.ToArray());
                    Dictionary<string, string> hodnoty = new Dictionary<string, string>()
                    {
                        {"heslo", "KoprovkaJeZloVytvoreneDablem" },
                        {"obr", byty }
                    };

                    IEnumerable<string> encodedItems = hodnoty.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
                    StringContent kontent = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");

                    Task<HttpResponseMessage> odpovedT = client.PostAsync(@"https://home.spsostrov.cz/~matema/dlouhodobka/php/nahraniSouboru.php", kontent);
                    odpovedT.Wait();

                    Task<string> zbyvaT = odpovedT.Result.Content.ReadAsStringAsync();
                    zbyvaT.Wait();

                    string vys = zbyvaT.Result;
                    VybranaUloha.Obrazek = vys;
                    VybranaUloha.obsahujeObrazek = true;
                }
            }
        }


        public editorUloh()
        {
            VytvorOFD();
            InitializeComponent();
            NactiOkno();
        }

        //Příprava okna
        private void NactiOkno()
        {
            DataContext = VybranaUloha;
            grOdpovedi.DataContext = this;
            lvSeznam.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            NacteniKategorii();
            NacteniUloh();
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            ItemContainerGenerator icg = (ItemContainerGenerator)sender;
            if (icg.Status == GeneratorStatus.ContainersGenerated)
            {
                NastavTlacitkaSeznamu(icg.Status);
            }
        }

        private void NacteniKategorii()
        {
            Kategorie.NastavHodnoty(((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_kategorie", true, Uzivatel.Id)[0][0]).Select(x => x.Key));
            lcbxKategorie.Seznam.NastavHodnoty(Kategorie);
            lcbxKategorie.VybranyItem = null;
        }

        private void NacteniUloh()
        {
            IEnumerable<Uloha> data = PraceSDB.ZavolejPrikaz("nacti_ulohy", true, Uzivatel.Id).Select(x => new Uloha((string)((object[])x[0])[0], (string)((object[])x[0])[1], (string)((object[])x[0])[2], (string)((object[])x[0])[3], (string)((object[])x[0])[4], (int)((object[])x[0])[5], (string)((object[])x[0])[6], (int)((object[])x[0])[7]));
            lcbxUloha.Seznam.NastavHodnoty(data);
        }

        //Připraví okno pro vybrání souboru
        private void VytvorOFD()
        {
            ofd = new OpenFileDialog();
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

        //Změna ToggleSwitche
        private void htsTypVysledku_CheckChanged(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("ABCDMoznosti");
        }

        //Oznámení, že vlastnost byla změněna pro správnou funkci bidingu
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //Otevření úpravy kategorií
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpravaKategorie uk = new UpravaKategorie();
            uk.ShowDialog();
            NacteniKategorii();
        }

        private bool poprve = true;
        private bool vrat = false;
        private bool vrat2 = false;
        private string Nazev;
        //Vybrání úlohy k editaci
        private void lcbxUloha_ZmenilVyber_1(object staryObjekt, object novyObjekt)
         {
            if (lcbxUloha.Seznam.Count == 0)
            {
                return;
            }

            if (vrat)
            {
                vrat = false;
                //lcbxUloha.VybranyItem = lcbxUloha.Seznam.First(u => ((Uloha)u).Nazev == Nazev);
                return;
            }

            if ((Uloha)novyObjekt != null && (Uloha)staryObjekt != null && staryObjekt != novyObjekt && ((Uloha)staryObjekt).ZmenilSe)
            {
                if (MessageBox.Show("Uložit změny?", "Změna úlohy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (((Uloha)staryObjekt).UlozSe(lcbxUloha.Seznam.Cast<Uloha>()))
                    {
                        //vrat = true;
                        Nazev = ((Uloha)novyObjekt).Nazev;
                        Uloha u = null;
                        if (((Uloha)novyObjekt).Nova)
                        {
                            u = (Uloha)novyObjekt;
                        }
                        NacteniKategorii();
                        NacteniUloh();
                        if (u != null)
                        {
                            lcbxUloha.Seznam.AddIfNotExists(u);
                        }
                        lcbxUloha.VybranyItem = lcbxUloha.Seznam.FirstOrDefault(u => ((Uloha)u).Nazev == Nazev);
                        if (lcbxUloha.VybranyItem == null)
                        {
                            lcbxUloha.VybranyItem = VybranaUloha;
                        }
                        VybranaUloha = (Uloha)lcbxUloha.VybranyItem;
                        return;
                    }
                    else
                    {
                        vrat = true;
                        lcbxUloha.VybranyItem = (Uloha)staryObjekt;
                        return;
                    }
                }
                else
                {
                    if(((Uloha)staryObjekt).Nova)
                    {
                        ((Uloha)staryObjekt).OdstranSe();
                        Nazev = ((Uloha)novyObjekt).Nazev;
                        NacteniKategorii();
                        NacteniUloh();
                        lcbxUloha.VybranyItem = lcbxUloha.Seznam.First(u => ((Uloha)u).Nazev == Nazev);
                        VybranaUloha = (Uloha)lcbxUloha.VybranyItem;
                    }
                    else
                    {
                        ((Uloha)staryObjekt).ObnovSe();
                    }
                }
                /*vrat = true;
                VybranaUloha = (Uloha)staryObjekt;
                lcbxUloha.VybranyItem = (Uloha)staryObjekt;
                return;*/
                //((Uloha)staryObjekt).UlozSe();
            }
            VybranaUloha = (Uloha)novyObjekt;
        }

        private void NastavTlacitkaSeznamu(GeneratorStatus gs)
        {
            btPridat.IsEnabled = VybranaUloha.otevreneVysledky.Count < 4;

            foreach (Par<string, string> p in lvSeznam.Items) 
            {
                ListViewItem lvi = (ListViewItem)lvSeznam.ItemContainerGenerator.ContainerFromItem(p);
                if (lvi != null)
                {
                    lvi.GetChildByName("btOdstranit").IsEnabled = VybranaUloha.otevreneVysledky.Count > 1;
                }
            }
        }

        private void btPridat_Click(object sender, RoutedEventArgs e)
        {
            VybranaUloha.otevreneVysledky.Add(new Par<string, string>("", ""));
        }

        private void btOdstranit_Click(object sender, RoutedEventArgs e)
        {
            Par<string, string> lvi = (Par<string, string>)((Button)sender).GetAncestorOfType<ListViewItem>().Content;
            int ind = lvSeznam.Items.IndexOf(lvi);
            VybranaUloha.otevreneVysledky.RemoveAt(ind);
        }

        private void btNovaUloha_Click(object sender, RoutedEventArgs e)
        {
            Uloha novaUloha = new Uloha();
            lcbxUloha.Seznam.Add(novaUloha);
            lcbxUloha.VybranyItem = novaUloha;
        }

        private void btUlozitUlohu_Click(object sender, RoutedEventArgs e)
        {
            Uloha uloha = (Uloha)lcbxUloha.VybranyItem;
            if(uloha == null)
            {
                return;
            }

            if (!uloha.Nova && !uloha.ZmenilSe)
            {
                MessageBox.Show("Nebyly provedeny žádné změny");
                return;
            }

            if (MessageBox.Show("Uložit změny?", "Změna úlohy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (uloha.UlozSe(lcbxUloha.Seznam.Cast<Uloha>()))
                {
                    //vrat = true;
                    Nazev = uloha.Nazev;
                    NacteniKategorii();
                    NacteniUloh();
                    lcbxUloha.VybranyItem = lcbxUloha.Seznam.First(u => ((Uloha)u).Nazev == Nazev);
                    VybranaUloha = (Uloha)lcbxUloha.VybranyItem;
                    return;
                }
            }
        }

        private void btOdstranUlohu_Click(object sender, RoutedEventArgs e)
        {
            if (lcbxUloha.VybranyItem!=null && MessageBox.Show($"Opravdu si přejetě odstranit úlohu '{lcbxUloha.VybranyItem}'?", "Změna úlohy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ((Uloha)lcbxUloha.VybranyItem).OdstranSe();

                NacteniKategorii();
                NacteniUloh();

                VybranaUloha = null;
            }
        }
    }
}
