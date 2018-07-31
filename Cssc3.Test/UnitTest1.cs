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
            var c1 = new Constant<int> { value = 1 };
            var c2 = new Constant<double> { value = 3.3 };
            var k1 = new Control { name = "K1" };
            var p1 = new Primitive
            {
                name = "P1",
                inputs = new UgenL(c1, c2),
                rate = Rate.RateKr,
                outputs = new RateList { Rate.RateKr, Rate.RateIr }
            };
            var p2 = new Primitive { name = "P2", rate = Rate.RateAr };

            var mc1 = new Mce { ugens = new UgenL(p1, p1) };
            var mc2 = new Mce { ugens = new UgenL(p1, p2) };
            var mc3 = new Mce { ugens = new UgenL(p1, p2, mc1) };
            var p3 = new Primitive
            {
                name = "P3",
                inputs = new UgenL(mc1, mc3),
                rate = Rate.RateKr,
                outputs = new RateList { Rate.RateIr }
            };
            var il1 = new UgenL(c1, p2);
            var il2 = new UgenL(c1, p2, c1, p2, c1);
            var mg1 = new Mrg { left = (object)p1, right = (object)mc1 };
            var mg2 = new Mrg { left = (object)p2, right = (object)p1 };
            var mg3 = new Mrg { left = (object)mc1, right = (object)p2 };
            //var ill1 = new List<List<int>>{new List<int>{1,2,3}, new List<int>{4, 5, 6}};
            var ill1 = new List<List<object>> { new List<object> { 1, 2, 3 }, new List<object> { 4, 5, 6 } };
            var ill2 = transposer(ill1);
            var exmg1 = mce_extend(3, mg1);
            var mc10 = mce_transform(p3);
            var mc11 = mce_channels(mg3);

            var nc1 = new NodeC { nid = 10, value = 3 };
            var nk1 = new NodeK { name = "nk1", nid = 11, deflt = 5 };
            var fpc1 = new FromPortC { port_nid = 100 };
            var fpk1 = new FromPortK { port_nid = 101 };
            var fpku1 = new FromPortU { port_nid = 102, port_idx = 13 };
            NodeC ndc1 = new NodeC { nid = 20, value = 320 };
            NodeC ndc2 = new NodeC { nid = 21, value = 321 };
            NodeK ndk1 = new NodeK { nid = 30, name = "ndk1" };
            NodeK ndk2 = new NodeK { nid = 31, name = "ndk2" };
            NodeU ndu1 = new NodeU
            {
                nid = 40,
                name = "ndu1",
                inputs = new UgenL(mg1, mg2),
                outputs = new List<Rate> { Rate.RateAr, Rate.RateKr, Rate.RateIr },
                ugenId = 2,
                rate = Rate.RateAr
            };
            NodeU ndu2 = new NodeU
            {
                nid = 41,
                name = "ndu2",
                inputs = new UgenL(),
                outputs = new List<Rate> { },
                ugenId = 3,
                rate = Rate.RateAr
            };
            Graph gr1 = new Graph
            {
                nextId = 11,
                constants = new List<NodeC> { ndc1, ndc2 },
                controls = new List<NodeK> { ndk1, ndk2 },
                ugens = new List<NodeU> { ndu1, ndu2 }
            };
            var m1 = mk_map(gr1);
            var mcs1 = m1.cs;
            var n1 = mk_node_c(new Constant<int>{value=320}, gr1);
            var nn = n1.Item1;
            var ck1 = new Control{name="ndk1",rate=Rate.RateKr,index=3};
            var n2 = mk_node_k(ck1, gr1);
            var nn2 = n2.Item1;
            var n3 = mk_node<NodeC, Constant<int>>(new Constant<int>{value=320}, gr1);
            var nn3 = n3.Item1;
            var cs1 = new Constant<int>{value=11};





            Assert.IsTrue(p1.name == "P1", "Primitive Name");
            Assert.IsTrue(p2.name == "P2", "Primitive Name 2");
            Assert.IsTrue(p1 is Primitive, "Primitive type");
            Assert.IsTrue(mc2 is Mce, "Mce type");
            Assert.IsTrue(mce_degree(mc1) == 2, "mce_degree");
            Assert.IsTrue(extend(il1.l, 7).Count == 7, "extend");
            Assert.IsTrue(mce_extend(3, mc1).Count == 3, "mce_extend");
            Assert.IsTrue(((Primitive)(object)(mce_extend(3, mc1)[2])).name == "P1", "mce_extend 2");
            Assert.IsTrue(((Primitive)(object)(mce_extend(3, mg2)[2])).name == "P2", "mce_extend 3");
            Assert.IsTrue(is_mce(mc1), "is_mce 1");
            Assert.IsFalse(is_mce(mg1), "is_mce 2");
            Assert.IsTrue(ill2.Count == 3, "transposer");
            Assert.IsTrue(exmg1.Count == 3, "mce_extend");
            Assert.IsTrue(mc10 is Mce, "mce_transform 1");
            Assert.IsTrue(((Primitive)(object)mc10.ugens.l[2]).name == "P3", "mce_transform 2");
            Assert.IsTrue(mc11.l.Count == 2, "mce_channels 1");
            Assert.IsTrue(mc11.l[0] is Mrg, "mce_channels 2");
            Assert.IsTrue(mc11.l[1] is Primitive, "mce_channels 3");
            Assert.IsTrue(node_c_value(nc1) == 3, "node_c_value");
            Assert.IsTrue(node_k_default(nk1) == 5, "node_k_default");
            Assert.IsTrue(m1.cs.SequenceEqual(new List<int>{20, 21}), "mmap cs");
            Assert.IsTrue(find_c_p(3, nc1), "find_c_p");
            Assert.IsTrue(find_k_p("ndk1", ndk1), "find_k_p");
            Assert.IsTrue(nn.nid == 20, "mk_node_c 1");
            Assert.IsTrue(nn2.nid == 30, "mk_node_c 2");
            Assert.IsTrue(nn3.nid == 20, "mk_node_c 3");
            Assert.IsTrue(fetch(31, mk_map(gr1).ks) == 1, "fetch");
        }
    }
}
