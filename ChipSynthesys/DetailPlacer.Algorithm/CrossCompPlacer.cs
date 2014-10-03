using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;
using DetailPlacer.Algorithm.CriterionPositionSearcher;
using PlaceModel;
using ChipSynthesys.Common.Classes;
using System.Drawing;


namespace DetailPlacer.Algorithm
{
    public class CrossCompPlacer : IDetailPlacer
    {

        public void Clear(int qtcells, int[] ValueCell, List<List<Component>> CompInCell)
        {
            for (int i = 0; i < qtcells; i++)
            {
                CompInCell[i].Clear();
                if (ValueCell[i] == -1)
                    continue;
                else
                    ValueCell[i] = 0;
            }
        }
        public void CreateCells(int width, int height, Design design, out int[] XCellCoord, out int[] YCellCoord, int qtcells)
        {
            int coord_x = design.field.beginx;
            int coord_y = design.field.beginy;
            XCellCoord = new int[qtcells];
            YCellCoord = new int[qtcells];

            for (int i = 0; i < height; i++)
            {
                coord_x = design.field.beginx;
                for (int j = 0; j < width; j++)
                {
                    XCellCoord[i * width + j] = coord_x;
                    YCellCoord[i * width + j] = coord_y;
                    coord_x++;
                }
                coord_y++;
            }
        }
        public int[] CreatCompValueCells(int size)
        {
            var result = new int[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = 0;
            }

            return result;
        }
        public List<List<Component>> InitCompInCell(int size)
        {
            var result = new List<List<Component>>();
            for (int i = 0; i < size; i++)
            {
                result.Add(new List<Component>());
            }

            return result;
        }
        public void FillCells(Design design, PlacementGlobal myApproximate, int[] XCellCoord, int[] YCellCoord, int qtcells, int[] ValueCell, int h, int w, List<List<Component>> CompInCell)
        {
            double xCoord;
            double yCoord;
            int sizex;
            int sizey;

            foreach (Component comp in design.components)
            {
                xCoord = myApproximate.x[comp];
                yCoord = myApproximate.y[comp];
                sizex = comp.sizex;
                sizey = comp.sizey;
                for (int i = 0; i < qtcells; i++)
                {
                    if (ValueCell[i] == -1)
                        continue;
                    else
                    {
                        if (XCellCoord[i] <= xCoord && xCoord < XCellCoord[i] + 1 && YCellCoord[i] <= yCoord && yCoord < YCellCoord[i] + 1)
                        {
                            if (XCellCoord[i] < xCoord && YCellCoord[i] < yCoord)
                            { sizex++; sizey++; }
                            for (int j = 0; j < sizey; j++)
                            {
                                for (int k = 0; k < sizex; k++)
                                {
                                    if (i + j * w + k < qtcells)
                                    {
                                        if (ValueCell[i + j * w + k] < 0)
                                            continue;
                                        ValueCell[i + j * w + k]++;
                                        CompInCell[i + j * w + k].Add(comp);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
        public PlacementGlobal CreateMyApproximate(PlacementGlobal approximate, Design design)
        {
            var myApproximate = new PlacementGlobal(design);
            foreach (Component c in design.components)
            {
                myApproximate.x[c] = approximate.x[c];
                myApproximate.y[c] = approximate.y[c];
                myApproximate.placed[c] = approximate.placed[c];
            }
            return myApproximate;
        }

        public PlacementGlobal CreateMyApproximate(PlacementDetail detail, Design design)
        {
            var myApproximate = new PlacementGlobal(design);
            foreach (Component c in design.components)
            {
                myApproximate.x[c] = detail.x[c];
                myApproximate.y[c] = detail.y[c];
                myApproximate.placed[c] = detail.placed[c];
            }
            return myApproximate;
        }
        public virtual void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            int width = design.field.cellsx;
            int height = design.field.cellsy;
            int qtcells = width * height;
            int[] XCellCoord;
            int[] YCellCoord;
            int enumerator;
            int indCell = 0;
            List<List<Component>> compInCell = InitCompInCell(qtcells);
            List<Component> fixedComponents = new List<Component>();
            var myApproximate = new PlacementGlobal(design);
            myApproximate = CreateMyApproximate(approximate, design);
            CreateCells(width, height, design, out XCellCoord, out YCellCoord, qtcells);
            int[] ValueCell = CreatCompValueCells(qtcells);
            result = new PlacementDetail(design);
            do
            {

                enumerator = 0;
                FillCells(design, myApproximate, XCellCoord, YCellCoord, qtcells, ValueCell, height, width, compInCell);
                for (int i = 0; i < qtcells; i++)
                {
                    if (ValueCell[i] > 1 && ValueCell[i] > enumerator)
                    {
                        enumerator = ValueCell[i];
                        indCell = i;
                    }
                }

                if (enumerator > 1)
                {
                    Component bestComp = GetComponentWithMaxSquare(compInCell[indCell], fixedComponents);
                    if (bestComp != null)
                    {
                        int bestCoord;
                        bestCoord = BestCell(XCellCoord, YCellCoord, ValueCell, qtcells, indCell, bestComp, design, myApproximate);
                        result.x[bestComp] = XCellCoord[bestCoord];
                        result.y[bestComp] = YCellCoord[bestCoord];
                        myApproximate.x[bestComp] = XCellCoord[bestCoord];
                        myApproximate.y[bestComp] = YCellCoord[bestCoord];
                        result.placed[bestComp] = true;
                        ValueCell[indCell] = -1;
                        // ValueCell[bestCoord] = -1;
                        fixedComponents.Add(bestComp);
                        int k = bestComp.id;
                        int h = 0;
                        DrawerHelper.SimpleDraw(design, result, new Size(600, 600), new Bitmap(600, 600), string.Format("iter {0}.png", k));
                        h++;
                    }
                    else
                    { ValueCell[indCell] = -1; }
                    Clear(qtcells, ValueCell, compInCell);

                }
            } while (fixedComponents.Count() != design.components.Count() && enumerator != 0);

            foreach (Component comp in design.components)
            {
                if (result.placed[comp] == false)
                {
                    FillCells(design, myApproximate, XCellCoord, YCellCoord, qtcells, ValueCell, height, width, compInCell);

                    int ind = GetCellIndex((int)myApproximate.x[comp], (int)myApproximate.y[comp], XCellCoord, YCellCoord, qtcells);
                    int Coord = BestCell(XCellCoord, YCellCoord, ValueCell, qtcells, ind, comp, design, myApproximate);
                    result.x[comp] = XCellCoord[Coord];
                    result.y[comp] = YCellCoord[Coord];
                    result.placed[comp] = true;
                    myApproximate.x[comp] = XCellCoord[Coord];
                    myApproximate.y[comp] = YCellCoord[Coord];
                    ValueCell[ind] = -1;
                    //ValueCell[Coord] = -1;
                    Clear(qtcells, ValueCell, compInCell);
                }
            }
            //for (int i = 0; i < qtcells; i++)
            //{
            //    ValueCell[i] = 0;
            //}

            //foreach (Component comp in design.components)
            //{
            //    FillCells(design, myApproximate, XCellCoord, YCellCoord, qtcells, ValueCell, height, width, compInCell);

            //    int ind = GetCellIndex((int)myApproximate.x[comp], (int)myApproximate.y[comp], XCellCoord, YCellCoord, qtcells);
            //    int Coord = BestCell(XCellCoord, YCellCoord, ValueCell, qtcells, ind, comp, design, myApproximate);
            //    //result.x[comp] = XCellCoord[Coord];
            //    //result.y[comp] = YCellCoord[Coord];
            //    //result.placed[comp] = true;
            //    myApproximate.x[comp] = XCellCoord[Coord];
            //    myApproximate.y[comp] = YCellCoord[Coord];
            //    ValueCell[ind] = -1;
            //    //ValueCell[Coord] = -1;
            //    Clear(qtcells, ValueCell, compInCell);
            //}

          

        }
        
        public int GetCellIndex(int x, int y, int[] XCoord, int[] YCoord, int qt)
        {
            for (int j = 0; j < qt; j++)
            {
                if (YCoord[j] == y && XCoord[j] == x)
                    return j;
            }
            return 0;
        }

        public Component GetComponentWithMaxSquare(List<Component> cell, List<Component> fixedComponents)
        {
            Component bestComponent = null;
            var maxSquare = 0;

            for (int i = 0; i < cell.Count; i++)
            {
                var component = cell[i];
                if (IsNotFixed(component, fixedComponents))
                {
                    var square = component.sizex * component.sizey;

                    if (square > maxSquare)
                    {
                        maxSquare = square;
                        bestComponent = component;
                    }
                }
            }

            return bestComponent;
        }

        private bool IsNotFixed(Component component, List<Component> fixedComponents)
        {
            foreach (Component c in fixedComponents)
            {
                if (component.id == c.id)
                    return false;
            }
            return true;
        }

        public int BestCell(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int qtcells, int indexcurrent, Component Current, Design design, PlacementGlobal myApproximate)
        {
            int indexOfBest = 0;
            double percentBest = 0;
            while (CanNotBePlaced(ValueCell[indexOfBest], XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design))
            {
                indexOfBest++;
            }

            percentBest = PercentCross2(XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design, myApproximate);
            double areaBest = CloselyCell(myApproximate, Current, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);
            double area2Best = NearNet(Current, myApproximate, design, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);

            for (int i = indexOfBest + 1; i < qtcells; i++)
            {
                if (CanNotBePlaced(ValueCell[i], XCellCoord[i], YCellCoord[i], Current, design))
                    continue;

                double percentCurrent = PercentCross2(XCellCoord[i], YCellCoord[i], Current, design, myApproximate);
                double areaCurrent = CloselyCell(myApproximate, Current, XCellCoord[i], YCellCoord[i]);
                double area2Current = NearNet(Current, myApproximate, design, XCellCoord[i], YCellCoord[i]);

                if (percentCurrent < percentBest)
                {
                    percentBest = percentCurrent;
                    areaBest = areaCurrent;
                    area2Best = area2Current;
                    indexOfBest = i;
                    continue;
                }
                if (percentCurrent == percentBest)
                {
                    if (area2Current < area2Best)
                    {
                        percentBest = percentCurrent;
                        areaBest = areaCurrent;
                        area2Best = area2Current;
                        indexOfBest = i;
                        continue;
                    }

                    if (area2Current == area2Best)
                    {
                        if (areaCurrent <= areaBest)
                        {
                            percentBest = percentCurrent;
                            areaBest = areaCurrent;
                            area2Best = area2Current;
                            indexOfBest = i;
                            continue;
                        }
                    }
                }
                if (percentCurrent > percentBest)
                { continue; }
            }

            return indexOfBest;
        }

        private bool CanNotBePlaced(int cell, int x, int y, Component component, Design design)
        {
            var field = design.field;

            return cell < 0 ||
                x + component.sizex > field.beginx + field.cellsx ||
                y + component.sizey > field.beginy + field.cellsy;
        }
        public double PercentCross2(int XCellCoord, int YCellCoord, Component Current, Design design, PlacementGlobal myApproximate)
        {
            double result = 0;
            int minXcurrent = XCellCoord;
            int maxXcurrent = XCellCoord + Current.sizex;
            int minYcurrent = YCellCoord;
            int maxYcurrent = YCellCoord + Current.sizey;
            double currentpercent = 0;

            foreach (Component next in design.components)
            {
                double minXnext = myApproximate.x[next];
                double maxXnext = myApproximate.x[next] + next.sizex;
                double minYnext = myApproximate.y[next];
                double maxYnext = myApproximate.y[next] + next.sizey;

                if (next == Current) continue;

                else
                {
                    double minX = Math.Min(minXcurrent, minXnext);
                    double maxX = Math.Max(maxXcurrent, maxXnext);
                    double minY = Math.Min(minYcurrent, minYnext);
                    double maxY = Math.Max(maxYcurrent, maxYnext);

                    if (maxX - minX < Current.sizex + next.sizex && maxY - minY < Current.sizey + next.sizey)
                    {
                        currentpercent = AreaCross(minXcurrent, maxXcurrent, minYcurrent, maxYcurrent, minXnext, maxXnext, minYnext, maxYnext);
                        result = currentpercent + result;
                        continue;
                    }
                }
            }
            return result;
        }

        public double PercentCross(int XCellCoord, int YCellCoord, Component Current, Design design, PlacementGlobal myApproximate)
        {
            double result = 0;
            int minXcurrent = XCellCoord;
            int maxXcurrent = XCellCoord + Current.sizex;
            int minYcurrent = YCellCoord;
            int maxYcurrent = YCellCoord + Current.sizey;
            double currentpercent = 0;
            foreach (Component next in design.components)
            {
                double minXnext = myApproximate.x[next];
                double maxXnext = myApproximate.x[next] + next.sizex;
                double minYnext = myApproximate.y[next];
                double maxYnext = myApproximate.y[next] + next.sizey;

                if (next == Current) continue;

                else
                {
                    if ((minXcurrent <= minXnext && minXnext < maxXcurrent && minYcurrent <= minYnext && minYnext < maxYcurrent)
                        || (minXcurrent < maxXnext && maxXnext <= maxXcurrent && minYcurrent <= minYnext && minYnext < maxYcurrent)
                        || (minXcurrent < minXnext && minXnext <= maxXcurrent && minYcurrent < maxYnext && maxYnext <= maxYcurrent)
                        || (minXcurrent < maxXnext && maxXnext <= maxXcurrent && minYcurrent < maxYnext && maxYnext <= maxYcurrent))
                    {
                        currentpercent = AreaCross(minXcurrent, maxXcurrent, minYcurrent, maxYcurrent, minXnext, maxXnext, minYnext, maxYnext);
                        result = currentpercent + result;
                        continue;
                    }

                    if ((minXnext <= minXcurrent && minXcurrent < maxXnext && minYnext <= minYcurrent && minYcurrent < maxYnext)
                        || (minXnext < maxXcurrent && maxXcurrent <= maxXnext && minYnext <= minYcurrent && minYcurrent < maxYnext)
                        || (minXnext < minXcurrent && minXcurrent <= maxXnext && minYnext < maxYcurrent && maxYcurrent <= maxYnext)
                        || (minXnext < maxXcurrent && maxXcurrent <= maxXnext && minYnext < maxYcurrent && maxYcurrent <= maxYnext))
                    {
                        currentpercent = AreaCross(minXcurrent, maxXcurrent, minYcurrent, maxYcurrent, minXnext, maxXnext, minYnext, maxYnext);
                        result = currentpercent + result;
                        continue;
                    }

                }
            }
            return result;
        }

        public double AreaCross(int minXcurrent, int maxXcurrent, int minYcurrent, int maxYcurrent, double minXnext, double maxXnext, double minYnext, double maxYnext)
        {
            double minX = Math.Max(minXcurrent, minXnext);
            double minY = Math.Max(minYcurrent, minYnext);
            double maxX = Math.Min(maxXcurrent, maxXnext);
            double maxY = Math.Min(maxYcurrent, maxYnext);
            int Scurrent = (maxYcurrent - minYcurrent) * (maxXcurrent - minXcurrent);
            double Scross = (maxX - minX) * (maxY - minY);
            double percent = (Scross * 100) / Scurrent;
            return percent;
        }

        public double CloselyCell(PlacementGlobal myApproximate, Component current, int XcurrCoord, int YcurrCoord)
        {
            double minX;
            double minY;
            double maxX;
            double maxY;
            minX = Math.Min(myApproximate.x[current], XcurrCoord);
            minY = Math.Min(myApproximate.y[current], YcurrCoord);
            maxX = Math.Max(myApproximate.x[current], XcurrCoord);
            maxY = Math.Max(myApproximate.y[current], YcurrCoord);
            double area = (maxX - minX) + (maxY - minY);
            return area;
        }

        public double NearNet(Component Current, PlacementGlobal myApproximate, Design design, int XCellCoord, int YCellCoord)
        {
            double minX = XCellCoord;
            double maxX = XCellCoord;
            double minY = YCellCoord;
            double maxY = YCellCoord;
            double area;
            foreach (Net net in design.Nets(Current))
            {
                foreach (Component next in net.items)
                {
                    if (next == Current)
                        continue;
                    minX = Math.Min(minX, myApproximate.x[next]);
                    maxX = Math.Max(maxX, myApproximate.x[next]);
                    minY = Math.Min(minY, myApproximate.y[next]);
                    maxY = Math.Max(maxY, myApproximate.y[next]);
                }
            }
            area = (maxX - minX) + (maxY - minY);
            return area;

        }


       
       
    }

}
