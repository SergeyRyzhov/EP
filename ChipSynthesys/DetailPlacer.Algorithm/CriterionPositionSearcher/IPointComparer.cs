using System.Collections.Generic;

namespace DetailPlacer.Algorithm.CriterionPositionSearcher
{
    public interface IPointComparer : IComparer<Point>
    {
        
    }

    public class PointComparer : IPointComparer
    {

        //��� ����� � ��� �� ���������� ������������� ������� 0 -1 ���� ������ ���� ������� 1 ���� �����
        //� ������������ ����� ������ ��������� ��� ���������� � ������ ��� ����� ��������� ��������� ������ ���������� ����������
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