using ChipSynthesys.Common;
using PlaceModel;
using System;
using System.Collections.Generic;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
   /* public abstract class PositionSearcherBase : IPositionSearcher
    {
        public override string ToString()
        {
            return "Поиск доступных позиций";
        }

        private readonly int m_maxCount;

        protected PositionSearcherBase()
            : this(TestsConstants.SearchSize)
        {
        }

        protected PositionSearcherBase(int maxCount)
        {
            m_maxCount = maxCount;
        }

        public void AlvailablePositions(PositionHelper helper, Design design, PlacementGlobal approximate, PlacementDetail result, Component current,
            out int[] x, out int[] y, out bool hasPosition)
        {
            int h = current.sizey;
            int w = current.sizex;

            var lx = new List<int>();
            var ly = new List<int>();

            int n = design.field.cellsx;
            int m = design.field.cellsy;

            var mask = helper.GetMask();
            var addIfTheLimitIsNotExceeded = new Func<int, int, bool>((a, b) =>
            {
                var av = IsAvailable(mask, current, a, b);
                if (!av) return true;

                lx.Add(a);
                ly.Add(b);

                return lx.Count != m_maxCount;
            });

            var success = DetourPositions(design, approximate, result, current, n, m, mask, addIfTheLimitIsNotExceeded);
            if (!success)
            {
                //ячейки найдены но не достаточно, либо не найдены совсем
            }
            x = lx.ToArray();
            y = ly.ToArray();
            hasPosition = x.Length > 0;
        }

        /// <summary>
        /// Обход позиций
        /// </summary>
        /// <param name="current"></param>
        /// <param name="n">Ширина сетки</param>
        /// <param name="m">Высота сетки</param>
        /// <param name="mask">Сетка</param>
        /// <param name="addIfTheLimitIsNotExceeded">Добавляет позицию, возвращя true если предел позиций не перевышен false в случае если предел достигнут</param>
        /// <param name="design"></param>
        /// <param name="approximate"></param>
        /// <param name="result"></param>
        /// <returns>true если искомое пчисло позиций найдено, false иначе</returns>
        protected abstract bool DetourPositions(Design design, PlacementGlobal approximate, PlacementDetail result,
            Component current, int n, int m, Mask mask, Func<int, int, bool> addIfTheLimitIsNotExceeded);

        protected virtual bool IsAvailable(Mask mask, Component c, int x, int y)
        {
            return mask.CanPlaceH(c, x, y);
        }
    }*/
}