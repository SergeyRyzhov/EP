using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetailPlacer.Algorithm;
using PlaceModel;


using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ChipSynthesys.UnitTests.PlaceModel
{
    [TestClass]
   public class SortCompTest
    {
        [TestMethod]
        public void SortTests()
        {
            var order = new CompontsOrderer();
            var p = new Component.Pool();
            p.Add(3,3);
            p.Add(3,3);
            p.Add(5,3);
            p.Add(2,3);
            p.Add(3,2);
            p.Add(2,2);
            var n = new Net.Pool();
            var design = new Design(new Field(0,0,20,20),p,n);
            var perm = new int[6];
            for (var i = 0; i < 6; i++) perm[i] = 0;
            var placement= new PlacementGlobal(design);
           
            placement.x[design.components[0]] = 2;
            placement.y[design.components[0]] = 2;
            placement.x[design.components[1]] = 4;
            placement.y[design.components[1]] = 4;
            placement.x[design.components[2]] = 6;
            placement.y[design.components[2]] = 6;
            placement.x[design.components[3]] = 9;
            placement.y[design.components[3]] = 2;
            placement.x[design.components[4]] = 10;
            placement.y[design.components[4]] = 1;
            placement.x[design.components[5]] = 12;
            placement.y[design.components[5]] = 5;
           
            
            order.SortComponents(design,placement,new PlacementDetail(design),design.components,ref perm);
            var res = new int[] { 1, 3, 5, 0, 2, 4 };
            for (var i = 0; i < 6; i++)
            {
                Assert.AreEqual(res[i], perm[i]);
            }
         }

    }
}
