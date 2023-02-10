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
    /// Interakční logika pro editorUloh.xaml
    /// </summary>
    public partial class editorUloh : Window
    {
        //string cesta = "http://home.spsostrov.cz/~matema/dlouhodobka/pozadi.jpg";
        string cesta = "https://d15-a.sdn.cz/d_15/c_img_E_G/vZQBeba.jpeg?fl=cro,0,0,800,533%7Cres,1280,,1%7Cwebp,75";
        public ObservableCollection<Uloha> seznamUloh = new ObservableCollection<Uloha>();
        public editorUloh()
        {
            InitializeComponent();
            DataContext = this;
            imgNahled.Source = new BitmapImage(new Uri(cesta, UriKind.Absolute));
        }
    }
}
