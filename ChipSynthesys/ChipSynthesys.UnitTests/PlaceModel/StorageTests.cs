using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;

namespace ChipSynthesys.UnitTests.PlaceModel
{
    [TestClass]
    public class StorageTests
    {
        [Ignore]
        [TestMethod]
        public void StoreLoadGlobalPlacement()
        {
            var d = GetDesign();
            var gp = new PlacementDetail(d);

            Component component = d.components.First();
            gp.x[component] = 1;
            gp.y[component] = 2;

            const string testXml = "GlobalPlacement_test.xml";
            //gp.Save(testXml);

            //var copy = PlacementGlobal.Load(testXml);

            //Assert.AreEqual(copy.x[component], 1);
            //Assert.AreEqual(copy.y[component], 2);
        }

        private Design GetDesign()
        {
            var p = new Component.Pool();
            p.Add(3, 3);
            p.Add(3, 3);
            p.Add(5, 3);
            p.Add(2, 3);
            p.Add(3, 2);
            p.Add(2, 2);
            var n = new Net.Pool();
            var design = new Design(new Field(0, 0, 20, 20), p, n);
            return design;
        }
    }
}
