using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter
{
    public interface IPositionsSorter
    {
        /// <summary>
        /// Упорядочивание позиций размещения
        /// </summary>
        /// <param name="design"></param>
        /// <param name="approximate"></param>
        /// <param name="result"></param>
        /// <param name="current"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="perm"></param>
        void SortPositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, int[] x, int[] y, ref int[] perm);
    }
}