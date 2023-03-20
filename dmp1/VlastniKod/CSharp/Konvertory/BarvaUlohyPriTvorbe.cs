using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace dmp1
{
    public class BarvaUlohyPriTvorbe : DependencyObject, IValueConverter
    {
        //public DruhSpusteni druhSpusteni { get; set; }

        public static readonly DependencyProperty druhSpusteniProperty = DependencyProperty.Register(
             "druhSpusteni", typeof(DruhSpusteni),
             typeof(BarvaUlohyPriTvorbe)
             );

        public DruhSpusteni druhSpusteni
        {
            get => (DruhSpusteni)GetValue(druhSpusteniProperty);
            set => SetValue(druhSpusteniProperty, value);
        }

        public static readonly DependencyProperty seznamDatProperty = DependencyProperty.Register(
             "seznamDat", typeof(ObservableCollection<string>),
             typeof(BarvaUlohyPriTvorbe)
             );

        public ObservableCollection<string> seznamDat
        {
            get => (ObservableCollection<string>)GetValue(seznamDatProperty);
            set => SetValue(seznamDatProperty, value);
        }

        public static readonly DependencyProperty seznamBooluProperty = DependencyProperty.Register(
             "seznamBoolu", typeof(ObservableCollection<bool>),
             typeof(BarvaUlohyPriTvorbe)
             );

        public ObservableCollection<bool> seznamBoolu
        {
            get => (ObservableCollection<bool>)GetValue(seznamBooluProperty);
            set => SetValue(seznamBooluProperty, value);
        }
        //public ObservableCollection<string> seznamDat { get; set; }
        //public ObservableCollection<bool> seznamBoolu { get; set; }

        public BarvaUlohyPriTvorbe()
        {
            druhSpusteni = DruhSpusteni.Test;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            if (druhSpusteni != DruhSpusteni.Test)
            {
                return Brushes.Red;
            }

            string nazev = (string)value;
            int index = seznamDat.IndexOf(nazev);

            if (!seznamBoolu[index])
            {
                return Brushes.Yellow;
            }

            return Brushes.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
