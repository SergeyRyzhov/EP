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
        public void Clear(int qtcells, int[] ValueCell, List<List<Component>> CompInCell)
        {
            for (int i = 0; i < qtcells; i++)
            {
                CompInCell[i].Clear();
                if (ValueCell[i] < 0) continue;
                else
                    ValueCell[i] = 0;
            }
        }
        public void CreateCells(int width, int height, Design design, out int[] XCellCoord, out int[] YCellCoord, int qtcells, out int[] ValueCells, List<List<Component>> CompInCell)
                {
                    int coord_x = design.field.beginx;
                    int coord_y = design.field.beginy;                   
                    XCellCoord = new int[qtcells];
                    YCellCoord = new int[qtcells];
                    ValueCells = new int[qtcells];
            
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
                    for (int i = 0; i < qtcells; i++)
                    {
                        ValueCells[i] = 0;
                        CompInCell.Add(new List<Component>());
                    }
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
                     if (ValueCell[i] < 0) continue;                     
                     if (XCellCoord[i] <= xCoord && xCoord < XCellCoord[i]+1 && YCellCoord[i] <= yCoord && yCoord < YCellCoord[i]+1)
                     {
                         
                         if (XCellCoord[i] < xCoord && YCellCoord[i] < yCoord)
                         { sizex++; sizey++; }
                         for (int j = 0; j < sizey; j++)
                         {
                             for (int k = 0; k < sizex; k++)
                             {                                 
                                 ValueCell[i+ j*w + k]++;                                 
                                 CompInCell[i + j * w + k].Add(comp);
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
            int[] ValueCell;
            int enumerator;
            int indCell=0;                       
            List<List<Component>> CompInCell = new List<List<Component>>();           
            List<Component> FixComp = new List<Component>();
            CreateCells(width, height, design,out XCellCoord, out YCellCoord, qtcells,out ValueCell,CompInCell);
            
            result = new PlacementDetail(design);
            do{
                enumerator=0; 
                
                FillCells(design, approximate, XCellCoord, YCellCoord, qtcells, ValueCell,height,width, CompInCell);
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
                 int X=0,Y=0;                 
                 Component bestComp;
                 BestComp(CompInCell,FixComp, indCell, out bestComp);// Критерии                 
                
                 BestCell(XCellCoord, YCellCoord, qtcells, indCell, bestComp, design, approximate, out X, out Y);
                 result.x[bestComp] = X;
                 result.y[bestComp] = Y;
                 result.placed[bestComp] = true;
                 approximate.x[bestComp] = X;
                 approximate.y[bestComp] = Y;
                 
                 ValueCell[indCell] = -1;
                 FixComp.Add(bestComp);
                 Clear(qtcells, ValueCell, CompInCell);           
                                  
                }
            }while(FixComp.Count() != design.components.Count());
        }

        public void BestComp(List<List<Component>> CompInCell,List<Component> FixComp, int index,out Component bestComp)
        {
            List<int> S = new List<int>();
            if (CompInCell.Count == 1)
                bestComp = CompInCell[index][0];
            else
            {
                foreach (Component comp in CompInCell[index])
                {
                    if (FixComp.Count != 0)
                    {
                        foreach (Component fix in FixComp)
                            if (fix == comp) S.Add(0);
                            else S.Add(comp.sizex * comp.sizey);
                    }
                    else
                         S.Add(comp.sizex * comp.sizey);
                }
                int max = S.Max();
                int i = S.IndexOf(max);
                bestComp = CompInCell[index][i];
            }
        }

        public void BestCell(int[] XCellCoord, int[] YCellCoord, int qtcells, int indexcurrent, Component Current, Design design, PlacementGlobal approximate, out int X,out int Y)
        {           
           double percentEarly=0,area2Early=0; int areaEarly=0;
           int x = 0, y =0; 
            for (int i = 0; i < qtcells; i++)
            {               
              double percentCurrent=0,area2Current=0;;            
              int areaCurrent=0;     
              PercentCross(XCellCoord[i], YCellCoord[i], Current, design, approximate, out percentCurrent);
              CloselyCell(XCellCoord[i], YCellCoord[i], XCellCoord[indexcurrent], YCellCoord[indexcurrent], out areaCurrent);
              NearNet(Current, approximate, design, XCellCoord[i], YCellCoord[i], out area2Current);
              if (i == 0)
              { percentEarly = percentCurrent; areaEarly = areaCurrent; area2Early = area2Current; x = XCellCoord[i]; y = YCellCoord[i]; }
              else
              {
                  if ((percentEarly <= percentCurrent && areaEarly <= areaCurrent && area2Early <= area2Current)
                      || (percentEarly <= percentCurrent && areaEarly <= areaCurrent && area2Early > area2Current)
                      || (percentEarly <= percentCurrent && areaEarly > areaCurrent && area2Early <= area2Current)
                      || (percentEarly > percentCurrent && areaEarly <= areaCurrent && area2Early <= area2Current))
                  { continue; }
                  else
                  { x = XCellCoord[i]; y = YCellCoord[i]; }
              }
            }
            X = x;
            Y = y;
        }
               
        public void PercentCross(int XCellCoord, int YCellCoord, Component Current, Design design, PlacementGlobal approximate, out double percent)
        {
            int minXcurrent = XCellCoord;
            int maxXcurrent = XCellCoord + Current.sizex;
            int minYcurrent = YCellCoord;
            int maxYcurrent = YCellCoord + Current.sizey;
            double currentpercent = 0;
            percent = 0;                        
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
                        percent = currentpercent + percent;
                        continue;
                    }

                    if ((minXnext <= minXcurrent && minXcurrent < maxXnext && minYnext <= minYcurrent && minYcurrent < maxYnext)
                        || (minXnext < maxXcurrent && maxXcurrent <= maxXnext && minYnext <= minYcurrent && minYcurrent < maxYnext)
                        || (minXnext < minXcurrent && minXcurrent <= maxXnext && minYnext < maxYcurrent && maxYcurrent <= maxYnext)
                        || (minXnext < maxXcurrent && maxXcurrent <= maxXnext && minYnext < maxYcurrent && maxYcurrent <= maxYnext))
                    {
                        AreaCross(minXcurrent, maxXcurrent, minYcurrent, maxYcurrent, minXnext, maxXnext, minYnext, maxYnext, out currentpercent);
                        percent = currentpercent + percent;
                        continue;
                    }

                    //else
                    //    percent += 0;
                   
                }
            }
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
        
        public void CloselyCell(int Xcoord, int Ycoord, int XcurrCoord, int YcurrCoord, out int area)
        {
            int minX;
            int minY;
            int maxX;
            int maxY;
            minX = Math.Min(Xcoord, XcurrCoord);
            minY = Math.Min(Ycoord, YcurrCoord);
            maxX = Math.Max(Xcoord+1, XcurrCoord+1);
            maxY = Math.Max(Ycoord+1, YcurrCoord+1);
            area = (maxX - minX)*(maxY - minY);
        }

        public void NearNet(Component Current,PlacementGlobal approximate, Design design, int XCellCoord, int YCellCoord, out double area)
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
            area = (maxX - minX) * (maxY - minY);
        }

    }

}
