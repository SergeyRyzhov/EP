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

        public RangeRandom(int min, int max, Random random)
        {
            this.m_min = min;
            this.m_max = max;
            this.m_random = random;
        }

        public int Next()
        {
            return this.m_random.Next(this.m_min, this.m_max);
        }

        public double? MathematicalExpectation()
        {
            return (double)(this.m_min + this.m_max) / 2;
        }
    }
}