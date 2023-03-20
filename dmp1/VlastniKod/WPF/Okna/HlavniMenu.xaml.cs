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

namespace dmp1.VlastniKod.WPF.Okna
{
    /// <summary>
    /// Interakční logika pro HlavniMenu.xaml
    /// </summary>
    public partial class HlavniMenu : Window
    {
        public HlavniMenu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new VyberUlohyPodleKategorie(this).Show();
            Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new editorSkupinUloh(this).Show();
            Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            new spravaSkupin(this).Show();
            Hide();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            new tvorbaHer(this).Show();
            Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
