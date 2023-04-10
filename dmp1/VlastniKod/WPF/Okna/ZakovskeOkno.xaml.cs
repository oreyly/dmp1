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
    /// Interakční logika pro ZakovskeOkno.xaml
    /// </summary>
    public partial class ZakovskeOkno : Window
    {
        public ZakovskeOkno()
        {
            InitializeComponent();
        }

        private void btSpustit_Click(object sender, RoutedEventArgs e)
        {
            new VyberHryKeSpusteni(this).Show();
            Hide();
        }

        private void btHistorie_Click(object sender, RoutedEventArgs e)
        {
            new ProhlednoutTesty(this).Show();
            Hide();
        }

        private void btObchod_Click(object sender, RoutedEventArgs e)
        {
            new Obchod(this).Show();
            Hide();
        }

        private void btPozadi_Click(object sender, RoutedEventArgs e)
        {
            new VyberProduktu(DruhProduktu.Pozadi).ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LepsiMessageBox.Show("Žáci úspěšně nahráni!");
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
