
using System;
using System.Collections.Generic;
using System.Linq;
using static Cssc3.SC3;
using static Cssc3.Osc;


//git remote add origin https://github.com/wegmarken2006/cssc3.git
//git push -u origin master

namespace MainTest
{
    class Program
    {
        static void Main(string[] args)
        {
 
            //sc_start();
            printUgen(UAbs(new Constant<int>{value=-3}));
            Console.WriteLine("END");
        }

        static IUgen UAbs(object ugen) {
            return mk_unary_operator(5, Math.Abs, ugen);
        }


    }
}
