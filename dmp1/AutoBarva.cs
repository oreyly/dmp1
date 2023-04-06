using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace dmp1
{
    [ValueConversion(typeof(Brush),typeof(Brush))]
    public class AutoBarva : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c = ((SolidColorBrush)value).Color;
            int cernota = (int)Math.Sqrt(
              c.R * c.R * .241 +
              c.G * c.G * .691 +
              c.B * c.B * .068);
            if (cernota < 130)
            {
                return Brushes.White;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
