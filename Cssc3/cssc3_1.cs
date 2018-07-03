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
        public static Rate max_rate(RateList rates, Rate start=Rate.RateIr) 
        {
            var maxr = start;
            foreach (var elem in rates) {
                if (elem > start) maxr = elem;
            }
            return maxr;
        }
        
        public static Rate rate_of<T>(T ugen) {
            if (ugen is Control) {
                return ((Control)(object)ugen).rate;
            }
            else if (ugen is Primitive) {
                return ((Primitive)(object)ugen).rate;
            }
            else if (ugen is Proxy) {
                return((Proxy)(object)ugen).primitive.rate;
            }
            else if (ugen is Mce) {
                var rates =  ((Mce)(object)ugen).ugens.Select(x => rate_of(x)).ToList();
                return max_rate((RateList)rates);   
            }
            else {
                throw new Exception("Error: rate_of");
            }
        }
    }
}