using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using Media = System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Npgsql;
using ObservableDictionary;
using NC = NCalc;
using dmp1;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.ComponentModel;

namespace dmp1
{
    public static class HlavniStatik
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public static Random rnd = new Random();
        public static readonly string[] Oddelovac = new string[] { "$$$" };

        private static HttpClient client = new HttpClient();

        public static string OpravFunkci(string funkce)
        {
            if (string.IsNullOrWhiteSpace(funkce))
            {
                return "";
            }

            funkce = funkce.ToLower();
            funkce = funkce.Replace("[", "(");
            funkce = funkce.Replace("]", ")");
            funkce = funkce.Replace("{", "(");
            funkce = funkce.Replace("}", ")");
            funkce = Regex.Replace(funkce, @"[^0-9+\-*/^()x]", "");

            for (int i = 0; i < funkce.Length; ++i)
            {
                if (funkce[i] == '^')
                {
                    string pred = funkce.HledejZavorky(i - 1, false);
                    string za = funkce.HledejZavorky(i + 1, true);
                    funkce = funkce.Replace(pred + '^' + za, $"{{{pred},{za}}}");
                    i = -1;
                }
            }

            funkce = funkce.Replace("{", "Pow(");
            funkce = funkce.Replace("}", ")");

            return funkce;
        }

        public static NC.Expression VytvorFunkci(string predpis)
        {
            NC.Expression ex;
            try
            {
                ex = new NC.Expression(OpravFunkci(predpis));
                ex.Parameters["x"] = 1;
                ex.Evaluate();
            }
            catch
            {
                return null;
            }
            return ex;
        }

        public static Media.Color SmichejBarvy(Media.Color c1, Media.Color c2)
        {
            byte r = (byte)(c1.R * 0.5 + c2.R * 0.5);
            byte g = (byte)(c1.G * 0.5 + c2.G * 0.5);
            byte b = (byte)(c1.B * 0.5 + c2.B * 0.5);

            return Media.Color.FromRgb(r, g, b);
        }

        public static T[,] To2D<T>(T[][] zdroj)
        {
            int prvniDim = zdroj.Length;
            int druhaDim = zdroj.GroupBy(row => row.Length).Single().Key;

            T[,] vysledek = new T[prvniDim, druhaDim];
            for (int i = 0; i < prvniDim; ++i)
            {
                for (int j = 0; j < druhaDim; ++j)
                {
                    vysledek[i, j] = zdroj[i][j];
                }
            }

            return vysledek;
        }

        public static T[][] Otoc90<T>(T[][] zdroj)
        {
            T[][] vysledek = new T[zdroj[0].Length][];

            for (int i = 0; i < zdroj[0].Length; ++i)
            {
                vysledek[i] = new T[zdroj.Length];

                for (int j = 0; j < zdroj.Length; ++j)
                {
                    vysledek[i][j] = zdroj[j][i];
                }
            }

            return vysledek;
        }

        public static string NahrajObrazek(string cesta)
        {
            BitmapImage bi = new BitmapImage(new Uri(cesta, UriKind.Absolute));
            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder enkoder = new JpegBitmapEncoder();
                enkoder.Frames.Add(BitmapFrame.Create(bi));
                enkoder.Save(ms);
                string byty = Convert.ToBase64String(ms.ToArray());
                Dictionary<string, string> hodnoty = new Dictionary<string, string>()
                    {
                        {"obr", byty }
                    };

                return PosliPost((URLAdresa)"php/nahraniSouboru.php", hodnoty);
            }
        }

        public static string PosliPost(string url, Dictionary<string, string> parametry)
        {
            parametry.Add("heslo", "KoprovkaJeZloVytvoreneDablem");
            IEnumerable<string> encodedItems = parametry.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
            StringContent kontent = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");

            Task<HttpResponseMessage> odpovedT = client.PostAsync(url, kontent);
            odpovedT.Wait();

            Task<string> zbyvaT = odpovedT.Result.Content.ReadAsStringAsync();
            zbyvaT.Wait();

            string vys = zbyvaT.Result;
            return vys;
        }

        public static void OdhlasitSe()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
