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
using System.Collections.Generic;
using System.Linq;

namespace DetailPlacer.Algorithm
{
    public class CrossCompPlacer : PlacerBase, IDetailPlacer
    {
       
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

        public void FillCells(Design design, PlacementGlobal approximate, int[] XCellCoord, int[] YCellCoord, int qtcells, int[] ValueCell,int h, int w, List<List<Component>> CompInCell)
         {
             double xCoord;
             double yCoord;
             int sizex;
             int sizey;
            
             foreach (Component comp in design.components)
             {
                 xCoord = approximate.x[comp];
                 yCoord = approximate.y[comp];
                 sizex = comp.sizex;
                 sizey = comp.sizey;
                 for (int i = 0; i < qtcells; i++)
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
                                         ValueCell[i + j * w + k]++;
                                         CompInCell[i + j * w + k].Add(comp);
                                     }
                                 }
                             }
                         }                     
                 }
                      
             }
         }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {                      
            int width = design.field.cellsx;
            int height = design.field.cellsy;
            int qtcells = width*height;
            int[] XCellCoord;
            int[] YCellCoord;           
            int enumerator;
            int indCell=0;          
            List<List<Component>> compInCell = InitCompInCell(qtcells);
            List<Component> fixedComponents = new List<Component>();        

            CreateCells(width, height, design,out XCellCoord, out YCellCoord, qtcells);
            int[] ValueCell = CreatCompValueCells(qtcells);
            FillCells(design, approximate, XCellCoord, YCellCoord, qtcells, ValueCell,height,width, compInCell);
            result = new PlacementDetail(design);
            do{
                enumerator=0;                 
               
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
                        bestCoord = BestCell(XCellCoord, YCellCoord, ValueCell, qtcells, indCell, bestComp, design, approximate);
                        result.x[bestComp] = XCellCoord[bestCoord];
                        result.y[bestComp] = YCellCoord[bestCoord];
                        result.placed[bestComp] = true;
                        
                        ValueCell[indCell] = -1;
                        for (int i = 0; i < bestComp.sizey; i++)
                        {
                            for (int j = 0; j < bestComp.sizex; j++)
                            {
                                if (bestCoord + j + i * width < qtcells)
                                {
                                    ValueCell[bestCoord + j + i * width] = -1;
                                }
                                else
                                    continue;
                            }
                        }
                        fixedComponents.Add(bestComp);

                    }
                    else
                    { ValueCell[indCell] = -1; }

                }
            }while(fixedComponents.Count() != design.components.Count() && enumerator != 0);
                    
            foreach (Component comp in design.components)
            {
                if (result.placed[comp] == false)
                {
                                       
                    result.x[comp] = (int)approximate.x[comp];
                    result.y[comp] = (int)approximate.y[comp];
                    int ind = GetCellIndex((int)approximate.x[comp],(int)approximate.y[comp], XCellCoord,YCellCoord,qtcells);
                    //int Coord = BestCell(XCellCoord, YCellCoord, ValueCell, qtcells, ind, comp, design, approximate);
                    //result.x[comp] = XCellCoord[Coord];
                    //result.y[comp] = YCellCoord[Coord];
                    result.placed[comp] = true;
                    ValueCell[ind] = -1;
                }
            }
         
        }
        public int GetCellIndex(int x, int y, int[] XCoord, int[] YCoord, int qt)
        {
          for( int j=0; j< qt; j++)
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

                    if (square >= maxSquare)
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
        
        public int BestCell(int[] XCellCoord, int[] YCellCoord, int[] ValueCell, int qtcells, int indexcurrent, Component Current, Design design, PlacementGlobal approximate)
        {
            int indexOfBest = 0;

            while (CanNotBePlaced(ValueCell[indexOfBest], XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design))
            {
                indexOfBest++;
            }

            double percentBest=PercentCross(XCellCoord[indexOfBest], YCellCoord[indexOfBest], Current, design, approximate);
            int areaBest=CloselyCell(XCellCoord[indexOfBest], YCellCoord[indexOfBest], XCellCoord[indexcurrent], YCellCoord[indexcurrent]);
            double area2Best=NearNet(Current, approximate, design, XCellCoord[indexOfBest], YCellCoord[indexOfBest]);

            for(int i = indexOfBest + 1; i < qtcells; i++)
            {
                if (CanNotBePlaced(ValueCell[i], XCellCoord[i], YCellCoord[i], Current, design))
                    continue;

                double percentCurrent = PercentCross(XCellCoord[i], YCellCoord[i], Current, design, approximate);
                int areaCurrent = CloselyCell(XCellCoord[i], YCellCoord[i], XCellCoord[indexcurrent], YCellCoord[indexcurrent]);
                double area2Current = NearNet(Current, approximate, design, XCellCoord[i], YCellCoord[i]);

                var percentOfBestIsBest = percentBest < percentCurrent;
                var areaOfBestIsBest = areaBest < areaCurrent;
                var area2OfBestIsBest = area2Best < area2Current;

               
                if(percentOfBestIsBest && areaOfBestIsBest ||
                   percentOfBestIsBest && area2OfBestIsBest ||
                   areaOfBestIsBest && area2OfBestIsBest)
                    continue;

                percentBest = percentCurrent;
                areaBest = areaCurrent;
                area2Best = area2Current;
                indexOfBest = i;
            }

            return  indexOfBest;
        }

        private bool CanNotBePlaced(int cell, int x, int y, Component component, Design design)
        {
            var field = design.field;

            return cell < 0 ||
                x + component.sizex > field.beginx + field.cellsx ||
                y + component.sizey > field.beginy + field.cellsy;
        }

        public double PercentCross(int XCellCoord, int YCellCoord, Component Current, Design design, PlacementGlobal approximate)
        {
            double result=0;
            int minXcurrent = XCellCoord;
            int maxXcurrent = XCellCoord + Current.sizex;
            int minYcurrent = YCellCoord;
            int maxYcurrent = YCellCoord + Current.sizey;
            double currentpercent = 0;                                 
            foreach (Component next in design.components)
            {
                double minXnext = approximate.x[next];
                double maxXnext = approximate.x[next] + next.sizex;
                double minYnext = approximate.y[next];
                double maxYnext = approximate.y[next] + next.sizey;

                if (next == Current) continue;

                else
                {                    
                    if ((minXcurrent <= minXnext && minXnext < maxXcurrent && minYcurrent <= minYnext && minYnext < maxYcurrent)
                        || (minXcurrent < maxXnext && maxXnext <= maxXcurrent && minYcurrent <= minYnext && minYnext < maxYcurrent)
                        || (minXcurrent < minXnext && minXnext <= maxXcurrent && minYcurrent < maxYnext && maxYnext <= maxYcurrent)
                        || (minXcurrent < maxXnext && maxXnext <= maxXcurrent && minYcurrent < maxYnext && maxYnext <= maxYcurrent))
                    {
                        AreaCross(minXcurrent, maxXcurrent, minYcurrent, maxYcurrent, minXnext, maxXnext, minYnext, maxYnext, out currentpercent);
                        result = currentpercent + result;
                        continue;
                    }

                    if ((minXnext <= minXcurrent && minXcurrent < maxXnext && minYnext <= minYcurrent && minYcurrent < maxYnext)
                        || (minXnext < maxXcurrent && maxXcurrent <= maxXnext && minYnext <= minYcurrent && minYcurrent < maxYnext)
                        || (minXnext < minXcurrent && minXcurrent <= maxXnext && minYnext < maxYcurrent && maxYcurrent <= maxYnext)
                        || (minXnext < maxXcurrent && maxXcurrent <= maxXnext && minYnext < maxYcurrent && maxYcurrent <= maxYnext))
                    {
                        AreaCross(minXcurrent, maxXcurrent, minYcurrent, maxYcurrent, minXnext, maxXnext, minYnext, maxYnext, out currentpercent);
                        result = currentpercent + result;
                        continue;
                    }
                  
                }
            }
            return result;
        }

        public void AreaCross(int minXcurrent, int maxXcurrent, int minYcurrent, int maxYcurrent, double minXnext, double maxXnext, double minYnext, double maxYnext,out double percent)
        {
            double minX = Math.Max(minXcurrent,minXnext);
            double minY = Math.Max(minYcurrent,minYnext);
            double maxX = Math.Min(maxXcurrent,maxXnext);
            double maxY = Math.Min(maxYcurrent,maxYnext);
            int Scurrent = (maxYcurrent - minYcurrent) * (maxXcurrent - minXcurrent);
            double Scross = (maxX - minX) * (maxY - minY);
            percent = (Scross*100)/Scurrent;

        }

        public int CloselyCell(int Xcoord, int Ycoord, int XcurrCoord, int YcurrCoord)
        {
            int minX;
            int minY;
            int maxX;
            int maxY;
            minX = Math.Min(Xcoord, XcurrCoord);
            minY = Math.Min(Ycoord, YcurrCoord);
            maxX = Math.Max(Xcoord+1, XcurrCoord+1);
            maxY = Math.Max(Ycoord+1, YcurrCoord+1);
            int area = (maxX - minX) * (maxY - minY);
            return area;
        }

        public double NearNet(Component Current, PlacementGlobal approximate, Design design, int XCellCoord, int YCellCoord)
        {
            double minX = approximate.x[Current] ;
            double maxX = approximate.x[Current] + Current.sizex;
            double minY = approximate.y[Current];
            double maxY = approximate.y[Current]+Current.sizey ;

            foreach( Net net in design.Nets(Current)) 
            {
                foreach (Component next in net.items)
                { 
                 minX = Math.Min(minX, approximate.x[next]);
                 maxX = Math.Max(maxX,approximate.x[next]+next.sizex);
                 minY = Math.Min(minY, approximate.y[next]);
                 maxY = Math.Max(maxY,approximate.y[Current] + Current.sizey);
                }
            }
            double area = (maxX - minX) * (maxY - minY);
            return area;
        }

    }

}
