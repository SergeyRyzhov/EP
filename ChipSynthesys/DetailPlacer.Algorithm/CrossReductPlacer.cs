using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using PlaceModel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DetailPlacer.Algorithm
{
    public class CrossReductPlacer : IDetailPlacer
    {
        private readonly IPositionSearcher m_positionSearcher;

        public CrossReductPlacer()
        {
            m_positionSearcher = new SpiralPositionSearcher();
        }

        public virtual void CreateRegions(Design design, out int[] regionsValues, out int[] xCoordRegions,
            out int[] yCoordRegions, out int wRegs, out int hRegs, out int regsCount)
        {
            var x = design.field.cellsx;
            var y = design.field.cellsy;
            regsCount = x * y;
            hRegs = 1;
            wRegs = 1;
            xCoordRegions = new int[regsCount];
            yCoordRegions = new int[regsCount];
            regionsValues = new int[regsCount];

            var X = design.field.beginx;
            var Y = design.field.beginy;

            for (var i = 0; i < x; i++)
            {
                X = design.field.beginx;
                for (var j = 0; j < y; j++)
                {
                    xCoordRegions[i * x + j] = X;
                    yCoordRegions[i * x + j] = Y;
                    X += wRegs;
                }
                Y += hRegs;
            }
        }

        public void CulcValuesInRegions(Design design, PlacementGlobal approximate, PlacementDetail result, out List<int>[] compInRegions, List<int>[] regsInComp, List<int> compsToUpdate, int[] regionsValues, int[] xCoordRegions,
             int[] yCoordRegions, int wRegs, int hRegs, int regsCount)
        {
            compInRegions = new List<int>[regsCount];
            //  var regionsInComp = new List<int>[design.components.Length];

            for (var i = 0; i < regsCount; i++)
            {
                compInRegions[i] = new List<int>();
                if (regionsValues[i] != -1)
                    regionsValues[i] = 0;
            }

            var updateCells = compsToUpdate != null;

            for (var c = 0; c < design.components.Length; c++)
            {
                if (updateCells)
                    if (!compsToUpdate.Contains(c)) continue;

                var r = design.components[c];
                var x = result.placed[r] ? result.x[r] : approximate.x[r];
                var y = result.placed[r] ? result.y[r] : approximate.y[r];
                var x1 = Math.Floor(x) - design.field.beginx;
                var y1 = Math.Floor(y) - design.field.beginy;
                var x2 = Math.Ceiling(x + r.sizex) - design.field.beginx;
                var y2 = Math.Ceiling(y + r.sizey) - design.field.beginy;
                var newsizex = x2 - x1;
                var newsizey = y2 - y1;

                var offset = (int)(design.field.cellsx * y1 + x1);

                regsInComp[c].Clear();

                for (var i = 0; i < newsizey; i++)
                {
                    var cellIndx = offset + design.field.cellsx * i;
                    regsInComp[c].Add(cellIndx);
                    for (var j = 1; j < newsizex; j++)
                    {
                        cellIndx++;
                        regsInComp[c].Add(cellIndx);
                    }
                }

            }
            for (var i = 0; i < regsInComp.Length; i++)
            {
                var regs = regsInComp[i];
                for (var j = 0; j < regs.Count; j++)
                {
                    compInRegions[regs[j]].Add(i);
                    regionsValues[regs[j]]++;
                }
            }
        }

        public int Metric(int xCoordReg, int yCoordReg, int wReg, int hReg, int x, int y)
        {
            var maxX = System.Math.Max(xCoordReg + wReg, x);
            var minX = System.Math.Min(xCoordReg, x);
            var maxY = System.Math.Max(yCoordReg + hReg, y);
            var minY = System.Math.Min(yCoordReg, y);
            return maxX - minX + maxY - minY;

        }

        public virtual void PlaceComponent(Mask mask, Design design, PlacementGlobal approximate, Component current, PlacementDetail result, int xCoordReg, int yCoordReg, int wRegs, int hRegs)
        {
            int[] x = new int[m_positionSearcher.PositionAmount];
            int[] y = new int[m_positionSearcher.PositionAmount];

            if (m_positionSearcher.AlvailablePositions(mask, current, (int)(approximate.x[current]), (int)approximate.y[current], x, y))
            {
                var bestMetric = double.MaxValue;
                var bestCoordInd = -1;

                for (var i = 0; i < x.Length; i++)
                {
                    if (!(x[i] + current.sizex <= xCoordReg || xCoordReg + wRegs <= x[i]
                        || y[i] + current.sizey <= yCoordReg || yCoordReg + hRegs <= y[i])) continue;
                    //  var m = Metric(xCoordReg, yCoordReg, wRegs, hRegs, x[i], y[i]);
                    var x1 = approximate.x[current] - x[i];
                    var y1 = approximate.y[current] - y[i];
                    var m = x1 * x1 + y1 * y1;

                    if (m < bestMetric)
                    {
                        bestMetric = m;
                        bestCoordInd = i;
                    }
                }
                if (bestCoordInd != -1)
                {
                    result.x[current] = x[bestCoordInd];
                    result.y[current] = y[bestCoordInd];
                    result.placed[current] = true;
                    mask.PlaceComponent(current, x[bestCoordInd], y[bestCoordInd]);
                }
            }
            else
            {
                Console.WriteLine("no positions for {0}", current);
                result.placed[current] = true;
                //placed = false;

                result.x[current] = (int)Math.Round(approximate.x[current]);
                result.y[current] = (int)Math.Round(approximate.y[current]);
            }
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            List<int>[] compInRegions;
            List<int>[] RegsInComp = new List<int>[design.components.Length];
            List<int> compsToUpdate = null;
            int[] regionsValues;
            int[] xCoordRegions;
            int[] yCoordRegions;
            int wRegs, hRegs, regsCount;
            result = new PlacementDetail(design);

            for (var c = 0; c < design.components.Length; c++)
                RegsInComp[c] = new List<int>();


            CreateRegions(design, out regionsValues, out xCoordRegions, out  yCoordRegions, out  wRegs, out hRegs, out regsCount);
            CulcValuesInRegions(design, approximate, result, out compInRegions, RegsInComp, null, regionsValues, xCoordRegions,
                yCoordRegions, wRegs, hRegs, regsCount);

            Mask helper = new Mask(design, result);
            helper.BuildUp();

            do
            {
                if (!regionsValues.Any(t => t > 1)) break;
                var maxVal = regionsValues.Max();
                bool hasChanged = false;
                for (var i = 0; i < regsCount; i++)
                {
                    if (regionsValues[i] != maxVal) continue;
                    if (regionsValues[i] == -1) continue;
                    var maxArea = 0;
                    var maxCompInd = -1;
                    var RegContainsFixedComps = false;
                    for (var j = 0; j < compInRegions[i].Count; j++)
                    {
                        if (!result.placed[design.components[compInRegions[i][j]]]) continue;
                        RegContainsFixedComps = true;
                        break;
                    }
                    if (!RegContainsFixedComps)
                    {
                        for (var j = 0; j < compInRegions[i].Count; j++)
                        {
                            var c = design.components[compInRegions[i][j]];
                            var area = c.sizex * c.sizey;
                            if (area <= maxArea) continue;
                            maxArea = area;
                            maxCompInd = compInRegions[i][j];
                        }

                        var current = design.components[maxCompInd];
                        result.placed[current] = true;
                        result.x[current] = (int)Math.Round(approximate.x[current]);
                        result.y[current] = (int)Math.Round(approximate.y[current]);

                        helper.PlaceComponent(current, result.x[current], result.y[current]);

                    }

                    for (var j = 0; j < compInRegions[i].Count; j++)
                    {
                        var compInd = compInRegions[i][j];
                        if (!RegContainsFixedComps)
                        {
                            if (compInd == maxCompInd) continue;
                        }
                        if (result.placed[design.components[compInd]]) continue;
                        PlaceComponent(helper, design, approximate, design.components[compInd], result, xCoordRegions[i], yCoordRegions[i], wRegs, hRegs);
                        hasChanged = true;
                    }

                    regionsValues[i] = -1;
                    compsToUpdate = compInRegions[i];
                    break;
                }
                if (!hasChanged)
                {
                    Console.WriteLine("no positions for current mask");
                    break;
                }
                var existUnplacedComps = false;
                foreach (var c in design.components)
                {
                    if (result.placed[c]) continue;
                    existUnplacedComps = true;
                    break;
                }
                if (!existUnplacedComps) break;

                CulcValuesInRegions(design, approximate, result, out compInRegions, RegsInComp, compsToUpdate, regionsValues,
                    xCoordRegions, yCoordRegions, wRegs, hRegs, regsCount);

            } while (true);

            foreach (var c in design.components)
            {
                if (result.placed[c] == true) continue;
                result.placed[c] = true;
                result.x[c] = (int)Math.Round(approximate.x[c]);
                result.y[c] = (int)Math.Round(approximate.y[c]);
            }
        }
    }
}