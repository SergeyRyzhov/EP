using DetailPlacer.Algorithm.CriterionPositionSearcher;
using PlaceModel;
using System;
using System.Collections.Generic;

namespace DetailPlacer.Algorithm.PositionSearcher.Impl
{
    public class WidePositionSearcher// : IPositionSearcher
    {
        private readonly int m_size;

        public override string ToString()
        {
            return "Позиции по спирали. C выходом за границы.";
        }

        public WidePositionSearcher()
            :this(3)
        {
        }

        public WidePositionSearcher(int size)
        {
            m_size = size;
        }

        public void AlvailablePositions(Design design, PlacementGlobal approximate, PlacementDetail result, Component current,
            out int[] x, out int[] y, out bool hasPosition)
        {
            var lx = new List<int>();
            var ly = new List<int>();
            int nx, ny;
            var mask = GenerateMask(design, approximate, result, current, out nx, out ny);

            var cx = (int)Math.Ceiling(approximate.x[current]);
            var cy = (int)Math.Ceiling(approximate.x[current]);

            var positions = SpiralGenerator.UnwindingSpiral(mask.Height, mask.Width, cx, cy);

            bool every = true;
            Point t = null;
            foreach (Point p in positions)
            {
                var placeType = mask.CanBePlaced(current, p.X, p.Y, every);
                if (placeType == PlaceType.Allow)
                {
                    lx.Add(p.X - nx);
                    ly.Add(p.Y - ny);

                    if (lx.Count == m_size)
                    {
                        break;
                    }
                    continue;
                }
                if (placeType == PlaceType.Outside)
                {
                    t = new Point(p.X - nx, p.Y-ny);
                    every = false;
                }
            }
            if (lx.Count == 0 && t != null)
            {
                lx.Add(t.X);
                ly.Add(t.Y);
            }

            x = lx.ToArray();
            y = ly.ToArray();
            hasPosition = lx.Count > 0;
        }

        protected virtual Mask GenerateMask(Design design, PlacementGlobal approximate, PlacementDetail result, Component current, out int nx, out int ny)
        {
            int minx = 0;
            int miny = 0;
            int maxx = design.field.cellsx;
            int maxy = design.field.cellsy;

            int maxw = 0;
            int maxh = 0;

            foreach (Component c in design.components)
            {
                if (result.placed[c])
                {
                    minx = Math.Min(minx, result.x[c]);
                    miny = Math.Min(miny, result.y[c]);
                    maxx = Math.Max(maxx, result.x[c] + c.sizex);
                    maxy = Math.Max(maxy, result.y[c] + c.sizey);
                }
                maxw = Math.Max(maxw, c.sizex);
                maxh = Math.Max(maxh, c.sizey);
            }

            nx = - minx + maxw;
            ny =  - miny + maxh;


            int height = maxy - miny;
            int width = maxx - minx;

            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;

            if (minx < 0)
                left = -minx;
            if (miny < 0)
                top = -minx;
            if (maxx > design.field.cellsx)
                right = maxx - design.field.cellsx;
            if (maxy > design.field.cellsy)
                bottom = maxy - design.field.cellsy;

            var m = new Mask(width, height, left, right, top, bottom, maxw, maxh);

            foreach (Component component in design.components)
            {
                if (result.placed[component])
                {
                    var cx = result.x[component] - minx + maxw;
                    var cy = result.y[component] - miny + maxh;

                    int ph = component.sizey;
                    int pw = component.sizex;

                    for (int k = 0; k < ph; k++)
                    {
                        for (int l = 0; l < pw; l++)
                        {
                            m[cx + l, cy + k] = CellType.Placed;
                        }
                    }
                }
            }

            for (int i = 0; i < m.Width; i++)
            {
                for (int j = 0; j < m.Height; j++)
                {
                    Console.Write("{0} ", (int)m[i,j]);
                }
                Console.WriteLine();
            }
            return m;
        }

        protected enum CellType
        {
            Empty = 0,
            Placed = 1,
            Outside = 2
        }

        protected enum PlaceType
        {
            Deny = 0,
            Allow = 1,
            Outside = 2
        }

        protected class Mask
        {
            public Mask(int width, int height, int left, int right, int top, int bottom, int maxw, int maxh)
            {
                Width = width + 2 * maxh;
                Height = height + 2 * maxw;
                Field = new int[Width, Height];
                CellType fillx;
                for (int i = 0; i < Width; i++)
                {
                    CellType fill;
                    if (i > left + maxw && i < Width - right - maxw)
                    {
                        fill = CellType.Empty;
                    }
                    else
                    {
                        fill = CellType.Outside;
                    }
                    fillx = fill;
                    for (int j = 0; j < Height; j++)
                    {
                        if (fill == CellType.Empty && j > top + maxh && j < Height - bottom - maxh)
                        {
                            fill = CellType.Empty;
                        }
                        else
                        {
                            fillx = fill;
                            fill = CellType.Outside;
                        }
                        this[i, j] = fill;
                        fill = fillx;
                    }
                }
            }

            public CellType this[int x, int y]
            {
                get { return (CellType)Field[x, y]; }
                set { Field[x, y] = (int)value; }
            }

            public PlaceType CanBePlaced(Component c, int x, int y, bool everywhere)
            {
                var result = PlaceType.Allow;

                for (int dy = 0; dy <= c.sizey; dy++)
                {
                    for (int dx = 0; dx <= c.sizex; dx++)
                    {
                        try
                        {
                            if (this[x + dx, y + dy] == CellType.Placed)
                            {
                                return PlaceType.Deny;
                            }

                            if (this[x + dx, y + dy] == CellType.Outside)
                            {
                                if (!everywhere)
                                    return PlaceType.Deny;
                                //return PlaceType.Deny;
                                result = PlaceType.Outside;
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            return PlaceType.Deny;
                        }
                    }
                }
                return result;
            }

            public int[,] Field { get; private set; }

            public int Width { get; private set; }

            public int Height { get; private set; }
        }
    }
}