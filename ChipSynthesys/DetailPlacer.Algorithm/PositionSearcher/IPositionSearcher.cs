using System;
using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSearcher
{
    public interface IPositionSearcher
    {
        int PositionAmount { get; set; }

        /// <summary>
        /// Поиск доступных позиция для размещения компонента
        /// </summary>
        bool AlvailablePositions(Mask mask, Component current,int startX, int startY, int[] resX, int[] resY);
    }

    public class Mask
    {
        private readonly Design m_design;
        private readonly PlacementDetail m_currentSolution;
        private int[][] m_hMask;
        private int[][] m_vMask;

        public Mask(Design design, PlacementDetail currentSolution)
        {
            m_design = design;
            m_currentSolution = currentSolution;
        }

        public void PrintMask()
        {
            Console.WriteLine("Horizontal mask");
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Console.Write("{0} ", m_hMask[row][col]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("Vertical mask"); 
            
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Console.Write("{0} ", m_vMask[col][row]);
                }
                Console.WriteLine();
            }
        }

        public int Height { get; private set; }

        public int Width { get; private set; }

        public int BuildUp()
        {
            //Console.WriteLine("Build up mask");
            var height = Height = m_design.field.cellsy;
            var width = Width = m_design.field.cellsx;

            m_hMask = new int[height][];
            for (int i = 0; i < height; i++)
            {
                m_hMask[i] = new int[width];
            }

            m_vMask = new int[width][];
            for (int i = 0; i < width; i++)
            {
                m_vMask[i] = new int[height];
            }

            for (int i = 0; i < m_design.components.Length; i++)
            {
                var c = m_design.components[i];
                if (m_currentSolution.placed[c])
                {
                    var x = m_currentSolution.x[c];
                    var y = m_currentSolution.y[c];
                    PlaceComponent(c, y, x);
                }
            }
            //PrintMask();
            return 0;
        }

        public void PlaceComponent(Component c, int x, int y)
        {
            x -= m_design.field.beginx;
            y -= m_design.field.beginy;

            var h = c.sizey;
            var w = c.sizex;

            for (int row = y; row < y + h; row++)
            {
                for (int dw = x; dw < x + w; dw++)
                {
                    m_hMask[row][dw]++;
                }
            }

            for (int col = x; col < x + w; col++)
            {
                for (int dh = y; dh < y + h; dh++)
                {
                    m_vMask[col][dh]++;
                }
            }
            //PrintMask();
        }

        public bool CanPlaceH(Component c, int x, int y)
        {
            x -= m_design.field.beginx;
            y -= m_design.field.beginy;

            var h = c.sizey;
            var w = c.sizex;

            if (x < 0 || Width < x + w)
            {
                return false;
            }

            if (y < 0 || Height < y + h)
            {
                return false;
            }

            for (int row = y; row < y + h; row++)
            {
                for (int dw = x; dw < x + w; dw++)
                {
                    if(m_hMask[row][dw] > 0)
                        return false;
                }
            }

            return true;
        }

        public bool CanPlaceV(Component c, int x, int y)
        {
            x -= m_design.field.beginx;
            y -= m_design.field.beginy;

            var h = c.sizey;
            var w = c.sizex;

            if (x < 0 || Width < x + w)
            {
                return false;
            }

            if (y < 0 || Height < y + h)
            {
                return false;
            }

            for (int col = x; col < x + w; col++)
            {
                for (int dh = y; dh < y + h; dh++)
                {
                    if(m_vMask[col][dh] > 0)
                        return false;
                }
            }
            return true;
        }

    }
}