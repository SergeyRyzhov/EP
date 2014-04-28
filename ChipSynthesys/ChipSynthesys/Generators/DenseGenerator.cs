using ChipSynthesys.Common.Randoms;
using PlaceModel;
using System;

namespace ChipSynthesys.Common.Generators
{
    public class DenseGenerator : IGenerator
    {
        private readonly IGenerator m_simple;

        public DenseGenerator()
        {
            m_simple = new RandomGenerator();
        }

        public void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design)
        {
            m_simple.NextDesign(components, nets, maxNetSize, percent, maxSizeX, maxSizeY, out design);
        }

        public void NextDesign(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY,
            out Design design)
        {
            m_simple.NextDesign(components, nets, maxNetSize, percent, maxSizeX, maxSizeY, out design);
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY,
            int width, int height, out Design design, out PlacementGlobal placement)
        {
            NextDesignWithPlacement(components, nets, new RangeRandom(1, maxNetSize), percent, new RangeRandom(1, maxSizeX),
                new RangeRandom(1, maxSizeY), width, height, out design, out placement);
        }

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX,
            IRandom<int> maxSizeY, int width, int height, out Design design, out PlacementGlobal placement)
        {
            m_simple.NextDesign(components, nets, maxNetSize, percent, maxSizeX, maxSizeY, out design);
            placement = new PlacementGlobal(design);

            double r = 2.0;
            int a = 0;
            double cx = design.field.cellsx / 2.0;
            double cy = design.field.cellsy / 2.0;

            foreach (Component c in design.components)
            {
                placement.x[c] = r * Math.Cos(a) + cx;
                placement.y[c] = r * Math.Sin(a) + cy;
                placement.placed[c] = true;
                a += 30;
                if (a == 360)
                {
                    r += 2;
                }
            }
        }
    }
}