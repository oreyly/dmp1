using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

public delegate void ZmenaVyberu(object staryObjekt, object novyObjekt);
namespace dmp1
{
    /// <summary>
    /// Interakční logika pro lepsiComboBox.xaml
    /// </summary>
    //ComboBox jenž má neutrální stav s popiskem
    public partial class lepsiComboBox : UserControl
    {
        public static readonly DependencyProperty VybranyItemProperty = DependencyProperty.Register(
        "VybranyItem", typeof(object),
        typeof(lepsiComboBox),
        new PropertyMetadata(OnCustomerChangedCallBack)
        );

        public object VybranyItem
        {
            get => (object)GetValue(VybranyItemProperty);
            set => SetValue(VybranyItemProperty, value);
        }

        private static void OnCustomerChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            lepsiComboBox c = sender as lepsiComboBox;
            if (c != null)
            {
                c.OnCustomerChanged();
            }
        }

        protected virtual void OnCustomerChanged()
        {
            comboBox1.SelectedItem = GetValue(VybranyItemProperty);
        }


        public static readonly DependencyProperty VybranaVlastnostProperty = DependencyProperty.Register(
        "VybranaVlastnost", typeof(string),
        typeof(lepsiComboBox)
        );

        public string VybranaVlastnost
        {
            get => (string)GetValue(VybranaVlastnostProperty);
            set => SetValue(VybranaVlastnostProperty, value);
        }

        public static readonly DependencyProperty SeznamProperty = DependencyProperty.Register(
        "Seznam", typeof(ObservableCollection<object>),
        typeof(lepsiComboBox)
        );

        public ObservableCollection<object> Seznam
        {
            get => (ObservableCollection<object>)GetValue(SeznamProperty);
            set => SetValue(SeznamProperty, value);
        }

        //public ObservableCollection<object> Seznam { get; set; } = new ObservableCollection<object>();
        public string PrazdnyText { get; set; } //Text zobrazený v neurčitém stavu


        public lepsiComboBox()
        {
            Seznam = new ObservableCollection<object>();
            InitializeComponent();
            comboBox1.DataContext = this;
            //DataContext = this;
        }

        private object stary; //Poslední vybraný Item
        public event ZmenaVyberu ZmenilVyber;
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox1.InvalidateVisual();
            comboBox1.InvalidateArrange();
            comboBox1.InvalidateMeasure();
            comboBox1.UpdateLayout();
            comboBox1.UpdateDefaultStyle();
            tbkVyber.Visibility = comboBox1.SelectedItem == null ? Visibility.Visible : Visibility.Hidden;
            ZmenilVyber?.Invoke(stary, comboBox1.SelectedItem);
            stary = comboBox1.SelectedItem;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
