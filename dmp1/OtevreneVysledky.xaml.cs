using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro OtevreneVysledky.xaml
    /// </summary>
    public partial class OtevreneVysledky : UserControl
    {
        // O$$$Ahoj$$$Jak se máš$$$Já se mám dobře$$$A co ty
        public ObservableCollection<Par<string, string>> VysledkyData { get; set; }//  = new Par[] { new Par("Zdarec", "Párec") };

        public static readonly DependencyProperty VysledkyProperty = DependencyProperty.Register(
            "Vysledky", typeof(string),
            typeof(OtevreneVysledky),
            new PropertyMetadata(OnCustomerChangedCallBack)
            );

        private static void OnCustomerChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OtevreneVysledky c = sender as OtevreneVysledky;
            if (c != null)
            {
                c.OnCustomerChanged();
            }
        }

        protected virtual void OnCustomerChanged()
        {
            string[] data = ((string)GetValue(VysledkyProperty)).Split(new string[] { "$$$" }, StringSplitOptions.None);
            if (data[0] != "O")
            {
                throw new Exception("Výsledek není ve formátu pro otevřené možností");
            }

            ObservableCollection<Par<string, string>> odpovedi = new ObservableCollection<Par<string, string>>();
            for (int i = 1; i < data.Length-1; i += 2)
            {
                odpovedi.Add(new Par<string, string>(data[i], data[i + 1]));
            }
            VysledkyData = odpovedi;
        }

        public string Vysledky
        {
            get => (string)GetValue(VysledkyProperty);
            set => SetValue(VysledkyProperty, value);
        }

        public OtevreneVysledky()
        {
            InitializeComponent();
        }
    }
    public class StarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListView listview = value as ListView;
            double width = listview.ActualWidth;
            GridView gv = listview.View as GridView;
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (!double.IsNaN(gv.Columns[i].ActualWidth))
                    width -= gv.Columns[i].ActualWidth;
            }
            return width - 5;// this is to take care of margin/padding
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
