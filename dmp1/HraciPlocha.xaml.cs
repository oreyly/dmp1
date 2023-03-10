using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace dmp1
{
    /// <summary>
    /// Interakční logika pro HraciPlocha.xaml
    /// </summary>
    //Okno vykreslující hru
    public partial class HraciPlocha : Window
    {
        private Hra _HraCoSeHraje;
        public Hra HraCoSeHraje
        {
            get
            {
                return _HraCoSeHraje;
            }

            set
            {
                _HraCoSeHraje = value;
                value.ZmenaAktualniUlohy += HraCoSeHraje_ZmenaAktualniUlohy;

                if (value.DruhSpusteni == DruhSpusteni.Test)
                {
                    HlidacCasu = new BackgroundWorker()
                    {
                        WorkerReportsProgress = true,
                        WorkerSupportsCancellation = true
                    };

                    HlidacCasu.DoWork += MerCas;
                    HlidacCasu.ProgressChanged += VypisCas;
                    HlidacCasu.RunWorkerCompleted += CasVyprsel;
                    HlidacCasu.RunWorkerAsync();
                }
            }
        }

        private void CasVyprsel(object sender, RunWorkerCompletedEventArgs e)
        {
            HraCoSeHraje.KonecHry();
        }

        private void VypisCas(object sender, ProgressChangedEventArgs e)
        {
            lbCas.TextKZobrazeni = $"Zbývá: {HraCoSeHraje.CasKonce - DateTime.Now:mm\\:ss}";
        }

        private void MerCas(object sender, DoWorkEventArgs e)
        {
            while(!HlidacCasu.CancellationPending && DateTime.Now < HraCoSeHraje.CasKonce)
            {
                HlidacCasu.ReportProgress(0);
                Thread.Sleep(1000);
            }
        }

        oknoPomoci op;
        BackgroundWorker HlidacCasu;
        //Nastavení základních hodnot
        public HraciPlocha(int id, int druh)
        {
            InitializeComponent();
            tlacitkaMoznosti = new RadioButton[] { btA, btB, btC, btD };
            Hra hr = Hra.NactiHru(id, (DruhSpusteni)druh);
            HraCoSeHraje = hr;
            DataContext = HraCoSeHraje;
            imgNapoveda.Source = Properties.Resources.icoNapoveda.ToImageSource();

            switch(druh)
            {
                case 1:

                    break;

                case 2:
                    op = new oknoPomoci(HraCoSeHraje, this);
                    op.Show();
                    break;

                case 4:

                    break;
            }
            ListBoxItem_MouseUp(null, null);
        }

        //Zobrazení / skrytí nápovědy
        private void imgNapoveda_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imgNapoveda.Visibility = Visibility.Collapsed;
            brNapoveda.Visibility = Visibility.Visible;
        }

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            brNapoveda.Visibility = Visibility.Collapsed;
            imgNapoveda.Visibility = Visibility.Visible;
        }

        //Vybrání úlohy v dané hře
        private void ListBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem lvi)
            {
                HraCoSeHraje.aktualniUloha = (Uloha)lvi.DataContext;
            }

            if (HraCoSeHraje.DruhSpusteni == DruhSpusteni.Uceni)
            {
                if (!HraCoSeHraje.aktualniUloha.OtevrenyVysledek)
                {
                    //tlacitkaMoznosti[HraCoSeHraje.aktualniUloha.SpravnyVysledek - 1].IsChecked = true;
                }
            }
        }

        RadioButton[] tlacitkaMoznosti;
        private void lbxSeznamUloh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbOdpoved_Loaded(object sender, RoutedEventArgs e)
        {
            /*if(HraCoSeHraje.DruhSpusteni == DruhSpusteni.Uceni)
            {
                ((TextBox)sender).Text = ((Par<string, string>)((TextBox)sender).GetAncestorOfType<ListViewItem>().DataContext).Hodnota;
            }*/
        }

        private void btOdeslat_Click(object sender, RoutedEventArgs e)
        {
            HraCoSeHraje.aktualniUloha.ZkontrolujOdpoved(true);

            if (HraCoSeHraje.DruhSpusteni == DruhSpusteni.Procvicovani)
            {
                if (HraCoSeHraje.aktualniUloha.stavUlohy == StavUlohy.Spravne)
                {
                    if (HraCoSeHraje.aktualniUloha.ZachytnyBod)
                    {
                        MessageBox.Show("Gratulace, dosáhl jsi záchytného bodu");
                    }

                    MessageBox.Show("Správná odpověď!");
                    try
                    {
                        HraCoSeHraje.aktualniUloha = HraCoSeHraje.Ulohy[HraCoSeHraje.Ulohy.IndexOf(HraCoSeHraje.aktualniUloha) + 1];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        MessageBox.Show("Gratulace, dosáhl jsi konce!");
                        btUkoncit_Click();
                        return;
                    }
                }
                else if (HraCoSeHraje.aktualniUloha.stavUlohy == StavUlohy.Spatne)
                {
                    MessageBox.Show("Špatná odpověď, padáš k nejbližšímu záchytnému bodu!");

                    for (int i = HraCoSeHraje.Ulohy.IndexOf(HraCoSeHraje.aktualniUloha) - 1; i >= 0; --i)
                    {
                        Uloha u = HraCoSeHraje.Ulohy[i];
                        if (u.ZachytnyBod)
                        {
                            break;
                        }
                        else
                        {
                            u.SpravnyVysledekOdpoved = 0;
                            u.stavUlohy = StavUlohy.Spatne;
                        }
                    }

                    btUkoncit_Click();
                }
            }
        }

        private void HraCoSeHraje_ZmenaAktualniUlohy(object sender, object stary, object novy)
        {
            foreach(RadioButton rb in tlacitkaMoznosti)
            {
                //rb.IsChecked = false;
            }
        }

        private void btUkoncit_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (HlidacCasu == null)
            {
                HraCoSeHraje.KonecHry();
            }
            else
            {
                HlidacCasu.CancelAsync();
            }
        }

        private void herniOkno_Closed(object sender, EventArgs e)
        {
            HraCoSeHraje.KonecHry();
        }
    }
}
