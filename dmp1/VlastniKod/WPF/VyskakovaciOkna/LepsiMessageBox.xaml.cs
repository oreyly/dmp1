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
    public enum DruhTlacitekLMB
    {
        OK,
        AnoNe,
        AnoNeZrusit
    }

    /// <summary>
    /// Interakční logika pro LepsiMessageBox.xaml
    /// </summary>
    public partial class LepsiMessageBox : Window
    {
        public DruhTlacitekLMB DTL { get; set; }
        MessageBoxResult MBR { get; set; }

        private LepsiMessageBox()
        {
            InitializeComponent();

            btOk.Focus();
        }

        private LepsiMessageBox(string text, DruhTlacitekLMB dtl) : this()
        {
            DTL = dtl;
            DataContext = this;
            tbText.Text = text;
        }

        public static MessageBoxResult Show(string text, DruhTlacitekLMB dtl = DruhTlacitekLMB.OK)
        {
            LepsiMessageBox lmb = new LepsiMessageBox(text, dtl);
            lmb.ShowDialog();

            return lmb.MBR;
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            MBR = MessageBoxResult.OK;
            Close();
        }

        private void btAno_Click(object sender, RoutedEventArgs e)
        {
            MBR = MessageBoxResult.Yes;
            Close();
        }

        private void btNe_Click(object sender, RoutedEventArgs e)
        {
            MBR = MessageBoxResult.No;
            Close();
        }

        private void btZrusit_Click(object sender, RoutedEventArgs e)
        {
            MBR = MessageBoxResult.Cancel;
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
