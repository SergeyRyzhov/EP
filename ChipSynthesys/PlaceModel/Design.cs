using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;

namespace PlaceModel
{
    /// <summary>
    /// Описание интегральной схемы или её фрагмента
    /// </summary>
    [Serializable()]
    public class Design
    {
        /// <summary>
        /// Параметры монтажного простанства
        /// </summary>
        public readonly Field field;

        /// <summary>
        /// Перечень компонентов
        /// </summary>
        public readonly Component[] components;

        /// <summary>
        /// Перечень цепей
        /// </summary>
        public readonly Net[] nets;

        /// <summary>
        /// Описание интегральной схемы, из которого получено исходное описание
        /// </summary>
        [NonSerialized()]
        public readonly Design parent;

        /// <summary>
        /// Возвращает базоване описание интегральной схемы
        /// </summary>
        public Design top
        {
            get
            {
                if (parent == null) return this;
                return parent.top;
            }
        }

        /// <summary>
        /// Для компонента возвращает перечень цепей, в которые он входит
        /// </summary>
        public Net[] Nets(Component сom)
        {
            if (componentNets == null) initComponentNets();
            return componentNets[сom.id];
        }

        [NonSerialized()]
        private SortedList<int, Net[]> componentNets = null;

        private void initComponentNets()
        {
            var cNets = new SortedList<int, List<Net>>();
            foreach (var n in nets)
                foreach (var c in n.items)
                {
                    if (!cNets.ContainsKey(c.id)) cNets.Add(c.id, new List<Net>());
                    if (!cNets[c.id].Contains(n)) cNets[c.id].Add(n);
                }
            componentNets = new SortedList<int, Net[]>();
            foreach (var c in components)
            {
                if (!cNets.ContainsKey(c.id))
                    componentNets.Add(c.id, new Net[0]);
                else
                    componentNets.Add(c.id, cNets[c.id].ToArray());
            }
        }

        /// <summary>
        /// Создает базовое описание для всей интегральной схемы
        /// </summary>
        public Design(Field field, Component.Pool components, Net.Pool nets)
        {
            this.field = field;
            this.components = components.Extract();
            this.nets = nets.Extract();
            this.parent = null;
        }

        /// <summary>
        /// Создает описание для прямоугольного фрагмента интегральной схемы
        /// </summary>
        public Design(Design parent, Field subfield, IEnumerable<Component> components)
        {
            if (parent == null) throw new Exception("Для подобласти обязательно должен быть определен родительский объект!");
            this.parent = parent;
            this.field = subfield;
            this.components = components.ToArray<Component>();
            var present = new bool[this.top.nets.Length];
            for (var i = 0; i < present.Length; i++) present[i] = false;
            foreach (var c in components)
                foreach (var n in this.top.Nets(c))
                    present[n.id] = true;
            this.nets = this.top.nets.Where(n => present[n.id]).ToArray();
        }

        public static void Save(Design design, string fname)
        {
            Stream stream = File.Open(fname, FileMode.Create);
            var formatter = new SoapFormatter();
            formatter.Serialize(stream, design);
            stream.Close();
        }

        public static Design Load(string fname)
        {
            Stream stream = File.Open(fname, FileMode.Open);
            var formatter = new SoapFormatter();
            var design = (Design)formatter.Deserialize(stream);
            stream.Close();
            return design;
        }
    }
}