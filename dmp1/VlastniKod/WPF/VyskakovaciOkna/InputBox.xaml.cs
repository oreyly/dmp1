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
        private int MinimalniDelka;
        private bool Heslo;
        //Nastavení základních hodnot
        public InputBox(string stareJmeno, string Nadpis, bool heslo = false, int minimalniDelka = 0)
        {
            InitializeComponent();
            DataContext = this;
            noveJmeno = stareJmeno;
            Title = Nadpis;

            if (heslo)
            {
                Heslo = heslo;
                MinimalniDelka = minimalniDelka;
                tbNazev.Visibility = Visibility.Collapsed;
                pbNazev.Visibility = Visibility.Visible;
            }
        }

        //Potvrzení vstupu a zavření
        private void btUlozit_Click(object sender, RoutedEventArgs e)
        {
            if (Heslo)
            {
                if (pbNazev.Password.Length < MinimalniDelka)
                {
                    LepsiMessageBox.Show($"Heslo musí mít minimálně {MinimalniDelka} znaků!");
                    return;
                }

                LepsiMessageBox.Show($"Heslo úspěšně uloženo!");
                noveJmeno = pbNazev.Password;
            }

            DialogResult = true;
        }

        //Zrušení okna bez uložení vstupu
        private void btZrusit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Nazev_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btUlozit_Click(null, null);
                e.Handled = true;
            }
        }
    }
}
