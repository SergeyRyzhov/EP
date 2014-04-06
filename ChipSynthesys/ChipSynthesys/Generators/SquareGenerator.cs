using ChipSynthesys.Common.Randoms;
using PlaceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, int width, int height, out Design design, out PlacementGlobal placement)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, int width, int height, out Design design, out PlacementGlobal placement)
        {      
           
            List<int> Parts = new List<int>();
            List<int> Coordinate = new List<int>();
            List<int> X_Coord = new List<int>();
            List<int> Y_Coord = new List<int>();
            var rnd = new Random();
            var SizeX=0;
            var SizeY=0;
            Square1(maxSizeX, maxSizeY, components, out SizeX, out SizeY);
            Division_Square(components,percent, SizeX, SizeY,maxSizeX,maxSizeY,Parts,Coordinate);  
            var c = new Component.Pool();
            var sum = 0;
            var num=0;                       
            for (int i = 0; i < Parts.Count - 1; i += 2)
                {
                    c.Add(Parts[i] , Parts[i + 1]);
                    X_Coord.Add(Coordinate[i]);
                    Y_Coord.Add(Coordinate[i + 1]);
                    sum += Parts[i] * Parts[i + 1];                  
                    num = num + 1;
                    if (components == num)
                        break;
                }
            if (sum != SizeX * SizeY)
            {
                Parts.Clear();
                Coordinate.Clear();
                X_Coord.Clear();
                Y_Coord.Clear();
                NextDesignWithPlacement(components, nets, maxNetSize, percent, maxSizeX, maxSizeY, width, height, out design, out placement);
            }
            sum = ((100 - percent) * sum) / percent;
            Add_Square(SizeX, SizeY, sum, out SizeX,out SizeY);            
            var n = new Net.Pool();
            design = new Design(new Field(0, 0, SizeX, SizeY), c, n);
            placement = new PlacementGlobal(design);
            int l=0;
            foreach( Component com in design.components)
            {
                placement.x[com] = X_Coord[l];
                placement.y[com] = Y_Coord[l];
                placement.placed[com] = true;
                l++;
            }
            var q=0; 
            List<int> Net = new List<int>();
            List<int> n_net = new List<int>();            
            for (int k = 0; k < nets; k++)
            {               
                Net.Add(q);                
                double x_q = placement.x[c[q]];
                double y_q = placement.y[c[q]];
                double w_q = c[q].sizex;
                double h_q = c[q].sizey;
                int i = 1;                
                for (int j = 0; j < components; j++)
                {
                    double x_j = placement.x[c[j]];
                    double y_j = placement.y[c[j]];
                    int w_j = c[j].sizex;
                    int h_j = c[j].sizey;

                    if ((x_q == x_j + w_j && y_q == y_j) || (x_q == x_j && y_q == y_j + h_j) || (x_q + w_q == x_j && y_q == y_j) || (x_q == x_j && y_q + h_q == y_j))
                    {
                        Net.Add(j);
                        i++;                        
                    }
                    if ((x_j + w_j == x_q && y_j < y_q && y_q < y_j + h_j) || (x_j + w_j == x_q && y_q < y_j && y_j < y_q + h_q) || (x_q + w_q == x_j && y_j < y_q && y_q < y_j + h_j) || (x_q + w_q == x_j && y_q < y_j && y_j < y_q + h_q))
                    {
                        Net.Add(j);
                        i++;
                    }
                    if ((y_j + h_j == y_q && x_j < x_q && x_q < w_j + x_j) || (y_j + h_j == y_q && x_q < x_j && x_j < x_q + w_q) || (y_q + h_q == y_j && x_j < x_q && x_q < w_j + x_j) || (y_q + h_q == y_j && x_q < x_j && x_j < x_q + w_q))
                    {
                        Net.Add(j);
                        i++;
                    }
                    if (i == maxNetSize || j==components-1)
                    { n_net.Add(i); break; }
                } 
                q++;              
            }
            var count=0;
            for (int i = 0; i < nets; i++)
            {
                
                n.Add(new Component[n_net[i]]);
                for (int j = 0; j <n[i].items.Length; j++)
                {
                    int component = Net[count];
                      n[i].items[j] = c[component];
                      count++;                
                }
            }
            design = new Design(new Field(0, 0, SizeX, SizeY), c, n); 
        
        }


        private void Division_Square(int components, int percent, int SizeX, int SizeY, int maxSizeX, int maxSizeY, List<int> Parts, List<int> Coordinate)
        {
            Division(SizeX, SizeY, maxSizeX, maxSizeY, 0, Parts, Coordinate);
            do
            {
                int i = Contorol(Parts, maxSizeX, maxSizeY, components);
                if (i == -1)
                { break; }
                Division(Parts[i], Parts[i + 1], maxSizeX, maxSizeY, i, Parts, Coordinate);
            } while (true);

        }
        
        private void Division(int x, int y, int maxX, int maxY, int ind, List<int> Parts, List<int> Coordinate)
        {
            int x1, x2, y1, y2;
            int c_x = 0;
            int c_y = 0;
            int n = 0;
            int k;
            Random rnd = new Random();

            if ((x > y) && x > 1 || (x == y && maxX <= maxY) || y == 1 || (x == y && maxX < x && maxY >= y))
            {
                if (x == maxX || x - maxX == 1)
                {
                    n = rnd.Next(1, maxX);                    
                }
                if (x <= maxX)
                { n = rnd.Next(1, x); }
                if (x - maxX > 1)
                { n = rnd.Next(2, x - 1); }

                x1 = n;
                x2 = x - n;
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
                    if (Coordinate.Count - 1 == ind + 1)
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

            if (y > x || (x == y && maxY < maxX) || x == 1 || (x == y && maxY < y && maxX >= x))
            {
                if (y == maxY || y - maxY == 1)
                { n = rnd.Next(1, maxY); }
                if (y <= maxY)
                { n = rnd.Next(1, y); }
                if (y - maxY > 1)
                { n = rnd.Next(2, y - 1); }

                y1 = n;
                y2 = y - n;
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
                    if (Coordinate.Count - 1 == ind + 1)
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
                    Parts.Add(x);
                    Coordinate.Add(c_x);
                    Parts.Add(y1);
                    Coordinate.Add(c_y);
                    Parts.Add(x);
                    Coordinate.Add(c_x);
                    Parts.Add(y2);
                    Coordinate.Add(n);
                }
            }


        }

        private int Contorol(List<int> Parts, int maxX, int maxY, int comp)
        {
            int max;
            int m;
            List<int> M = new List<int>();
            for (int i = 0; i < Parts.Count - 1; i += 2)
            {
                if (Parts[i] > maxX || Parts[i + 1] > maxY)
                {
                    M.Clear();
                    return i;
                }
                if (Parts[i] <= maxX && Parts[i + 1] <= maxY && comp != 0)
                {
                    max = Parts[i] * Parts[i + 1];
                    M.Add(max);
                    comp--;
                }
                if (i == Parts.Count - 2 && comp != 0)
                {
                    m = M.Max();
                    m = M.IndexOf(m) * 2;
                    return m;
                }
                if (comp == 0)
                { return -1; }
            }
            return -1;
        }       

        private void Add_Square(int w, int h, int sum, out int SizeX, out int SizeY)
        {
            do
            {
                if (sum >= w + h)
                {
                    sum = sum - w;
                    h++;
                    sum = sum - h;
                    w++;
                }
                else
                {
                    if (sum <= w || sum <= h)
                    {
                        if (sum <= w && w / 2 + 1 < sum)
                        { sum = 0; h++; }
                        if (sum <= h && h / 2 + 1 < sum)
                        { sum = 0; w++; }
                        else { sum = 0; }
                    }
                    else
                    {
                        if (w >= h)
                        {
                            sum = sum - w;
                            h++;
                            if (sum == w)
                            { sum = 0; h++; }
                            if (sum <= h && h / 2 + 1 < sum)
                            { sum = 0; w++; }
                            else { sum = 0; }
                        }
                        if (h > w && sum != 0)
                        {
                            sum = sum - h;
                            w++;
                            if (sum == h)
                            { sum = 0; w++; }
                            if (sum <= w && w / 2 + 1 < sum)
                            { sum = 0; h++; }
                            else { sum = 0; }
                        }
                    }
                }
            } while (sum != 0);
            SizeX = w;
            SizeY = h;
        }

        private void Square1(int X, int Y, int comp, out  int sizeX, out int sizeY)
        {
            int s = 0;
            int i = 0;
            int j = 0;
            int n;
            Random rnd = new Random();
            do
            {
                i = rnd.Next(1, X + 1);
                j = rnd.Next(1, Y + 1);
                s = i * j + s;
                comp--;
                if (comp == 0)
                { break; }
            } while (true);
            if (s % 2 != 0)
            { s = s + 1; }

            sizeX = Convert.ToInt16(Math.Sqrt(s));
            sizeY = s / sizeX;
        }          

    }
}
