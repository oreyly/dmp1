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
using dmp1;
using Npgsql;
using Xceed.Wpf.AvalonDock.Controls;

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
            InitializeComponent();
            
            NabidniAutoPrihlaseni();

            tbJmeno.Focus();
        }

        private string AutoUzivatel;

        private void NabidniAutoPrihlaseni()
        {
            AutoUzivatel = "matema2";//WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            if ((bool)PraceSDB.ZavolejPrikaz("existuje_prihlasovaci_nazev", true, AutoUzivatel)[0][0])
            {
                btPrimePrihlaseni.FindLogicalChildren<Label>().ElementAt(0).Content = $"Přihlásit jako {AutoUzivatel}";
                btPrimePrihlaseni.Visibility = Visibility.Visible;
            }
            else
            {
                btPrimePrihlaseni.Visibility = Visibility.Collapsed;
            }
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
            object hash = PraceSDB.ZavolejPrikaz("nacti_heslo", true, tbJmeno.Text)[0][0];
            if (hash is not DBNull)
            {
                if (BCrypt.Net.BCrypt.EnhancedVerify(tbHeslo.Password, Encoding.UTF8.GetString((byte[])hash))) //Ověří heslo
                {
                    if (tbJmeno.Text != "ADMIN" && (bool)PraceSDB.ZavolejPrikaz("over_jednotne_prihlaseni", true, tbJmeno.Text)[0][0])
                    {
                        LepsiMessageBox.Show("Uživatel je již přihlášen na jiném zařízení!");
                        return;
                    }

                    Uzivatel.NactiUzivatele(tbJmeno.Text);
                    switch(Uzivatel.Prava)
                    {
                        case UrovenPrav.Administrator:
                            new AdminOkno().Show();
                            break;

                        case UrovenPrav.Ucitel:
                            new UcitelskeOkno().Show();
                            break;

                        case UrovenPrav.Zak:
                            new ZakovskeOkno().Show();
                            break;
                    }
                    Close();
                }
                else
                {
                    LepsiMessageBox.Show("Špatné heslo!");
                    bdrHeslo.BorderBrush = Brushes.Red;
                    bdrHeslo.BorderThickness = new Thickness(2);
                }
            }
            else
            {
                LepsiMessageBox.Show("Uživatelské jméno neexistuje nebo si uživatel ještě nevytvořil heslo!");
                bdrJmeno.BorderBrush = Brushes.Red;
                bdrJmeno.BorderThickness = new Thickness(2);
            }
        }

        private void btPrimePrihlaseni_Click(object sender, RoutedEventArgs e)
        {
            object hash = PraceSDB.ZavolejPrikaz("nacti_heslo", true, AutoUzivatel)[0][0];
            if(hash is DBNull)
            {
                InputBox ib = new InputBox("", "Zadej nové heslo:", true, 5);
                if(ib.ShowDialog() == true)
                {
                    byte[] noveHeslo = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.EnhancedHashPassword(ib.noveJmeno));
                    PraceSDB.ZavolejPrikaz("nastav_heslo_uctu", false, AutoUzivatel, noveHeslo);
                }
            }

            Uzivatel.NactiUzivatele(AutoUzivatel);
            switch (Uzivatel.Prava)
            {
                case UrovenPrav.Administrator:
                    new AdminOkno().Show();
                    break;

                case UrovenPrav.Ucitel:
                    new UcitelskeOkno().Show();
                    break;

                case UrovenPrav.Zak:
                    new ZakovskeOkno().Show();
                    break;
            }
            Close();
        }

        private void tbJmeno_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbHeslo.Focus();
            }
        }

        private void tbHeslo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btPrihlasit.Focus();
                //PrihlasSe(null, null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("nazev", "nadpis", true, 5);
            ib.ShowDialog();
        }
    }
}
