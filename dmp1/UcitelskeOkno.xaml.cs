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
    /// Interakční logika pro UcitelskeOkno.xaml
    /// </summary>
    public partial class UcitelskeOkno : Window
    {
        public UcitelskeOkno()
        {
            InitializeComponent();
        }

        private void btEditorUloh_Click(object sender, RoutedEventArgs e)
        {
            new VyberUlohyPodleKategorie(this).Show();
            Hide();
        }

        private void btSpravaSkupinUloh_Click(object sender, RoutedEventArgs e)
        {
            new editorSkupinUloh(this).Show();
            Hide();
        }

        private void btSpravaSkupinHracu_Click(object sender, RoutedEventArgs e)
        {
            new spravaSkupin(this).Show();
            Hide();
        }

        private void btTvorbaHer_Click(object sender, RoutedEventArgs e)
        {
            new tvorbaHer(this).Show();
            Hide();
        }

        private void btVysledky_Click(object sender, RoutedEventArgs e)
        {
            new ProhlednoutTesty(this).Show();
            Hide();
        }
    }
}
