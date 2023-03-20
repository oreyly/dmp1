using System;
using System.Data;
using System.Windows.Markup;

namespace dmp1
{
    //Výpočet jednoduchých příkladů ve WPF
    public class Calculate : MarkupExtension
    {
        string Exp;

        public Calculate()
        {

        }

        public Calculate(string exp)
        {
            Exp = exp;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Convert.ToDouble(new DataTable().Compute(Exp, "").ToString());
        }
    }
}
