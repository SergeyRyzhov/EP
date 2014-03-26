using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public class PositionComparer : IPositionComparer
    {
        public bool Better(Design design, PlacementDetail placement, Component current, int firstX, int firstY, int secondX, int secondY)
        {
            if (firstX < secondX)
                if(firstY < secondY)
                    return true;
            return false;
        }
    }
}