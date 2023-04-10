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
    /// Interakční logika pro UkladaniVysledku.xaml
    /// </summary>
    //Uživatelský prvek v němž se zobrazí buď seznam políček pro odpověď, nebo možnosti ABCD
    public partial class UkladaniVysledku : UserControl
    {
        public static readonly DependencyProperty VysledkyProperty = DependencyProperty.Register(
            "Vysledky", typeof(string),
            typeof(UkladaniVysledku),
            new PropertyMetadata(ZmenaVysledku)
            );

        private static void ZmenaVysledku(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UkladaniVysledku c = sender as UkladaniVysledku;
            if (c != null)
            {
                c.OnCustomerChanged();
            }
        }

        //Vytvoření ABCD nebo doplňovacího okna
        protected virtual void OnCustomerChanged()
        {
            string[] data = ((string)GetValue(VysledkyProperty)).RozdelDolary();
            if (data[0] == "O")
            {
                grObsah.Children.Clear();
                grObsah.Children.Add(new OtevreneVysledky() { Vysledky = Vysledky });
            }
            else if(data[0] == "M")
            {
                grObsah.Children.Clear();
                grObsah.Children.Add(new NabidkaMoznosti() { Vysledky = Vysledky });
            }
            else
            {
                throw new Exception("Výsledek není ve formátu pro otevřené možností");
            }
        }

        public string Vysledky
        {
            get => (string)GetValue(VysledkyProperty);
            set => SetValue(VysledkyProperty, value);
        }

        public UkladaniVysledku()
        {
            InitializeComponent();
        }
    }
}