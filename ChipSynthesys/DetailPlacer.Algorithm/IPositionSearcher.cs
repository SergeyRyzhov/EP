using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface IPositionSearcher
    {
        void AlvailablePositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, out int[] x, out int[] y, out bool hasPosition);
    }
}