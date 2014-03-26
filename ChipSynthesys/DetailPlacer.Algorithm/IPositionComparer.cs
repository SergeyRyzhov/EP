using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface IPositionComparer
    {
        bool Better(Design design, PlacementDetail placement, int firstX, int firstY, int secondX, int secondY);
    }
}