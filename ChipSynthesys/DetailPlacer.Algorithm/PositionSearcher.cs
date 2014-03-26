using System;
using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public class PositionSearcher : IPositionSearcher
    {
        private Random m_rnd;

        public PositionSearcher()
        {
            m_rnd = new Random();
        }

        public void AlvailablePositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, out int[] x, out int[] y, out bool hasPosition)
        {
            x = new[] {m_rnd.Next(design.field.cellsx)};
            y = new[] {m_rnd.Next(design.field.cellsy)};
            hasPosition = true;
        }
    }
}