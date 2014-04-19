using System;
using System.Collections.Generic;
using System.Linq;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class DivergingSpiralPositionSearcher : PositionSearcherBase
    {
        protected class Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X;
            public int Y;
        }
        public override string ToString()
        {
            return "Перебор доступных позиций по спирали от краёв области";
        }

        private static IEnumerable<Point> DivergingSpiral(int h, int w)
        {
            int summ = w * h;
            int cy = 0;
            int cx = 0;
            int count = 1;
            while (h > 0)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < ((w < h) ? h : w); x++)
                    {
                        if (y == 0 && x < w - cx && count++ <= summ)
                            yield return new Point(y + cy, x + cx);
                        if (y == 1 && x < h - cy && x != 0 && count++ <= summ)
                            yield return new Point(x + cy, w - 1);
                        if (y == 2 && x < w - cx && x != 0 && count++ <= summ)
                            yield return new Point(h - 1, w - (x + 1));
                        if (y == 3 && x < h - (cy + 1) && x != 0 && count++ <= summ)
                            yield return new Point(h - (x + 1), cy);
                    }
                }
                h--;
                w--;
                cy++;
                cx++;
            }
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

            return DivergingSpiral(m, n).Any(point => checker(point.X, point.Y));
        }

        public DivergingSpiralPositionSearcher()
            : base(64)
        {
        }

        public DivergingSpiralPositionSearcher(int maxCount)
            : base(maxCount)
        {
        }
    }
}