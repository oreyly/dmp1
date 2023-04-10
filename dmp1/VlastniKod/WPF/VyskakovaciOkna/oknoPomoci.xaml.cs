using Newtonsoft.Json.Linq;
using PostSharp.Patterns.Diagnostics;
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
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro oknoPomoci.xaml
    /// </summary>
    public partial class oknoPomoci : Window
    {
        readonly List<Brush> stetce = new List<Brush>() { Brushes.Aqua, Brushes.DarkMagenta, Brushes.Gold, Brushes.Lime, Brushes.Red, Brushes.Orange, Brushes.Blue };
        List<Par<string, int>> hodnotyGrafu;

        public oknoPomoci()
        {
            InitializeComponent();

            while (stetce.Count > 4)
            {
                stetce.RemoveAt(HlavniStatik.rnd.Next(stetce.Count));
            }
            stetce.ZamichejList();

            imgPomoc1.Source = Properties.Resources.icoNapoveda.ToImageSource();
            imgPomoc2.Source = Properties.Resources.icoNapoveda.ToImageSource();
            imgPomoc3.Source = Properties.Resources.icoNapoveda.ToImageSource();
        }

        Hra hra { get; set; }
        HraciPlocha okno;

        public oknoPomoci(Hra h, HraciPlocha w)
        {
            InitializeComponent();
            hra = h;
            okno = w;
            DataContext = hra;
        }

        private void NakresliGraf()
        {
            double vyskaPopisku = 22;
            double sirkaSloupce = cnvGraf.ActualWidth * 2 / (hodnotyGrafu.Count * 3 - 1);
            double sirkaSDirou = cnvGraf.ActualWidth * 3 / (hodnotyGrafu.Count * 3 - 1);
            double vyskaSloupce = cnvGraf.ActualHeight - vyskaPopisku * 2;
            int nejvyssi = hodnotyGrafu.Max(p => p.Hodnota);

            cnvGraf.Children.Clear();
            for (int i = 0; i < hodnotyGrafu.Count; ++i)
            {
                Rectangle sloupec = new Rectangle()
                {
                    Fill = stetce[i],
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
                    Padding = new Thickness(0, 0, 0, 3)
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
                    Padding = new Thickness(0, 0, 0, 3)
                };

                cnvGraf.Children.Add(popisekDole);
                Canvas.SetLeft(popisekDole, i * sirkaSDirou);
                Canvas.SetBottom(popisekDole, 0);
            }
        }

        private int[] Rozsah(int[] cisla, float[] pravdepodobnosti, int pocet)
        {
            int[] data = Data(pravdepodobnosti, pocet);
            int[] pocty = new int[cisla.Length];
            for (int i = 0; i < cisla.Length; ++i)
            {
                pocty[i] = data.Count(dato => dato == cisla[i]);
            }

            return pocty;
        }

        private int[] Data(float[] pravdepodobnosti, int pocet)
        {
            int[] data = new int[pocet];

            for (int i = 0; i < pocet; ++i)
            {
                data[i] = Cislo(pravdepodobnosti);
            }

            return data;
        }

        private int Cislo(float[] pravdepodobnosti)
        {
            float u = (float)HlavniStatik.rnd.NextDouble();

            if (u < pravdepodobnosti[0])
            {
                return 1;
            }

            for (int i = 1; i < pravdepodobnosti.Length + 1; ++i)
            {
                if (pravdepodobnosti.Take(i).Sum() < u && pravdepodobnosti.Take(i + 1).Sum() > u)
                {
                    return i + 1;
                }
            }

            throw new Exception("Chyba algoritmu");
        }

        private void imgPomoc1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (hra.DruhSpusteni == DruhSpusteni.Uceni)
            {
                lbPomoc.Text = hra.aktualniUloha.Napoveda;
                grPomoc.Visibility = Visibility.Visible;
                scwPomoc.Visibility = Visibility.Visible;
                return;
            }

            imgPomoc1.Source = Properties.Resources.icoNapovedaNe.ToImageSource();
            imgPomoc1.IsEnabled = false;

            int[] moznosti = new int[2];
            moznosti[0] = hra.aktualniUloha.SpravnyVysledek;

            do
            {
                moznosti[1] = HlavniStatik.rnd.Next(1, 5);
            } while (moznosti[0] == moznosti[1]);

            if (!moznosti.Contains(1))
            {
                okno.btA.Visibility = Visibility.Collapsed;
            }
            if (!moznosti.Contains(2))
            {
                okno.btB.Visibility = Visibility.Collapsed;
            }
            if (!moznosti.Contains(3))
            {
                okno.btC.Visibility = Visibility.Collapsed;
            }
            if (!moznosti.Contains(4))
            {
                okno.btD.Visibility = Visibility.Collapsed;
            }
        }

        private void imgPomoc2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgPomoc2.Source = Properties.Resources.icoNapovedaNe.ToImageSource();
            imgPomoc2.IsEnabled = false;
            grPomoc.Visibility = Visibility.Visible;
            scwPomoc.Visibility = Visibility.Visible;

            lbPomoc.Text = hra.aktualniUloha.Napoveda;
        }

        private void imgPomoc3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgPomoc3.Source = Properties.Resources.icoNapovedaNe.ToImageSource();
            imgPomoc3.IsEnabled = false;
            grPomoc.Visibility = Visibility.Visible;
            cnvGraf.Visibility = Visibility.Visible;

            float sance = hra.Ulohy.IndexOf(hra.aktualniUloha) / ((float)hra.Ulohy.Count - 1) * (0.25f - 0.6f) + 0.6f;
            float[] jednotliveSance = new float[4];

            for (int i = 0; i < 4; ++i)
            {
                jednotliveSance[i] = (i + 1) == hra.aktualniUloha.SpravnyVysledek ? sance : (1 - sance) / 3;
            }

            int[] hlasy = Rozsah(new int[] { 1, 2, 3, 4 }, jednotliveSance, 50);
            hodnotyGrafu = hlasy.Zip(new string[] { "A", "B", "C", "D" }, (i, s) => new Par<string, int>(s, i)).ToList();

            NakresliGraf();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            grPomoc.Visibility = Visibility.Hidden;
            scwPomoc.Visibility = Visibility.Hidden;
            cnvGraf.Visibility = Visibility.Hidden;
        }
    }
}
