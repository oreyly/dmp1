using PostSharp.Patterns.Model;
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
    /// Interakční logika pro InputBox.xaml
    /// </summary>
    [NotifyPropertyChanged]
    public partial class InputBox : Window
    {
        public string noveJmeno { get; set; }

        public InputBox(string stareJmeno)
        {
            InitializeComponent();
            DataContext = this;
            noveJmeno = stareJmeno;
        }

        private void btUlozit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btZrusit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
