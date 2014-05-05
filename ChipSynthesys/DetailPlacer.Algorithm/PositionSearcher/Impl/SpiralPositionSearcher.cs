using PlaceModel;
using System;
using System.Linq;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class SpiralPositionSearcher : PositionSearcherBase
    {
        private readonly bool m_twisting;
        
        public override string ToString()
        {
            return string.Format("Спираль от {0}",
                m_twisting ? "краёв области" : "текущих координат");
        }

        protected override bool DetourPositions(Design design, PlacementGlobal approximate, PlacementDetail result,
            Component current, int n, int m, int[,] mask, Func<int, int, bool> addIfTheLimitIsNotExceeded)
        {
            var cx = (int)Math.Ceiling(approximate.x[current]);
            var cy = (int)Math.Ceiling(approximate.x[current]);
            var checker = new Func<int, int, bool>((a, b) =>
            {
                try
                {
                    if (mask[a, b] == 0)
                    {
                        if (!addIfTheLimitIsNotExceeded(a, b))
                        {
                            return true;
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
                return false;
            });

            return m_twisting
                ? SpiralGenerator.DivergingSpiral(m, n).Any(point => checker(point.X, point.Y))
                : SpiralGenerator.UnwindingSpiral(m, n, cx, cy).Any(point => checker(point.X, point.Y));
        }

        public SpiralPositionSearcher()
            : this(16, false)
        {
        }

        public SpiralPositionSearcher(int maxCount)
            : this(maxCount, false)
        {
        }

        public SpiralPositionSearcher(int maxCount, bool enableTwisting)
            : base(maxCount)
        {
            m_twisting = enableTwisting;
        }
    }
}