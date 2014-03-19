using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public abstract class GlobalPlacerBase : PlacerBase, IGlobalPlacer
    {
        public abstract void Place(Design design, PlacementGlobal result);
    }
}