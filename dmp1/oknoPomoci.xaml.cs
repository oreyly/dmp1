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

        private void imgPomoc1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgPomoc2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgPomoc3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
