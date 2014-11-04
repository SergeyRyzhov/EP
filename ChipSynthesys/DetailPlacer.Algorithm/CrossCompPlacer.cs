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
        private readonly object _lock1 = new object();
        public int Width;
        public int Height;
        public int QtCells;
        public void CreateCells(Design design, out int[] XCellCoord, out int[] YCellCoord)
        {
            var coord_x = design.field.beginx;
            var coord_y = design.field.beginy;
            var index = 0;
            XCellCoord = new int[QtCells];
            YCellCoord = new int[QtCells];


            coord_y = design.field.beginy;
            index = 0;
            for (int i = 0; i < Height; i++)
            {
                coord_x = design.field.beginx; ;
                for (int j = 0; j < Width; j++)
                {
                    XCellCoord[index] = coord_x;
                    YCellCoord[index] = coord_y;
                    coord_x++;
                    index++;
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

        public int GetNomberCell( int[] XCellCoord, int[] YCellCoord, int x, int y)
        {
        var result=0;
            for ( int i =0; i < QtCells; i++)
            {
             if (XCellCoord[i] == x && YCellCoord[i] == y)
             { result = i; break; }              
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

            for (int i = 0; i < QtCells; i++)
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

                index = GetNomberCell(XCellCoord, YCellCoord, x, y);

                if (x < xCoord && y < yCoord)
                { sizex++; sizey++; }
                if (x < xCoord && y == yCoord)
                { sizex++; }
                if (x == xCoord && y < yCoord)
                { sizey++; }

                for (int j = 0; j < sizey; j++)
                {
                    for (int k = 0; k < sizex; k++)
                    {
                        if (xCoord + k <= Width && xCoord + k >= design.field.beginx && yCoord + j <= Height && yCoord + j >= design.field.beginy)
                        {                           
                            ValueCell[index + j * Width + k]++;
                            CompInCell[index + j * Width + k].Add(comp);
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

        public void ClearCells(int[] ValueCells, Design design, PlacementGlobal myApproximate, int[] XCellCoord, int[] YCellCoord, Component current, List<List<Component>> CompInCell)
        {

            double xCoord = myApproximate.x[current];
            double yCoord = myApproximate.y[current];          
            int indexfirst = GetNomberCell(XCellCoord, YCellCoord, (int)xCoord, (int)yCoord);
            int sizex = current.sizex;
            int sizey = current.sizey;

            for (int i = 0; i < sizey; i++)
            {
                for (int j = 0; j < sizex; j++)
                {
                    if (ValueCells[indexfirst + j + i * Width] > 0)
                    {
                        if (xCoord + j <= Width && xCoord + j >= design.field.beginx && yCoord + i <= Height && yCoord + i >= design.field.beginy)
                        {
                            ValueCells[indexfirst + j + i * Width]--;
                            CompInCell[indexfirst + j + i * Width].Remove(current);
                        }
                    }

                }
            }
           
        }
        public void AddCells(int[] ValueCells, Design design, PlacementGlobal myApproximate,int indexbest, int[] XCellCoord, int[] YCellCoord, Component current, List<List<Component>> CompInCell)
        {
            int xCoord = (int)myApproximate.x[current];
            int yCoord = (int)myApproximate.y[current];
            int sizex = current.sizex;
            int sizey = current.sizey;
            
            for (int i = 0; i < sizey; i++)
            {
                for (int j = 0; j < sizex; j++)
                {
                    if (ValueCells[indexbest + j + i * Width] != -1 && xCoord + j <= Width && xCoord + j >= design.field.beginx && yCoord + i <= Height && yCoord + i >= design.field.beginy)
                    {
                        ValueCells[indexbest + j + i * Width]++;
                        CompInCell[indexbest + j + i * Width].Add(current);
                    }
                }
            }
            
        }



        public virtual void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            Width = design.field.cellsx;
            Height = design.field.cellsy;
            QtCells = Width * Height;
                       
            int[] XCellCoord;
            int[] YCellCoord;
            
            int enumerator;
            int indCell = 0;

            
            List<List<Component>> compInCell = InitCompInCell(QtCells);
            List<Component> fixedComponents = new List<Component>();
            var myApproximate = new PlacementGlobal(design);
            myApproximate = CreateMyApproximate(approximate, design);
            CreateCells(design, out XCellCoord, out YCellCoord);
            int[] ValueCell = CreatCompValueCells(QtCells);
            result = new PlacementDetail(design);
                      
            FillCells(design, myApproximate, XCellCoord, YCellCoord, ValueCell, compInCell);
            do
            {                  
                enumerator = ValueCell.Max();                
                indCell = Array.IndexOf(ValueCell, enumerator);

                if (enumerator > 1)
                {  
                
                    Component bestComp = GetComponentWithMaxSquare(compInCell[indCell], result);
                    if (bestComp != null)
                    {
                        List<int> array = new List<int>();
                        for (int i = 0; i < QtCells; i++)
                        {
                            if (ValueCell[i] == 0) { array.Add(i); }
                        }
                        int[] arraycell = array.ToArray();
                        // выбор лучшей позиции 
                        int bestCoord = GetBestCell(XCellCoord, YCellCoord, ValueCell, indCell, bestComp, design, myApproximate, arraycell);
                        ClearCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord,  bestComp,compInCell);
                        // поиск позиции текущего компонента 
                        result.x[bestComp] = XCellCoord[bestCoord];
                        result.y[bestComp] = YCellCoord[bestCoord];
                        myApproximate.x[bestComp] = XCellCoord[bestCoord];
                        myApproximate.y[bestComp] = YCellCoord[bestCoord];
                        result.placed[bestComp] = true;
                        // очищение ячейки 
                        if (ValueCell[indCell] <= 2)
                        {
                            ValueCell[indCell] = -1;
                            compInCell[indCell].Clear();
                        }
                        // удаление и добавление значений для компоненты в ячейки
                        
                        AddCells(ValueCell, design, myApproximate,bestCoord, XCellCoord, YCellCoord,  bestComp, compInCell);
                       
                        // фиксация компонента                      
                        fixedComponents.Add(bestComp);

                    }
                    else
                    { ValueCell[indCell] = -1; compInCell[indCell].Clear(); }
                }
            } while (fixedComponents.Count() != design.components.Count() && enumerator != 1);

            // размещение не размещенных компонентов

            foreach (Component comp in design.components)
            {                
                if (result.placed[comp] == false)
                {
                    List<int> array = new List<int>();
                    
                    for (int i = 0; i < QtCells; i++)
                    {
                        if (ValueCell[i] == 0) { array.Add(i); }                        
                    }

                    int[] arraycell = array.ToArray();

                    int indCurrent = GetNomberCell(XCellCoord, YCellCoord, (int)myApproximate.x[comp], (int)myApproximate.y[comp]);
                    
                    int bestCell = GetBestCell(XCellCoord, YCellCoord, ValueCell, indCurrent, comp, design, myApproximate, arraycell);
                    ClearCells(ValueCell, design, myApproximate, XCellCoord, YCellCoord, comp, compInCell);
                    
                    
                    ValueCell[indCurrent] = -1;
                    compInCell[indCurrent].Clear();                      
                    
                    result.x[comp] = XCellCoord[bestCell];
                    result.y[comp] = YCellCoord[bestCell];
                    result.placed[comp] = true;
                    myApproximate.x[comp] = XCellCoord[bestCell];
                    myApproximate.y[comp] = YCellCoord[bestCell];

                    AddCells(ValueCell, design, myApproximate,bestCell, XCellCoord, YCellCoord,  comp, compInCell);
                }
            }

        }


        public Component GetComponentWithMaxSquare(List<Component> cell, PlacementDetail result)
        {
            Component bestComponent = null;
            var maxSquare = 0;

            for (int i = 0; i < cell.Count; i++)
            {
                var component = cell[i];
                if (result.placed[component] == false)
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

        public int GetBestCell(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int indexcurrent, Component Current, Design design, PlacementGlobal myApproximate, int[] array )
        {
            var part = array.Length / 2;

            Task<double[]> task1 = new Task<double[]>(() => GetBestCoord(XCellCoord, YCellCoord, ValueCell, Current, design, myApproximate,array, 0, part - 1));
            Task<double[]> task2 = new Task<double[]>(() => GetBestCoord(XCellCoord, YCellCoord, ValueCell, Current, design, myApproximate, array, part, array.Length - 1));

            task1.Start();
            task2.Start();
            Task.WaitAll();

            double[] massdata1 = task1.Result;
            double[] massdata2 = task2.Result;

          //  double[] first = GetBestCoord(XCellCoord, YCellCoord, ValueCell, Current, design, myApproximate, array, 0, array.Length - 1);
            double[] first = GetBestIndex(massdata1, massdata2);


            var indexOfBest = Convert.ToInt32(first[0]);
            return indexOfBest;

        }

        public double[] GetBestCoord(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, Component Current, Design design, PlacementGlobal myApproximate,int[] array, int first, int end)
        {
            var result = new double[4];
            var indexOfBest = array[first];

            while (CanNotBePlaced(ValueCell[indexOfBest], XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design))
            {
                indexOfBest = array[first];
                first++;
            }
            double percentBest = PercentCross(XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design, myApproximate);
            double areaBest = CloselyCell(myApproximate, Current, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);
            double area2Best = NearNet(Current, myApproximate, design, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);

            int p = (array[end] - indexOfBest) / 500;
            if (p <= 2) p = 1;
            for (var ind = indexOfBest; ind < array[end]; ind+=p)
            {              
                
                if (CanNotBePlaced(ValueCell[ind], XCellCoord[ind], YCellCoord[ind], Current, design))
                    continue;

                    double percentCurrent = PercentCross(XCellCoord[ind], YCellCoord[ind], Current, design, myApproximate);
                    double areaCurrent = 0;
                    double area2Current = 0;

                    if (percentCurrent < percentBest)
                    {
                        areaCurrent = CloselyCell(myApproximate, Current, XCellCoord[ind], YCellCoord[ind]);
                        area2Current = NearNet(Current, myApproximate, design, XCellCoord[ind], YCellCoord[ind]);
                        percentBest = percentCurrent;
                        areaBest = areaCurrent;
                        area2Best = area2Current;
                        indexOfBest = ind;
                        continue;
                    }
                    if (percentCurrent == percentBest)
                    {
                        area2Current = NearNet(Current, myApproximate, design, XCellCoord[ind], YCellCoord[ind]);
                        areaCurrent = CloselyCell(myApproximate, Current, XCellCoord[ind], YCellCoord[ind]);
                        if (area2Current < area2Best)
                        {
                            areaCurrent = CloselyCell(myApproximate, Current, XCellCoord[ind], YCellCoord[ind]);
                            percentBest = percentCurrent;
                            areaBest = areaCurrent;
                            area2Best = area2Current;
                            indexOfBest = ind;
                            continue;
                        }

                        if (area2Current == area2Best)
                        {                            
                            if (areaCurrent <= areaBest)
                            {
                                percentBest = percentCurrent;
                                areaBest = areaCurrent;
                                area2Best = area2Current;
                                indexOfBest = ind;
                                continue;
                            }
                        }
                    }
                    if (percentCurrent > percentBest)
                    { continue; }  
                                 
            }
           result[0] = indexOfBest;
           result[1] = percentBest;
           result[2] = areaBest;
           result[3] = area2Best;

            return result;
        }
       
        private double[] GetBestIndex(double[] mass1, double[] mass2)
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

       
        public double PercentCross(int XCellCoord, int YCellCoord, Component Current, Design design, PlacementGlobal myApproximate)
        {
            double result = 0;
            var minXcurrent = XCellCoord;
            var maxXcurrent = XCellCoord + Current.sizex;
            var minYcurrent = YCellCoord;
            var maxYcurrent = YCellCoord + Current.sizey;
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
            var Scurrent = (maxYcurrent - minYcurrent) * (maxXcurrent - minXcurrent);
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
                
        private bool CanNotBePlaced(int cell, int x, int y, Component component, Design design)
        {
            var field = design.field;
            
            return  cell < 0 ||          
                x + component.sizex >field.cellsx + field.beginx ||
                y + component.sizey >field.cellsy + field.beginy;
        }
       

    }

}
