using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

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
}
