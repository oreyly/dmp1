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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro editorUloh.xaml
    /// </summary>
    //Okno pro editaci a přidávání úloh
    public partial class editorUloh : Window, INotifyPropertyChanged
    {
        private OpenFileDialog ofd { get; set; } //Okno pro výběr souboru

        //Pomocná proměnná pro přepínání ToggleSwitchů
        public bool ABCDMoznosti
        {
            get
            {
                return htsTypVysledku.IsChecked;
            }
        }

        //public ObservableCollection<Uloha> seznamUloh { get; set; } = new ObservableCollection<Uloha>();

        Uloha uloha;

        //Výběr nového obrázku
        private void imgNahled_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                uloha.Obrazek = null;
                return;
            }

            if (ofd.ShowDialog() == true)
            {
                //MessageBox.Show(ofd.FileName);
                //imgNahled.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));

                uloha.Obrazek = HlavniStatik.NahrajObrazek(ofd.FileName);
                uloha.obsahujeObrazek = true;
            }
        }


        private editorUloh()
        {
            VytvorOFD();
            InitializeComponent();
        }

        private Window Rodic;
        public editorUloh(Window rodic, int id, string kategorie) : this()
        {
            if (id >= 0)
            {
                object[] data = (object[])PraceSDB.ZavolejPrikaz("nacti_ulohu", true, id)[0][0];
                uloha = new Uloha((string)data[0], (string)data[1], (string)data[2], (string)data[3], (string)data[4], (int)data[5], id, kategorie: kategorie);
            }
            else
            {
                uloha = new Uloha(kategorie);
            }

            NactiOkno();

            Rodic = rodic;
        }

        //Příprava okna
        private void NactiOkno()
        {
            DataContext = uloha;
            grOdpovedi.DataContext = this;
            lvOtevreneUlohy.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            ItemContainerGenerator icg = (ItemContainerGenerator)sender;
            if (icg.Status == GeneratorStatus.ContainersGenerated)
            {
                NastavTlacitkaSeznamu(icg.Status);
            }
        }

        //Připraví okno pro vybrání souboru
        private void VytvorOFD()
        {
            ofd = new OpenFileDialog(); 

            ofd.FileOk += delegate (object s, CancelEventArgs ev) {
                var size = new FileInfo(ofd.FileName).Length;
                if (size > 4_000_000)
                {
                    LepsiMessageBox.Show("Soubor je moc velký, zkus něco jiného! Mě nenachytáš MARKU");
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
        }

        private bool poprve = true;
        private bool vrat = false;
        private bool vrat2 = false;
        private string Nazev;

        private void NastavTlacitkaSeznamu(GeneratorStatus gs)
        {
            btPridat.IsEnabled = uloha.otevreneVysledky.Count < 4;

            foreach (Par<string, string> p in lvOtevreneUlohy.Items) 
            {
                ListViewItem lvi = (ListViewItem)lvOtevreneUlohy.ItemContainerGenerator.ContainerFromItem(p);
                if (lvi != null)
                {
                    lvi.GetChildByName("btOdstranit").IsEnabled = uloha.otevreneVysledky.Count > 1;
                }
            }
        }

        private void btPridat_Click(object sender, RoutedEventArgs e)
        {
            uloha.otevreneVysledky.Add(new Par<string, string>("", ""));
        }

        private void btOdstranit_Click(object sender, RoutedEventArgs e)
        {
            Par<string, string> lvi = (Par<string, string>)((Button)sender).GetAncestorOfType<ListViewItem>().Content;
            int ind = lvOtevreneUlohy.Items.IndexOf(lvi);
            uloha.otevreneVysledky.RemoveAt(ind);
        }

        private void btUlozitUlohu_Click(object sender, RoutedEventArgs e)
        {
            if (!uloha.Nova && !uloha.ZmenilSe)
            {
                LepsiMessageBox.Show("Nebyly provedeny žádné změny");
                return;
            }

            uloha.UlozSe();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(!uloha.UlozSe())
            {
                e.Cancel = true;
                return;
            }

            Rodic.Show();
        }
    }
}
