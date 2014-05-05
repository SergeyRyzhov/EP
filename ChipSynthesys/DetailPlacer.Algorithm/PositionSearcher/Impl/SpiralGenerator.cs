using System;
using System.Collections.Generic;
using DetailPlacer.Algorithm.CriterionPositionSearcher;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public static class SpiralGenerator
    {
        public static IEnumerable<Point> DivergingSpiral(int h, int w)
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

        public static IEnumerable<Point> UnwindingSpiral(int h, int w, int sx, int sy)
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
    }
}