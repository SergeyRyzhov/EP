using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ChipSynthesys.Common.Classes;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;
using PlaceModel;
using Point = DetailPlacer.Algorithm.CriterionPositionSearcher.Point;
using PointF = DetailPlacer.Algorithm.CriterionPositionSearcher.PointF;

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
                DrawerHelper.SimpleDraw(design, result,new Size(600,600),new Bitmap(600,600), string.Format("FD {0:##}.png", maxIteration));
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
                int bestI = 0;
                int bestMark = design.field.cellsx*design.field.cellsy;
                for (int i = 0; i < directions.Length; i++)
                {
                    int mark = CriterionHelper.MarkPosition(design, result, c, directions[i].X, directions[i].Y);
                    if (mark < bestMark)
                    {
                        bestMark = mark;
                        bestI = i;
                    }
                }
                noChanges = noChanges && x == directions[bestI].X && y == directions[bestI].Y;
                result.x[c] = directions[bestI].X;
                result.y[c] = directions[bestI].Y;
            }
            return !noChanges;
        }

        protected virtual IEnumerable<PointF> GenerateForces(Component component, double x, double y)
        {
            yield return new PointF(x,y);
            for (double a = 0; a < Math.PI * 2; a += Math.PI / 4)
            {
                yield return new PointF(x + Math.Cos(a) * m_forceStep, y + Math.Sin(a) * m_forceStep);
            }
        }

        protected virtual IEnumerable<Point> GenerateForces(Component component, int x, int y)
        {
            yield return new Point(x, y);
            for (double a = 0; a < Math.PI * 2; a += Math.PI / 4)
            {
                yield return new Point(x + (int)(Math.Cos(a) * m_forceStep), y + (int)(Math.Sin(a) * m_forceStep));
            }
        }
    }
}
