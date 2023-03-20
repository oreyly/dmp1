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
    /// Interakční logika pro VyberAkceEditoruHer.xaml
    /// </summary>
    public partial class VyberAkceEditoruHer : Window
    {
        Window Rodic;

        private VyberAkceEditoruHer()
        {
            InitializeComponent();
        }

        public VyberAkceEditoruHer(Window rodic)
        {
            Rodic = rodic;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Rodic.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
