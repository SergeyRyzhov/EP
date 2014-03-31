using ChipSynthesys.Common.Randoms;
using PlaceModel;
using System;
using System.Collections.Generic;

namespace ChipSynthesys.Common.Generators
{
    public class SquareGenerator : IGenerator
    {
        public void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesign(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, int width, int height, out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, int width, int height, out Design design, out PlacementDetail placement)
        {
           
            var Parts = new List<int>();
            var Coordinate = new List<int>();
            var X_Coord = new List<int>();
            var Y_Coord = new List<int>();
            var rnd = new Random();
            int SizeX, SizeY;
            SizeX = components * maxSizeX;
            SizeY = components * maxSizeY;
            Division_Square(components,percent, SizeX, SizeY,maxSizeX,maxSizeY,Parts,Coordinate);  
            var c = new Component.Pool();
            var sum = 0;
            var num=0;
            var h=0;
            var w = 0;
            
                for (var i = 0; i < Parts.Count - 1; i += 2)
                {
                    c.Add(Parts[i] , Parts[i + 1]);
                    X_Coord.Add(Coordinate[i]);
                    Y_Coord.Add(Coordinate[i + 1]);
                    sum += Parts[i] * Parts[i + 1];
                    if (Coordinate[i] + Parts[i] > w)
                    {w = Coordinate[i] + Parts[i]; }
                    if (Coordinate[i + 1] + Parts[i + 1] > h)
                    { h = Coordinate[i + 1] + Parts[i + 1]; }
                    num = num + 1;
                    if (components == num)
                        break;
                }
          
            Add_Square(w, h, sum, percent);
            SizeX = w;
            SizeY = h;
            var n = new Net.Pool();
            design = new Design(new Field(0, 0, SizeX, SizeY), c, n); 
            placement = new PlacementDetail(design);
            var l=0;
            foreach( var com in design.components)
            {
                placement.x[com] = X_Coord[l];
                placement.y[com] = Y_Coord[l];
                placement.placed[com] = true;
                l++;
            }
             
            var Net = new List<int>();
            var n_net = new List<int>();            
            for (var k = 0; k < nets; k++)
            {
                var q = rnd.Next(components);
                Net.Add(q);                
                var x_q = placement.x[c[q]];
                var y_q = placement.y[c[q]];
                var w_q = c[q].sizex;
                var h_q = c[q].sizey;
                var i=1;                
                for (var j = 0; j < components; j++)
                {
                    var x_j = placement.x[c[j]];
                    var y_j = placement.y[c[j]];
                    var w_j = c[j].sizex;
                    var h_j = c[j].sizey;

                    if ((x_q == x_j + w_j && y_q == y_j) || (x_q == x_j && y_q == y_j + h_j) || (x_q + w_q == x_j && y_q == y_j) || (x_q == x_j && y_q + h_q == y_j))
                    {
                        Net.Add(j);
                        i++;
                    }
                    //if ((x_q == x_j + w_j && y_j < y_q && y_q < y_j + h_j) || (x_q == x_j + w_j && y_q < y_j && y_j < y_q + h_q) || (x_q + w_q == x_j && y_j < y_q && y_q < y_j + h_j) || (x_q + w_q == x_j && y_q <y_j && y_j < y_q + h_q))
                    //{
                    //    Net.Add(j);
                    //    i++;
                    //}
                    //if ((y_q + h_q == y_j && x_j < x_q && x_q < x_j + w_j) || (y_q + h_q == y_j && x_q < x_j && x_j < x_q + w_q) || (y_q == y_j + h_j && x_j < x_q && x_q < x_j + w_j) || (y_q == y_j + h_j && x_q < x_j && x_j < x_q + w_q))
                    //{
                    //    Net.Add(j);
                    //    i++;
                    //}
                    if (i == maxNetSize || j==components-1)
                    { n_net.Add(i); break; }
                }
                
            }
            var count=0;
            for (var i = 0; i < nets; i++)
            {
                
                n.Add(new Component[n_net[i]]);
                for (var j = 0; j <n[i].items.Length; j++)
                {
                    var component = Net[count];
                      n[i].items[j] = c[component];
                      count++;                
                }
            }


            width = SizeX; //это кстати не прокатит для чисел они передаются по значению и не возвращаются  gоkjб[ратно
            height = SizeY;
        }

        private void Division_Square(int components, int percent,int SizeX, int SizeY, int maxSizeX, int maxSizeY, List<int> Parts, List<int> Coordinate)
        {
            int Square;          
            Square = SizeX * SizeY ;           
            Division(SizeX, SizeY, maxSizeX, maxSizeY,SizeX/2,SizeY/2, 0, Parts, Coordinate);
            do
            {
                var i = Control_First(Parts, maxSizeX, maxSizeY);            
                if ( i<0) break;                
                if (Parts[i] == 1 || Parts[i + 1] == 1)
                { break; }
                else
                { Division(Parts[i], Parts[i + 1], maxSizeX, maxSizeY, Parts[i] / 2, Parts[i + 1] / 2, i, Parts, Coordinate); }
            } while(true);
            do
            {

                var i = Contorol(Parts, maxSizeX, maxSizeY, components);
                if (i < 0) break;
                Division(Parts[i], Parts[i+1], maxSizeX, maxSizeY,maxSizeX,maxSizeY, i, Parts, Coordinate);
            } while (true);

        }

