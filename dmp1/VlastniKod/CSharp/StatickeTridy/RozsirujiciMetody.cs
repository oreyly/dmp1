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

        //Vyhledá člen od indexu dál nebo zpět
        public static string HledejZavorky(this string str, int index, bool dopredu)
        {
            int i = index;
            if (dopredu)
            {
                switch (str[i])
                {
                    case 'x':
                        return "x";

                    case '-':
                    case '+':
                    case '^':
                        return str[i] + HledejZavorky(str, index + 1, dopredu);

                    case '(':
                    case '{':
                        int pocet1 = 0;
                        int pocet2 = 0;

                        for (; i < str.Length; ++i)
                        {
                            switch (str[i])
                            {
                                case '(':
                                    pocet1 += 1;
                                    break;

                                case ')':
                                    pocet1 -= 1;
                                    break;

                                case '{':
                                    pocet2 += 1;
                                    break;

                                case '}':
                                    pocet2 += 1;
                                    break;
                            }

                            if (pocet1 < 0 || pocet2 < 0)
                            {
                                throw new Exception("Chyba ve výrazu!");
                            }

                            if (pocet1 == 0 && pocet2 == 0)
                            {
                                return str.Substring(index, i - index + 1);
                            }
                        }

                        if (pocet1 == 0 && pocet2 == 0)
                        {
                            return str.Substring(index, i - index + 1);
                        }

                        throw new Exception("Chyba ve výrazu!");

                    default:
                        if (char.IsDigit(str[i]))
                        {
                            string vysledek = "";
                            for (; i < str.Length; ++i)
                            {
                                if (char.IsDigit(str[i]))
                                {
                                    vysledek += str[i];
                                }
                                else
                                {
                                    return vysledek;
                                }
                            }
                            return vysledek;
                        }

                        throw new Exception("Chyba ve výrazu!");
                }
            }
            else
            {
                switch (str[i])
                {
                    case 'x':
                        return "x";

                    case ')':
                    case '}':
                        int pocet1 = 0;
                        int pocet2 = 0;

                        for (; i >= 0; --i)
                        {
                            switch (str[i])
                            {
                                case '(':
                                    pocet1 += 1;
                                    break;

                                case ')':
                                    pocet1 -= 1;
                                    break;

                                case '{':
                                    pocet2 += 1;
                                    break;

                                case '}':
                                    pocet2 -= 1;
                                    break;
                            }

                            if (pocet1 > 0 || pocet2 > 0)
                            {
                                throw new Exception("Chyba ve výrazu!");
                            }

                            if (pocet1 == 0 && pocet2 == 0)
                            {
                                return str.Substring(i, index - i + 1);
                            }
                        }

                        if (pocet1 == 0 && pocet2 == 0)
                        {
                            return str.Substring(i - index + 1, index);
                        }

                        throw new Exception("Chyba ve výrazu!");

                    default:
                        if (char.IsDigit(str[i]))
                        {
                            string vysledek = "";
                            for (; i >= 0; --i)
                            {
                                if (char.IsDigit(str[i]))
                                {
                                    vysledek = str[i] + vysledek;
                                }
                                else
                                {
                                    return vysledek;
                                }
                            }
                            return vysledek;
                        }

                        throw new Exception("Chyba ve výrazu!");
                }
            }
        }
        public static void AddIfNotExists<T>(this IList<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

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

        public static void ZamichejList<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int j = HlavniStatik.rnd.Next(0, i);

                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        public static T[] ZiskejPole<T>(this string pole)
        {
            return pole.Replace("{", "").Replace("}", "").Split(',').Select(id => (T)Convert.ChangeType(id, typeof(T))).ToArray();
        }

        public static string[] RozdelDolary(this string pole)
        {
            return pole.Split(HlavniStatik.Oddelovac, StringSplitOptions.None);
        }
    }
}
