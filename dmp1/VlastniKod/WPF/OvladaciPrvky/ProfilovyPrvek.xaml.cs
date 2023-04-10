using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro ProfilovyPrvek.xaml
    /// </summary>
    public partial class ProfilovyPrvek : UserControl
    {
        public ProfilovyPrvek()
        {
            InitializeComponent();
            llJmeno.Content = Uzivatel.Jmeno.OdeberZavorku();
            lbBodyPocet.Visibility = lbBodySlovo.Visibility = Uzivatel.Prava == UrovenPrav.Zak ? Visibility.Visible : Visibility.Collapsed;
            obrVelky.Cursor = Uzivatel.Prava == UrovenPrav.Zak ? Cursors.Hand : Cursors.Arrow;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*if (e.WidthChanged)
            {
                Height = e.NewSize.Width / 4;
            }*/
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Uzivatel.Prava == UrovenPrav.Zak)
            {
                ppNabidka.StaysOpen = true;
                VyberProduktu vp = new VyberProduktu(DruhProduktu.ProfilovaFotka);
                vp.ShowDialog();
                ppNabidka.StaysOpen = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HlavniStatik.OdhlasitSe();
        }
    }
}
