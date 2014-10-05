using System;
using System.Security.Cryptography;

using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher
{
    public interface IPositionSearcher
    {
        /// <summary>
        /// Поиск доступных позиция для размещения компонента
        /// </summary>
        void AlvailablePositions(PositionHelper helper, Design design, PlacementGlobal approximate, PlacementDetail result, Component current, out int[] x, out int[] y, out bool hasPosition);
    }

    public class PositionHelper
    {
        private readonly Design m_design;

        private readonly PlacementDetail m_result;

        private readonly int m_n;

        private readonly int m_m;

        private int[,] m_map;

        public PositionHelper(Design design, PlacementDetail result)
        {
            this.m_design = design;
            this.m_result = result;
            this.m_n = design.field.cellsx;
            this.m_m = design.field.cellsy;
        }

        public void Build()
        {
            this.m_map = GenerateMask(m_design, this.m_result, this.m_n, this.m_m);
        }

        public int[,] GetMask()
        {
            if (m_map == null)
            {
                throw new InvalidOperationException("Map was not built");
            }

            return m_map;
        }

        public bool MoveComponent(Component c, int x, int y)
        {
            if (m_map == null)
            {
                throw new InvalidOperationException("Map was not built");
            }

            int ph = c.sizey;
            int pw = c.sizex;

            for (int k = 0; k < ph; k++)
            {
                for (int l = 0; l < pw; l++)
                {
                    m_map[x + l, y + k]++;
                }
            }

            return true;
        }

        protected virtual int[,] GenerateMask(Design design, PlacementDetail result, int n, int m)
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
                            mask[cx + l, cy + k]++;
                        }
                    }
                }
            }
            return mask;
        }
    }
}