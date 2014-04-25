using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using PlaceModel;

namespace DetailPlacer.Algorithm.CriterionPositionSearcher
{
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override string ToString()
        {
            return string.Format("({0};{1})", X, Y);
        }
    }
}
