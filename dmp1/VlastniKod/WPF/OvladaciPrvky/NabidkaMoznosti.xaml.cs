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
    /// Interakční logika pro NabidkaMoznosti.xaml
    /// </summary>
    public partial class NabidkaMoznosti : UserControl
    {
        // M$$$Ahoj$$$Jak se máš$$$Já se mám dobře$$$A co ty$$$2
        private string[] _VysledkyData;
        public string[] VysledkyData
        {
            get
            {
                return _VysledkyData;
            }

            private set
            {
                _VysledkyData = value;

                btA.TextKZobrazeni = _VysledkyData[1];
                btB.TextKZobrazeni = _VysledkyData[2];
                btC.TextKZobrazeni = _VysledkyData[3];
                btD.TextKZobrazeni = _VysledkyData[4];
            }
        }

        public static readonly DependencyProperty VysledkyProperty = DependencyProperty.Register(
            "Vysledky", typeof(string),
            typeof(NabidkaMoznosti),
            new PropertyMetadata(ZmenaVysledku)
            );

        private static void ZmenaVysledku(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NabidkaMoznosti c = sender as NabidkaMoznosti;
            if (c != null)
            {
                c.OnCustomerChanged();
            }
        }

        protected virtual void OnCustomerChanged()
        {
            string[] data = ((string)GetValue(VysledkyProperty)).RozdelDolary();
            if (data[0] != "M")
            {
                throw new Exception("Výsledek není ve formátu pro výběr 4 možností");
            }

            VysledkyData = data;
        }
        public string Vysledky
        {
            get => (string)GetValue(VysledkyProperty);
            set => SetValue(VysledkyProperty, value);
        }

        public NabidkaMoznosti()
        {
            InitializeComponent();
        }
    }
}
