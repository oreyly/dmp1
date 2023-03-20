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
    /// Interakční logika pro ProhlednoutTestyLidi.xaml
    /// </summary>
    public partial class ProhlednoutTestyLidi : Window
    {
        private Window Rodic;
        private int IdHry;


        public ObservableCollection<string> Data { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Skupiny { get; set; } = new ObservableCollection<string>() { "SK", "CZ" };
        public ObservableCollection<string> Hraci { get; set; } = new ObservableCollection<string>() { "Pepek", "Z Depek" };
        public ProhlednoutTestyLidi()
        {
            InitializeComponent();
            DataContext = this;
            lvSkupiny.SelectedIndex = 0;
        }

        public ProhlednoutTestyLidi(int idHry, Window rodic) : this()
        {
            Rodic = rodic;
            IdHry = idHry;
            NactiData();
        }

        private void NactiData()
        {
            string[] infoOHre = (string[])PraceSDB.ZavolejPrikaz("nacti_info_hry", true, IdHry)[0][0];
            Data.NastavHodnoty(infoOHre);

            List<string> skupiny = ((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_skupiny", true, Uzivatel.Id)[0][0]).Select(dvojice => dvojice.Key).ToList();
            skupiny.Insert(0, "Všichni");
            Skupiny.NastavHodnoty(skupiny);
        }

        private void btSpustit_Click(object sender, RoutedEventArgs e)
        {
            new VysledkoveOkno(IdHry, ((string)((Button)sender).GetAncestorOfType<ListViewItem>().Content).ZiskejZavorku(), this).Show();
            Hide();
        }

        private void lvSkupiny_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] hraci;
            if (lvSkupiny.SelectedIndex == 0)
            {
                hraci = (string[])PraceSDB.ZavolejPrikaz("nacti_hrace_hry", true, IdHry)[0][0];
            }
            else
            {
                hraci = (string[])PraceSDB.ZavolejPrikaz("nacti_hrace_skupiny_do_vysledku", true, Uzivatel.Id, (string)lvSkupiny.SelectedItem, IdHry)[0][0];
            }
            Hraci.NastavHodnoty(hraci);
        }
    }
}
