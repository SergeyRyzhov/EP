using System;
using System.Collections.Generic;

using ChipSynthesys.Common.Randoms;

using PlaceModel;

namespace ChipSynthesys.Common.Generators
{
    public class SquareGenerator : IGenerator
    {

        public void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesign(int components, IRandom<int> nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design, out PlacementDetail placement)
        {
            int sizeX = 5;
            int sizeY = 3;
            var rnd = new Random();
            List<int> parts = new List<int>();
            this.All_Division(sizeX, sizeY, components, maxSizeX, maxSizeY, percent, parts);

            var c = new Component.Pool();
            int percent_square = sizeX * sizeY * percent / 100;
            for (int i = 0; i < parts.Count - 1; i += 2)
            {
                int k = parts[i] * parts[i + 1];
                if (k < percent_square && components != 0)
                {
                    c.Add(parts[i], parts[i + 1]);
                    percent_square = percent_square - k;
                    components = components - 1;
                    k = 0;
                }
                else
                {
                    k = 0;
                }

            }
            var n = new Net.Pool();
            for (var j = 0; j < nets; j++)
            {
                n.Add(new Component[rnd.Next(maxNetSize - 1) + 2]);
                for (var i = 0; i < n[j].items.Length; i++)
                {
                    var q = rnd.Next(components);
                    n[j].items[i] = c[q];
                }
            }
            design = new Design(new Field(0, 0, sizeX, sizeY), c, n);
            placement = new PlacementDetail(design);
        }

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent, IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }
        public void All_Division(int sizeX, int sizeY, int components, int MaxSizeX, int MaxSizeY, int percent, List<int> parts)
        {

            int square = sizeX * sizeY;
            int percent_square = sizeX * sizeY * percent / 100;
            if (components <= percent_square)
            {
                // List<int> parts = new List<int>();
                this.Division(sizeX, sizeY, parts, 0);
                do
                {
                    int i = this.Control(percent_square, parts, MaxSizeX, MaxSizeY);
                    if (i < 0) break;
                    this.Division(parts[i], parts[i + 1], parts, i);

                } while (true);
            }
            else
            {
                //return
            }
        }
        public void Division(int x, int y, List<int> parts, int ind)
        {
            int x1, x2, y1, y2;

            Random rnd = new Random();

            if (x > y)
            {
                int n = 1 + rnd.Next(x - 1);
                x1 = n;
                x2 = x - n;
                if (parts.Count != 0)
                {

                    parts.RemoveAt(ind + 1);
                    parts.RemoveAt(ind);
                    parts.Insert(ind, x1);
                    parts.Insert(ind + 1, y);
                    parts.Insert(ind + 2, x2);
                    parts.Insert(ind + 3, y);
                }
                else
                {
                    parts.Add(x1);
                    parts.Add(y);
                    parts.Add(x2);
                    parts.Add(y);
                }
            }
            if (x == y)
            {
                int n = 1 + rnd.Next(x - 1);
                x1 = n;
                x2 = x - n;
                if (parts.Count != 0)
                {
                    parts.RemoveAt(ind + 1);
                    parts.RemoveAt(ind);
                    parts.Insert(ind, x1);
                    parts.Insert(ind + 1, y);
                    parts.Insert(ind + 2, x2);
                    parts.Insert(ind + 3, y);
                }
                else
                {
                    parts.Add(x1);
                    parts.Add(y);
                    parts.Add(x2);
                    parts.Add(y);
                }
            }
            if (x < y)
            {
                int n = 1 + rnd.Next(y - 1);
                y1 = n;
                y2 = y - n;
                if (parts.Count != 0)
                {
                    parts.RemoveAt(ind + 1);
                    parts.RemoveAt(ind);
                    parts.Insert(ind, x);
                    parts.Insert(ind + 1, y1);
                    parts.Insert(ind + 2, x);
                    parts.Insert(ind + 3, y2);
                }
                else
                {
                    parts.Add(x);
                    parts.Add(y1);
                    parts.Add(x);
                    parts.Add(y2);
                }
            }

        }
        public int Control(int perc_sq, List<int> parts, int maxX, int maxY)
        {
            for (int i = 0; i < parts.Count - 1; i += 2)
            {
                int k = parts[i] * parts[i + 1];
                if (k >= perc_sq - 1 || parts[i] > maxX || parts[i + 1] > maxY)
                {
                    return i;
                }
            }
            return -1;
        }



    }
}
