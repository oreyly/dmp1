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
                if (value.DruhSpusteniI == 1)
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
                }

                _HraCoSeHraje = value;
            }
        }

        //Nastavení základních hodnot
        public HraciPlocha()
        {
            InitializeComponent();
            DataContext = HraCoSeHraje;
            imgNapoveda.Source = Properties.Resources.icoNapoveda.ToImageSource();
            
            Graf gr = new Graf(cnvGRaf, this);
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
            HraCoSeHraje.aktualniUloha = (Uloha)((ListViewItem)sender).DataContext;
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
