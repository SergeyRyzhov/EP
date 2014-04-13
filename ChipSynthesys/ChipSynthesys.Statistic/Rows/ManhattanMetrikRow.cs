using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChipSynthesys.Statistic.Common;
using ChipSynthesys.Statistic.Interfaces;
using PlaceModel;

namespace ChipSynthesys.Statistic.Rows
{
    class ManhattanMetrikRow : ISatisticRow<double>
    {
        public Design Design { get; set; }

        public PlacementDetail DetailPlacement { get; set; }

        public PlacementGlobal GlobalPlacement { get; set; }

        public static string Key { get { return StatisticKeys.ManhattanMetrik; } }

        string ISatisticRow<double>.Key
        {
            get { return Key; }
        }

        public double Compute()
        {
            if (DetailPlacement != null)
            {
                return ComputeMetrik(Design, DetailPlacement);
            }
            return ComputeMetrik(Design, GlobalPlacement);
        }

        private static int ComputeMetrik(Design design, PlacementDetail placement)
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
        
        private static double ComputeMetrik(Design design, PlacementGlobal placement)
        {
            double summ = 0;

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
    }
}
