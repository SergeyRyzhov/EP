using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ChipSynthesys.Common;
using ChipSynthesys.Common.Classes;
using PlaceModel;
using Point = DetailPlacer.Algorithm.CriterionPositionSearcher.Point;

namespace DetailPlacer.Algorithm
{
    public class ForceDirectedDetailPlacer : IDetailPlacer
    {
        private readonly int m_forceDepth;

        private readonly int m_maxIteration;

        public ForceDirectedDetailPlacer()
            : this(TestsConstants.ForceDirectedMaxIteration)
        {
        }

        public ForceDirectedDetailPlacer(int iterations)
        {
            m_forceDepth = 1;
            m_maxIteration = iterations;
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            var forceDepth = m_forceDepth;

            var localResultDetail = new PlacementDetail(design);

            var cm = int.MaxValue;
            var ca = int.MaxValue;
            var workPlacement = new PlacementDetail(design);


            Parallel.ForEach(
                design.components,
                c =>
                    {
                        workPlacement.x[c] = (int)Math.Round(approximate.x[c]);
                        workPlacement.y[c] = (int)Math.Round(approximate.y[c]);
                        workPlacement.placed[c] = approximate.placed[c];

                        localResultDetail.x[c] = workPlacement.x[c];
                        localResultDetail.y[c] = workPlacement.y[c];
                        localResultDetail.placed[c] = approximate.placed[c];
                    });

            int maxIteration = m_maxIteration;

            while (Iteration(design, workPlacement, forceDepth) && maxIteration > 0)
            {
                maxIteration--;
                var m = CriterionHelper.ComputeMetrik(design, workPlacement);
                var a = CriterionHelper.AreaOfCrossing(design, workPlacement);


                if (m < cm || a < ca)
                {
                    forceDepth = m_forceDepth;
                    cm = m;
                    ca = a;


                    Parallel.ForEach(
                        design.components,
                        c =>
                            {
                                localResultDetail.x[c] = workPlacement.x[c];
                                localResultDetail.y[c] = workPlacement.y[c];
                                localResultDetail.placed[c] = approximate.placed[c];
                            });
                }
                else
                {
                    if (cm == m && ca == 0)
                    {
                        break;
                    }

                    forceDepth++;
                }
            }

            result = localResultDetail;
        }

        public bool Iteration(Design design, PlacementDetail result, int forceDepth)
        {
            var noChanges = true;
            CriterionHelper.MarkPosition(design, result, design.components[0], 0, 0);

            Parallel.ForEach(
                design.components,
                c =>
                    {
                        int x = result.x[c];
                        int y = result.y[c];

                        Point[] directions = GenerateForces(c, x, y, forceDepth).ToArray();
                        var dd = new SortedDictionary<DirectionInfo, int>(new DirectionComparer());


                        bool infoInitialized = false;
                        for (int i = 0; i < directions.Length; i++)
                        {
                            int cx = directions[i].X;
                            int cy = directions[i].Y;

                            if (cx < design.field.beginx || cy < design.field.beginy
                                || cx > design.field.cellsx - c.sizex || cy > design.field.cellsy - c.sizey)
                            {
                                continue;
                            }


                            DirectionInfo directionInfo = null;

                            directionInfo = new DirectionInfo(i)
                                                {
                                                    Mark =
                                                        CriterionHelper.MarkPosition(
                                                            design,
                                                            result,
                                                            c,
                                                            cx,
                                                            cy),
                                                    Area =
                                                        CriterionHelper.AreaOfCrossing(
                                                            design,
                                                            result,
                                                            c,
                                                            cx,
                                                            cy)
                                                };



                            dd.Add(directionInfo, i);
                            infoInitialized = true;

                        }

                        if (infoInitialized)
                        {
                            var index = dd.Values.First();
                            noChanges = noChanges && x == directions[index].X && y == directions[index].Y;
                            result.x[c] = directions[index].X;
                            result.y[c] = directions[index].Y;
                        }
                    });

            return !noChanges;
        }

        protected virtual IEnumerable<Point> GenerateForces(Component component, double x, double y, int forceDepth)
        {
            return GenerateForces(component, (int)Math.Round(x), (int)Math.Round(y), forceDepth);
        }

        protected virtual IEnumerable<Point> GenerateForces(Component component, int x, int y, int forceDepth)
        {
            yield return new Point(x, y);
            //            for (int i = Math.Max(component.sizex,component.sizey); i > 0; i--)
            for (int i = forceDepth; i > 0; i--)
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

            public DirectionInfo(int id)
            {
                this.Id = id;
            }

            public int Id { get; private set; }
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
                return x.Id == 0 ? 1 : y.Id == 0 ? -1 : x.Id == y.Id ? 0 : x.Id < y.Id ? 1 : -1;
            }
        }
    }
}
