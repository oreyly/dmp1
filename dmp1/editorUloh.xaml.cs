using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public ObservableCollection<Uloha> seznamUloh = new ObservableCollection<Uloha>();

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

        //Výběr nového obrázku
        private void imgNahled_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                MessageBox.Show(ofd.FileName);
                imgNahled.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
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
            NacteniKategorii();
            NacteniUloh();
        }

        private void NacteniKategorii()
        {
            Kategorie.NastavHodnoty(((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_kategorie", true, Uzivatel.Id)[0][0]).Select(x => x.Key));
            lcbxKategorie.Seznam.NastavHodnoty(Kategorie);
        }

        private void NacteniUloh()
        {
            IEnumerable<Uloha> data = PraceSDB.ZavolejPrikaz("nacti_ulohy", true, Uzivatel.Id).Select(x => new Uloha((string)((object[])x[0])[0], (string)((object[])x[0])[1], (string)((object[])x[0])[2], (string)((object[])x[0])[3], (string)((object[])x[0])[4], (int)((object[])x[0])[5], (string)((object[])x[0])[6]));
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

        //Vybrání úlohy k editaci
        private void lcbxUloha_ZmenilVyber_1(object staryObjekt, object novyObjekt)
        {
            VybranaUloha = (Uloha)novyObjekt;
        }
    }

}
