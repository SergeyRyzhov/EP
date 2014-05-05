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

    public class PointF
    {
        public PointF(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;
        
        public override string ToString()
        {
            return string.Format("({0};{1})", X, Y);
        }
    }
}