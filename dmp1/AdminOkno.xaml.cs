using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
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

            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(PraceSDB).TypeHandle);

            //data = HlavniStatik.Otoc90(File.ReadAllLines("data.txt", Encoding.Default).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2], radek[4] }).ToArray());
            //data2 = HlavniStatik.Otoc90(File.ReadAllLines("data2.txt", Encoding.Default).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2] }).ToArray());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PraceSDB.ZavolejPrikaz("nastav_ucty_hracu", false, HlavniStatik.To2D(data));
            //PraceSDB.ZavolejPrikaz("nastav_ucty_ucitelu", false, HlavniStatik.To2D(data2));
            MessageBox.Show("Hotovo");
        }

        private void btZaci_Click(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                DateTime dt = DateTime.Now;
                string[][] data = HlavniStatik.Otoc90(File.ReadAllLines(ofd.FileName, Encoding.Default).Skip(1).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Where(radek => radek.Length >= 5).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2], radek[4] }).ToArray());
                //int pocet = data[0].Distinct().Count();
                PraceSDB.ZavolejPrikaz("nastav_ucty_hracu", false, HlavniStatik.To2D(data));
                MessageBox.Show("Žáci úspěšně nahráni" + (DateTime.Now - dt));
                
            }
        }

        private void btUcitele_Click(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                DateTime dt = DateTime.Now;
                string[][] data = HlavniStatik.Otoc90(File.ReadAllLines(ofd.FileName, Encoding.Default).Skip(1).Select(radek => radek.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Select(radek => new string[] { radek[0].Substring(0, radek[0].IndexOf('@')), radek[1] + " " + radek[2] }).ToArray());
                PraceSDB.ZavolejPrikaz("nastav_ucty_ucitelu", false, HlavniStatik.To2D(data));
                MessageBox.Show("Učitelé úspěšně nahráni" + (DateTime.Now - dt));
            }
        }

        private void btUcty_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btObchod_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btHesloAdmin_Click(object sender, RoutedEventArgs e)
        {
            InputBox ib = new InputBox("", "Nové heslo");

            if (ib.ShowDialog() == true)
            {
                byte[] noveHeslo = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.EnhancedHashPassword(ib.noveJmeno));

                PraceSDB.ZavolejPrikaz("zmen_admin_heslo", false, noveHeslo);

                MessageBox.Show("Heslo změněno");
            }
        }
    }
}
