using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface ICompontsOrderer
    {
        void SortComponents(Design design, PlacementGlobal approximate, PlacementDetail result, Component[] unplacedComponents, ref int[] perm);
    }
}