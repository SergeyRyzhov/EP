using System;

namespace ChipSynthesys.Common.Classes
{
    /// <summary>
    /// Вспомогательный класс для хранения пары элементов. Например, координаты, границы диапазона или размеры прямоугольника
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
        /// Первое значение.
        /// </summary>
        public T A
        {
            get { return m_pair[0]; }
            set { m_pair[0] = value; }
        }

        /// <summary>
        /// Второе значение
        /// </summary>
        public T B
        {
            get { return m_pair[1]; }
            set { m_pair[1] = value; }
        }

        public double? Length
        {
            get
            {
                try
                {
                    return Convert.ToDouble(B) - Convert.ToDouble(A);
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Тоже самое, что и A.
        /// </summary>
        public T X
        {
            get { return A; }
            set { A = value; }
        }

        /// <summary>
        /// Тоже самое, что и B.
        /// </summary>
        public T Y
        {
            get { return B; }
            set { B = value; }
        }

        /// <summary>
        /// Тоже самое, что и A.
        /// </summary>
        public T Left
        {
            get { return A; }
            set { A = value; }
        }

        /// <summary>
        /// Тоже самое, что и B.
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