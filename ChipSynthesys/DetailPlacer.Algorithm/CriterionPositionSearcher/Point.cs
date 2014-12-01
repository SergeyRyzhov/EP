namespace DetailPlacer.Algorithm.CriterionPositionSearcher
{
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y)
            : this((int) x, (int) y)
        {
            
        }

        public int X;
        public int Y;

        public override string ToString()
        {
            return string.Format(@"({0};{1})", X, Y);
        }
    }
}