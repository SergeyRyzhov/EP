using System;
using System.Linq;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl
{
    public class NetsPositionComparer : IPositionComparer
    {
        public override string ToString()
        {
            return "Сравнение позиций согласно суммарной Манхеттенской метрики";
        }

        public bool Better(Design design, PlacementDetail placement, Component current, int firstX, int firstY, int secondX,
            int secondY)
        {
            int firstMark = MarkPosition(design, placement, current, firstX, firstY);
            int secondMark = MarkPosition(design, placement, current, secondX, secondY);

            return firstMark < secondMark;
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
                var first = n.items.Where(cc => placement.placed[cc]).Select((com,i) => new {Com = com, I = i}).First();
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