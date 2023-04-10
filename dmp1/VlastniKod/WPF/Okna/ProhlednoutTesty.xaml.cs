using dmp1.Properties;
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
using System.Windows.Shapes;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro ProhlednoutTesty.xaml
    /// </summary>
    public partial class ProhlednoutTesty : Window
    {
        Label[] zahlavi;
        private ProhlednoutTesty()
        {
            InitializeComponent();
            lbAutor.Content = Uzivatel.Prava == UrovenPrav.Zak ? "Autor" : "Počet hráčů";
            lbTermin.Content = Uzivatel.Prava == UrovenPrav.Zak ? "Čas dokončení" : "Termín";
            lbNadpis.TextKZobrazeni = Title = Uzivatel.Prava == UrovenPrav.Zak ? "Výběr hry k prohlédnutí" : "Výběr hry ke zkontrolování";
            colAutor.Width = new GridLength(Uzivatel.Prava == UrovenPrav.Zak ? 200 : 50, GridUnitType.Star);
            zahlavi = new Label[] { lbNazev, lbUlohy, lbAutor, lbVytvoreni, lbTermin };
            DataContext = this;
            rb1.IsChecked = true;
        }

        private Window Rodic;
        public ProhlednoutTesty(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
        }

        public ObservableCollection<object[]> seznamHer { get; set; } = new ObservableCollection<object[]>();

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rb1 == null || rb2 == null)
            {
                return;
            }

            string[] data;
            if (Uzivatel.Prava == UrovenPrav.Zak)
            {
                data = (string[])PraceSDB.ZavolejPrikaz("nacti_seznam_vysledku_zak", true, Uzivatel.Id, (byte)((bool)rb1.IsChecked ? 2 : 4))[0][0];
            }
            else
            {
                data = (string[])PraceSDB.ZavolejPrikaz("nacti_seznam_vysledku_ucitel", true, Uzivatel.Id, (byte)((bool)rb1.IsChecked ? 2 : 4))[0][0];
            }

            seznamHer.NastavHodnoty(data.Select(dato => dato.Split(HlavniStatik.Oddelovac, StringSplitOptions.None)).Select(skupinaDat => new object[] { skupinaDat[0], skupinaDat[1], skupinaDat[2], skupinaDat[3], skupinaDat[4], Convert.ToInt32(skupinaDat[5]) }));
        }

        private void btSpustit_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((object[])((Button)sender).GetAncestorOfType<ListViewItem>().Content)[5];

            if(Uzivatel.Prava == UrovenPrav.Ucitel)
            {
                new ProhlednoutTestyLidi(id, this).Show();
            }
            else if(Uzivatel.Prava == UrovenPrav.Zak)
            {
                new VysledkoveOkno(id, Uzivatel.Jmeno.ZiskejZavorku(), this).Show();
            }

            Hide();
        }
        private void lbNazev_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (zahlavi.FirstOrDefault(l => l != lbNazev && (l.FontStyle == FontStyles.Italic || l.FontStyle == FontStyles.Oblique)) is Label lb)
            {
                lb.FontStyle = FontStyles.Normal;
            }

            if (lbNazev.FontStyle == FontStyles.Italic)
            {
                seznamHer.NastavHodnoty(seznamHer.OrderByDescending(h => h[0]).ToArray());
                lbNazev.FontStyle = FontStyles.Oblique;
            }
            else
            {
                seznamHer.NastavHodnoty(seznamHer.OrderBy(h => h[0]).ToArray());
                lbNazev.FontStyle = FontStyles.Italic;
            }
        }

        private void lbUlohy_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (zahlavi.FirstOrDefault(l => l != lbUlohy && (l.FontStyle == FontStyles.Italic || l.FontStyle == FontStyles.Oblique)) is Label lb)
            {
                lb.FontStyle = FontStyles.Normal;
            }

            if (lbUlohy.FontStyle == FontStyles.Italic)
            {
                seznamHer.NastavHodnoty(seznamHer.OrderByDescending(h => Convert.ToInt32(h[1])).ToArray());
                lbUlohy.FontStyle = FontStyles.Oblique;
            }
            else
            {
                seznamHer.NastavHodnoty(seznamHer.OrderBy(h => Convert.ToInt32(h[1])).ToArray());
                lbUlohy.FontStyle = FontStyles.Italic;
            }
        }

        private void lbAutor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (zahlavi.FirstOrDefault(l => l != lbAutor && (l.FontStyle == FontStyles.Italic || l.FontStyle == FontStyles.Oblique)) is Label lb)
            {
                lb.FontStyle = FontStyles.Normal;
            }

            if (lbAutor.FontStyle == FontStyles.Italic)
            {
                seznamHer.NastavHodnoty(seznamHer.OrderByDescending(h => Uzivatel.Prava == UrovenPrav.Zak ? (object)((string)h[2]).ZiskejZavorku() : h[2]).ToArray());
                lbAutor.FontStyle = FontStyles.Oblique;
            }
            else
            {
                seznamHer.NastavHodnoty(seznamHer.OrderBy(h => Uzivatel.Prava == UrovenPrav.Zak ? (object)((string)h[2]).ZiskejZavorku() : h[2]).ToArray());
                lbAutor.FontStyle = FontStyles.Italic;
            }
        }

        private void lbVytvoreni_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (zahlavi.FirstOrDefault(l => l != lbVytvoreni && (l.FontStyle == FontStyles.Italic || l.FontStyle == FontStyles.Oblique)) is Label lb)
            {
                lb.FontStyle = FontStyles.Normal;
            }

            if (lbVytvoreni.FontStyle == FontStyles.Italic)
            {
                seznamHer.NastavHodnoty(seznamHer.OrderByDescending(h => DateTime.Parse((string)h[3])).ToArray());
                lbVytvoreni.FontStyle = FontStyles.Oblique;
            }
            else
            {
                seznamHer.NastavHodnoty(seznamHer.OrderBy(h => DateTime.Parse((string)h[3])).ToArray());
                lbVytvoreni.FontStyle = FontStyles.Italic;
            }
        }

        private void lbTermin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (zahlavi.FirstOrDefault(l => l != lbTermin && (l.FontStyle == FontStyles.Italic || l.FontStyle == FontStyles.Oblique)) is Label lb)
            {
                lb.FontStyle = FontStyles.Normal;
            }

            if (lbTermin.FontStyle == FontStyles.Italic)
            {
                seznamHer.NastavHodnoty(seznamHer.OrderByDescending(h => DateTime.Parse((string)h[4])).ToArray());
                lbTermin.FontStyle = FontStyles.Oblique;
            }
            else
            {
                seznamHer.NastavHodnoty(seznamHer.OrderBy(h => DateTime.Parse((string)h[4])).ToArray());
                lbTermin.FontStyle = FontStyles.Italic;
            }
        }
    }
}
