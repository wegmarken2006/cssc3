using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using static Cssc3.SC3;

namespace Cssc3.Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
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


            Assert.IsTrue(p1.name == "P1", "Primitive Name");
            Assert.IsTrue(p2.name == "P2", "Primitive Name 2");
            Assert.IsTrue(p1 is Primitive, "Primitive type");
            Assert.IsTrue(mc2 is Mce, "Mce type");
            Assert.IsTrue(mce_degree(mc1) == 2, "mce_degree");
            Assert.IsTrue(extend(il1, 7).Count == 7, "extend");
            Assert.IsTrue(mce_extend(3, mc1).Count == 3, "mce_extend");
            Assert.IsTrue(((Primitive)(object)(mce_extend(3, mc1)[2])).name == "P1", "mce_extend 2");
            Assert.IsTrue(((Primitive)(object)(mce_extend(3, mg2)[2])).name == "P2", "mce_extend 3");
        }
    }
}
