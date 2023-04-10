using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro seznamSTlacitky.xaml
    /// </summary>

    public partial class seznamSTlacitky : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty stylProperty = DependencyProperty.Register(
             "styl", typeof(Style),
             typeof(seznamSTlacitky),
             new PropertyMetadata(ZmenaStylu)
             );

        public Style styl
        {
            get => (Style)GetValue(stylProperty);
            set => SetValue(stylProperty, value);
        }

        public ObservableCollection<string> Seznam { get; set; } = new ObservableCollection<string>();

        public static readonly DependencyProperty druhTlacitkaProperty = DependencyProperty.Register(
             "druhTlacitka", typeof(druhTlacitkaVSeznamu),
             typeof(seznamSTlacitky),
             new PropertyMetadata(ZmenaStylu)
             );

        public druhTlacitkaVSeznamu druhTlacitka
        {
            get => (druhTlacitkaVSeznamu)GetValue(druhTlacitkaProperty);
            set => SetValue(druhTlacitkaProperty, value);
        }

        public static readonly DependencyProperty zapnutoProperty = DependencyProperty.Register(
             "Zapnuto", typeof(bool),
             typeof(seznamSTlacitky),
             new PropertyMetadata(ZmenaStylu)
             );

        public bool Zapnuto
        {
            get => (bool)GetValue(zapnutoProperty);
            set => SetValue(zapnutoProperty, value);
        }

        private static void ZmenaStylu(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            seznamSTlacitky c = sender as seznamSTlacitky;
            if (c != null)
            {
                c.OnCustomerChanged();
            }
        }

        protected virtual void OnCustomerChanged()
        {
            //MessageBox.Show("More");
            switch ((druhTlacitkaVSeznamu)GetValue(druhTlacitkaProperty))
            {
                case druhTlacitkaVSeznamu.Pridat:
                    znakTlacitka = "+";
                    break;
                case druhTlacitkaVSeznamu.Smazat:
                    znakTlacitka = "X";
                    break;
                default:
                    znakTlacitka = "?";
                    break;
            }

            OnPropertyChanged("znakTlacitka");
            OnPropertyChanged("barvaTlacitka");
            OnPropertyChanged("Zapnuto");
        }


        public string znakTlacitka { get; set; }
        public Brush barvaTlacitka
        {
            get
            {
                if (znakTlacitka == "X")
                {
                    return Brushes.Red;
                }

                if (znakTlacitka == "+")
                {
                    return Brushes.Green;
                }

                return Brushes.Gray;
            }
        }

        public seznamSTlacitky()
        {
            styl = new Style();
            styl.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));
            InitializeComponent();
            DataContext = this;
            //druhTlacitka = druhTlacitka;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //MessageBox.Show(sender.ToString());
        }


        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        public string VybranyRadek
        {
            get
            {
                return (string)seznamOtazek.SelectedItem;
            }
        }

        private void seznamOtazek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("VybranyRadek");
        }

        private void ListViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
