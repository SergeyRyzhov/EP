using System.Linq;
using ChipSynthesys.Common.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;

namespace ChipSynthesys.UnitTests.PlaceModel
{
    [TestClass]
    public class StorageTests
    {
        [TestMethod]
        public void StoreLoadGlobalPlacement()
        {
            {
                var d = GetDesign();
                var gp = new PlacementGlobal(d);

                Component component = d.components.First();
                gp.x[component] = 1.0;
                gp.y[component] = 2.0;
                var task = new ChipTask(d, gp);
                task.Save("test.bin");
            }
            {
                var loadedTask = ChipTask.Load("test.bin");

                var loadedComponent = loadedTask.Design.components.First();

                Assert.AreEqual(loadedTask.Approximate.x[loadedComponent], 1.0);
                Assert.AreEqual(loadedTask.Approximate.y[loadedComponent], 2.0);
            }

        }

        private static Design GetDesign()
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
