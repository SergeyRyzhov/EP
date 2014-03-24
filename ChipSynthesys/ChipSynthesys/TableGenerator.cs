using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaceModel;

namespace ChipSynthesys
{
    class TableGenerator: IGenerator
    {
        public void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesign(int components, IRandom<int> nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design)
        {
            int fullness = 0;
            var r= new Random();
            var c = new Component.Pool();

            for (var i = 0; i < components; i++)
            {
                c.Add(maxSizeX.Next(), maxSizeY.Next());//?? maxSizeX.Next()+1, maxSizeY.Next()+1
                fullness += c[i].sizex * c[i].sizey;
            }
            var cells = (int)Math.Ceiling(Math.Sqrt(fullness * (100.0 / percent)));
            var n = new Net.Pool();

            var countOfNets = nets.Next();
            for (var i = 0; i < countOfNets; i++)
            {
                n.Add(new Component[maxNetSize.Next()]);//?? maxNetSize.Next()+1
                for (var j = 0; j < n[i].items.Length; j++)
                {
                    var id = r.Next(components);
                    n[i].items[j] = c[id];
                }
            }

            design = new Design(new Field(0, 0, cells, cells), c, n);
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }
    }
}
