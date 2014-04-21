using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;
using PlaceModel;
using System.Collections.Generic;
using System.Linq;


namespace DetailPlacer.Algorithm
{
    public class CrossReductPlacer : PlacerBase, IDetailPlacer
    {
        private readonly IPositionSearcher m_positionSearcher;
        public CrossReductPlacer()
        {
            m_positionSearcher = new SpiralPositionSearcher();

        }
        public virtual void CreateRegions(Design design, out int[] RegionsValues, out int[] xCoordRegions,
            out int[] yCoordRegions, out int wRegs, out int hRegs, out int regsCount)
        {
            var x = design.field.cellsx;
            var y = design.field.cellsy;
            regsCount = x * y;
            hRegs = 1;
            wRegs = 1;
            xCoordRegions = new int[regsCount];
            yCoordRegions = new int[regsCount];
            RegionsValues = new int[regsCount];

            var X = 0;
            var Y = 0;

            for (var i = 0; i < x; i++)
            {
                X = 0;
                for (var j = 0; j < y; j++)
                {
                    xCoordRegions[i * x + j] = X;
                    yCoordRegions[i * x + j] = Y;
                    X += wRegs;
                }
                Y += hRegs;
            }
        }

        private void CulcValuesInRegions(Design design, PlacementGlobal approximate, PlacementDetail result, out List<int>[] compInRegions, int[] RegionsValues, int[] xCoordRegions,
             int[] yCoordRegions, int wRegs, int hRegs, int regsCount)
        {
            compInRegions = new List<int>[regsCount];
            for (var i = 0; i < regsCount; i++)
            {
                compInRegions[i] = new List<int>();
                if (RegionsValues[i] != -1) RegionsValues[i] = 0;

            }

            for (var i = 0; i < design.components.Length; i++)
            {
                var r1 = design.components[i];
                var x1 = result.placed[r1] ? result.x[r1] : approximate.x[r1];
                var y1 = result.placed[r1] ? result.y[r1] : approximate.y[r1];
                for (var j = 0; j < regsCount; j++)
                {
                    if (RegionsValues[j] == -1) continue;

                    var x2 = xCoordRegions[j];
                    var y2 = yCoordRegions[j];
                    if (x1 + r1.sizex <= x2 || x2 + wRegs <= x1
                        || y1 + r1.sizey <= y2 || y2 + hRegs <= y1) continue;
                    RegionsValues[j]++;
                    compInRegions[j].Add(i);
                }
            }
        }
        private int Metric(int xCoordReg, int yCoordReg, int wReg, int hReg, int x, int y)
        {
            var maxX = System.Math.Max(xCoordReg + wReg, x);
            var minX = System.Math.Min(xCoordReg, x);
            var maxY = System.Math.Max(yCoordReg + hReg, y);
            var minY = System.Math.Min(yCoordReg, y);
            return maxX - minX + maxY - minY;
        }

        private void PlaceComponent(Design design, PlacementGlobal approximate, Component current, PlacementDetail result, out bool placed, int xCoordReg, int yCoordReg, int wRegs, int hRegs)
        {
            int[] x;
            int[] y;
            bool hasPosition;

            m_positionSearcher.AlvailablePositions(design, approximate, result, current, out x, out y, out hasPosition);
            if (hasPosition)
            {
                var bestMetric = int.MaxValue;
                var bestCoordInd = 0;

                for (var i = 0; i < x.Length; i++)
                {

                    if (!(x[i] + current.sizex <= xCoordReg || xCoordReg + wRegs <= x[i]
                        || y[i] + current.sizey <= yCoordReg || yCoordReg + hRegs <= y[i])) continue;
                    var m = Metric(xCoordReg, yCoordReg, wRegs, hRegs, x[i], y[i]);
                    if (m < bestMetric)
                    {
                        bestMetric = m;
                        bestCoordInd = i;
                    }
                }

                result.x[current] = x[bestCoordInd];
                result.y[current] = y[bestCoordInd];
                result.placed[current] = true;
                placed = true;
            }
            else
            {
                result.placed[current] = false;
                placed = false;
            }
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            List<int>[] compInRegions;
            int[] RegionsValues;
            int[] xCoordRegions;
            int[] yCoordRegions;
            int wRegs, hRegs, regsCount;
            CreateRegions(design, out RegionsValues, out xCoordRegions, out  yCoordRegions, out  wRegs, out hRegs, out regsCount);
            result = new PlacementDetail(design);
            CulcValuesInRegions(design, approximate, result, out compInRegions, RegionsValues, xCoordRegions, yCoordRegions, wRegs, hRegs, regsCount);

            do
            {
                var maxVal = RegionsValues.Max();

                for (var i = 0; i < regsCount; i++)
                {
                    if (RegionsValues[i] != maxVal) continue;
                    if (RegionsValues[i] == -1) continue;
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
                    }

                    bool placed = false;

                    for (var j = 0; j < compInRegions[i].Count; j++)
                    {
                        var compInd = compInRegions[i][j];
                        if (!RegContainsFixedComps)
                        {
                            if (compInd == maxCompInd) continue;
                        }
                        if (result.placed[design.components[compInd]]) continue;
                        PlaceComponent(design, approximate, design.components[compInd], result, out placed, xCoordRegions[i], yCoordRegions[i], wRegs, hRegs);

                    }

                    RegionsValues[i] = -1;
                    break;
                }

                CulcValuesInRegions(design, approximate, result, out compInRegions, RegionsValues, xCoordRegions, yCoordRegions, wRegs, hRegs, regsCount);
                if (!RegionsValues.Any(t => t > 1)) break;

            } while (true);

        }
    }
}
