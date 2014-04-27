using PlaceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class SpiralPositionSearcher : PositionSearcherBase
    {
        private readonly bool m_twisting;

        protected class Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X;
            public int Y;

            public override string ToString()
            {
                return string.Format("({0};{1})", X, Y);
            }
        }

        public override string ToString()
        {
            return string.Format("Перебор доступных позиций по спирали от {0}",
                m_twisting ? "краёв области" : "текущих координат");
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

        private static IEnumerable<Point> UnwindingSpiral(int h, int w, int sx, int sy)
        {
            int maxWSide = Math.Max(w - sx - 1, sx);
            int maxHSide = Math.Max(h - sy - 1, sy);
            int maxSide = Math.Max(maxWSide, maxHSide);

            for (int side = 1; side <= maxSide; side++)
            {
                for (int i = sx - side; i <= sx + side - 1; i++)
                {
                    if (i >= 0 && sy - side >= 0)
                        yield return new Point(i, sy - side);
                }
                for (int j = sy - side; j <= sy + side - 1; j++)
                {
                    if (sx + side >= 0 && j >= 0)
                        yield return new Point(sx + side, j);
                }
                for (int i = sx + side; i >= sx - side + 1; i--)
                {
                    if (i >= 0 && sy + side >= 0)
                        yield return new Point(i, sy + side);
                }
                for (int j = sy + side; j >= sy - side + 1; j--)
                {
                    if (sx - side >= 0 && j >= 0)
                        yield return new Point(sx - side, j);
                }
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

            return m_twisting
                ? DivergingSpiral(m, n).Any(point => checker(point.X, point.Y))
                : UnwindingSpiral(m, n, cx, cy).Any(point => checker(point.X, point.Y));
        }

        public SpiralPositionSearcher()
            : this(64, false)
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