using System;
using System.Collections.Generic;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class PositionSearcher : IPositionSearcher
    {
        private readonly int m_maxCount;

        public PositionSearcher() : this(16)
        {
        }

        public PositionSearcher(int maxCount)
        {
            m_maxCount = maxCount;
        }

        public void AlvailablePositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, out int[] x, out int[] y, out bool hasPosition)
        {
            int h = current.sizey;
            int w = current.sizex;

            var lx = new List<int>();
            var ly = new List<int>();

            int n = design.field.cellsx;
            int m = design.field.cellsy;

            var mask = new int[n, m]; //mask of placed elemnts
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

            //перебор позиций
            for (int i = 0; i < n; i++)
            {
                if (lx.Count == m_maxCount)
                {
                    break;
                }

                for (int j = 0; j < m; j++)
                {
                    if (lx.Count == m_maxCount)
                    {
                        break;
                    }

                    if (mask[i, j] == 0)
                    {
                        bool may = true;
                        //проверка текущего положения
                        for (int k = 0; k < h; k++)
                        {
                            if (!may)
                            {
                                break;
                            }
                            for (int l = 0; l < w; l++)
                            {
                                try
                                {
                                    if (mask[i + l, j + k] == 1)
                                    {
                                        may = false;
                                        break;
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    may = false;
                                    break;
                                }
                            }
                        }

                        if (may)
                        {
                            lx.Add(i);
                            ly.Add(j);

                            if (lx.Count == m_maxCount)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            x = lx.ToArray();
            y = ly.ToArray();
            hasPosition = x.Length > 0;
        }
    }
}