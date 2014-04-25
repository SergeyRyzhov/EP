using System.Collections.Generic;

namespace DetailPlacer.Algorithm.CriterionPositionSearcher
{
    public interface ICriterionPositionSearcher
    {
        IEnumerable<Point> Search(IPointComparer comparer);
    }
}