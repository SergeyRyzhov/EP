using System;

namespace ChipSynthesys.Common.Classes
{
    //todo вынести в общую сборку
    /// <summary>
    /// Вспомогательный класс для хранения пары элементов. Например, координаты, границы диапазона или размеры прямоугольника
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
        /// Первое значение.
        /// </summary>
        public T A
        {
            get { return this.m_pair[0]; }
            set { this.m_pair[0] = value; }
        }

        /// <summary>
        /// Второе значение
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
        /// Тоже самое, что и A.
        /// </summary>
        public T X
        {
            get { return this.A; }
            set { this.A = value; }
        }

        /// <summary>
        /// Тоже самое, что и B.
        /// </summary>
        public T Y
        {
            get { return this.B; }
            set { this.B = value; }
        }

        /// <summary>
        /// Тоже самое, что и A.
        /// </summary>
        public T Left
        {
            get { return this.A; }
            set { this.A = value; }
        }

        /// <summary>
        /// Тоже самое, что и B.
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