using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmp1
{
    public static class PraceSDB
    {
        private static readonly NpgsqlConnection conn; //Připojení k DB
        private static readonly NpgsqlCommand cmd; //Příkazová řádka v DB
        private static NpgsqlDataReader ndr; //Čtečka dat z DB

        //Připojení se k databázi a nastavení základních hodnot
        static PraceSDB()
        {
            try
            {
                //conn = new NpgsqlConnection("User Id=postgres;Password=phdrKKWs5xNOfcIm;Server=db.nkgdnehaiwnkcwbptbgm.supabase.co;Port=5432;Database=postgres");
                conn = new NpgsqlConnection("User Id=uzivatel_aplikace;Password=koprovkanenijidlo;Server=db.nkgdnehaiwnkcwbptbgm.supabase.co;Port=5432;Database=postgres");
                conn.Open();

                cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
            }
            catch
            {
                LepsiMessageBox.Show("Nepodařilo se připojit k databázi!");
                LepsiMessageBox.Show("Zkuste zkontrolova připojení k internetu, případně kontaktujte správce aplikace.");
                Environment.Exit(0);
            }
        }

        //Poslání příkazu do DB formou funkce
        public static List<List<object>> ZavolejPrikaz(string nazevFunkce, bool vraciNeco, params object[] parametry)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = $"select {nazevFunkce}({string.Join(", ", Enumerable.Range(0, parametry.Length).Select(cislo => $"@{cislo}"))})";

            for (int i = 0; i < parametry.Length; ++i)
            {
                cmd.Parameters.AddWithValue($"@{i}", parametry[i]);
            }

            if (vraciNeco)
            {
                ndr = cmd.ExecuteReader();
                List<List<object>> vys = new List<List<object>>();

                while (ndr.Read())
                {
                    vys.Add(new List<object>());
                    for (int i = 0; i < ndr.FieldCount; ++i)
                    {
                        vys.Last().Add(ndr[i]);
                    }
                }

                ndr.Close();
                cmd.Cancel();

                return vys;
            }
            else
            {
                int i = cmd.ExecuteNonQuery();
                return null;
            }
        }
    }
}
