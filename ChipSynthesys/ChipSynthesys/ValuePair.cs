namespace ChipSynthesys
{
    //todo ������� � ����� ������
    /// <summary>
    /// ��������������� ����� ��� �������� ���� ���������. ��������, ����������, ������� ��������� ��� ������� ��������������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValuePair<T>
    {
        private readonly T[] m_pair;

        public ValuePair()
        {
            m_pair = new T[2];
        }

        /// <summary>
        /// ������ ��������.
        /// </summary>
        public T A
        {
            get { return m_pair[0]; }
            set { m_pair[0] = value; }
        }

        /// <summary>
        /// ������ ��������
        /// </summary>
        public T B
        {
            get { return m_pair[1]; }
            set { m_pair[1] = value; }
        }

        /// <summary>
        /// ���� ����� ��� � A.
        /// </summary>
        public T X
        {
            get { return A; }
            set { A = value; }
        }

        /// <summary>
        /// ���� ����� ��� � B.
        /// </summary>
        public T Y
        {
            get { return B; }
            set { B = value; }
        }

        /// <summary>
        /// ���� ����� ��� � A.
        /// </summary>
        public T Left
        {
            get { return A; }
            set { A = value; }
        }

        /// <summary>
        /// ���� ����� ��� � B.
        /// </summary>
        public T Right
        {
            get { return B; }
            set { B = value; }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", A, B);
        }
    }
}