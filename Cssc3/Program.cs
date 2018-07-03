
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
            var c1 = new Constant<int>(12);
            var c2 = new Constant<double>(13.1);
            var k1 = new Control(name: "K1");
            var lu1 = new UgenList();
            lu1.Add(c1);
            lu1.Add(k1);
            var p1 = new Primitive(name: "P1", inputs: lu1, rate: Rate.RateAr);
            
            Console.WriteLine(rate_of(p1));

            //Map Filter example (SYstem.Linq dependent)
            var l1 = new List<int>{1, 2, 3};
            var l2 = l1.Select(x => x*2);
            var l3 = l1.Where(x => x > 2);

            Console.WriteLine("END");
        }
    }
}
