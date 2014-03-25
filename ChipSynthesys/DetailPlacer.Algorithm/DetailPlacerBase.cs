using System;
using PlaceModel;
using System.Collections.Generic;
using System.Linq;

namespace DetailPlacer.Algorithm
{
    /// <summary>
    /// 1. метод для создания перечня
    /// 2. *Упорядочивание (перестановка)
    /// 3. Статегия для размещения 1 элемента
    /// 3.1 *эвристика (тоже стратегию)
    /// --пул для позиций(длина - параметр)
    /// 3,2 *выбрать лучшую
    /// **стратегия сравнения
    /// 3,3 присвоение
    /// </summary>
    public abstract class DetailPlacerBase : PlacerBase, IDetailPlacer
    {
        private Random m_rnd;

        protected DetailPlacerBase()
        {

            m_rnd = new Random();
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            IEnumerable<Component> workItems;
            GetUnplacedComponents(design, approximate, out workItems);

            result = new PlacementDetail(design);

            SortComponents(design, approximate, result, workItems, out workItems);

            var components = workItems as Component[] ?? workItems.ToArray();
            foreach (Component current in components)
            {
                IEnumerable<Pair<int>> positions;
                GetAvailablePositions(design, approximate, result, components, out positions);

                SortPositions(design, approximate, result, components, positions, out positions);
                //place one 

                var resultPosition = positions.FirstOrDefault();
                if (resultPosition != null)
                {
                    //var element = components.FirstOrDefault();
                    //if (element != null)
                    {
                        result.x[current] = resultPosition.X;
                        result.y[current] = resultPosition.Y;
                        result.placed[current] = true;
                    }
                }
            }

        }

        public void GetUnplacedComponents(Design design, PlacementGlobal current, out IEnumerable<Component> unplaced)
        {
            unplaced = design.components.Where(component => !current.placed[component]);
        }

        public virtual void SortComponents(Design design, PlacementGlobal current, PlacementDetail result, IEnumerable<Component> components, out IEnumerable<Component> sortedComponents)
        {
            sortedComponents = components;
        }

        public virtual void GetAvailablePositions(Design design, PlacementGlobal current, PlacementDetail result,
            IEnumerable<Component> components, out IEnumerable<Pair<int>> positions)
        {
            var x = m_rnd.Next(design.field.cellsx);
            var y = m_rnd.Next(design.field.cellsy);
            positions = new[] { new Pair<int> { X = 0, Y = 0 }, new Pair<int> { X = x, Y = y } };
        }

        public virtual void SortPositions(Design design, PlacementGlobal current, PlacementDetail result,
            IEnumerable<Component> components, IEnumerable<Pair<int>> positions,
            out IEnumerable<Pair<int>> sortedPositions)
        {
            var sorted = new List<Pair<int>>();
            var enumerable = positions as Pair<int>[] ?? positions.ToArray();

            foreach (Pair<int> first in enumerable)
            {
                Pair<int> tmp = first;
                foreach (Pair<int> second in enumerable)
                {
                    if (!Better(tmp, second))
                    {
                        tmp = second;
                    }
                }
                sorted.Add(tmp);
            }
            sortedPositions = sorted;


        }

        public virtual bool Better(Pair<int> first, Pair<int> second)
        {
            return false;
        }
    }

    public class DetailPlacerImpl : DetailPlacerBase
    {
    }

    //use values class from placemodel
    public class Pair<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
    }
}