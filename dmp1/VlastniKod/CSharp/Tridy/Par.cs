using PostSharp.Patterns.Model;

namespace dmp1
{
    [NotifyPropertyChanged]
    //Třída pro jednoduché spojení libovolných 2 typů
    public class Par<T,Q>
    {
        //První hodnota
        public T Klic { get; set; }
        //Druhá hodnota
        public Q Hodnota { get; set; }

        //Konsturktor pro nastavení výchozích hodnot
        public Par(T klic, Q hodnota)
        {
            Klic = klic;
            Hodnota = hodnota;
        }

        //Vylepšené porovnávání 2 párů
        public override bool Equals(object obj)
        {
            if(obj is Par<T,Q> p)
            {
                return Klic.Equals(p.Klic) && Hodnota.Equals(p.Hodnota);
            }
            return base.Equals(obj);
        }

        //Overridnuto pouze aby se Visual Studio nerozčilovalo
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
