using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace dmp1
{
    /// <summary>
    /// Interakční logika pro HraciPlocha.xaml
    /// </summary>
    //Okno vykreslující hru
    public partial class HraciPlocha : Window
    {
        private Hra _HraCoSeHraje;
        public Hra HraCoSeHraje
        {
            get
            {
                return _HraCoSeHraje;
            }

            set
            {
                //Nastavení zobrazení okna na základě druhu spuštění
                /*if (value.DruhSpusteni == DruhSpusteni.Uceni)
                {
                    Grid.SetColumn(UVukladani, 0);
                    Grid.SetRow(UVukladani, 0);
                    Grid.SetColumnSpan(UVukladani, 2);
                    Grid.SetRowSpan(UVukladani, 2);

                    btOdeslat.Visibility = Visibility.Collapsed;
                    btUkoncit.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Grid.SetColumn(UVukladani, 1);
                    Grid.SetRow(UVukladani, 0);
                    Grid.SetColumnSpan(UVukladani, 1);
                    Grid.SetRowSpan(UVukladani, 2);

                    btOdeslat.Visibility = Visibility.Visible;
                    btUkoncit.Visibility = Visibility.Visible;
                }*/

                _HraCoSeHraje = value;
            }
        }

        oknoPomoci op;
        //Nastavení základních hodnot
        public HraciPlocha(int id, int druh)
        {
            InitializeComponent();
            tlacitkaMoznosti = new RadioButton[] { btA, btB, btC, btD };
            Hra hr = Hra.NactiHru(id, (DruhSpusteni)druh);
            HraCoSeHraje = hr;
            DataContext = HraCoSeHraje;
            imgNapoveda.Source = Properties.Resources.icoNapoveda.ToImageSource();

            switch(druh)
            {
                case 1:

                    break;

                case 2:
                    op = new oknoPomoci();
                    op.Show();
                    break;

                case 4:

                    break;
            }
            ListBoxItem_MouseUp(null, null);
        }

        //Zobrazení / skrytí nápovědy
        private void imgNapoveda_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgNapoveda.Visibility = Visibility.Collapsed;
            brNapoveda.Visibility = Visibility.Visible;
        }

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            brNapoveda.Visibility = Visibility.Collapsed;
            imgNapoveda.Visibility = Visibility.Visible;
        }

        //Vybrání úlohy v dané hře
        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem lvi)
            {
                HraCoSeHraje.aktualniUloha = (Uloha)lvi.DataContext;
            }

            if (HraCoSeHraje.DruhSpusteni == DruhSpusteni.Uceni)
            {
                if (!HraCoSeHraje.aktualniUloha.OtevrenyVysledek)
                {
                    tlacitkaMoznosti[HraCoSeHraje.aktualniUloha.SpravnyVysledek - 1].IsChecked = true;
                }
            }
        }

        RadioButton[] tlacitkaMoznosti;
        private void lbxSeznamUloh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void tbOdpoved_Loaded(object sender, RoutedEventArgs e)
        {
            if(HraCoSeHraje.DruhSpusteni == DruhSpusteni.Uceni)
            {
                ((TextBox)sender).Text = ((Par<string, string>)((TextBox)sender).GetAncestorOfType<ListViewItem>().DataContext).Hodnota;
            }
        }
    }

    /// <summary>
    /// Will return a*value + b
    /// </summary>
    public class FirstDegreeFunctionConverter : IValueConverter
    {
        public double Kolik { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double x = GetDoubleValue(value);
            Debug.WriteLine($"{x} --- {x / Kolik}");
            return x / Kolik - 0.2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double y = GetDoubleValue(value);
            return y * Kolik;
        }

        #endregion


        private double GetDoubleValue(object parameter)
        {
            double a;
            if (parameter != null)
                a = System.Convert.ToDouble(parameter);
            else
                throw new Exception("Nelze zpracovat hodnotu!");
            return a;
        }
    }
}
