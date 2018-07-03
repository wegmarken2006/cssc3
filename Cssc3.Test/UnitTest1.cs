using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var lu1 = new UgenList();
            var lr1 = new RateList();
            lu1.Add(c1);
            lu1.Add(c2);
            lr1.Add(Rate.RateKr);
            lr1.Add(Rate.RateIr);
            var p1 = new Primitive(name: "P1", inputs: lu1, rate: Rate.RateKr, 
            outputs: lr1);
            var p2 = new Primitive(name: "P2", rate: Rate.RateAr); 

            Assert.IsTrue(p1.name == "P1", "Primitive Name");
            Assert.IsTrue(p2.name == "P2", "Primitive Name 2");
            Assert.IsTrue(p1 is Primitive, "Primitive type");
        }
    }
}
