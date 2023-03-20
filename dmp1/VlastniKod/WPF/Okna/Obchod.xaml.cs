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
    /// Interakční logika pro Obchod.xaml
    /// </summary>
    public partial class Obchod : Window
    {
        public ObservableCollection<Produkt> seznamProduktu { get; set; } = new ObservableCollection<Produkt>();
        public Obchod()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void htsTypVysledku_Checked(object sender, RoutedEventArgs e)
        {
            if (htsTypVysledku.IsChecked)
            {
                string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_vsechny_temata", true, Uzivatel.Id)[0][0];
                seznamProduktu.NastavHodnoty(data.Select(radek => radek.RozdelDolary()).Select(pole => new Produkt(Convert.ToInt32(pole[0]), pole[1], Convert.ToInt32(pole[2]), DruhProduktu.Pozadi, Convert.ToBoolean(pole[3]))));
            }
            else
            {
                string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_vsechny_avatary", true, Uzivatel.Id)[0][0];
                seznamProduktu.NastavHodnoty(data.Select(radek => radek.RozdelDolary()).Select(pole => new Produkt(Convert.ToInt32(pole[0]), pole[1], Convert.ToInt32(pole[2]), DruhProduktu.ProfilovaFotka, Convert.ToBoolean(pole[3]))));
            }
        }
    }
}
