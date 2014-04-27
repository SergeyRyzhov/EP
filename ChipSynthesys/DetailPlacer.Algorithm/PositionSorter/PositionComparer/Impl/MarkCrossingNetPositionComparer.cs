using PlaceModel;
using System;
using System.Linq;

namespace DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl
{
    public class MarkCrossingNetPositionComparer : IPositionComparer
    {
        protected class PosInfo
        {
            public PosInfo(int id)
            {
                Id = id;
            }

            public readonly int Id;
            public int Mark;
            public int Area;
        }

        public override string ToString()
        {
            return "Сравнение позиций с учётом вклада в критерий и минимизации пересечений";
        }

        public bool Better(Design design, PlacementGlobal approximate, PlacementDetail placement, Component current, int firstX, int firstY, int secondX,
            int secondY)
        {
            int firstMark = MarkPosition(design, placement, current, firstX, firstY);
            int secondMark = MarkPosition(design, placement, current, secondX, secondY);

            int firstArea = MarkArea(design, approximate, placement, current, firstX, firstY);
            int secondArea = MarkArea(design, approximate, placement, current, secondX, secondY);

            if (firstMark < secondMark)
            {
                return true;
            }
            if (firstMark == secondMark)
            {
                return firstArea < secondArea;
            }
            return false;
        }

        private static int MarkArea(Design design, PlacementGlobal approximate, PlacementDetail result,
            Component current, int x, int y)
        {
            int res = 0;
            foreach (Component c in design.components)
            {
                if (c.id == current.id)
                {
                    continue;
                }

                int ox = result.placed[c] ? result.x[c] : (int)Math.Round(approximate.x[c]);
                int oy = result.placed[c] ? result.y[c] : (int)Math.Round(approximate.y[c]);

                var cx = Math.Max(x, ox);
                var cy = Math.Max(y, oy);
                var cxw = Math.Min(x + current.sizex, ox + c.sizex);
                var cyh = Math.Min(y + current.sizey, oy + c.sizey);
                if ((cxw - cx > 0) && (cyh - cy > 0))
                    res += (cxw - cx) * (cyh - cy);
            }
            return res;
        }

        protected virtual int[,] GenerateMask(Design design, PlacementGlobal approximate, PlacementDetail result, int n, int m)
        {
            var mask = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    mask[i, j] = 0;
                }
            }

            foreach (Component component in design.components)
            {
                if (result.placed[component])
                {
                    var cx = result.x[component];
                    var cy = result.y[component];

                    int ph = component.sizey;
                    int pw = component.sizex;

                    for (int k = 0; k < ph; k++)
                    {
                        for (int l = 0; l < pw; l++)
                        {
                            mask[cx + l, cy + k] = 1;
                        }
                    }
                }
                /*else
                {
                    var cx = (int) Math.Round(approximate.x[component]);
                    var cy = (int) Math.Round(approximate.y[component]);

                    int ph = component.sizey;
                    int pw = component.sizex;

                    for (int k = 0; k < ph; k++)
                    {
                        for (int l = 0; l < pw; l++)
                        {
                            mask[cx + l, cy + k] = 1;
                        }
                    }
                }*/
            }
            return mask;
        }

        private static int MarkPosition(Design design, PlacementDetail placement, Component current, int x, int y)
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
    }
}