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
    /// Interakční logika pro oknoPomoci.xaml
    /// </summary>
    public partial class oknoPomoci : Window
    {
        public oknoPomoci()
        {
            InitializeComponent();
            imgPomoc1.Source = Properties.Resources.icoNapoveda.ToImageSource();
            imgPomoc2.Source = Properties.Resources.icoNapoveda.ToImageSource();
            imgPomoc3.Source = Properties.Resources.icoNapoveda.ToImageSource();
        }

        Hra hra { get; set; }
        Window okno;

        public oknoPomoci(Hra h, Window w)
        {
            InitializeComponent();
            hra = h;
            okno = w;
            DataContext = hra;
        }

        private void imgPomoc1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgPomoc1.Source = Properties.Resources.icoNapovedaNe.ToImageSource();
            imgPomoc1.IsEnabled = false;
            grPomoc.Visibility = Visibility.Visible;

            List<string> vsechnyMoznosti = hra.aktualniUloha.CastiVysledku4.ToList();
            string[] moznosti = new string[2];
            int i = HlavniStatik.rnd.Next(0, vsechnyMoznosti.Count);
            moznosti[0] = vsechnyMoznosti[i];
            vsechnyMoznosti.RemoveAt(i);
            moznosti[1] = vsechnyMoznosti[HlavniStatik.rnd.Next(0, vsechnyMoznosti.Count)];
            bool b = HlavniStatik.rnd.Next(0, 2) == 0;
            lbPomoc.Content = $"\"{moznosti[b ? 0 : 1]}\" <-> \"{moznosti[b ? 1 : 0]}\"";
        }

        private void imgPomoc2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgPomoc2.Source = Properties.Resources.icoNapovedaNe.ToImageSource();
            imgPomoc2.IsEnabled = false;
            grPomoc.Visibility = Visibility.Visible;

            lbPomoc.Content = hra.aktualniUloha.Napoveda;
        }

        private void imgPomoc3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgPomoc3.Source = Properties.Resources.icoNapovedaNe.ToImageSource();
            imgPomoc3.IsEnabled = false;
            grPomoc.Visibility = Visibility.Visible;

            lbPomoc.Content = "Graf se domyslí později";
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            grPomoc.Visibility = Visibility.Hidden;
        }
    }
}
