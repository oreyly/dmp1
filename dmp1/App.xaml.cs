using PostSharp.Aspects.Advices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace dmp1
{
    /// <summary>
    /// Interakční logika pro App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Uzivatel.Jmeno))
            {
                PraceSDB.ZavolejPrikaz("odhlasit", false, Uzivatel.Jmeno.ZiskejZavorku());
            }
        }
    }
}
