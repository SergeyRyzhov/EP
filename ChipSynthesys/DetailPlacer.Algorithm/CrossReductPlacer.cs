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
        protected readonly IPositionSearcher m_positionSearcher;

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

        public void CulcValuesInRegions(Design design, PlacementGlobal approximate, PlacementDetail result, out List<int>[] compInRegions,List<int>[] RegsInComp,List<int> compsToUpdate, int[] RegionsValues, int[] xCoordRegions,
             int[] yCoordRegions, int wRegs, int hRegs, int regsCount)
        {
            compInRegions = new List<int>[regsCount];
          //  var regionsInComp = new List<int>[design.components.Length];

            for (var i = 0; i < regsCount; i++)
            {
                compInRegions[i] = new List<int>();
                if (RegionsValues[i] != -1)
                    RegionsValues[i] = 0;
            }

            var updateCells = (compsToUpdate != null) ? true : false;           

            for (var c = 0; c < design.components.Length; c++)
            {             
                if(updateCells)
                    if(!compsToUpdate.Contains(c)) continue;

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

                RegsInComp[c].Clear();

                for (var i = 0; i < newsizey; i++)
                {
                    var cellIndx = offset + design.field.cellsx * i;
                    RegsInComp[c].Add(cellIndx);
                    for (var j = 1; j < newsizex; j++)
                    {
                        cellIndx++;
                        RegsInComp[c].Add(cellIndx); 
                    }
                }

            }
            for (var i = 0; i < RegsInComp.Length; i++)
            {
                var regs = RegsInComp[i];
                for (var j = 0; j < regs.Count; j++)
                {
                    compInRegions[regs[j]].Add(i);
                    RegionsValues[regs[j]]++;
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

        public virtual void PlaceComponent(PositionHelper helper, Design design, PlacementGlobal approximate, Component current, PlacementDetail result, out bool placed, int xCoordReg, int yCoordReg, int wRegs, int hRegs)
        {
            int[] x;
            int[] y;
            bool hasPosition;

            m_positionSearcher.AlvailablePositions(helper, design, approximate, result, current, out x, out y, out hasPosition);
            if (hasPosition)
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
                    placed = true;
                    helper.MoveComponent(current, x[bestCoordInd], y[bestCoordInd]);
                }
                else placed = false;
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
            List<int>[] RegsInComp=new List<int>[design.components.Length];
            List<int> compsToUpdate=null;
            int[] regionsValues;
            int[] xCoordRegions;
            int[] yCoordRegions;
            int wRegs, hRegs, regsCount;
            result = new PlacementDetail(design);

            for (var c = 0; c < design.components.Length; c++)            
                RegsInComp[c] = new List<int>();
            

            CreateRegions(design, out regionsValues, out xCoordRegions, out  yCoordRegions, out  wRegs, out hRegs, out regsCount);            
            CulcValuesInRegions(design, approximate, result, out compInRegions,RegsInComp,null, regionsValues, xCoordRegions,
                yCoordRegions, wRegs, hRegs, regsCount);

            PositionHelper helper = new PositionHelper(design, result);
            helper.Build();

            do
            {
                if (!regionsValues.Any(t => t > 1)) break;       
                var maxVal = regionsValues.Max();                  

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

                        helper.MoveComponent(current, result.x[current], result.y[current]);
                    
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
                        PlaceComponent(helper, design, approximate, design.components[compInd], result, out placed, xCoordRegions[i], yCoordRegions[i], wRegs, hRegs);
                    }                    

                    regionsValues[i] = -1;
                    compsToUpdate = compInRegions[i];
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