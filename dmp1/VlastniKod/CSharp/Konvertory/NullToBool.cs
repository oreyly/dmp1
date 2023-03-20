using System;
using System.Globalization;
using System.Windows.Data;

namespace dmp1
{
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
}
