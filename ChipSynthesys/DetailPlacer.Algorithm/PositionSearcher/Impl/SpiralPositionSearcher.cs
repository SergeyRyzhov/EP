using System;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class SpiralPositionSearcher : PositionSearcherBase
    {
        public override string ToString()
        {
            return "Перебор доступных позиций по спирали от текущей позиции";
        }

        protected override bool DetourPositions(int n, int m, int[,] mask, Func<int, int, bool> addIfTheLimitIsNotExceeded)
        {
            //todo rewrite
            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (mask[i, j] == 0)
                    {
                        if (!addIfTheLimitIsNotExceeded(i, j))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public SpiralPositionSearcher() : base(64)
        {
        }

        public SpiralPositionSearcher(int maxCount) : base(maxCount)
        {
        }
    }
}