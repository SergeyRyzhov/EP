using PlaceModel;
using System;
using System.Collections.Generic;

namespace ChipSynthesys
{
    internal class RandomGeneratorWithPlacement : IGenerator
    {
        public void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY,
            out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesign(int components, IRandom<int> nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design)
        {
            throw new NotImplementedException();
        }

        public void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX,
            int maxSizeY,
            out Design design, out PlacementDetail placement)
        {
            const double balance = 1.1;
            double volume = components * maxSizeX * maxSizeY * (100 / (100 - percent)) * balance;
            double side = Math.Ceiling(Math.Sqrt(volume));

            int sizeX = Convert.ToInt32(side);
            int sizeY = Convert.ToInt32(side);

            NextDesignWithPlacement(components, nets, maxNetSize, percent, maxSizeX, maxSizeY, sizeX, sizeY, out design,
                out placement);
        }

        public void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design, out PlacementDetail placement)
        {
            throw new NotImplementedException();
        }

        protected virtual void Split(List<ValuePair<int>> xCoord, List<ValuePair<int>> yCoord, int indx,
            ref int compCount)
        {
            if ((xCoord[indx].B - xCoord[indx].A == 1) && (yCoord[indx].B - yCoord[indx].A == 1)) return;
            var r = new Random();
            List<ValuePair<int>> l1, l2;

            if (xCoord[indx].B - xCoord[indx].A >= yCoord[indx].B - yCoord[indx].A)
            {
                l1 = xCoord;
                l2 = yCoord;
            }
            else
            {
                l1 = yCoord;
                l2 = xCoord;
            }

            var values = new ValuePair<int> { A = l1[indx].A, B = l1[indx].B };

            int newCoord = r.Next(values.A + 1, values.B);
            l1[indx].A = values.A;
            l1[indx].B = newCoord;
            if (indx + 1 != compCount)
            {
                l1.Insert(indx + 1, new ValuePair<int> { A = newCoord, B = values.B });
                l2.Insert(indx + 1, new ValuePair<int> { A = l2[indx].A, B = l2[indx].B });
            }
            else
            {
                l1.Add(new ValuePair<int> { A = newCoord, B = values.B });
                l2.Add(new ValuePair<int> { A = l2[indx].A, B = l2[indx].B });
            }
            compCount++;
        }

        protected virtual void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent,
            int maxSizeX, int maxSizeY, int sizeX, int sizeY, out Design design, out PlacementDetail placement)
        {
            var xCoord = new List<ValuePair<int>>();
            var yCoord = new List<ValuePair<int>>();
            int compCount = 1;
            xCoord.Add(new ValuePair<int> { A = 0, B = sizeX });
            yCoord.Add(new ValuePair<int> { A = 0, B = sizeY });
            var r = new Random();

            int fullnesArea = sizeX * sizeY * percent / 100;
            int count = sizeX * sizeY * percent / 100;//components * percent / 100;
            // учесть можно ли вообще разбить область на такое кол-во элементов

            do
            {
                int idx = r.Next(compCount);
                Split(xCoord, yCoord, idx, ref compCount);
            } while (compCount != count); //?? когда останавливать деление? учесть maxSizex maxSizey

            // удалить часть элементов
            while (compCount != components)
            {
                int k = r.Next(compCount);
                xCoord.RemoveAt(k);
                yCoord.RemoveAt(k);
                compCount--;
            }
            var c = new Component.Pool();
            for (int i = 0; i < compCount; i++)
            {
                c.Add(xCoord[i].B - xCoord[i].A, yCoord[i].B - yCoord[i].A);
            }
            var n = new Net.Pool();
            for (int j = 0; j < nets; j++)
            {
                n.Add(new Component[r.Next(maxNetSize - 1) + 2]);
                for (int i = 0; i < n[j].items.Length; i++)
                {
                    int q = r.Next(compCount);
                    n[j].items[i] = c[q];
                }
            }
            design = new Design(new Field(0, 0, sizeX, sizeY), c, n);
            placement = new PlacementDetail(design);

            int ind = 0;
            foreach (Component com in design.components)
            {
                placement.x[com] = xCoord[ind].A;
                placement.y[com] = yCoord[ind].A;
                ind++;
            }
        }
    }
}