using System.Collections;
using System.Collections.Generic;
using PlaceModel;

namespace ChipSynthesys.Statistic.Models
{
    public class ComponentsMetrik<T> : IEnumerable<KeyValuePair<int, T>>
    {
        private readonly Dictionary<int, T> m_data;

        public ComponentsMetrik()
        {
            m_data = new Dictionary<int, T>();
        }

        public T this[Component component]
        {
            get { return m_data[component.id]; }
            set { m_data[component.id] = value; }
        }

        public Dictionary<int, T> Data
        {
            get { return m_data; }
        } 

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return m_data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ChartPair<TA,TO>
    {
        public TO Ordinate { get; set; }
        public TA Abscissa { get; set; }

    }
}