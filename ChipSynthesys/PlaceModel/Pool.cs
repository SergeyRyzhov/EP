using System.Collections.Generic;
using System.Linq;

namespace PlaceModel
{
    public class Factory<T>
    {
        private List<T> items = new List<T>();

        protected int next_id() { return items.Count; }

        protected void Add(T obj)
        {
            items.Add(obj);
        }

        public T this[int index] { get { return items[index]; } }

        public T[] Extract()
        {
            return items.ToArray<T>();
        }
    }
}
