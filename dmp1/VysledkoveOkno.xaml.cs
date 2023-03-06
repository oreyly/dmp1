using System;
using System.Collections.Generic;
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
    /// Interakční logika pro VysledkoveOkno.xaml
    /// </summary>
    public partial class VysledkoveOkno : Window
    {
        private VysledkoveOkno()
        {
            InitializeComponent();
            DataContext = this;
        }

        public VysledkoveOkno(int hraId, string hrac):this()
        {
            string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_vysledek_hry", true, hraId, hrac)[0][0];
            //Asi při mně tady stojí všichni svatí, ale zatím všechno nějak funguje
        }

        private void btProhlidka_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btPryc_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
