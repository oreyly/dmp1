namespace dmp1
{
    public class Par
    {
        public string Otazka { get; set; }
        public string Odpoved { get; set; }

        public Par(string otazka, string odpoved)
        {
            Otazka = otazka;
            Odpoved = odpoved;
        }
    }
}
