using System;
using System.Collections.Generic;
using System.Linq;

using ChipSynthesys.Common.Classes;

namespace ChipSynthesys.Common.Randoms
{
    /// <summary>
    /// Генератор случайных величин, вероятности которох заданны.
    /// </summary>
    /// <typeparam name="T">Тип генерируемых величин</typeparam>
    public class TableRandom<T> : IRandom<T>
    {
        private readonly List<T> m_values;
        private readonly List<ValuePair<int>> m_pointRanges;
        private readonly Random m_random;
        private int m_pointsSum;

        public TableRandom()
            : this(new Random())
        {
        }

        public TableRandom(Random random)
        {
            this.m_random = random;
            this.m_values = new List<T>();
            this.m_pointRanges = new List<ValuePair<int>>();
        }

        public void Add(T value, int pointsProbability)
        {
            this.m_values.Add(value);
            var old = this.m_pointsSum;
            this.m_pointRanges.Add(new ValuePair<int> { A = old, B = this.m_pointsSum += pointsProbability });
        }

        public T Next()
        {
            var points = this.m_random.Next(this.m_pointsSum);
            var index = this.m_pointRanges.FindIndex(a => a.Left <= points && a.Right > points);
            return this.m_values[index];
        }

        public double? MathematicalExpectation()
        {
            if (!this.m_values.Any())
                return null;
            try
            {
                return this.m_values.Select((t, i) => Convert.ToDouble(t) * this.m_pointRanges[i].Length / this.m_pointsSum).Sum();
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
    }
}