using System;
using System.Globalization;
using System.Windows.Data;

namespace dmp1
{
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
}
