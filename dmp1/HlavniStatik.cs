using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Npgsql;
using ObservableDictionary;

namespace dmp1
{
    public static class HlavniStatik
    {
        //Import pomocné systémové metody
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        //Převod bitmapy na ImageSource použitelný v komponentě Image
        public static ImageSource ToImageSource(this Bitmap bmp)
        {
            IntPtr handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle); 
            }
        }

        //Vymaže ObservableCollection a uloží nové prvky bez potřeby vytvoření nové kolekce
        public static void NastavHodnoty<T>(this ObservableCollection<T> oc, IEnumerable<T> ie)
        {
            oc.Clear();

            foreach(T t in ie)
            {
                oc.Add(t);
            }
        }

        //Algoritmus na získání zkratky v závorce
        public static string ZiskejZavorku(this string str)
        {
            return str.Substring(str.LastIndexOf("(") + 1, str.LastIndexOf(")") - str.LastIndexOf("(") - 1);
        }

        //Najde rodiče Elementu jež odpovídá typu T
        public static T GetAncestorOfType<T>(this FrameworkElement child) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent != null && !(parent is T))
            {
                return (T)GetAncestorOfType<T>((FrameworkElement)parent);
            }

            return (T)parent;
        }
    }
}
