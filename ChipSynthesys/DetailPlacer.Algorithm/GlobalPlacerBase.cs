using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public abstract class GlobalPlacerBase : IGlobalPlacer
    {
        public abstract void Place(Design design, PlacementGlobal result);
    }
}