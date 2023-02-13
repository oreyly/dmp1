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
        public static readonly DependencyProperty VybranyIndexProperty = DependencyProperty.Register(
        "VybranyItem", typeof(object),
        typeof(lepsiComboBox)
        );

        public object VybranyItem
        {
            get => (object)GetValue(VybranyIndexProperty);
            set => SetValue(VybranyIndexProperty, value);
        }

        public ObservableCollection<object> Seznam { get; set; } = new ObservableCollection<object>();
        public string PrazdnyText { get; set; } //Text zobrazený v neurčitém stavu


        public lepsiComboBox()
        {
            InitializeComponent();
        }

        private object stary; //Poslední vybraný Item
        public event ZmenaVyberu ZmenilVyber;
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbkVyber.Visibility = comboBox1.SelectedItem == null ? Visibility.Visible : Visibility.Hidden;
            ZmenilVyber?.Invoke(stary, comboBox1.SelectedItem);
            stary = comboBox1.SelectedItem;
        }
    }
}
