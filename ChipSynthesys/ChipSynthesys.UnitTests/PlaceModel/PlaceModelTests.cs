using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;
using System;
using System.Linq;

namespace ChipSynthesys.UnitTests.PlaceModel
{
    [TestClass]
    public class PlaceModelTests
    {
        [TestMethod]
        public void Tests()
        {
            run_PlaceModel();
        }

        private static void log(string text)
        {
            Console.Write(text);
        }

        private static void test(string name, bool result)
        {
            Console.Write("\n{0}\t", name);
            Console.Write("{0}", (result) ? "OK" : "ERROR");
            Assert.IsTrue(result);
        }

        private static void run_PlaceModel()
        {
            log("\nPlaceModel tests start");
            test("Design.Nets(Component)", DesignNetsComponent());
            test("Design.Save & Load", DesignSaveLoad());
            test("Placement\t", Placement());
            log("\nPlaceModel tests final\n");
        }

        private static bool DesignNetsComponent()
        {
            return MicroCheck(Micro());
        }

        private static Design Micro()
        {
            var f = new Field(0, 0, 10, 10);
            var c = new Component.Pool();
            c.Add(1, 3);
            c.Add(2, 2);
            c.Add(3, 1);
            var n = new Net.Pool();
            n.Add(new Component[] { c[0], c[1] });
            n.Add(new Component[] { c[1], c[2] });
            n.Add(new Component[] { c[2], c[0] });
            return new Design(f, c, n);
        }

        private static bool MicroCheck(Design d)
        {
            var c = d.components;
            var n = d.nets;
            if (d.Nets(c[0]).Length != 2) return false;
            if (d.Nets(c[1]).Length != 2) return false;
            if (d.Nets(c[2]).Length != 2) return false;
            if (!d.Nets(c[0]).Contains(n[0])) return false;
            if (!d.Nets(c[0]).Contains(n[2])) return false;
            if (!d.Nets(c[1]).Contains(n[0])) return false;
            if (!d.Nets(c[1]).Contains(n[1])) return false;
            if (!d.Nets(c[2]).Contains(n[1])) return false;
            if (!d.Nets(c[2]).Contains(n[2])) return false;
            return true;
        }

        private static bool DesignSaveLoad()
        {
            Design.Save(Micro(), "micro.xml");
            return MicroCheck(Design.Load("micro.xml"));
        }

        private static bool Placement()
        {
            var d0 = Micro();
            var p = new PlacementGlobal(d0);
            p.x[d0.components[0]] = 1;
            p.y[d0.components[0]] = 1;
            p.x[d0.components[1]] = 2;
            p.y[d0.components[1]] = 2;
            p.x[d0.components[2]] = 2;
            p.y[d0.components[2]] = 2;
            var d1 = new Design(d0, new Field(1, 1, 2, 2), d0.components.Where(c => p.x[c] == 2 && p.y[c] == 2));
            if (d1.components.Length != 2) return false;
            if (d1.nets.Length != 3) return false;
            p.Editable(d1);
            try
            {
                p.x[d1.components[0]] = 3;
                p.y[d1.components[0]] = 3;
                p.x[d1.components[1]] = 3;
                p.y[d1.components[1]] = 3;
            }
            catch
            {
                return false;
            }
            try
            {
                p.x[d0.components[0]] = 2;
                return false;
            }
            catch { }
            try
            {
                p.y[d0.components[0]] = 2;
                return false;
            }
            catch { }
            if (p.x[d1.components[0]] != 3) return false;
            if (p.y[d1.components[0]] != 3) return false;
            if (p.x[d1.components[1]] != 3) return false;
            if (p.y[d1.components[1]] != 3) return false;
            return true;
        }
    }
}