using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace dmp1
{
    [ValueConversion(typeof(UrovenPrav), typeof(Visibility), ParameterType = typeof(UrovenPrav))]
    public class PravaToVisibility : IValueConverter
    {
        public Visibility TrueValue = Visibility.Visible;
        public Visibility FalseValue = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (UrovenPrav)value == (UrovenPrav)parameter ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
