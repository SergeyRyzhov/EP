using System;

namespace PlaceModel
{
    /// <summary>
    /// Координаты размещения компонентов
    /// </summary>
    public class Placement<T>
    {
        /// <summary>
        /// Вектор значений 
        /// </summary>
        public class Values<R>
        {
            private Placement<T> owner;
            private R[] vals;

            public Values(Placement<T> owner, int count)
            {
                this.owner = owner;
                this.vals = new R[count];
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
            int count = design.top.components.Length;
            ids = 0;
            editable = new int[count];
            for (int i = 0; i < count; i++) editable[i] = ids;
            Editable(design);
            x = new Values<T>(this, count);
            y = new Values<T>(this, count);
            placed = new Values<bool>(this, count);
        }
    }

    /// <summary>
    /// Решение задачи глобального размещения (вещественные координаты)
    /// </summary>
    public class PlacementGlobal : Placement<double>
    {
        public PlacementGlobal(Design design) : base(design) { } 
    }

    /// <summary>
    /// Решение задачи детального размещения (целочисленные координаты)
    /// </summary>
    public class PlacementDetail : Placement<int>
    {
        public PlacementDetail(Design design) : base(design) { }
    }

}
