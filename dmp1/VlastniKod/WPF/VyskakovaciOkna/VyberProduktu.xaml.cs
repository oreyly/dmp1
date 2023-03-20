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
        public ObservableCollection<Par<string, bool>> seznamProduktu { get; set; } = new ObservableCollection<Par<string, bool>>();
        public VyberProduktu()
        {
            InitializeComponent();
            NactiProdukty();
            DataContext = this;
        }

        private void NactiProdukty()
        {
            string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_hracovo_avatary", true, Uzivatel.Id)[0][0];
            seznamProduktu.NastavHodnoty(data.Select(dato => new Par<string, bool>(dato, dato == Uzivatel.ObrazekProfil)));
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

        private void ObrazekVKrouzku_MouseUp(object sender, MouseButtonEventArgs e)
        {
            seznamProduktu.First(p => p.Hodnota).Hodnota = false;
            ((Par<string, bool>)((ObrazekVKrouzku)sender).GetAncestorOfType<ContentPresenter>().Content).Hodnota = true;
        }
    }
}
