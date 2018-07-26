using System;
using System.Collections.Generic;
using System.Linq;


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
            public Ugen left, right;
            public Mrg(Ugen left, Ugen right)
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

        public static UgenList extend(UgenList iList, int newLen)
        {
            var ln = iList.Count;
            var vout = new UgenList();
            if (ln > newLen)
            {
                return (UgenList)(iList.GetRange(0, newLen));
            }
            else
            {
                vout.AddRange(iList);
                Console.WriteLine(vout.Count);
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

        public static UgenList mce_extend<T>(int n, T ugen)
        {
            if (ugen is Mce)
            {
                return extend(((Mce)(object)ugen).ugens, n);
            }
            /* 
            else if (ugen is Mrg)
            {
                return;
            }
            */
            else
            {
                throw new Exception("Error: mce_extend");
            }
        }

