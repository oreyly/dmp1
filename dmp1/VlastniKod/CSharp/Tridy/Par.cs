using PostSharp.Patterns.Model;

namespace dmp1
{
    [NotifyPropertyChanged]
    public class Par<T,Q>
    {
        public T Klic { get; set; }
        public Q Hodnota { get; set; }

        public Par(T klic, Q hodnota)
        {
            Klic = klic;
            Hodnota = hodnota;
        }

        public override bool Equals(object obj)
        {
            if(obj is Par<T,Q> p)
            {
                return Klic.Equals(p.Klic) && Hodnota.Equals(p.Hodnota);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
