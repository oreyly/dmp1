using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace dmp1
{
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
}
