using ChipSynthesys.Common.Classes;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl
{
    public class NetsPositionComparer : IPositionComparer
    {
        public override string ToString()
        {
            return "Net comparer";
        }

        public bool Better(Design design, PlacementGlobal approximate, PlacementDetail placement, Component current, int firstX, int firstY, int secondX,
            int secondY)
        {
            int firstMark = CriterionHelper.MarkPosition(design, placement, current, firstX, firstY);
            int secondMark = CriterionHelper.MarkPosition(design, placement, current, secondX, secondY);

            return firstMark < secondMark;
        }
    }
}