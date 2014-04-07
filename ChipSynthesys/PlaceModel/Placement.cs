using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PlaceModel
{
    /// <summary>
    /// Координаты размещения компонентов
    /// </summary>
    [Serializable]
    public class Placement<T>
    {
        /// <summary>
        /// Вектор значений
        /// </summary>
        [Serializable]
        public class Values<R>
        {
            private Placement<T> owner;
            private R[] vals;

            public Values(Placement<T> owner, int count)
            {
                this.owner = owner;
                vals = new R[count];
            }

            public R this[Component c]
            {
                get { return vals[c.id]; }
                set
                {
                    if (owner.editable[c.id] != owner.ids) throw new Exception(string.Format("Попытка изменения позиции для периферийного элемента {0}", c.id));
                    vals[c.id] = value;
                }
            }
        }

        private int[] editable;
        private int ids;

        public void Editable(Design design)
        {
            ids++;
            foreach (var c in design.components) editable[c.id] = ids;
        }

        /// <summary>
        /// x-координаты компонент
        /// </summary>
        public readonly Values<T> x;

        /// <summary>
        /// y-координаты компонент
        /// </summary>
        public readonly Values<T> y;

        /// <summary>
        /// признаки размещения компонент
        /// </summary>
        public readonly Values<bool> placed;

        public Placement(Design design)
        {
            var count = design.top.components.Length;
            ids = 0;
            editable = new int[count];
            for (var i = 0; i < count; i++) editable[i] = ids;
            Editable(design);
            x = new Values<T>(this, count);
            y = new Values<T>(this, count);
            placed = new Values<bool>(this, count);
        }
    }

    /// <summary>
    /// Решение задачи глобального размещения (вещественные координаты)
    /// </summary>
    [Serializable]
    public class PlacementGlobal : Placement<double>
    {
        public PlacementGlobal(Design design)
            : base(design)
        {
        }
    }

    /// <summary>
    /// Решение задачи детального размещения (целочисленные координаты)
    /// </summary>
    [Serializable]
    public class PlacementDetail : Placement<int>
    {
        public PlacementDetail(Design design)
            : base(design)
        {
        }
    }
}