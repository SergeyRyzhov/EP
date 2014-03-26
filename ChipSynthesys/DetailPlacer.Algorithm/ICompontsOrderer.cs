using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface ICompontsOrderer
    {
        /// <summary>
        /// ��������� ������������ ��� ����������� �����������
        /// </summary>
        /// <param name="design">�����</param>
        /// <param name="approximate">����������� ����������</param>
        /// <param name="result">������� ����������</param>
        /// <param name="unplacedComponents">������������� ����������</param>
        /// <param name="perm">������������ ��� ������������� �����������</param>
        void SortComponents(Design design, PlacementGlobal approximate, PlacementDetail result, Component[] unplacedComponents, ref int[] perm);
    }
}