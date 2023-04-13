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
    /// Interakční logika pro SpravaUloziste.xaml
    /// </summary>
    public partial class SpravaUloziste : Window
    {
        private static bool ZmenenaAdresa; //Jestli byla změněna adresa

        public SpravaUloziste()
        {
            InitializeComponent();
        }

        //Otestuje nové uložiště a případně ho zapíše do databáze
        private void btUlozit_Click(object sender, RoutedEventArgs e)
        {
            if (ZmenenaAdresa)
            {
                LepsiMessageBox.Show("Adresa již byla změněna, nyní restartujte aplikaci!");
                return;
            }

            if (tbAdresa.Text != URLAdresa.Koren)
            {
                LepsiMessageBox.Show("Adresa se nezměnila!");
                return;
            }


            string[] seznamSouboru = (string[])PraceSDB.ZavolejPrikaz("nacti_odpad", true)[0][0];


            string odezva = HlavniStatik.PosliPost(tbAdresa.Text + "php/verifikaceCesty.php", new Dictionary<string, string>() { { "soubory", string.Join(HlavniStatik.Oddelovac[0], seznamSouboru) } }).Trim();
            switch(odezva)
            {
                case "1":
                    LepsiMessageBox.Show("Adresa změněna, změny se však projeví až po restartu aplikace!");
                    PraceSDB.ZavolejPrikaz("nastav_konstantu", false, "korenova_adresa", tbAdresa.Text);
                    break;
                case "2":
                    LepsiMessageBox.Show("Nové uložiště nemá validní strukturu souborů!");
                    break;
                default:
                    LepsiMessageBox.Show("Uložiště neexistuje!");
                    break;
            }
        }

        //Odstraní z databáze nepoužívané obrázky
        private void btVycistit_Click(object sender, RoutedEventArgs e)
        {
            string[] seznamSouboru = (string[])PraceSDB.ZavolejPrikaz("nacti_odpad", true)[0][0];
            string[] vys = HlavniStatik.PosliPost((URLAdresa)"php/smazaniOdpadu.php", new Dictionary<string, string>() { { "soubory", string.Join(HlavniStatik.Oddelovac[0], seznamSouboru) } }).RozdelDolary();
            LepsiMessageBox.Show($"Úspěšně vyčištěno {vys[0]} souborů o celkové velikosti {string.Format("{0:0.##}", Convert.ToInt32(vys[1]) / 1_000_000f)} MB");
        }
    }
}
