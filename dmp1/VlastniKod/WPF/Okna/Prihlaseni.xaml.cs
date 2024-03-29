﻿using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
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
        public MainWindow()
        {
            InitializeComponent();

            Thread tr = new Thread(NajdiAutoUzivatele);
            tr.Start();

            tbJmeno.Focus();
        }

        private string AutoUzivatel; //Přihlašovací jméno uživatele ve školní síti

        //Zjistí, jestli je plikace připojena ze školní sítě
        private void NajdiAutoUzivatele()
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
                DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://" + de.Properties["defaultNamingContext"][0].ToString());

                DirectorySearcher search = new DirectorySearcher(myLdapConnection)
                {
                    Filter = $"(cn={Environment.UserName})",
                    Sort = new SortOption("cn", SortDirection.Ascending)
                };

                if (myLdapConnection.Path == $"LDAP://DC=spsoad,DC=spsostrov,DC=cz" && search.FindAll().Count == 1)
                {
                    AutoUzivatel = Environment.UserName;
                }

                Dispatcher.Invoke(UkazAutoPrihlaseni);
            }
            catch 
            {
            }

        }

        //Zjistí jestli je AutoUzivatel v databázi a případně ukáže tlačítko pro přímé přihlášení
        private void UkazAutoPrihlaseni()
        {
            if (!string.IsNullOrWhiteSpace(AutoUzivatel) && (bool)PraceSDB.ZavolejPrikaz("existuje_prihlasovaci_nazev", true, AutoUzivatel)[0][0])
            {
                btPrimePrihlaseni.FindLogicalChildren<Label>().ElementAt(0).Content = $"Přihlásit jako {AutoUzivatel}";
                btPrimePrihlaseni.Visibility = Visibility.Visible;
            }
        }

        //Nastaví černý ohraničení, pokud není
        private void Viewbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border b && b.BorderBrush != Brushes.Black)
            {
                b.BorderBrush = Brushes.Black;
                b.BorderThickness = new Thickness(1);
            }

            ((Viewbox)((Border)sender).Child).Child.Focus();
        }

        //Přihlásí uživatele na základě uživatelského jména a hesla
        private void PrihlasSe(object sender, RoutedEventArgs e)
        {
            bdrJmeno.BorderBrush = Brushes.Black;
            object hash = PraceSDB.ZavolejPrikaz("nacti_heslo", true, tbJmeno.Text)[0][0];
            if (hash is not DBNull)
            {
                try
                {
                    if (BCrypt.Net.BCrypt.EnhancedVerify(tbHeslo.Password, Encoding.UTF8.GetString((byte[])hash))) //Ověří heslo
                    {
                        if (tbJmeno.Text != "ADMIN" && (bool)PraceSDB.ZavolejPrikaz("over_jednotne_prihlaseni", true, tbJmeno.Text)[0][0])
                        {
                            LepsiMessageBox.Show("Uživatel je již přihlášen na jiném zařízení!");
                            return;
                        }

                        Uzivatel.NactiUzivatele(tbJmeno.Text);
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
                    else
                    {
                        LepsiMessageBox.Show("Špatné heslo!");
                        bdrHeslo.BorderBrush = Brushes.Red;
                        bdrHeslo.BorderThickness = new Thickness(2);
                    }
                }
                catch
                {
                    LepsiMessageBox.Show("Heslo je poškozeno!");
                }
            }
            else
            {
                LepsiMessageBox.Show("Uživatelské jméno neexistuje nebo si uživatel ještě nevytvořil heslo!");
                bdrJmeno.BorderBrush = Brushes.Red;
                bdrJmeno.BorderThickness = new Thickness(2);
            }
        }

        //Přihlásí uživatele bez potřeby hesla
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

        //Přepne klávesou enter na textové pole s heslem
        private void tbJmeno_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbHeslo.Focus();
            }
        }

        //Přepne na tlačítko přihlásit
        private void tbHeslo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btPrihlasit.Focus();
            }
        }
    }
}
