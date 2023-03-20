using System;
using System.Collections.Generic;
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
    /// Interakční logika pro ObrazekVKrouzku.xaml
    /// </summary>
    public partial class ObrazekVKrouzku : UserControl
    {
        public static readonly DependencyProperty ZdrojProperty = DependencyProperty.Register(
        "Zdroj", typeof(string),
        typeof(ObrazekVKrouzku),
        new PropertyMetadata(OnCustomerChangedCallBack)
        );

        public string Zdroj
        {
            get => (string)GetValue(ZdrojProperty);
            set => SetValue(ZdrojProperty, value);
        }

        private static void OnCustomerChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ObrazekVKrouzku c = sender as ObrazekVKrouzku;
            if (c != null)
            {
                c.OnCustomerChanged();
            }
        }

        protected virtual void OnCustomerChanged()
        {

        }

        public static readonly DependencyProperty ZaobleniProperty = DependencyProperty.Register(
        "Zaobleni", typeof(int),
        typeof(ObrazekVKrouzku),
        new PropertyMetadata(90)
        );

        public int Zaobleni
        {
            get => (int)GetValue(ZaobleniProperty);
            set => SetValue(ZaobleniProperty, value);
        }

        public ObrazekVKrouzku()
        {
            InitializeComponent();
            wpObr.DataContext = this;
        }
    }
}
