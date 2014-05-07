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
            var res = 0.0;
            foreach (var n in d.nets)
            {
                var c = n.items[0];
                var l = p.x[c];
                var r = p.x[c] + c.sizex;
                var t = p.y[c];
                var b = p.y[c] + c.sizey;
                for (var i = 1; i < n.items.Length; i++)
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
            var res = 0;
            foreach (var n in d.nets)
            {
                var c = n.items[0];
                var l = p.x[c];
                var r = p.x[c] + c.sizex;
                var t = p.y[c];
                var b = p.y[c] + c.sizey;
                for (var i = 1; i < n.items.Length; i++)
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