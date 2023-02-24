using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace dmp1
{
    //Konvertory užívané ve WPF

    //Převedení stringu na obrázek
    public class FuncToNcalc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            string funkce = (string)value;

            if (string.IsNullOrWhiteSpace(funkce)||true)
            {
                return "";
            }

            funkce = funkce.ToLower();
            funkce = funkce.Replace("[", "(");
            funkce = funkce.Replace("]", ")");
            funkce = funkce.Replace("{", "(");
            funkce = funkce.Replace("}", ")");
            funkce = Regex.Replace(funkce, @"[^0-9+\-*/^()x]", "");

            for (int i = 0; i < funkce.Length; ++i)
            {
                if (funkce[i] == '^')
                {
                    string pred = funkce.HledejZavorky(i - 1, false);
                    string za = funkce.HledejZavorky(i + 1, true);
                    funkce = funkce.Replace(pred + '^' + za, $"{{{pred},{za}}}");
                    i = -1;
                }
            }

            funkce = funkce.Replace("{", "Pow(");
            funkce = funkce.Replace("}", ")");

            return funkce;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Y = " + value;
        }
    }

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
            if (string.IsNullOrWhiteSpace(cesta))
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
            FalseValue = Visibility.Collapsed;
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
