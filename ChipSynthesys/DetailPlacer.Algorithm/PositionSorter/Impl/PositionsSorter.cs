using DetailPlacer.Algorithm.PositionSorter.PositionComparer;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter.Impl
{
    public class PositionsSorter : IPositionsSorter
    {
        public override string ToString()
        {
            return "Сортировка пузырьком всех позиций";
        }

        private readonly IPositionComparer m_positionComparer;

        public PositionsSorter(IPositionComparer positionComparer)
        {
            m_positionComparer = positionComparer;
        }

        public void SortPositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, int[] x, int[] y, ref int[] perm)
        {
            int length = x.Length;
            var mask = new int[length];

            for (int i = 0; i < length; i++)
            {
                int best = i;
                for (int j = i+1; j < length; j++)
                {
                    if (mask[j] == 1)
                        continue;

                    if (m_positionComparer.Better(design, result, current, x[j], y[j], x[best], y[best]))
                        best = j;
                }
                perm[i] = best;
                mask[best] = 1;
            }
        }
    }
}