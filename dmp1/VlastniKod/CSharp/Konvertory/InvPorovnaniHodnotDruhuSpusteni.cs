﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace dmp1
{
    public class InvPorovnaniHodnotDruhuSpusteni : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? true : !value.Equals(Enum.Parse(typeof(DruhSpusteni), (string)parameter, true));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
