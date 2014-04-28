using System;

namespace ChipSynthesys.Common.Randoms
{
    /// <summary>
    /// Класс-обёртка над генератором случайных чисел типа int. Равномерное распределение.
    /// </summary>
    public class RangeRandom : IRandom<int>
    {
        private readonly int m_min;
        private readonly int m_max;
        private readonly Random m_random;

        public RangeRandom(int max)
            : this(0, max, new Random())
        {
        }

        public RangeRandom(int min, int max)
            : this(min, max, new Random())
        {
        }

        public RangeRandom(int min, int max, Random random)
        {
            m_min = min;
            m_max = max;
            m_random = random;
        }

        public int Next()
        {
            return m_random.Next(m_min, m_max);
        }

        public double? MathematicalExpectation()
        {
            return (double)(m_min + m_max) / 2;
        }
    }
}