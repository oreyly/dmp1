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
    /// Interakční logika pro OknoVObchode.xaml
    /// </summary>
    public partial class OknoVObchode : UserControl
    {
        public static readonly DependencyProperty ProduktKZobrazeniProperty = DependencyProperty.Register(
        "ProduktKZobrazeni", typeof(Produkt),
        typeof(OknoVObchode),
        new PropertyMetadata(OnCustomerChangedCallBack)
        );

        public Produkt ProduktKZobrazeni
        {
            get => (Produkt)GetValue(ProduktKZobrazeniProperty);
            set => SetValue(ProduktKZobrazeniProperty, value);
        }

        private static void OnCustomerChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OknoVObchode c = sender as OknoVObchode;
            if (c != null)
            {
                c.OnCustomerChanged((Produkt)e.NewValue);
            }
        }

        protected virtual void OnCustomerChanged(Produkt novyProdukt)
        {
            DataContext = novyProdukt;
        }

        public OknoVObchode()
        {
            InitializeComponent();
            //DataContext = ProduktKZobrazeni;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.WidthChanged)
            {
                Height = e.NewSize.Width;
            }
            /*else if(e.HeightChanged)
            {
                Width = e.NewSize.Height;
            }*/
            /*
            if (e.HeightChanged && ActualHeight != ActualWidth)
            {
                Width = ActualHeight;
            }
            else if(e.WidthChanged && ActualHeight != ActualWidth)
            {
                Height = ActualWidth;
            }*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProduktKZobrazeni.Koupit();
        }
    }
}
