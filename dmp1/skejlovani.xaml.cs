using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro skejlovani.xaml
    /// </summary>
    public partial class skejlovani : Window
    {
        Random rnd = new Random();
        public skejlovani()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        List<Par<string, int>> hodnotyGrafu = new List<Par<string, int>>()
        {
            new Par<string, int>("Ahoj", 67),
            new Par<string, int>("Ahoj", 38),
            new Par<string, int>("Ahoj", 12),
            new Par<string, int>("Ahoj", 49),
        };

        readonly List<Brush> stetce = new List<Brush>() { Brushes.Aqua, Brushes.DarkMagenta, Brushes.Gold, Brushes.Lime, Brushes.Red, Brushes.Orange, Brushes.Blue };

        public void NakresliGraf()
        {
            double vyskaPopisku = 22;
            double sirkaSloupce = cnvGraf.ActualWidth * 2 / (hodnotyGrafu.Count * 3 - 1);
            double sirkaSDirou = cnvGraf.ActualWidth * 3 / (hodnotyGrafu.Count * 3 - 1);
            double vyskaSloupce = cnvGraf.ActualHeight - vyskaPopisku * 2;
            int nejvyssi = hodnotyGrafu.Max(p => p.Hodnota);

            cnvGraf.Children.Clear();
            List<Brush> pouziteStetce = new List<Brush>();
            for (int i = 0; i < hodnotyGrafu.Count; ++i)
            {
                Brush stetec;
                do
                {
                    stetec = stetce[rnd.Next(stetce.Count)];
                } while (pouziteStetce.Contains(stetec));

                pouziteStetce.Add(stetec);

                Rectangle sloupec = new Rectangle()
                {
                    Fill = stetec,
                    Width = sirkaSloupce,
                    Height = vyskaSloupce * hodnotyGrafu[i].Hodnota / nejvyssi
                };

                cnvGraf.Children.Add(sloupec);
                Canvas.SetLeft(sloupec, i * sirkaSDirou);
                Canvas.SetBottom(sloupec, vyskaPopisku);

                Label popisekHore = new Label()
                {
                    Content = hodnotyGrafu[i].Hodnota,
                    Width = sirkaSloupce,
                    Height = vyskaPopisku,
                    FontSize = 16,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Bottom,
                    Padding = new Thickness(0, 0, 0, 0),
                };

                cnvGraf.Children.Add(popisekHore);
                Canvas.SetLeft(popisekHore, i * sirkaSDirou);
                Canvas.SetTop(popisekHore, vyskaSloupce - vyskaSloupce * hodnotyGrafu[i].Hodnota / nejvyssi);

                Label popisekDole = new Label()
                {
                    Content = hodnotyGrafu[i].Klic,
                    Width = sirkaSloupce,
                    Height = vyskaPopisku,
                    FontSize = 16,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(0, 0, 0, 0),
                };

                cnvGraf.Children.Add(popisekDole);
                Canvas.SetLeft(popisekDole, i * sirkaSDirou);
                Canvas.SetBottom(popisekDole, 0);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cnvGraf_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NakresliGraf();
        }

        Window W;
        public skejlovani(Window w) : this()
        {
            W = w;
            Closed += delegate (object sender, EventArgs e) { W?.Show(); };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new skejlovani(this).Show();
            Hide();
        }
    }
}
