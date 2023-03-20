using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace dmp1
{
    public class InvDruhToVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null ? false : value.Equals(Enum.Parse(typeof(DruhSpusteni), (string)parameter, true))) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
