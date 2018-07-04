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

        public struct UgenL
        {
            public List<object> l;
            //vararg constructor
            public UgenL(params object[] values)
            {
                l = new List<object>();
                foreach (var value in values)
                {
                    this.l.Add(value);
                }
            }
        }

        public struct Constant<T>
        {
            public T value;
            public Constant(T value) => this.value = value;
        }

        public struct Mce
        {
            public UgenL ugens;
            public Mce(UgenL ugens) => this.ugens = ugens;
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
            public UgenL inputs;
            public RateList outputs;
            public string name;
            public int special, index;
            public Primitive(string name, UgenL inputs = new UgenL(), RateList outputs = null,
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
                if (elem > maxr) maxr = elem;
            }
            return maxr;
        }

        public static int max_num(List<int> nums, int start)
        {
            var max1 = start;
            foreach (var elem in nums)
            {
                if (elem > max1) max1 = elem;
            }
            return max1;
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
                var rates = ((Mce)(object)ugen).ugens.l.Select(x => rate_of(x)).ToList();
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
                Console.WriteLine("Mce: " + ((Mce)(object)ugen).ugens.l.Count.ToString());
                printUgens(((Mce)(object)ugen).ugens.l);
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
            else
            {
                Console.Write(ugen);
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
                return ((Mce)(object)ugen).ugens.l.Count;
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
                var iList = ((Mce)(object)ugen).ugens.l;
                return extend(iList, n);
            }
            else if (ugen is Mrg)
            {
                var ex = mce_extend(n, ((Mrg)(object)ugen).left);
                var len = ex.Count;
                if (len > 0)
                {
                    var outv = new List<object> { ugen };
                    outv.AddRange(ex.GetRange(1, n - 1));
                    return outv;
                }
                else
                {
                    throw new Exception("mce_extend");
                }
            }
            else
            {
                var outv = new List<object>();
                for (var ind = 0; ind < n; ind++)
                {
                    outv.Add(ugen);
                }
                return outv;
            }
        }
        public static bool is_mce<T>(T ugen)
        {
            if (ugen is Mce)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void printLList<T>(List<List<T>> iList)
        {
            var len1 = iList.Count;
            var len2 = iList[0].Count;

            for (var ind1 = 0; ind1 < len1; ind1++)
            {
                for (var ind2 = 0; ind2 < len2; ind2++)
                {
                    printUgen(iList[ind1][ind2]);
                    Console.Write(" ");
                }
                Console.WriteLine("");
            }
        }

        public static List<List<object>> transposer(List<List<object>> iList)
        {
            var len1 = iList.Count;
            var len2 = iList[0].Count;
            var outv = new List<List<object>>();
            for (var ind = 0; ind < len2; ind++)
            {
                outv.Add(new List<object>());
            }
            for (var ind2 = 0; ind2 < len2; ind2++)
            {
                for (var ind1 = 0; ind1 < len1; ind1++)
                {
                    outv[ind2].Add(iList[ind1][ind2]);
                }
            }
            return outv;
        }

        public static Mce mce_transform<T>(T ugen)
        {
            if (ugen is Primitive)
            {
                var prim = ((Primitive)(object)ugen);
                var inputs = prim.inputs;
                var ins = inputs.l.Where(x => is_mce(x));
                var degs = new List<int>();
                foreach (var elem in ins)
                {
                    degs.Add(mce_degree(elem));
                }
                var upr = max_num(degs, 0);
                var ext = new List<List<object>>();
                foreach (var elem in inputs.l)
                {
                    ext.Add(mce_extend(upr, elem));
                }
                var iet = transposer(ext);
                var outv = new List<object>();
                //var outv = new UgenL();
                foreach (var elem2 in iet)
                {
                    var newInps = new UgenL();
                    newInps.l = elem2;
                    var p = new Primitive(name: prim.name, inputs: newInps, outputs: prim.outputs,
                     rate: prim.rate, special: prim.special, index: prim.index);
                    //outv.l.Add(p);
                    outv.Add(p);

                }
                var newOut = new UgenL();
                newOut.l = outv;
                //return new Mce(ugens: outv);
                return new Mce(ugens: newOut);
            }
            else
            {
                throw new Exception("Error: mce_transform");
            }
        }

    }
}