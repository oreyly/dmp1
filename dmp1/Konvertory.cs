using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace dmp1
{
    //Konvertory užívané ve WPF

    //Porovnání parametru s jeho hodnotou
    public class RadioBoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            int integer = (int)value;
            if (integer == int.Parse(parametr.ToString()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    //Převedení stringu na obrázek
    public class StringToSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            string cesta = (string)value;
            if (cesta == null)
            {
                return null;
            }
            Uri u = new Uri(cesta, UriKind.Absolute);
            return new BitmapImage(u);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    //Převeod null hodnoty na false
    public class NullToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    //Výpočet jednoduchých příkladů ve WPF
    public class Calculate : MarkupExtension
    {
        string Exp;

        public Calculate()
        {

        }

        public Calculate(string exp)
        {
            Exp = exp;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Convert.ToDouble(new DataTable().Compute(Exp, "").ToString());
        }
    }

    //Převedení boolové hodnoty na viditelnost
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            FalseValue = Visibility.Hidden;
            TrueValue = Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
