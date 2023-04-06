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
    /// Interakční logika pro Nahled.xaml
    /// </summary>
    public partial class Nahled2 : UserControl
    {
        public static readonly DependencyProperty VybranaUlohaProperty = DependencyProperty.Register(
        "VybranaUloha", typeof(Uloha),
        typeof(Nahled2),
        new PropertyMetadata(OnCustomerChangedCallBack)
        );

        public Uloha VybranaUloha
        {
            get => (Uloha)GetValue(VybranaUlohaProperty);
            set => SetValue(VybranaUlohaProperty, value);
        }

        private static void OnCustomerChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Nahled2 c = sender as Nahled2;
            if (c != null)
            {
                c.OnCustomerChanged((Uloha)e.NewValue);
            }
        }

        protected virtual void OnCustomerChanged(Uloha u)
        {
            DataContext = u;
        }

        public Nahled2()
        {
            InitializeComponent();
        }
    }
}
