using System;
using System.Globalization;
using System.Windows.Data;

namespace dmp1
{
    public class RozdeleniVysek : IValueConverter
    {
        public double Kolik { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double x = GetDoubleValue(value);
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
