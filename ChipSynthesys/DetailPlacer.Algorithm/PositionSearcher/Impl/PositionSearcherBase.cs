using ChipSynthesys.Common;
using PlaceModel;
using System;
using System.Collections.Generic;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public abstract class PositionSearcherBase : IPositionSearcher
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

        public void AlvailablePositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current,
            out int[] x, out int[] y, out bool hasPosition)
        {
            int h = current.sizey;
            int w = current.sizex;

            var lx = new List<int>();
            var ly = new List<int>();

            int n = design.field.cellsx;
            int m = design.field.cellsy;

            var mask = GenerateMask(design, approximate, result, n, m);
            var addIfTheLimitIsNotExceeded = new Func<int, int, bool>((a, b) =>
            {
                var av = IsAvailable(mask, w, h, a, b);
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
            Component current, int n, int m, int[,] mask, Func<int, int, bool> addIfTheLimitIsNotExceeded);

        protected virtual int[,] GenerateMask(Design design, PlacementGlobal approximate, PlacementDetail result, int n, int m)
        {
            var mask = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    mask[i, j] = 0;
                }
            }

            foreach (Component component in design.components)
            {
                if (result.placed[component])
                {
                    var cx = result.x[component];
                    var cy = result.y[component];

                    int ph = component.sizey;
                    int pw = component.sizex;

                    for (int k = 0; k < ph; k++)
                    {
                        for (int l = 0; l < pw; l++)
                        {
                            mask[cx + l, cy + k] = 1;
                        }
                    }
                }
            }
            return mask;
        }

        protected virtual bool IsAvailable(int[,] mask, int width, int heigth, int x, int y)
        {
            for (int k = 0; k < heigth; k++)
            {
                for (int l = 0; l < width; l++)
                {
                    try
                    {
                        if (mask[x + l, y + k] == 1)
                        {
                            return false;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}