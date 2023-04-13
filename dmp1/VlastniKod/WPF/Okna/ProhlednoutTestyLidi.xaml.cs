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
        public ObservableCollection<string> Skupiny { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Par<string, int>> Hraci { get; set; } = new ObservableCollection<Par<string, int>>();
        private ProhlednoutTestyLidi()
        {
            InitializeComponent();
            DataContext = this;
            lvSkupiny.SelectedIndex = 0;
        }

        public ProhlednoutTestyLidi(int idHry, Window rodic) : this()
        {
            Rodic = rodic;
            Closed += delegate (object sender, EventArgs e) { Rodic.Show(); };
            IdHry = idHry;
            NactiData();
        }

        private void NactiData()
        {
            string[] infoOHre = (string[])PraceSDB.ZavolejPrikaz("nacti_info_hry", true, IdHry)[0][0];
            Data.NastavHodnoty(new string[] { infoOHre[0], infoOHre[1], Convert.ToBoolean(infoOHre[2]) ? "Ano" : "Ne", infoOHre[3], Convert.ToInt32(infoOHre[4]) > 0 ? Convert.ToInt32(infoOHre[4]) / 60 + " min." : "Není", Convert.ToBoolean(infoOHre[5]) ? "Ano" : "Ne", infoOHre[6], infoOHre[7] });

            List<string> skupiny = ((Dictionary<string, string>)PraceSDB.ZavolejPrikaz("nacti_skupiny", true, Uzivatel.Id)[0][0]).OrderBy(dvojice => dvojice.Key).Select(dvojice => dvojice.Key).ToList();
            skupiny.Insert(0, "Všichni");
            Skupiny.NastavHodnoty(skupiny);
        }

        private void btSpustit_Click(object sender, RoutedEventArgs e)
        {
            new VysledkoveOkno(IdHry, ((Par<string, int>)((Button)sender).GetAncestorOfType<ListViewItem>().Content).Klic.ZiskejZavorku(), this).Show();
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

            Hraci.NastavHodnoty(hraci.Select(hrac => hrac.RozdelDolary()).Select(hrac => new Par<string, int>(hrac[0], Convert.ToInt32(hrac[1]))));
        }
    }
}
