using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interakční logika pro VyberHryKeSpusteni.xaml
    /// </summary>
    public partial class VyberHryKeSpusteni : Window
    {
        public VyberHryKeSpusteni()
        {
            InitializeComponent();
            DataContext = this;
            rb1.IsChecked = true;
        }

        public ObservableCollection<Par<Par<Par<string, string>, Par<string, string>>,int>> seznamHer { get; set; } = new ObservableCollection<Par<Par<Par<string, string>, Par<string, string>>, int>>();

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rb1 == null || rb2 == null)
            {
                return;
            }

            string[] data = (string[])PraceSDB.ZavolejPrikaz("nacti_hry", true, (byte)((bool)rb1.IsChecked ? 1 : (bool)rb2.IsChecked ? 2 : 4), Uzivatel.Id)[0][0];
            seznamHer.NastavHodnoty(data.Select(dato => dato.Split(HlavniStatik.Oddelovac, StringSplitOptions.None)).Select(ctverice => new Par<Par<Par<string, string>, Par<string, string>>, int>(new Par<Par<string, string>, Par<string, string>>(new Par<string, string>(ctverice[0], ctverice[1]), new Par<string, string>(ctverice[2], ctverice[3])), Convert.ToInt32(ctverice[4]))).ToArray());
        }

        private void btSpustit_Click(object sender, RoutedEventArgs e)
        {
            HraciPlocha hp = new HraciPlocha(((Par<Par<Par<string, string>, Par<string, string>>, int>)((Button)sender).GetAncestorOfType<ListViewItem>().Content).Hodnota, rb1.IsChecked == true ? 1 : (bool)rb2.IsChecked == true ? 2 : 4);
            hp.Show();
            Close();
        }
    }
}
