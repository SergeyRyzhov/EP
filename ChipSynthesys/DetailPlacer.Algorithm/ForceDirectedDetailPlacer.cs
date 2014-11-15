using System;
using System.Collections.Generic;
using System.Linq;
using ChipSynthesys.Common.Classes;
using PlaceModel;
using Point = DetailPlacer.Algorithm.CriterionPositionSearcher.Point;

namespace DetailPlacer.Algorithm
{
    public class ForceDirectedDetailPlacer : IDetailPlacer
    {
        private int m_forceDepth;

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            const int forceDepth = 3;
            m_forceDepth = forceDepth; //todo parameter

            result = new PlacementDetail(design);

            var cm = int.MaxValue;
            var ca = int.MaxValue;
            var workPlacement = new PlacementDetail(design);

            foreach (Component c in design.components)
            {
                workPlacement.x[c] = (int)Math.Round(approximate.x[c]);
                workPlacement.y[c] = (int)Math.Round(approximate.y[c]);
                workPlacement.placed[c] = true;

                result.x[c] = workPlacement.x[c];
                result.y[c] = workPlacement.y[c];
                result.placed[c] = workPlacement.placed[c];
            }

            int maxIteration = design.components.Length * 3; //todo parameter
            //DrawerHelper.SimpleDraw(design, workPlacement, new Size(600, 600), new Bitmap(600, 600), string.Format("FD {0:##}.png", maxIteration));
            while (Iteration(design, workPlacement) && maxIteration > 0)
            {
                maxIteration--;
                //DrawerHelper.SimpleDraw(design, workPlacement, new Size(600, 600), new Bitmap(600, 600), string.Format("FD {0:##}.png", maxIteration));
                var m = CriterionHelper.ComputeMetrik(design, workPlacement);
                var a = CriterionHelper.AreaOfCrossing(design, workPlacement);
                if (m < cm || a < ca)
                {
                    m_forceDepth = forceDepth;
                    cm = m;
                    ca = a;
                    foreach (Component c in design.components)
                    {
                        result.x[c] = workPlacement.x[c];
                        result.y[c] = workPlacement.y[c];
                        result.placed[c] = workPlacement.placed[c];
                    }
                }
                else
                {
                    if (cm == m && ca == 0)
                        break;
                    m_forceDepth++;
                }

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
                var indx = dd.Values.First();
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
//            for (int i = Math.Max(component.sizex,component.sizey); i > 0; i--)
            for (int i = m_forceDepth; i > 0; i--)
            {
                for (double a = 0; a < Math.PI * 2; a += Math.PI / 4)
                {
                    yield return new Point(x + Math.Cos(a) * i, y + Math.Sin(a) * i);
                }
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
            public int Compare(DirectionInfo y, DirectionInfo x)
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
                return x.Id == 0 
                        ? 1 
                        : y.Id == 0 
                            ? -1 
                            : x.Id == y.Id 
                                ? 0
                                : x.Id < y.Id 
                                    ? 1 
                                    : -1;
            }
        }
    }
}
