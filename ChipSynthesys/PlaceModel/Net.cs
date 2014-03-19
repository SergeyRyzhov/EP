using System;

namespace PlaceModel
{
    /// <summary>
    /// Цепь, связывающая заданный набор элементов интегральной схемы
    /// </summary>
    [Serializable()]
    public class Net
    {
        /// <summary>
        /// Уникальный номер цепи (не совпадает с номером в списке цепей интегральной схемы)
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Элементы цепи
        /// </summary>
        public readonly Component[] items;

        private Net(int id, Component[] items)
        {
            this.id = id;
            this.items = items;
        }

        public override string ToString()
        {
            return string.Format("Net id={0} items={1}", id, items.Length);
        }


        /// <summary>
        /// Фабрика для создания цепей интегральной схемы
        /// </summary>
        public class Pool : Factory<Net>
        {
            public void Add(Component[] items)
            {
                this.Add(new Net(next_id(), items));
            }
        }

    }
}
