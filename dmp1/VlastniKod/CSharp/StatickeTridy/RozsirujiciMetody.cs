using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace dmp1
{
    public static class RozsirujiciMetody
    {
        //Import pomocná systémová metoda pro převod obrázku
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

            foreach (T t in ie)
            {
                oc.Add(t);
            }
        }

        //Algoritmus na získání zkratky v závorce
        public static string ZiskejZavorku(this string str)
        {
            return str.Substring(str.LastIndexOf("(") + 1, str.LastIndexOf(")") - str.LastIndexOf("(") - 1);
        }

        //Algoritmus na odebrání zkratky v závorce
        public static string OdeberZavorku(this string str)
        {
            return str.Replace($" ({str.ZiskejZavorku()})", "");
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

        //Najde dítě podle jména
        public static FrameworkElement GetChildByName(this FrameworkElement element, string name)
        {
            List<FrameworkElement> children = new List<FrameworkElement>();
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement fe)
                {
                    if (fe.Name == name)
                    {
                        return fe;
                    }
                    else
                    {
                        children.Add(fe);
                    }
                }
            }

            foreach (FrameworkElement child in children)
            {
                FrameworkElement fe = child.GetChildByName(name);

                if (fe != null)
                {
                    return fe;
                }
            }

            return null;
        }

        //Přidá element do listu pokud tam ještě není
        public static void AddIfNotExists<T>(this IList<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        //Přidá elementy do listu pokud tam ještě nejsou
        public static void AddIfNotExists<T>(this IList<T> list, IList<T> values)
        {
            foreach (T value in values)
            {
                if (!list.Contains(value))
                {
                    list.Add(value);
                }
            }
        }

        //Zamíchá elementy v listu
        public static void ZamichejList<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int j = HlavniStatik.rnd.Next(0, i);

                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        //Vytvoří pole ze stringu ve fromě pole
        public static T[] ZiskejPole<T>(this string pole)
        {
            return pole.Replace("{", "").Replace("}", "").Split(',').Select(id => (T)Convert.ChangeType(id, typeof(T))).ToArray();
        }

        //Rozdělí string spojený dolary na pole
        public static string[] RozdelDolary(this string pole)
        {
            return pole.Split(HlavniStatik.Oddelovac, StringSplitOptions.None);
        }
    }
}
