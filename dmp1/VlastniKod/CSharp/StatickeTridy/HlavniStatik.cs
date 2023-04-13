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
    //Statická třída sdružující proměnné a funkce potřebné napříč programem
    public static class HlavniStatik
    {
        //Event hlídající případné změny kvůli bindingu v XAML
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        //Instance Randomu pro celý kód
        public static readonly Random rnd = new Random();
        //Oddelovac využívaný na spojování dat v DB
        public static readonly string[] Oddelovac = new string[] { "$$$" };

        //Klient pro připojení k HTTP stránkám
        private static HttpClient client = new HttpClient();

        //Převede pole polí do 2D pole
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

        //Otočí pole polí o 90 stupňů
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

        //Nahraje obrázek na server a vrátí jeho jméno
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

        //Odešle požadavek metodou POST
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

        //Odhlasi účet z oplikace jejím restartem
        public static void OdhlasitSe()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
