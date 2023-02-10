using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Npgsql;

namespace dmp1
{
    public static class HlavniStatik
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

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

        public static void NastavHodnoty<T>(this ObservableCollection<T> oc, IEnumerable<T> ie)
        {
            oc.Clear();

            foreach(T t in ie)
            {
                oc.Add(t);
            }
        }

        public static string ZiskejZavorku(this string str)
        {
            return str.Substring(str.LastIndexOf("(") + 1, str.LastIndexOf(")") - str.LastIndexOf("(") - 1);
        }

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
