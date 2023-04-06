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
    /// Interakční logika pro VyberProduktu.xaml
    /// </summary>
    public partial class VyberProduktu : Window
    {
        public ObservableCollection<Par<URLAdresa, bool>> seznamProduktu { get; set; } = new ObservableCollection<Par<URLAdresa, bool>>();
        private VyberProduktu()
        {
            InitializeComponent();
            Topmost = true;
            DataContext = this;
        }

        DruhProduktu druhProduktu;

        public VyberProduktu(DruhProduktu dp) : this()
        {
            druhProduktu = dp;
            NactiProdukty();
        }

        private void NactiProdukty()
        {
            URLAdresa[] data;
            if (druhProduktu == DruhProduktu.ProfilovaFotka)
            {
                data = ((string[])PraceSDB.ZavolejPrikaz("nacti_hracovo_avatary", true, Uzivatel.Id)[0][0]).Select(adresa => (URLAdresa)adresa).ToArray();
            }
            else
            {
                data = ((string[])PraceSDB.ZavolejPrikaz("nacti_hracovo_temata", true, Uzivatel.Id)[0][0]).Select(adresa => (URLAdresa)adresa).ToArray();
            }

            seznamProduktu.NastavHodnoty(data.Select(dato => new Par<URLAdresa, bool>(dato, dato == (druhProduktu == DruhProduktu.ProfilovaFotka ? Uzivatel.ObrazekProfil : Uzivatel.ObrazekPozadi))));
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ObrazekVKrouzku_MouseUp(object sender, MouseButtonEventArgs e)
        {
            seznamProduktu.First(p => p.Hodnota).Hodnota = false;
            ((Par<URLAdresa, bool>)((ObrazekVKrouzku)sender).GetAncestorOfType<ContentPresenter>().Content).Hodnota = true;
        }

        private void ObrazekVKrouzku_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Uzivatel.NastavProdukt(druhProduktu, seznamProduktu.First(p => p.Hodnota).Klic);
            Close();
        }
    }
}
