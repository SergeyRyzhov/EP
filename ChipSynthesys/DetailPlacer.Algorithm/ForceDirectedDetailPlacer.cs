using System;
using System.Collections.Generic;
using DetailPlacer.Algorithm.CriterionPositionSearcher;
using PlaceModel;

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
            return;
            while (Iteration(design, result))
            {
                
            }
        }

        public bool Iteration(Design design, PlacementDetail result)
        {


            return true;
        }

        protected virtual IEnumerable<PointF> GenerateForces(Component component, double x, double y)
        {
            for (double a = 0; a < Math.PI * 2; a += Math.PI / 4)
            {
                yield return new PointF(x + Math.Cos(a) * m_forceStep, y + Math.Sin(a) * m_forceStep);
            }
        }
    }
}
