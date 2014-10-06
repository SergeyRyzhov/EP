using DetailPlacer.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaceModel;

namespace ChipSynthesys.UnitTests.Placement
{
    [TestClass]
    public class CrossReductPlacerTest
    {
        [TestMethod]
        public void CrossReductPlacerTestImplTest()
        {
            var p = new Component.Pool();
            p.Add(3, 3);
            p.Add(4, 2);
            p.Add(2, 4);
            p.Add(6, 2);
            var n = new Net.Pool();
            var design = new Design(new Field(-2, -2, 10, 10), p, n);

            var approximate = new PlacementGlobal(design);
            approximate.x[p[0]] = 2.5;
            approximate.y[p[0]] = -0.5;
            approximate.x[p[1]] = 0.0;
            approximate.y[p[1]] = 0.0;
            approximate.x[p[2]] = 1.0;
            approximate.y[p[2]] = 1.0;
            approximate.x[p[3]] = 0.0;
            approximate.y[p[3]] = 4.0;
            PlacementDetail result;
            IDetailPlacer placer = new CrossReductPlacer();
            placer.Place(design, approximate, out result);
        }
    }
}