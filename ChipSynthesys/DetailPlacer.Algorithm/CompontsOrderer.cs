using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public class CompontsOrderer : ICompontsOrderer
    {
        public void SortComponents(Design design, PlacementGlobal approximate, PlacementDetail result, Component[] unplacedComponents, ref int[] perm)
        {
            for (int i = 0; i < unplacedComponents.Length; i++)
            {
                perm[i] = i;
            }
        }
    }
}