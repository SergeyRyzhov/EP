using System;

namespace ChipSynthesys.Common.Classes
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
            this.m_pair = new T[2];
        }

        /// <summary>
        /// ������ ��������.
        /// </summary>
        public T A
        {
            get { return this.m_pair[0]; }
            set { this.m_pair[0] = value; }
        }

        /// <summary>
        /// ������ ��������
        /// </summary>
        public T B
        {
            get { return this.m_pair[1]; }
            set { this.m_pair[1] = value; }
        }

        public double? Length
        {
            get
            {
                try
                {
                    return Convert.ToDouble(this.B) - Convert.ToDouble(this.A);
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// ���� �����, ��� � A.
        /// </summary>
        public T X
        {
            get { return this.A; }
            set { this.A = value; }
        }

        /// <summary>
        /// ���� �����, ��� � B.
        /// </summary>
        public T Y
        {
            get { return this.B; }
            set { this.B = value; }
        }

        /// <summary>
        /// ���� �����, ��� � A.
        /// </summary>
        public T Left
        {
            get { return this.A; }
            set { this.A = value; }
        }

        /// <summary>
        /// ���� �����, ��� � B.
        /// </summary>
        public T Right
        {
            get { return this.B; }
            set { this.B = value; }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", this.A, this.B);
        }
    }
}