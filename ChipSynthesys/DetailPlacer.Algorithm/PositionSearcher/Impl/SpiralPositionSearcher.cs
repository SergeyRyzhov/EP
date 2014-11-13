using System;

using ChipSynthesys.Common;

using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
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
                throw new InvalidOperationException(string.Format("Incorrect input {0}", PositionAmount));
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

            int maxWidthSide = Math.Max(mask.Width - sx - 1, sx);
            int maxHeightSide = Math.Max(mask.Height - sy - 1, sy);
            int maxSide = Math.Max(maxWidthSide, maxHeightSide);

            for (int side = 1; side <= maxSide; side++)
            {
                for (int i = sx - side; i <= sx + side - 1; i++)
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
                }

                for (int j = sy - side; j <= sy + side - 1; j++)
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
                }

                for (int i = sx + side; i >= sx - side + 1; i--)
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
                }

                for (int j = sy + side; j >= sy - side + 1; j--)
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
                }
            }

            return amount == resX.Length;
        }

        public override string ToString()
        {
            return "Search position by spiral";
        }
    }
}