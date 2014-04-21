using System;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class LinearPositionSearcher : PositionSearcherBase
    {
        public override string ToString()
        {
            return "�������� ������� ��������� �������";
        }

        protected override bool DetourPositions(Design design, PlacementGlobal approximate, PlacementDetail result,
            Component current, int n, int m, int[,] mask, Func<int, int, bool> addIfTheLimitIsNotExceeded)
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

        public LinearPositionSearcher()
            : base(64)
        {
        }

        public LinearPositionSearcher(int maxCount)
            : base(maxCount)
        {
        }
    }
}