        private void Division(int x, int y, int maxX, int maxY, int d_x, int d_y, int ind, List<int> Parts, List<int> Coordinate)
        {
            int x1, x2, y1, y2;
            var c_x = 0;
            var c_y = 0;
            var rnd = new Random();

            if ((x > y && x>maxX) || (x==y && maxY > maxX )||(y==maxY && x>maxX) || ( y==1 && x>maxX))
            {
                if (maxX > d_x && d_x!=1)
                { d_x = maxX; }
               
                var n = 1 + rnd.Next(d_x);
                x1 = n;
                x2 = x - n;
                if (x1 < 0 || x2 < 0 || x1==0 ||x2==0)
                {
                    var k = 21;
                }
                if (Parts.Count != 0)
                {
                    c_y = Coordinate[ind + 1];
                    c_x = n + Coordinate[ind];
                    Parts.RemoveAt(ind + 1);
                    Parts.RemoveAt(ind);
                    Parts.Insert(ind, x1);
                    Parts.Insert(ind + 1, y);
                    Parts.Insert(ind + 2, x2);
                    Parts.Insert(ind + 3, y);
                    if (Coordinate.Count-1 == ind + 1)
                    {
                        Coordinate.Add(c_x);
                        Coordinate.Add(c_y);
                    }
                    else
                    {
                        Coordinate.Insert(ind + 2, c_x);                        
                        Coordinate.Insert(ind + 3, c_y);
                    }
                }
                else
                {

                    Parts.Add(x1);
                    Coordinate.Add(c_x);
                    Parts.Add(y);
                    Coordinate.Add(c_y);
                    Parts.Add(x2);
                    Coordinate.Add(n);
                    Parts.Add(y);
                    Coordinate.Add(c_y);
                }

            }

            if ((y > x&& y>maxY) || ( x==y && maxX > maxY) || ( x == maxX && y > maxY) || ( x==1 && y>maxY))
            {
                if (maxY > d_y && d_y != 1)
                { d_y = maxY; }
                var n = 1 + rnd.Next(d_y);
                y1 = n;
                y2 = y - n;
                if ( y1 <=0  || y2 <=0)
                { var h=23;}
                if (Parts.Count != 0)
                {   
                    c_x = Coordinate[ind];
                    c_y = Coordinate[ind + 1] + n;
                    Parts.RemoveAt(ind + 1);
                    Parts.RemoveAt(ind);
                    Parts.Insert(ind, x);
                    Parts.Insert(ind + 1, y1);
                    Parts.Insert(ind + 2, x);
                    Parts.Insert(ind + 3, y2);
                    if (Coordinate.Count-1 == ind + 1)
                    {
                        Coordinate.Add(c_x);
                        Coordinate.Add(c_y);
                    }
                    else
                    {
                        Coordinate.Insert(ind + 2, c_x);
                        Coordinate.Insert(ind + 3, c_y);
                    }
                }
                else
                {
                    Parts.Add(y1);
                    Coordinate.Add(c_x);
                    Parts.Add(x);
                    Coordinate.Add(c_y);
                    Parts.Add(y2);
                    Coordinate.Add(c_x);
                    Parts.Add(x);
                    Coordinate.Add(n);
                }
            }
          

        }

        private int Contorol(List<int> Parts, int maxX, int maxY, int comp)
        {
            for (var i = 0; i < Parts.Count-1; i += 2)
            {
                if (Parts[i] > maxX || Parts[i + 1] > maxY)
                {
                    return i;
                }
                if (Parts[i] <= maxX && Parts[i + 1] <=  maxY &&  comp != 0)
                {
                    comp = comp - 1;                    
                }
                if (comp == 0)
                { return -1; }
            }
            return -1;
        }

        private int Control_First(List<int> Parts, int maxX, int maxY)
        {
            for (var i = 0; i < Parts.Count - 1; i+=2)
            {
                if (Parts[i] * Parts[i + 1] > maxX * maxY * 2)
                {
                    return i;
                }                
            }
            return -1;
        }

        private void Add_Square(int w, int h, int sum, int percent)
        {
            int col;            
            col = ((100 - percent) * sum) / percent;
            col = col - (w * h - sum);// add square;
            if (col > w && col > h)
            {
                if (col % w == 0 || col % h == 0)
                {
                    if (col % w == 0)
                    {
                        h = h + (col / w);
                    }
                    if (col % h == 0)
                    {
                        w = w + (col / h);
                    }
                }
                else
                {
                    if (h < w)
                    { Size(col, h, w); }
                    if (w < h)
                    { Size(col, w, h); }
                }
            }
            else
            {
                w = w - 0;
                h = h - 0;
            } 
            
        }

        private void Size(int col, int h, int w)
        {
         do
                    {
                        col = col - h;
                        w = w + 1;
                        if (col < h)
                        { break; }                        
                    } while (true);
        }
    }
}
