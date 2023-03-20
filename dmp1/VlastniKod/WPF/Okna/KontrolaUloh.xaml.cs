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
    /// Interakční logika pro KontrolaUloh.xaml
    /// </summary>
    public partial class KontrolaUloh : Window
    {
        private Window Rodic;
        public Hra HraCoSeKontroluje { get; set; }

        private KontrolaUloh()
        {
            InitializeComponent();
        }

        public KontrolaUloh(Hra hra, Window rodic) : this()
        {
            HraCoSeKontroluje = hra;
            DataContext = HraCoSeKontroluje;
            Rodic = rodic;
            //ListBoxItem_MouseUp(null, null);
        }

        private void btUznat_Click(object sender, RoutedEventArgs e)
        {
            HraCoSeKontroluje.aktualniUloha.PrehodnotStav(1);
        }

        private void btUkoncit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Rodic.Show();
        }

        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem lvi)
            {
                HraCoSeKontroluje.aktualniUloha = (Uloha)lvi.DataContext;
            }
        }

        private void btNeuznat_Click(object sender, RoutedEventArgs e)
        {
            HraCoSeKontroluje.aktualniUloha.PrehodnotStav(2);
        }
    }
}
