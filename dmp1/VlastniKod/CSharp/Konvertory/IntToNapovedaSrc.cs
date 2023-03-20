using System;
using System.Globalization;
using System.Windows.Data;

namespace dmp1
{
    public class IntToNapovedaSrc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((string)parameter)
            {
                case "napoveda":
                    return Properties.Resources.icoNapoveda.ToImageSource();
                case "graf":
                    return Properties.Resources.icoNapoveda.ToImageSource();
                case "50":
                    return Properties.Resources.icoNapoveda.ToImageSource();
            }

            return value == null ? true : !value.Equals(Enum.Parse(typeof(DruhSpusteni), (string)parameter, true));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
