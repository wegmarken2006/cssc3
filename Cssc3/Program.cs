
using System;
using System.Collections.Generic;
using System.Linq;
using static Cssc3.SC3;


//git remote add origin https://github.com/wegmarken2006/cssc3.git
//git push -u origin master

namespace MainTest
{
    class Program
    {        
        static void Main(string[] args)
        {
            var c1 = new Constant<int>(1);
            var c2 = new Constant<double>(3.3);
            var k1 = new Control(name: "K1");
            var p1 = new Primitive(name: "P1", inputs: new UgenList { c1, c2 }, rate: Rate.RateKr,
                outputs: new RateList { Rate.RateKr, Rate.RateIr });
            var p2 = new Primitive(name: "P2", rate: Rate.RateAr);
            var mc1 = new Mce(ugens: new UgenList { p1, p1 });
            var mc2 = new Mce(ugens: new UgenList { p1, p2 });
            var il1 = new UgenList{c1, p2};
            var il2 = new UgenList{c1, p2, c1, p2, c1};
            var mg1 = new Mrg(left: (object)p1, right: (object)mc1);
            var mg2 = new Mrg(left: (object)p2, right: (object)p1);
            var mg3 = new Mrg(left: (object)mc1, right: (object)p2);



            Console.WriteLine("Mce Extend");
            printUgens(mce_extend(3, mg2));
            Console.WriteLine("Mce Extend");
            printUgens(mce_extend(4, p2));
            

            Console.WriteLine("END");
        }
    }
}
