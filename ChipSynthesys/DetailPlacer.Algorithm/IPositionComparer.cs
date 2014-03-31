using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface IPositionComparer
    {
        /// <summary>
        /// Сравнение двух позиций компонентов схемы в текущем размещении
        /// </summary>
        /// <param name="design"></param>
        /// <param name="placement"></param>
        /// <param name="current"></param>
        /// <param name="firstX"></param>
        /// <param name="firstY"></param>
        /// <param name="secondX"></param>
        /// <param name="secondY"></param>
        /// <returns></returns>
        bool Better(Design design, PlacementDetail placement, Component current, int firstX, int firstY, int secondX, int secondY);
    }

    public class NetsPositionComparer : IPositionComparer
    {
        public bool Better(Design design, PlacementDetail placement, Component current, int firstX, int firstY, int secondX,
            int secondY)
        {
            return false;
        }

        private double MarkPosition(Design design, PlacementDetail placement, Component current, int x, int y)
        {
            double summ = 0.0;

            return summ;
        }
    }
}