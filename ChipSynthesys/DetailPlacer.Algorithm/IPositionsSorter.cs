using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface IPositionsSorter
    {
        void SortPositions(Design design, PlacementGlobal approximate, PlacementDetail result, int[] x, int[] y, ref int[] perm);
    }
}