using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher
{
    public interface IPositionSearcher
    {
        /// <summary>
        /// Поиск доступных позиция для размещения компонента
        /// </summary>
        /// <param name="design"></param>
        /// <param name="approximate"></param>
        /// <param name="result"></param>
        /// <param name="current"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="hasPosition"></param>
        void AlvailablePositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, out int[] x, out int[] y, out bool hasPosition);
    }
}