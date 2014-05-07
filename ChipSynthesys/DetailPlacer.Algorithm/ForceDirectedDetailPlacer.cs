using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ChipSynthesys.Common.Classes;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;
using PlaceModel;
using Point = DetailPlacer.Algorithm.CriterionPositionSearcher.Point;

namespace DetailPlacer.Algorithm
{
    public class ForceDirectedDetailPlacer : IDetailPlacer
    {
        private int m_forceStep;

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            m_forceStep = 1;

            result = new PlacementDetail(design);

            foreach (Component c in design.components)
            {
                result.x[c] = (int)Math.Round(approximate.x[c]);
                result.y[c] = (int)Math.Round(approximate.y[c]);
                result.placed[c] = true;
            }
            int maxIteration = design.components.Length;
            DrawerHelper.SimpleDraw(design, result, new Size(600, 600), new Bitmap(600, 600), string.Format("FD {0:##}.png", maxIteration));
            while (Iteration(design, result) && maxIteration > 0)
            {
                maxIteration--;
                DrawerHelper.SimpleDraw(design, result, new Size(600, 600), new Bitmap(600, 600), string.Format("FD {0:##}.png", maxIteration));
            }
        }

        public bool Iteration(Design design, PlacementDetail result)
        {
            var noChanges = true;
            foreach (Component c in design.components)
            {
                int x = result.x[c];
                int y = result.y[c];

                Point[] directions = GenerateForces(c, x, y).ToArray();
                var dd = new SortedDictionary<DirectionInfo, int>(new DirectionComparer());

                for (int i = 0; i < directions.Length; i++)
                {
                    int cx = directions[i].X;
                    int cy = directions[i].Y;

                    if (cx < 0 || cy < 0 || cx > design.field.cellsx - c.sizex || cy > design.field.cellsy - c.sizey)
                        continue;
                    var directionInfo = new DirectionInfo(i)
                    {
                        Mark = CriterionHelper.MarkPosition(design, result, c, cx, cy),
                        Area = CriterionHelper.AreaOfCrossing(design, result, c, cx, cy)
                    };
                    dd.Add(directionInfo, i);
                    
                }
                var indx = dd.Values.FirstOrDefault();
                noChanges = noChanges && x == directions[indx].X && y == directions[indx].Y;
                result.x[c] = directions[indx].X;
                result.y[c] = directions[indx].Y;
            }
            return !noChanges;
        }

        protected virtual IEnumerable<Point> GenerateForces(Component component, double x, double y)
        {
            return GenerateForces(component, (int)Math.Round(x), (int)Math.Round(y));
        }

        protected virtual IEnumerable<Point> GenerateForces(Component component, int x, int y)
        {
            yield return new Point(x, y);
            for (double a = 0; a < Math.PI * 2; a += Math.PI / 4)
            {
                yield return new Point(x + Math.Cos(a) * m_forceStep, y + Math.Sin(a) * m_forceStep);
            }
        }

        private class DirectionInfo
        {
            public int Mark;
            public int Area;
            public int Id { get; private set; }
            public DirectionInfo(int id)
            {
                Id = id;
            }
        }

        private class DirectionComparer : IComparer<DirectionInfo>
        {
            public int Compare(DirectionInfo x, DirectionInfo y)
            {
                if (x.Mark < y.Mark)
                {
                    return 1;
                }

                if (x.Mark > y.Mark)
                {
                    return -1;
                }

                if (x.Area < y.Area)
                {
                    return 1;
                }

                if (x.Area > y.Area)
                {
                    return -1;
                }
                return x.Id == y.Id ? 0 : 1;
            }
        }
    }
}
