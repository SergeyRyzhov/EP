using System;
using System.Linq;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl
{
    public class CriterionHelper
    {
        public static int MarkPosition(Design design, PlacementDetail placement, Component current, int x, int y)
        {
            int summ = 0;
            Net[] nets = design.Nets(current);
            bool oldValue = placement.placed[current];

            placement.x[current] = x;
            placement.y[current] = y;
            placement.placed[current] = true;

            foreach (var n in nets)
            {
                var first = n.items.Where(cc => placement.placed[cc]).Select((com, i) => new { Com = com, I = i }).First();
                var c = first.Com;

                var l = placement.x[c];
                var r = placement.x[c] + c.sizex;
                var t = placement.y[c];
                var b = placement.y[c] + c.sizey;
                for (var i = first.I + 1; i < n.items.Length; i++)
                {
                    c = n.items[i];
                    if (!placement.placed[c])
                    {
                        continue;
                    }
                    l = Math.Min(l, placement.x[c]);
                    r = Math.Max(r, placement.x[c] + c.sizex);
                    t = Math.Min(t, placement.y[c]);
                    b = Math.Min(b, placement.y[c] + c.sizey);
                }
                summ += (r - l) + (b - t);
            }

            placement.placed[current] = oldValue;

            return summ;
        }

        public static int AreaOfCrossing(Design design, PlacementDetail placement, Component current, int x, int y)
        {
            int area = 0;
            foreach (Component other in design.components)
            {
                if (!placement.placed[other])
                {
                    continue;
                }

                area += AreaOfCrossing(x, y, current.sizex, current.sizey, placement.x[other], placement.y[other],
                    other.sizex, other.sizey);
            }
            return area;
        }

        protected static int AreaOfCrossing(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
        {
            var x11 = Math.Max(Math.Min(x1, x1 + w1), Math.Min(x2, x2 + w2));
            var x12 = Math.Min(Math.Max(x1, x1 + w1), Math.Max(x2, x2 + w2));
            var y11 = Math.Max(Math.Min(y1, y1 + h1), Math.Min(y2, y2 + h2));
            var y12 = Math.Min(Math.Max(y1, y1 + h1), Math.Max(y2, y2 + h2));
            if ((x12 - x11 > 0) && (y12 - y11 > 0))
                return (x12 - x11) * (y12 - y11);
            return 0;
        }
    }
}