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
        private static NpgsqlConnection conn;
        private static NpgsqlCommand cmd;
        private static NpgsqlDataReader ndr;

        static PraceSDB()
        {
            conn = new NpgsqlConnection("User Id=postgres;Password=phdrKKWs5xNOfcIm;Server=db.nkgdnehaiwnkcwbptbgm.supabase.co;Port=5432;Database=postgres");
            conn.Open();

            cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
        }

        public static List<List<object>> ZavolejPrikaz(string prikaz, bool vraciNeco, params object[] parametry)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = $"select {prikaz}({string.Join(", ", Enumerable.Range(0, parametry.Length).Select(cislo => $"@{cislo}"))})";
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
                cmd.ExecuteNonQuery();
                return null;
            }
        }
    }
}
