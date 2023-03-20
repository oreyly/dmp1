using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
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
using Npgsql;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //phdrKKWs5xNOfcIm
        //http://home.spsostrov.cz/~matema/piejcpi/formik/gulas.jpg
        public MainWindow()
        {
            InitializeComponent();/*
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri("http://home.spsostrov.cz/~matema/piejcpi/formik/gulas.jpg"), @"image.jpg");
            }*/
            this.Title = WindowsIdentity.GetCurrent().Name;
        }

        private void Viewbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.BorderBrush != Brushes.Black)
            {
                b.BorderBrush = Brushes.Black;
                b.BorderThickness = new Thickness(1);
            }

            ((Viewbox)((Border)sender).Child).Child.Focus();
        }

        private void PrihlasSe(object sender, RoutedEventArgs e)
        {
            bdrJmeno.BorderBrush = Brushes.Black;
            object h = PraceSDB.ZavolejPrikaz("prihlasHrace", true, tbJmeno.Text)[0][0];
            if (h is not DBNull)
            {
                if (BCrypt.Net.BCrypt.EnhancedVerify(tbHeslo.Password, Encoding.UTF8.GetString((byte[])h))) //Ověří heslo
                {
                    MessageBox.Show("Přihlášeno!");
                }
                else
                {
                    MessageBox.Show("Špatné heslo!");
                    bdrHeslo.BorderBrush = Brushes.Red;
                    bdrHeslo.BorderThickness = new Thickness(2);
                }
            }
            else
            {
                MessageBox.Show("Uživatelské jméno neexistuje!");
                bdrJmeno.BorderBrush = Brushes.Red;
                bdrJmeno.BorderThickness = new Thickness(2);
            }
        }
    }
}
