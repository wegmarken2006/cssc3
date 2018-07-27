
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
            var c1 = new Constant<int> { value = 1 };
            var c2 = new Constant<double> { value = 3.3 };

            var k1 = new Control{name="K1"};
            var p1 = new Primitive{name="P1", inputs=new UgenL(c1, c2), rate=Rate.RateKr,
                outputs=new RateList { Rate.RateKr, Rate.RateIr }};
            var p2 = new Primitive{name="P2", rate=Rate.RateAr};

            var mc1 = new Mce{ugens=new UgenL(p1, p1)};
            var mc2 = new Mce{ugens=new UgenL(p1, p2)};
            var mc3 = new Mce{ugens=new UgenL(p1, p2, mc1)};
            var p3 = new Primitive{name="P3", inputs=new UgenL(mc1, mc3), rate=Rate.RateKr,
                outputs=new RateList { Rate.RateIr }};

            var il1 = new UgenL(c1, p2);
            var il2 = new UgenL(c1, p2, c1, p2, c1);
            var mg1 = new Mrg{left=(object)p1, right=(object)mc1};
            var mg2 = new Mrg{left=(object)p2, right=(object)p1};
            var mg3 = new Mrg{left=(object)mc1, right=(object)p2};
            var exmg1 = mce_extend(3, mg1);
            var mc10 = mce_transform(p3);
            var mx1 = mce_expand(mc10);
            var mx2 = mce_expand(mg1);
            var mx3 = mce_expand(p1);
            var mc11 = mce_channels(mg3);

            //printUgen(mx2);
            //printUgen(mx3);

            Console.WriteLine("END");
        }
    }
}
