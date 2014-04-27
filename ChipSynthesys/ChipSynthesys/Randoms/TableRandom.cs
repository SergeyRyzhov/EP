using ChipSynthesys.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

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
            m_random = random;
            m_values = new List<T>();
            m_pointRanges = new List<ValuePair<int>>();
        }

        public void Add(T value, int pointsProbability)
        {
            m_values.Add(value);
            var old = m_pointsSum;
            m_pointRanges.Add(new ValuePair<int> { A = old, B = m_pointsSum += pointsProbability });
        }

        public T Next()
        {
            var points = m_random.Next(m_pointsSum);
            var index = m_pointRanges.FindIndex(a => a.Left <= points && a.Right > points);
            return m_values[index];
        }

        public double? MathematicalExpectation()
        {
            if (!m_values.Any())
                return null;
            try
            {
                return m_values.Select((t, i) => Convert.ToDouble(t) * m_pointRanges[i].Length / m_pointsSum).Sum();
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
    }
}