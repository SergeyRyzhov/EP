using ChipSynthesys.Common;
using PlaceModel;
using System;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    /*public class SpiralPositionSearcher : PositionSearcherBase
    {
        private readonly bool m_twisting;

        public override string ToString()
        {
            return string.Format("Спираль от {0}",
                m_twisting ? "краёв области" : "текущих координат");
        }

        protected override bool DetourPositions(Design design, PlacementGlobal approximate, PlacementDetail result,
            Component current, int n, int m, Mask mask, Func<int, int, bool> addIfTheLimitIsNotExceeded)
        {
            var cx = (int)Math.Ceiling(approximate.x[current]);
            var cy = (int)Math.Ceiling(approximate.x[current]);
            var checker = new Func<int, int, bool>((a, b) =>
            {
                try
                {
                    if (!addIfTheLimitIsNotExceeded(a, b))
                    {
                        return true;
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
            : this(TestsConstants.SearchSize, false)
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
    }*/
    public class SpiralPositionSearcher : IPositionSearcher
    {
        public SpiralPositionSearcher()
            : this(TestsConstants.SearchSize)
        {
        }

        public SpiralPositionSearcher(int maxCount)
        {
            PositionAmount = maxCount;
        }

        public int PositionAmount { get; set; }

        public bool AlvailablePositions(Mask mask, Component current, int startX, int startY, int[] resX, int[] resY)
        {
            if (resX.Length != PositionAmount || resY.Length != PositionAmount)
            {
                throw new InvalidOperationException(string.Format("Массивы для результатов должен быть размера {0}",
                    PositionAmount));
            }

            int amount = 0;
            var sx = startX;
            var sy = startY;
            if (mask.CanPlaceH(current, sx, sy))
            {
                resX[amount] = sx;
                resY[amount] = sy;
                if (++amount == resX.Length)
                {
                    return true;
                }
            }


            int maxWSide = Math.Max(mask.Width - sx - 1, sx);
            int maxHSide = Math.Max(mask.Height - sy - 1, sy);
            int maxSide = Math.Max(maxWSide, maxHSide);

            for (int side = 1; side <= maxSide; side++)
            {
                for (int i = sx - side; i <= sx + side - 1; i++)
                {
                    //if (i >= 0 && sy - side >= 0)
                    {
                        if (mask.CanPlaceH(current, i, sy - side))
                        {
                            resX[amount] = i;
                            resY[amount] = sy - side;
                            if (++amount == resX.Length)
                            {
                                return true;
                            }
                        }

                        //yield return new Point(i, sy - side);
                    }
                }
                for (int j = sy - side; j <= sy + side - 1; j++)
                {
                    //if (sx + side >= 0 && j >= 0)
                    {
                        if (mask.CanPlaceV(current, sx + side, j))
                        {
                            resX[amount] = sx + side;
                            resY[amount] = j;
                            if (++amount == resX.Length)
                            {
                                return true;
                            }
                        }
                        //yield return new Point(sx + side, j);
                    }
                }
                for (int i = sx + side; i >= sx - side + 1; i--)
                {
                    //if (i >= 0 && sy + side >= 0)
                    {
                        if (mask.CanPlaceH(current, i, sy + side))
                        {
                            resX[amount] = i;
                            resY[amount] = sy + side;
                            if (++amount == resX.Length)
                            {
                                return true;
                            }
                        }
                        //yield return new Point(i, sy + side);
                    }
                }
                for (int j = sy + side; j >= sy - side + 1; j--)
                {
                    //if (sx - side >= 0 && j >= 0)
                    {
                        if (mask.CanPlaceV(current, sx - side, j))
                        {
                            resX[amount] = sx - side;
                            resY[amount] = j;
                            if (++amount == resX.Length)
                            {
                                return true;
                            }
                        }
                        //yield return new Point(sx - side, j);
                    }
                }
            }
            return amount == resX.Length;
        }

        public override string ToString()
        {
            return "Поиск доступных позиций по спирали";
        }
    }
}