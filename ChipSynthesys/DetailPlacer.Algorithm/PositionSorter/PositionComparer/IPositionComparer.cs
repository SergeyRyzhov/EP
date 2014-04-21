using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter.PositionComparer
{
    public interface IPositionComparer
    {
        /// <summary>
        /// Сравнение двух позиций компонентов схемы в текущем размещении
        /// </summary>
        bool Better(Design design, PlacementGlobal approximate, PlacementDetail placement, Component current, int firstX, int firstY, int secondX, int secondY);
    }
}