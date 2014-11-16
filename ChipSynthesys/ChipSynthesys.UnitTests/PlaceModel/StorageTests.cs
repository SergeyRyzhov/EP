using ChipSynthesys.Common.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;
using System.Linq;

namespace ChipSynthesys.UnitTests.PlaceModel
{
    [TestClass]
    public class StorageTests : TestsBase
    {
        /*[TestMethod]
        public void SaveAndLoadChipTask()
        {
            var path = TestFilePath("SaveAndLoadChipTask.bin");

            var design = TestDesign();
            var placementGlobal = new PlacementGlobal(design);

            Component component = design.components.First();
            placementGlobal.x[component] = 1.0;
            placementGlobal.y[component] = 2.0;
            var task = new ChipTask(design, placementGlobal);
            task.Save(path);

            var loadedTask = ChipTask.Load(path);
            var loadedComponent = loadedTask.Design.components.First();
            Assert.AreEqual(loadedTask.Global.x[loadedComponent], 1.0);
            Assert.AreEqual(loadedTask.Global.y[loadedComponent], 2.0);
        }*/

        /*[TestMethod]
        public void SaveAndLoadChipTaskResult()
        {
            var path = TestFilePath("SaveAndLoadChipTaskResult.bin");

            var design = TestDesign();
            var placementGlobal = new PlacementGlobal(design);
            var placementDetail = new PlacementDetail(design);

            Component component = design.components.First();
            placementGlobal.x[component] = 1.0;
            placementGlobal.y[component] = 2.0;

            placementDetail.x[component] = 2;
            placementDetail.y[component] = 1;

            var task = new ChipTaskResult(design, placementGlobal, placementDetail);
            task.Save(path);

            var loadedTask = ChipTaskResult.Load(path);
            var loadedComponent = loadedTask.Design.components.First();
            Assert.AreEqual(loadedTask.Global.x[loadedComponent], 1.0);
            Assert.AreEqual(loadedTask.Global.y[loadedComponent], 2.0);

            Assert.AreEqual(loadedTask.Detail.x[loadedComponent], 2);
            Assert.AreEqual(loadedTask.Detail.y[loadedComponent], 1);
        }*/
    }
}