using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro AdminOkno.xaml
    /// </summary>
    public partial class AdminOkno : Window
    {
        private OpenFileDialog ofd { get; set; } //Okno pro výběr souboru
        public AdminOkno()
        {
            InitializeComponent();

            ofd = new OpenFileDialog()
            {
                Filter = "Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*"
            };
        }

        //Načte žáky do databáze
        private void btZaci_Click(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string[][] data = HlavniStatik.Otoc90(File.ReadAllLines(ofd.FileName, Encoding.Default).Skip(1).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Where(radek => radek.Length >= 5).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2], radek[4] }).ToArray());
                    PraceSDB.ZavolejPrikaz("nastav_ucty_hracu", false, HlavniStatik.To2D(data));
                    LepsiMessageBox.Show("Žáci úspěšně nahráni!");
                }
                catch
                {
                    LepsiMessageBox.Show("Špatný formát dat!");
                }
            }
        }

        //Načte učitele do databáze
        private void btUcitele_Click(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    DateTime dt = DateTime.Now;
                    string[][] data = HlavniStatik.Otoc90(File.ReadAllLines(ofd.FileName, Encoding.Default).Skip(1).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2] }).ToArray());
                    PraceSDB.ZavolejPrikaz("nastav_ucty_ucitelu", false, HlavniStatik.To2D(data));
                    LepsiMessageBox.Show("Učitelé úspěšně nahráni!");
                }
                catch
                {
                    LepsiMessageBox.Show("Špatný formát dat!");
                }
            }
        }

        //Otevře správu uživatelů
        private void btUcty_Click(object sender, RoutedEventArgs e)
        {
            new SpravaUzivatelu(this).Show();
            Hide();
        }

        //Otevře správu obchodu
        private void btObchod_Click(object sender, RoutedEventArgs e)
        {
            new EditorObchodu(this).Show();
            Hide();
        }

        //Změní heslo administrátorovi
        private void btHesloAdmin_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Nové heslo administrátora", true, 8);

            if (ib.ShowDialog() == true)
            {
                byte[] noveHeslo = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.EnhancedHashPassword(ib.noveJmeno));

                PraceSDB.ZavolejPrikaz("zmen_admin_heslo", false, noveHeslo);
            }
        }

        //Otevře správu uložiště
        private void tbUloziste_Click(object sender, RoutedEventArgs e)
        {
            new SpravaUloziste().ShowDialog();
        }

        //Odhlásí uživatele
        private void btOdhlasit_Click(object sender, RoutedEventArgs e)
        {
            HlavniStatik.OdhlasitSe();
        }
    }
}
