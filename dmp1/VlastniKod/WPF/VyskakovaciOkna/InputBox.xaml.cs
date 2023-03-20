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
    //Lepší MessageBox s textovým vstupem
    [NotifyPropertyChanged]
    public partial class InputBox : Window
    {
        public string noveJmeno { get; set; } //Hodnota z textboxu

        //Nastavení základních hodnot
        public InputBox(string stareJmeno, string Nadpis)
        {
            InitializeComponent();
            DataContext = this;
            noveJmeno = stareJmeno;
            Title = Nadpis;
        }

        //Potvrzení vstupu a zavření
        private void btUlozit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        //Zrušení okna bez uložení vstupu
        private void btZrusit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
