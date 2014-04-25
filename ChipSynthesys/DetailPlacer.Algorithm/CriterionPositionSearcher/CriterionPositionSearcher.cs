using System;
using System.Collections.Generic;
using System.Linq;
using PlaceModel;

namespace DetailPlacer.Algorithm.CriterionPositionSearcher
{
    public class CriterionPositionSearcher : ICriterionPositionSearcher
    {
        private readonly Design m_design;
        private readonly PlacementGlobal m_approximate;
        private readonly PlacementDetail m_detail;
        private readonly Component m_current;
        private List<Point> m_points;

        public CriterionPositionSearcher(Design design, PlacementGlobal approximate, PlacementDetail detail, Component current)
        {
            m_design = design;
            m_approximate = approximate;
            m_detail = detail;
            m_current = current;
        }

        public void Build()
        {
            int w = m_design.field.cellsx;
            int h = m_design.field.cellsy;

            var x = (int)Math.Ceiling(m_approximate.x[m_current]);
            var y = (int)Math.Ceiling(m_approximate.x[m_current]);

            m_points = UnwindingSpiral(h, w, x, y).ToList();
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

        public IEnumerable<Point> Search(IPointComparer comparer)
        {
            var sorted = m_points.ToList();
            
            sorted.Sort(comparer);
            return sorted;
        }
    }
}