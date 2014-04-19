using System;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class LinearPositionSearcher : PositionSearcherBase
    {
        public override string ToString()
        {
            return "Линейный перебор доступных позиций";
        }

        public LinearPositionSearcher() : base(64)
        {
        }

        public LinearPositionSearcher(int maxCount) : base(maxCount)
        {
        }

        protected override bool DetourPositions(int n, int m, int[,] mask, Func<int, int, bool> addIfTheLimitIsNotExceeded)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
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
    }
}