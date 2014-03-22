using PlaceModel;
using System;

namespace ChipSynthesys
{
    /// <summary>
    /// Генератор тестовых примеров интегральных схем
    /// </summary>
    public class RandomGenerator : IGenerator
    {
        public void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design)
        {

            var fullness = 0;
            var rnd = new Random();
            var c = new Component.Pool();
            for (var i = 0; i < components; i++)
            {
                c.Add(rnd.Next(maxSizeX) + 1, rnd.Next(maxSizeY));
                fullness += c[i].sizex * c[i].sizey;
            }
            var cells = (int)Math.Ceiling(Math.Sqrt(fullness * (100.0 / percent)));
            var n = new Net.Pool();
            for (var j = 0; j < nets; j++)
            {
                n.Add(new Component[rnd.Next(maxNetSize - 1) + 2]);
                for (var i = 0; i < n[j].items.Length; i++)
                {
                    var q = rnd.Next(components);
                    n[j].items[i] = c[q];
                }
            }
            design = new Design(new Field(0, 0, cells, cells), c, n);
        }

        public void NextDesign(int components, IRandom<int> nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY,
            out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY,
            out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX,
            IRandom<int> maxSizeY, out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }
    }
}