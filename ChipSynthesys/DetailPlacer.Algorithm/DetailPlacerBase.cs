using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public abstract class DetailPlacerBase : PlacerBase, IDetailPlacer
    {
        public abstract void Place(Design design, PlacementGlobal approximate, PlacementDetail result);
    }
}
