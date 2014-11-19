using System;
using System.Linq;
using PlaceModel;

namespace ChipSynthesys.Common.Classes
{
    public class CriterionHelper
    {

        public static int ComputeMetrik(Design design, PlacementDetail placement)
        {
            int summ = 0;

            foreach (var n in design.nets)
            {
                var first = n.items.Where(cc => placement.placed[cc]).Select((com, i) => new { Com = com, I = i }).FirstOrDefault();
                if (first == null)
                {
                    continue;
                }

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

            return summ;
        }

        public static double ComputeMetrik(Design design, PlacementGlobal placement)
        {
            double summ = 0;

            foreach (var n in design.nets)
            {
                var first =
                    n.items//.Where(cc => placement.placed[cc])
                        .Select((com, i) => new { Com = com, I = i })
                        .FirstOrDefault();
                if (first == null)
                {
                    continue;
                }

                var c = first.Com;

                var l = placement.x[c];
                var r = placement.x[c] + c.sizex;
                var t = placement.y[c];
                var b = placement.y[c] + c.sizey;
                for (var i = first.I + 1; i < n.items.Length; i++)
                {
                    c = n.items[i];
                    /*if (!placement.placed[c])
                    {
                        continue;
                    }*/

                    l = Math.Min(l, placement.x[c]);
                    r = Math.Max(r, placement.x[c] + c.sizex);
                    t = Math.Min(t, placement.y[c]);
                    b = Math.Min(b, placement.y[c] + c.sizey);
                }

                summ += (r - l) + (b - t);
            }

            return summ;
        }

        public static int AreaOfCrossing(Design design, PlacementDetail placement)
        {
            int area = 0;
            for (int i = 0; i < design.components.Length; i++)
            {
                var current = design.components[i];
                for (int j = i+1; j < design.components.Length; j++)
                {
                    var other = design.components[j];
                    area += AreaOfCrossing(placement.x[current], placement.y[current],
                        current.sizex, current.sizey, placement.x[other], placement.y[other],
                        other.sizex, other.sizey);
                }
            }
            return area;
        }
        public static double AreaOfCrossing(Design design, PlacementGlobal placement)
        {
            double area = 0;
            for (int i = 0; i < design.components.Length; i++)
            {
                var current = design.components[i];
                for (int j = i+1; j < design.components.Length; j++)
                {
                    var other = design.components[j];
                    area += AreaOfCrossing(placement.x[current], placement.y[current],
                        current.sizex, current.sizey, placement.x[other], placement.y[other],
                        other.sizex, other.sizey);
                }
            }
            return area;
        }

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
                if (!placement.placed[other] || other.id == current.id)
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
            if (x1 + w1 <= x2 || x2 + w2 <= x1 || y1 + h1 <= y2 || y2 + h2 <= y1)
                return 0;
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1 + w1, x2 + w2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1 + h1, y2 + h2);

            var Area = (w1 + w2 - (maxX - minX)) * (h1 + h2 - (maxY - minY));
            return Area;
        }

        protected static double AreaOfCrossing(double x1, double y1, double w1, double h1, double x2, double y2, double w2, double h2)
        {
            if (x1 + w1 <= x2 || x2 + w2 <= x1 || y1 + h1 <= y2 || y2 + h2 <= y1)
                return 0;
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1 + w1, x2 + w2);
            var minY = Math.Min(y1, y2);
            var maxY = Math.Max(y1 + h1, y2 + h2);

            var Area = (w1 + w2 - (maxX - minX)) * (h1 + h2 - (maxY - minY));
            return Area;
        }

        public static int CountOfCrossings(Design design, PlacementDetail placement)
        {
            var countOfCrossings = 0;
            for (var i = 0; i < design.components.Length; i++)
            {
                var r1 = design.components[i];
                var x1 = placement.x[r1];
                var y1 = placement.y[r1];
                for (var j = i + 1; j < design.components.Length; j++)
                {
                    var r2 = design.components[j];
                    var x2 = placement.x[r2];
                    var y2 = placement.y[r2];
                    if (x1 + r1.sizex <= x2 || x2 + r2.sizex <= x1
                        || y1 + r1.sizey <= y2 || y2 + r2.sizey <= y1) continue;
                    countOfCrossings++;
                }
            }
            return countOfCrossings;
        }

        public static int CountOfCrossings(Design design, PlacementGlobal placement)
        {
            var countOfCrossings = 0;
            for (var i = 0; i < design.components.Length; i++)
            {
                var r1 = design.components[i];
                var x1 = placement.x[r1];
                var y1 = placement.y[r1];
                for (var j = i + 1; j < design.components.Length; j++)
                {
                    var r2 = design.components[j];
                    var x2 = placement.x[r2];
                    var y2 = placement.y[r2];
                    if (x1 + r1.sizex <= x2 || x2 + r2.sizex <= x1
                        || y1 + r1.sizey <= y2 || y2 + r2.sizey <= y1) continue;
                    countOfCrossings++;
                }
            }
            return countOfCrossings;
        }
    }
}