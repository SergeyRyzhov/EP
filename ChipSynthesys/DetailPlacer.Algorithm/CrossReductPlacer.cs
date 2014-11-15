using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using PlaceModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DetailPlacer.Algorithm
{

    /* public class PrePlacer : IGlobalPlacer
     {
        /* public void Place(Design design, PlacementGlobal approximate, out PlacementGlobal result)
         {
             result = new PlacementGlobal(design);
             foreach (var component in design.components)
             {
                 result.placed[component] = approximate.placed[component];
                 result.x[component] = (int)approximate.x[component];
                 result.y[component] = (int)approximate.y[component];
             }
         }*
         public void Place(Design design, out PlacementGlobal result)
         {
             throw new NotImplementedException();
         }
     }*/
    public class CrossReductPlacer : IDetailPlacer
    {
        protected readonly IPositionSearcher m_positionSearcher;

        public CrossReductPlacer()
        {
            m_positionSearcher = new SpiralPositionSearcher();
        }


        public void CulcValuesInRegions(Design design, PlacementGlobal approximate, PlacementDetail result,
            List<int>[] regsInComp, int[] valuesInRegions, ref List<int> compsToUpdate)
        {

            var cellsX = design.field.cellsx;
            var cellsY = design.field.cellsy;
            var cellsCount = design.field.cellsx * design.field.cellsy;

            for (var i = 0; i < cellsCount; i++)
                valuesInRegions[i] = 0;


            var updateCells = (compsToUpdate.Count != 0) ? true : false;

            for (var c = 0; c < design.components.Length; c++)
            {
                if (updateCells)
                    if (!compsToUpdate.Contains(c)) continue;

                regsInComp[c].Clear();
                var r = design.components[c];
                var x = result.placed[r] ? result.x[r] : approximate.x[r];
                var y = result.placed[r] ? result.y[r] : approximate.y[r];
                var x1 = Math.Floor(x) - design.field.beginx;
                var y1 = Math.Floor(y) - design.field.beginy;
                var x2 = Math.Ceiling(x + r.sizex) - design.field.beginx;
                var y2 = Math.Ceiling(y + r.sizey) - design.field.beginy;
                if (x1 < 0) x1 = 0;
                if (y1 < 0) y1 = 0;
                if (x2 > cellsX) x2 = cellsX;
                if (y2 > cellsY) y2 = cellsY;
                if (x1 >= cellsX || y1 >= cellsY || x2 <= 0 || y2 <= 0)
                    continue;
                var newsizex = x2 - x1;
                var newsizey = y2 - y1;

                var offset = (int)(design.field.cellsx * y1 + x1);
                for (var i = 0; i < newsizey; i++)
                {
                    var cellIndx = offset + design.field.cellsx * i;
                    for (var j = 0; j < newsizex; j++)
                    {
                        regsInComp[c].Add(cellIndx);
                        cellIndx++;
                    }
                }

            }


            for (var c = 0; c < regsInComp.Length; c++)
            {
                for (var i = 0; i < regsInComp[c].Count; i++)
                {
                    var reg = regsInComp[c][i];
                    valuesInRegions[reg]++;
                }

            }

            var maxValue = valuesInRegions.Max();
            compsToUpdate.Clear();
            if (maxValue == 1) return;


            for (var i = 0; i < cellsCount; i++)
            {
                if (valuesInRegions[i] != maxValue) continue;
                for (var j = 0; j < regsInComp.Length; j++)
                {
                    if (!regsInComp[j].Contains(i)) continue;
                    if (result.placed[design.components[j]]) continue;
                    compsToUpdate.Add(j);
                }
                if (compsToUpdate.Count == 0) continue;
                break;
            }

        }


        public virtual void PlaceComponent(Mask helper, Design design, PlacementGlobal approximate, Component current, PlacementDetail result)
        {
            int[] x = new int[m_positionSearcher.PositionAmount];
            int[] y = new int[m_positionSearcher.PositionAmount];
            if (m_positionSearcher.AlvailablePositions(helper, current, (int)(approximate.x[current]), (int)approximate.y[current], x, y))
            {
                var bestMetric = double.MaxValue;
                var bestCoordInd = 0;

                for (var i = 0; i < x.Length; i++)
                {
                    var x1 = approximate.x[current] - x[i];
                    var y1 = approximate.y[current] - y[i];
                    var metric = x1 * x1 + y1 * y1;

                    if (metric < bestMetric)
                    {
                        bestMetric = metric;
                        bestCoordInd = i;
                    }
                }
                result.x[current] = x[bestCoordInd];
                result.y[current] = y[bestCoordInd];

                helper.PlaceComponent(current, x[bestCoordInd], y[bestCoordInd]);
                result.placed[current] = true;

            }
            else
            {

                result.x[current] = (int)Math.Round(approximate.x[current]);
                result.y[current] = (int)Math.Round(approximate.y[current]);
                helper.PlaceComponent(current, result.x[current], result.y[current]);
                result.placed[current] = true;
            }
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            var regsInComp = new List<int>[design.components.Length];
            var compsToUpdate = new List<int>();
            var valuesInRegions = new int[design.field.cellsx * design.field.cellsy];
            result = new PlacementDetail(design);
            var countOfPlacedComps = 0;

            for (var c = 0; c < design.components.Length; c++)
                regsInComp[c] = new List<int>();

            CulcValuesInRegions(design, approximate, result, regsInComp, valuesInRegions, ref compsToUpdate);
            Mask helper = new Mask(design, result);
            helper.BuildUp();

            do
            {
                if (compsToUpdate.Count == 0) break;

                for (var j = 0; j < compsToUpdate.Count; j++)
                    PlaceComponent(helper, design, approximate, design.components[compsToUpdate[j]], result);

                countOfPlacedComps += compsToUpdate.Count;

                if (countOfPlacedComps == design.components.Length) break;
                CulcValuesInRegions(design, approximate, result, regsInComp, valuesInRegions, ref compsToUpdate);

            } while (true);

            foreach (var comp in design.components)
            {
                if (result.placed[comp]) continue;
                result.x[comp] = (int)Math.Round(approximate.x[comp]);
                result.y[comp] = (int)Math.Round(approximate.y[comp]);
                var x = result.x[comp];
                var y = result.y[comp];
                var w = comp.sizex;
                var h = comp.sizey;
                bool inField = x >= design.field.beginx && (x + w) <= (design.field.beginx + design.field.cellsx);
                inField &= y >= design.field.beginy && (y + h) <= (design.field.beginy + design.field.cellsy);

                result.placed[comp] = inField;
                //helper.PlaceComponent(comp, result.x[comp], result.y[comp]);
            }

        }
    }
}