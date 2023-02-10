using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PostSharp.Patterns.Model;

namespace dmp1
{
    [NotifyPropertyChanged]
    public class Uloha
    {
        public string Nazev { get; set; } = "Funkce 1";
        public string Popis { get; set; } = "Mauris dictum facilisis augue. Nulla quis diam. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Vivamus porttitor turpis ac leo. Donec vitae arcu. Aenean id metus id velit ullamcorper pulvinar. Nullam eget nisl. Praesent dapibus. Curabitur vitae diam non enim vestibulum interdum. Pellentesque arcu. Nullam eget nisl. Nam quis nulla. Nunc auctor. Etiam dui sem, fermentum vitae, sagittis id, malesuada in, quam. Quisque porta. Aliquam erat volutpat. Duis risus. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat. Mauris dictum facilisis augue. Nunc auctor. ";

        public string Vysledek { get; set; } = "O$$$Ahoj$$$Jak se máš$$$Já se mám dobře$$$A co ty$$$Ahoj$$$Jak se máš$$$Já se mám dobře$$$A co ty$$$Ahoj$$$Jak se máš$$$Já se mám dobře$$$A co ty$$$";
        public string Napoveda { get; set; } = "More zkus trochu zapojit mozek, já nemám čas ti pořád napovídat. U maturity taky nebudeš mít nápovědu, tak se laskavě vzpamatuj!!!";

        public string ObrazekPredpis;
        public int Body { get; set; }

        public bool Otevrena;
        public Brush Barva
        {
            get
            {
                if (Body < 50)
                {
                    return new SolidColorBrush(Colors.Red);
                }

                return new SolidColorBrush(Colors.Green);
            }
        }

        public Uloha(int body)
        {
            Body = body;
            Nazev += body;
        }

        public override string ToString()
        {
            return Nazev;
        }
    }
}
