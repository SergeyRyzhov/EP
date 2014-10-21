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
using System.Threading;


namespace DetailPlacer.Algorithm
{
    public class CrossCompPlacer : IDetailPlacer
    {
        private readonly object _lock = new object();
        private readonly object  _lock1 = new object();
        public int width;
        public int height;
        public int qtcells;
        public void CreateCells(Design design, out int[] XCellCoord, out int[] YCellCoord)
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


        public void FillCells(Design design, PlacementGlobal myApproximate, int[] XCellCoord, int[] YCellCoord,int[] ValueCell, List<List<Component>> CompInCell)
        {

            double xCoord;
            double yCoord;
            int sizex;
            int sizey;
            int index;
            int x; int y;

            for (int i = 0; i < qtcells; i++)
            {
                ValueCell[i] = 0;
            }

            
            foreach (Component comp in design.components)
            {
                                 
                xCoord = myApproximate.x[comp];
                yCoord = myApproximate.y[comp];
                sizex = comp.sizex;
                sizey = comp.sizey;
                x = (int)xCoord;
                y = (int)yCoord;
                index = y*width + x;
                if (index < 0)
                    continue;     
                if (XCellCoord[index] < xCoord && YCellCoord[index] < yCoord)
                { sizex++; sizey++; }
                if (XCellCoord[index] < xCoord && YCellCoord[index] == yCoord)
                { sizex++; }
                if (XCellCoord[index] == xCoord && YCellCoord[index] < yCoord)
                { sizey++; }

                for (int j = 0; j < sizey; j++)
                {
                    for (int k = 0; k < sizex; k++)
                    {
                        if (index + j * width + k < qtcells && xCoord + k <= width && xCoord + k >= 0 && yCoord + j <= height && yCoord + j >= 0)
                        {
                            ValueCell[index + j * width + k]++;
                            CompInCell[index + j * width + k].Add(comp);                        
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

        public void ClearCells(int[] ValueCells, Design design, PlacementGlobal myApproximate, int[] XCellCoord, int[] YCellCoord, int indexfirst, Component current, List<List<Component>> CompInCell)
        {

            double xCoord = myApproximate.x[current];
            double yCoord = myApproximate.y[current];
            int sizex = current.sizex;
            int sizey = current.sizey;

            for (int i = 0; i <= current.sizey; i++)
             {
                 for (int j = 0; j <= current.sizex; j++)
                 {
                     if (indexfirst + j + i * width < qtcells && indexfirst + j + i * width > 0)
                     {
                         for (int k = 0; k < CompInCell[indexfirst + j + i * width].Count(); k++)
                         {
                             if (CompInCell[indexfirst + j + i * width][k] == current)
                             {
                                 if (ValueCells[indexfirst + j + i * width] > 0 && xCoord + j <= width && xCoord + k >= 0 && yCoord + j <= height && yCoord + j >= 0)
                                 {
                                     ValueCells[indexfirst + j + i * width]--;
                                     CompInCell[indexfirst + j + i * width].Remove(current);
                                 }
                             }
                         }
                     }

                 }
             }
        }
        public void AddCells(int[] ValueCells, Design design, PlacementGlobal myApproximate, int[] XCellCoord, int[] YCellCoord, int indexbest, Component current, List<List<Component>> CompInCell)
        {
            double xCoord = myApproximate.x[current];
            double yCoord = myApproximate.y[current];
            int sizex = current.sizex;
            int sizey = current.sizey;
            
             for (int i = 0; i < current.sizey; i++)
            {
                for (int j = 0; j < current.sizex; j++)
                {
                    if (indexbest + j + i * width < qtcells && indexbest + j + i * width >= 0 && ValueCells[indexbest + j + i * width] != -1 && xCoord + j <= width && xCoord + j >= 0 && yCoord + i <= height && yCoord + i >= 0)
                    {
                        int val = ValueCells[indexbest + j + i * width];
                        ValueCells[indexbest + j + i * width] = val + 1;
                        CompInCell[indexbest + j + i * width].Add(current);
                    }
                }
            }
        }


        public virtual void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            width = design.field.cellsx;
            height = design.field.cellsy;
            qtcells = width * height;
            int[] XCellCoord;
            int[] YCellCoord;
            int enumerator;
            int indCell = 0;
            int[] masswind = new int[4];
            List<List<Component>> compInCell = InitCompInCell(qtcells);
            List<Component> fixedComponents = new List<Component>();
            var myApproximate = new PlacementGlobal(design);
            myApproximate = CreateMyApproximate(approximate, design);
            CreateCells(design, out XCellCoord, out YCellCoord);
            int[] ValueCell = CreatCompValueCells(qtcells);
            result = new PlacementDetail(design);
            FillCells(design, myApproximate, XCellCoord, YCellCoord, ValueCell, compInCell);
            do
            {
                enumerator = 0;
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
                       // masswind[0] = 0;
                       // masswind[1] = width;
                       // masswind[2] = 0;
                       // masswind[3] = height;
                        // выбор лучшей позиции 
                        int bestCoord;
                        masswind =  GetWindow(design, myApproximate, bestComp, XCellCoord, YCellCoord, ValueCell, height / 60, width / 60);
                        bestCoord = BestCell2(XCellCoord, YCellCoord, ValueCell, indCell, bestComp, design, myApproximate,masswind);
                        // поиск позиции текущего компонента                  
                        int indCurrent = (int)myApproximate.x[bestComp] + (int)myApproximate.y[bestComp] * width;

                        result.x[bestComp] = XCellCoord[bestCoord];
                        result.y[bestComp] = YCellCoord[bestCoord];
                        myApproximate.x[bestComp] = XCellCoord[bestCoord];
                        myApproximate.y[bestComp] = YCellCoord[bestCoord];
                        result.placed[bestComp] = true;
                        // очищение ячейки 
                        ValueCell[indCell] = -1;
                        compInCell[indCell].Clear();
                        // удаление и добавление значений для компоненты в ячейки
                        ClearCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, indCurrent, bestComp,compInCell);
                        AddCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, bestCoord, bestComp, compInCell);
                       
                        // фиксация компонента                      
                        fixedComponents.Add(bestComp);

                       // int k = bestComp.id;
                        //int h = 0;
                        //DrawerHelper.SimpleDraw(design, result, new Size(600, 600), new Bitmap(600, 600), string.Format("iter {0}.png", k));
                       // h++;
                    }
                    else
                    { ValueCell[indCell] = -1; }
                }
            } while (fixedComponents.Count() != design.components.Count() && enumerator != 0);

            // размещение не размещенных компонентов

            foreach (Component comp in design.components)
            {
                if (result.placed[comp] == false)
                {
                    masswind = GetWindow(design, myApproximate, comp, XCellCoord, YCellCoord, ValueCell, height / 60, width / 60);
                    int ind = (int)myApproximate.x[comp] + (int)myApproximate.y[comp] * width;
                    int Coord = BestCell2(XCellCoord, YCellCoord, ValueCell, ind, comp, design, myApproximate, masswind);
                    int indCurrent = (int)myApproximate.x[comp] + (int)myApproximate.y[comp] * width;

                    result.x[comp] = XCellCoord[Coord];
                    result.y[comp] = YCellCoord[Coord];
                    result.placed[comp] = true;
                    myApproximate.x[comp] = XCellCoord[Coord];
                    myApproximate.y[comp] = YCellCoord[Coord];

                    if (ind < 0)
                    {
                        ClearCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, indCurrent, comp, compInCell);
                        AddCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, Coord, comp, compInCell);
                    }
                    else
                    {
                        ValueCell[ind] = -1;
                        compInCell[ind].Clear();

                        ClearCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, indCurrent, comp, compInCell);
                        AddCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, Coord, comp, compInCell);
                    }

                }
            }


        }

        public int[] GetWindow(Design design, PlacementGlobal myApproximate, Component Current, int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int wind_h, int wind_w)
        {
            int[] masswindow = new int[4];
            // середина компонента
            int x = (int)myApproximate.x[Current]; //+ Current.sizex / 2);
            int y = (int)myApproximate.y[Current]; //+ Current.sizey / 2);
            x = x + Current.sizex / 2;
            y = y+ Current.sizey / 2;
            
            //
            int x1 = x - wind_w;
            int x2 = x + wind_w;
            int y1 = y - wind_h;
            int y2 = y + wind_h;

            masswindow[0] = x1;
            masswindow[1] = x2;
            masswindow[2] = y1;
            masswindow[3] = y2;


            if (x1 < 0)
            {
                x1 = 0;
                x2 = wind_w * 2;
                masswindow[0] = x1;
                masswindow[1] = x2;
            }
            if (x2 > width)
            {
                x1 = width - wind_w * 2;
                x2 = width;
                masswindow[0] = x1;
                masswindow[1] = x2;
            }

            if (y1 < 0)
            {
                y1 = 0;
                y2 = wind_h * 2;
                masswindow[2] = y1;
                masswindow[3] = y2;
            }

            if (y2 > height)
            {
                y1 = height - wind_h * 2;
                y2 = height;
                masswindow[2] = y1;
                masswindow[3] = y2;
            }

            return masswindow;
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

        public int BestCell2(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int indexcurrent, Component Current, Design design, PlacementGlobal myApproximate, int[] masswindow)
        {
            int part;
            int indexOfBest;
            part = (masswindow[3] - masswindow[2]) / 2;
            int[] mass1 = new int[4] { masswindow[0], masswindow[1], masswindow[2], masswindow[2] + part };
            int[] mass2 = new int[4] { masswindow[0], masswindow[1], masswindow[2] + part, masswindow[3] };



            Task<double[]> task1 = new Task<double[]>(() => GetBestCoord2(XCellCoord, YCellCoord, ValueCell, Current, design, myApproximate, mass1));

            Task<double[]> task2 = new Task<double[]>(() => GetBestCoord2(XCellCoord, YCellCoord, ValueCell, Current, design, myApproximate, mass2));

            task1.Start();
            task2.Start();
            Task.WaitAll();

            double[] massdata1 = task1.Result;
            double[] massdata2 = task2.Result;

            //double[] first = GetBestCoord2(XCellCoord, YCellCoord, ValueCell, Current, design, myApproximate, masswindow);            
            double[] first = GetBest(massdata1, massdata2);

            indexOfBest = Convert.ToInt32(first[0]);
            return indexOfBest;

        }

        public double[] GetBestCoord2(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, Component Current, Design design, PlacementGlobal myApproximate, int[] masswindow)
        {
            double[] data = new double[4];
            int indexOfBest = masswindow[0] + masswindow[2] * width;
           

            while (CanNotBePlaced(ValueCell[indexOfBest], XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design))
            {
                indexOfBest++;
            }
            double percentBest = PercentCross2(XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design, myApproximate);
            double areaBest = CloselyCell(myApproximate, Current, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);
            double area2Best = NearNet(Current, myApproximate, design, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);

            double[] mass1 = new double[4] { indexOfBest, percentBest, areaBest, area2Best };

            for (int j = masswindow[2]; j < masswindow[3]; j++)
            {
                for (int i = masswindow[0] ; i < masswindow[1]; i++)
                {
                    if (indexOfBest == j * width + i) continue;
                    int ind = j * width + i;

                    if (CanNotBePlaced(ValueCell[ind], XCellCoord[ind], YCellCoord[ind], Current, design))
                        continue;

                    double percentCurrent = PercentCross2(XCellCoord[ind], YCellCoord[ind], Current, design, myApproximate);
                    double areaCurrent = CloselyCell(myApproximate, Current, XCellCoord[ind], YCellCoord[ind]);
                    double area2Current = NearNet(Current, myApproximate, design, XCellCoord[ind], YCellCoord[ind]);

                    double[] mass2 = new double[4] { ind, percentCurrent, areaCurrent, area2Current };
                    mass1 = GetBest(mass1, mass2);
                }
            }
            data[0] = mass1[0];
            data[1] = mass1[1];
            data[2] = mass1[2];
            data[3] = mass1[3];

            return data;
        }
       
        public double[] GetBestCoord(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int firstcoord, int qtcells, Component Current, Design design, PlacementGlobal myApproximate)
        {
            double[] data = new double[4];
            int indexOfBest = firstcoord;
            //double percentBest = 0;
            while (CanNotBePlaced(ValueCell[indexOfBest], XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design))
            {
                indexOfBest++;
            }
            double percentBest = PercentCross2(XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design, myApproximate);
            double areaBest = CloselyCell(myApproximate, Current, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);
            double area2Best = NearNet(Current, myApproximate, design, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);
           
            double[] mass1 = new double[4] { indexOfBest, percentBest, areaBest, area2Best };
            
            for (int i = indexOfBest + 1; i < qtcells; i++)
            {
                if (CanNotBePlaced(ValueCell[i], XCellCoord[i], YCellCoord[i], Current, design))
                    continue;

                double percentCurrent = PercentCross2(XCellCoord[i], YCellCoord[i], Current, design, myApproximate);
                double areaCurrent = CloselyCell(myApproximate, Current, XCellCoord[i], YCellCoord[i]);
                double area2Current = NearNet(Current, myApproximate, design, XCellCoord[i], YCellCoord[i]);
                                
                double[] mass2 = new double[4] { i, percentCurrent,areaCurrent, area2Current};
                mass1 = GetBest(mass1, mass2);
            }

            data[0] = mass1[0];
            data[1] = mass1[1];
            data[2] = mass1[2]; 
            data[3] = mass1[3];

            return data;
        }

        

        private double[] GetBest(double[] mass1, double[] mass2)
        {
            if (mass2[1] < mass1[1])
            {
                return mass2;
            }
            if (mass2[1] == mass1[1])
            {
                if (mass2[3] < mass1[3])
                {
                    return mass2;
                }

                if (mass2[3] == mass1[3])
                {
                    if (mass2[2] <= mass1[2])
                    {
                        return mass2;
                    }
                }
            }
            if (mass1[1] > mass2[1])
            { return mass1; }

            return mass1;
        }

        public int BestCell(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int indexcurrent, Component Current, Design design, PlacementGlobal myApproximate)
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
            lock (_lock)
            {
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
            }
            area = (maxX - minX) + (maxY - minY);
            return area;

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
        private bool CanNotBePlaced(int cell, int x, int y, Component component, Design design)
        {
            var field = design.field;
            //cell < 0 ||
            return
                x + component.sizex > field.beginx + field.cellsx ||
                y + component.sizey > field.beginy + field.cellsy;
        }
        //private double[] GetBest2(double[] mass1, double[] mass2)         
        //{
        //if (percentCurrent < percentBest)
        //{
        //    percentBest = percentCurrent;
        //    areaBest = areaCurrent;
        //    area2Best = area2Current;
        //    indexOfBest = i;
        //    continue;
        //}
        //if (percentCurrent == percentBest)
        //{
        //    if (area2Current < area2Best)
        //    {
        //        percentBest = percentCurrent;
        //        areaBest = areaCurrent;
        //        area2Best = area2Current;
        //        indexOfBest = i;
        //        continue;
        //    }

        //    if (area2Current == area2Best)
        //    {
        //        if (areaCurrent <= areaBest)
        //        {
        //            percentBest = percentCurrent;
        //            areaBest = areaCurrent;
        //            area2Best = area2Current;
        //            indexOfBest = i;
        //            continue;
        //        }
        //    }
        //}
        //if (percentCurrent > percentBest)
        //{ continue; }           
        //}

    }

}
