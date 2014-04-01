using ChipSynthesys.Common.Randoms;

using PlaceModel;

namespace ChipSynthesys.Common.Generators
{
    /// <summary>
    /// ��������� ���������� ����� � ����������
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// ��������� ��������� (����������� ����� �������������)
        /// </summary>
        /// <param name="components">����� ���������</param>
        /// <param name="nets">����� �����</param>
        /// <param name="maxNetSize">������������ ����� ��������� � ����</param>
        /// <param name="percent">������� ������������� (0,100)</param>
        /// <param name="maxSizeX">������������ ������ ��������</param>
        /// <param name="maxSizeY">������������ ������ ��������</param>
        /// <param name="design">���������� �����</param>
        void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design);

        /// <summary>
        /// ��������� ��������� (�������� ����� �������������)
        /// </summary>
        /// <param name="components">����� ���������</param>
        /// <param name="nets">����� �����</param>
        /// <param name="maxNetSize">��������� �������� ���� �����</param>
        /// <param name="percent">������� ������������� (0,100)</param>
        /// <param name="maxSizeX">��������� �������� ������������ ������ ��������</param>
        /// <param name="maxSizeY">��������� �������� ������������ ������ ��������</param>
        /// <param name="design">���������� �����</param>
        void NextDesign(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design);

        void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX,
            int maxSizeY, int width, int height, out Design design, out PlacementGlobal placement);

        void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, int width, int height, out Design design, out PlacementGlobal placement);
    }
}