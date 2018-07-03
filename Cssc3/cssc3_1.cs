using System;
using System.Collections.Generic;
using System.Linq;


//git remote add origin https://github.com/wegmarken2006/cssc3.git
//git push -u origin master

namespace Cssc3
{

    static public class SC3
    {
        public enum Rate
        {
            RateIr = 0,
            RateKr = 1,
            RateAr = 2,
            RateDr = 3
        }
        public class RateList : List<Rate> { }
        public class Ugen : object { }
        public class UgenList : List<object> { }

        public struct Constant<T>
        {
            public T value;
            public Constant(T value) => this.value = value;
        }

        public struct Mce
        {
            public UgenList ugens;
            public Mce(UgenList ugens) => this.ugens = ugens;
        }
        public struct Mrg
        {
            //public Ugen left, right;
            //public Mrg(Ugen left, Ugen right)
            public object left, right;
            public Mrg(object left, object right)
            {
                this.left = left;
                this.right = right;
            }
        }

        public struct Control
        {
            public string name;
            public Rate rate;
            public Control(string name, Rate rate = Rate.RateKr)
            {
                this.name = name;
                this.rate = rate;
            }
        }

        public struct Primitive
        {
            public Rate rate;
            public UgenList inputs;
            public RateList outputs;
            public string name;
            public int special, index;
            public Primitive(string name, UgenList inputs = null, RateList outputs = null,
            Rate rate = Rate.RateKr, int special = 0, int index = 0)
            {
                this.rate = rate;
                this.name = name;
                this.inputs = inputs;
                this.outputs = outputs;
                this.special = special;
                this.index = index;
            }
        }

        public struct Proxy
        {
            public Primitive primitive;
            public int index;
            public Proxy(Primitive primitive, int index = 0)
            {
                this.primitive = primitive;
                this.index = index;
            }
        }
        public static Rate max_rate(RateList rates, Rate start = Rate.RateIr)
        {
            var maxr = start;
            foreach (var elem in rates)
            {
                if (elem > start) maxr = elem;
            }
            return maxr;
        }

        public static Rate rate_of<T>(T ugen)
        {
            if (ugen is Control)
            {
                return ((Control)(object)ugen).rate;
            }
            else if (ugen is Primitive)
            {
                return ((Primitive)(object)ugen).rate;
            }
            else if (ugen is Proxy)
            {
                return ((Proxy)(object)ugen).primitive.rate;
            }
            else if (ugen is Mce)
            {
                var rates = ((Mce)(object)ugen).ugens.Select(x => rate_of(x)).ToList();
                return max_rate((RateList)rates);
            }
            else
            {
                throw new Exception("Error: rate_of");
            }
        }

        public static void printUgen<T>(T ugen)
        {
            if (ugen is Control)
            {
                Console.WriteLine("K: " + ((Control)(object)ugen).name);
            }
            else if (ugen is Primitive)
            {
                Console.WriteLine("P: " + ((Primitive)(object)ugen).name);
            }
            else if (ugen is Constant<int>)
            {
                Console.WriteLine("C: " + ((Constant<int>)(object)ugen).value.ToString());
            }
            else if (ugen is Constant<double>)
            {
                Console.WriteLine("C: " + ((Constant<double>)(object)ugen).value.ToString());
            }
            else if (ugen is Mce)
            {
                Console.WriteLine("Mce: " + ((Mce)(object)ugen).ugens.Count.ToString());
                printUgens(((Mce)(object)ugen).ugens);
            }
            else if (ugen is Mrg)
            {
                Console.WriteLine("Mrg: ");
                Console.Write(" * left: ");
                printUgen(((Mrg)(object)ugen).left);
                Console.Write(" * right: ");
                printUgen(((Mrg)(object)ugen).right);
            }
            else if (ugen is Proxy)
            {
                Console.WriteLine("Proxy: ");
            }


        }
        public static void printUgens(List<object> ugens)
        {
            foreach (var ugen in ugens)
            {
                Console.Write(" - ");
                printUgen(ugen);
            }

        }

        public static List<object> extend(List<object> iList, int newLen)
        {
            var ln = iList.Count;
            var vout = new List<object>();
            if (ln > newLen)
            {
                return iList.GetRange(0, newLen);
            }
            else
            {
                vout.AddRange(iList);
                vout.AddRange(iList);
                return extend(vout, newLen);
            }
        }
        public static int mce_degree<T>(T ugen)
        {
            if (ugen is Mce)
            {
                return ((Mce)(object)ugen).ugens.Count;
            }
            else if (ugen is Mrg)
            {
                return mce_degree(((Mrg)(object)ugen).left);
            }
            else
            {
                throw new Exception("Error: mce_degree");
            }
        }

        public static List<object> mce_extend<T>(int n, T ugen)
        {
            if (ugen is Mce)
            {
                var toExtend = ((Mce)(object)ugen).ugens;
                var iList = (List<object>)toExtend;
                return extend(iList, n);
            }
            else if (ugen is Mrg)
            {
                var ex = mce_extend(n, ((Mrg)(object)ugen).left);
                var len = ex.Count;
                if (len > 0) {
                    var outv = new List<object>{ugen};
                    outv.AddRange(ex.GetRange(1, n - 1));
                    return outv;
                }
                else {
                    throw new Exception("mce_extend");
                }
            }
            else
            {
                var outv = new List<object>();
                for (var ind = 0; ind < n; ind++) {
                    outv.Add(ugen);
                }
                return outv;
            }
        }

    }
}