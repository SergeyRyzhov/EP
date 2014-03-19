using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaceModel
{
    /// <summary>
    /// Описание габаритов компонента интегральной схемы 
    /// </summary>
    [Serializable()]
    public class Component
    {
        /// <summary>
        /// Уникальный номер компонента 
        /// (не совпадает с номером в списке компонентов интегральной схемы)
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Число занимаемых посадочных мест по горизонтали
        /// </summary>
        public readonly int sizex;

        /// <summary>
        /// Число занимаемых посадочных мест по вертикали
        /// </summary>
        public readonly int sizey;

        private Component(int id, int sizex, int sizey)
        {
            this.id = id;
            this.sizex = sizex;
            this.sizey = sizey;
        }

        public override string ToString()
        {
            return string.Format("Component {0}", id);
        }

        /// <summary>
        /// Фабрика для создания компонентов интегральной схемы
        /// </summary>
        public class Pool : Factory<Component>
        {
            public void Add(int sizex, int sizey)
            {
                this.Add(new Component(next_id(), sizex, sizey));
            }
        }
    }
}
