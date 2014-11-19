using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using PlaceModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DetailPlacer.Algorithm.PositionSorter.Impl
{
    public class PositionsSorter : IPositionsSorter
    {
        public override string ToString()
        {
            return "Bubble sort with" + m_positionComparer.ToString();
        }

        internal readonly IPositionComparer m_positionComparer;

        public PositionsSorter(IPositionComparer positionComparer)
        {
            m_positionComparer = positionComparer;
        }

        protected class PosInfo
        {
            public PosInfo(int id)
            {
                Id = id;
            }

            public readonly int Id;
            public int X;
            public int Y;
        }

        protected class PosComparer : IComparer<PosInfo>
        {
            private readonly Func<int, int, int, int, bool> m_compare;

            public PosComparer(Func<int, int, int, int, bool> compare)
            {
                m_compare = compare;
            }

            public int Compare(PosInfo x, PosInfo y)
            {
                //-1 x < y хуже, 0 x == y, 1 x > y лучше
                return m_compare(x.X, x.Y, y.X, y.Y) ? 1 : 0;
            }
        }

        public void SortPositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, int[] x, int[] y, ref int[] perm)
        {
            int length = x.Length;
            //var mask = new int[length];

            var indx = new List<PosInfo>();
            for (int i = 0; i < length; i++)
            {
                indx.Add(new PosInfo(i) { X = x[i], Y = y[i] });
            }
            indx.Sort(new PosComparer((a, b, c, d) => m_positionComparer.Better(design, approximate, result, current, a, b, c, d)));

            var ind = indx.Select(i => i.Id).ToArray();
            for (int i = 0; i < length; i++)
            {
                perm[i] = ind[i];
            }
            //return;
            /*for (int i = 0; i < length; i++)
            {
                int best = i;
                for (int j = i+1; j < length; j++)
                {
                    if (mask[j] == 1)
                        continue;

                    if (m_positionComparer.Better(design, approximate, result, current, x[j], y[j], x[best], y[best]))
                        best = j;
                }
                perm[i] = best;
                mask[best] = 1;
            }*/
        }
    }
}