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
            return "����� ��������� �������";
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
                var av = IsAvailable(mask, w, h, a, b);
                if (!av) return true;

                lx.Add(a);
                ly.Add(b);

                return lx.Count != m_maxCount;
            });

            var success = DetourPositions(design, approximate, result, current, n, m, mask, addIfTheLimitIsNotExceeded);
            if (!success)
            {
                //������ ������� �� �� ����������, ���� �� ������� ������
            }
            x = lx.ToArray();
            y = ly.ToArray();
            hasPosition = x.Length > 0;
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        /// <param name="current"></param>
        /// <param name="n">������ �����</param>
        /// <param name="m">������ �����</param>
        /// <param name="mask">�����</param>
        /// <param name="addIfTheLimitIsNotExceeded">��������� �������, �������� true ���� ������ ������� �� ��������� false � ������ ���� ������ ���������</param>
        /// <param name="design"></param>
        /// <param name="approximate"></param>
        /// <param name="result"></param>
        /// <returns>true ���� ������� ������ ������� �������, false �����</returns>
        protected abstract bool DetourPositions(Design design, PlacementGlobal approximate, PlacementDetail result,
            Component current, int n, int m, int[,] mask, Func<int, int, bool> addIfTheLimitIsNotExceeded);

       

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