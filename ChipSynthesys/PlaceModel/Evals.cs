using System;

namespace PlaceModel
{
    /// <summary>
    /// Метрики, используемые для оценки качества размещения
    /// </summary>
    public class Evals
    {
        /// <summary>
        /// Манхэттоновская метрика
        /// </summary>
        /// <param name="d">Описание интегральной схемы</param>
        /// <param name="p">Глобальное размещение</param>
        /// <returns></returns>
        static public double Manhattan(Design d, PlacementGlobal p)
        {
            double res = 0.0;
            foreach (var n in d.nets)
            {
                Component c = n.items[0];
                double l = p.x[c];
                double r = p.x[c] + c.sizex;
                double t = p.y[c];
                double b = p.y[c] + c.sizey;
                for (int i = 1; i < n.items.Length; i++)
                {
                    c = n.items[i];
                    l = Math.Min(l, p.x[c]);
                    r = Math.Max(r, p.x[c] + c.sizex);
                    t = Math.Min(t, p.y[c]);
                    b = Math.Min(b, p.y[c] + c.sizey);
                }
                res += (r - l) + (b - t);
            }
            return res;
        }

        /// <summary>
        /// Манхэттоновская метрика
        /// </summary>
        /// <param name="d">Описание интегральной схемы</param>
        /// <param name="p">Детальное размещение</param>
        /// <returns></returns>
        static public int Manhattan(Design d, PlacementDetail p)
        {
            int res = 0;
            foreach (var n in d.nets)
            {
                Component c = n.items[0];
                int l = p.x[c];
                int r = p.x[c] + c.sizex;
                int t = p.y[c];
                int b = p.y[c] + c.sizey;
                for (int i = 1; i < n.items.Length; i++)
                {
                    c = n.items[i];
                    l = Math.Min(l, p.x[c]);
                    r = Math.Max(r, p.x[c] + c.sizex);
                    t = Math.Min(t, p.y[c]);
                    b = Math.Min(b, p.y[c] + c.sizey);
                }
                res += (r - l) + (b - t);
            }
            return res;
        }
    }
}