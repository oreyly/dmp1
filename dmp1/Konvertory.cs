using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace dmp1
{
    //Konvertory užívané ve WPF

    //Převedení stringu na obrázek
    public class FuncToNcalc : IValueConverter
    {
        public object Convert(object value, Type targetType, object parametr, CultureInfo culture)
        {
            string funkce = (string)value;

            if (string.IsNullOrWhiteSpace(funkce)||true)
            {
                return "";
            }

            funkce = funkce.ToLower();
            funkce = funkce.Replace("[", "(");
            funkce = funkce.Replace("]", ")");
            funkce = funkce.Replace("{", "(");
            funkce = funkce.Replace("}", ")");
            funkce = Regex.Replace(funkce, @"[^0-9+\-*/^()x]", "");

            for (int i = 0; i < funkce.Length; ++i)
            {
                if (funkce[i] == '^')
                {
                    string pred = funkce.HledejZavorky(i - 1, false);
                    string za = funkce.HledejZavorky(i + 1, true);
                    funkce = funkce.Replace(pred + '^' + za, $"{{{pred},{za}}}");
                    i = -1;
                }
            }

            funkce = funkce.Replace("{", "Pow(");
            funkce = funkce.Replace("}", ")");

            return funkce;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Y = " + value;
        }
    }

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

    //Výpočet jednoduchých příkladů ve WPF
    public class Calculate : MarkupExtension
    {
        string Exp;

        public Calculate()
        {

        }

        public Calculate(string exp)
        {
            Exp = exp;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Convert.ToDouble(new DataTable().Compute(Exp, "").ToString());
        }
    }

    //Převedení boolové hodnoty na viditelnost
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            FalseValue = Visibility.Collapsed;
            TrueValue = Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PorovnaniHodnotDruhuSpusteni : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : value.Equals(Enum.Parse(typeof(DruhSpusteni), (string)parameter, true));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

    public class BarvaUlohyPriTvorbe: DependencyObject, IValueConverter
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

    public class DruhToVis : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null ? false : value.Equals(Enum.Parse(typeof(DruhSpusteni), (string)parameter, true))) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
    public class FirstDegreeFunctionConverter : IValueConverter
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
