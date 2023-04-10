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

            RuntimeHelpers.RunClassConstructor(typeof(PraceSDB).TypeHandle);

            //data = HlavniStatik.Otoc90(File.ReadAllLines("data.txt", Encoding.Default).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2], radek[4] }).ToArray());
            //data2 = HlavniStatik.Otoc90(File.ReadAllLines("data2.txt", Encoding.Default).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2] }).ToArray());
        }

        Window Rodic;
        public AdminOkno(Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object o, EventArgs e) { Rodic.Show(); };
        }

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

        private void btUcty_Click(object sender, RoutedEventArgs e)
        {
            new SpravaUzivatelu(this).Show();
            Hide();
        }

        private void btObchod_Click(object sender, RoutedEventArgs e)
        {
            new EditorObchodu(this).Show();
            Hide();
        }

        private void btHesloAdmin_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Nové heslo administrátora", true, 8);

            if (ib.ShowDialog() == true)
            {
                byte[] noveHeslo = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.EnhancedHashPassword(ib.noveJmeno));

                PraceSDB.ZavolejPrikaz("zmen_admin_heslo", false, noveHeslo);
            }
        }

        private void tbUloziste_Click(object sender, RoutedEventArgs e)
        {
            new SpravaUloziste().ShowDialog();
        }

        private void btOdhlasit_Click(object sender, RoutedEventArgs e)
        {
            HlavniStatik.OdhlasitSe();
        }
    }
}
