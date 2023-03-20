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
        private Window Rodic;

        public string Nazev { get; set; }
        public string ZbyvajiciCas { get; set; }

        private int IdHrace;
        public Hra HraKeKontrole { get; set; }

        private VysledkoveOkno()
        {
            InitializeComponent();
            DataContext = this;
        }

        public VysledkoveOkno(int hraId, string hrac, Window rodic) : this()
        {
            string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_vysledek_hry", true, hraId, hrac)[0][0];
            IdHrace = Convert.ToInt32(data[0]);
            Nazev = data[1];
            ZbyvajiciCas = data[2];
            int[] vysledky = data[3].ZiskejPole<int>();
            HraKeKontrole = new Hra(hraId, vysledky);
            Rodic = rodic;
        }

        private void btProhlidka_Click(object sender, RoutedEventArgs e)
        {
            new KontrolaUloh(HraKeKontrole, this).Show();
            Hide();
        }

        private void btPryc_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Rodic.Show();
        }
    }
}
