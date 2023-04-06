using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interakční logika pro VysledkoveOkno.xaml
    /// </summary>
    public partial class VysledkoveOkno : Window, INotifyPropertyChanged
    {
        private Window Rodic;

        public string Nazev { get; set; } = "Ahoj0";
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
            Rodic = rodic;
            HraId = hraId;
            Hrac = hrac;
        }


        private int HraId;
        private string Hrac;

        private void NactiHru()
        {
            string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_vysledek_hry", true, HraId, Hrac)[0][0];
            IdHrace = Convert.ToInt32(data[0]);
            Nazev = data[1];
            if (int.TryParse(data[2], out int cas))
            {
                ZbyvajiciCas = $"{cas / 60}:{cas % 60}";
            }
            else
            {
                ZbyvajiciCas = data[2];
            }
            int[] vysledky = data[3].ZiskejPole<int>();
            HraKeKontrole = new Hra(HraId, vysledky, Convert.ToInt32(data[4]));
            OnPropertyChanged("Nazev");
            OnPropertyChanged("ZbyvajiciCas");
            OnPropertyChanged("HraKeKontrole");
            DataContext = this;
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                NactiHru();
            }
        }

        //Oznámení, že vlastnost byla změněna pro správnou funkci bidingu
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
