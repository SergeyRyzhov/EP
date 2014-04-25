using System.Collections.Generic;

namespace DetailPlacer.Algorithm.CriterionPositionSearcher
{
    public interface IPointComparer : IComparer<Point>
    {
        
    }

    public class PointComparer : IPointComparer
    {

        //для одной и той же координаты принципиально вернуть 0 -1 если первое хуже второго 1 если лучше
        //в конструкторе этого класса предвайте все прейсменты и прочее для своих критериев создавать другие реализации интерфейса
        public int Compare(Point x, Point y)
        {
            if (x.X == y.X && x.Y == y.Y)
            {
                return 0;
            }
            return x.X > y.X ? 1 : x.Y > y.Y ? 1 : -1;
        }
    }
}