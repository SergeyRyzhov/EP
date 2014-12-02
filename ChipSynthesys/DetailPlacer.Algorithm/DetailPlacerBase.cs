using System.Collections.Generic;
using System.Linq;
using System.Text;

using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;

using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public abstract class DetailPlacerBase : IDetailPlacer
    {
        internal readonly ICompontsOrderer ComponentsOrderer;

        internal readonly IPositionSearcher PositionSearcher;

        internal readonly IPositionsSorter PositionsSorter;

        protected DetailPlacerBase(ICompontsOrderer componentsOrderer, IPositionSearcher positionSearcher, IPositionsSorter positionsSorter)
        {
            this.ComponentsOrderer = componentsOrderer;
            this.PositionSearcher = positionSearcher;
            this.PositionsSorter = positionsSorter;
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            result = new PlacementDetail(design);
            foreach (Component component in design.components)
            {
                result.placed[component] = approximate.placed[component];
                result.x[component] = (int)approximate.x[component];
                result.y[component] = (int)approximate.y[component];
            }

            var helper = new Mask(design, result);
            helper.BuildUp();
            var notPlaced = new List<Component>();
            Component[] unplacedComponents;
            do
            {
                UpdatePlaced(design, approximate, result, notPlaced, out unplacedComponents);
                if (unplacedComponents.Length == 0)
                {
                    break;
                }

                var perm = new int[unplacedComponents.Length];
                this.ComponentsOrderer.SortComponents(design, approximate, result, unplacedComponents, ref perm);

                //ReorderArray(perm, ref unplacedComponents);

                Component current = null; //.FirstOrDefault();

                if (perm.Length > 0 && unplacedComponents.Length > 0)
                {
                    current = unplacedComponents[perm[0]];
                }

                bool placed;
                PlaceComponent(helper, design, approximate, current, result, out placed);

                if (!placed)
                {
                    notPlaced.Add(current);
                }
            } 
            while (unplacedComponents.Length > 0);

            foreach (Component component in notPlaced)
            {
                result.x[component] = (int)approximate.x[component];
                result.y[component] = (int)approximate.y[component];
                result.placed[component] = false;
            }
        }

        protected virtual void PlaceComponent(Mask helper, Design design, PlacementGlobal approximate, Component current, PlacementDetail result, out bool placed)
        {
            int[] x = new int[this.PositionSearcher.PositionAmount];
            int[] y = new int[this.PositionSearcher.PositionAmount];

            if (this.PositionSearcher.AlvailablePositions(helper, current, (int)approximate.x[current], (int)approximate.y[current], x, y))
            {
                var perm = new int[x.Length];
                this.PositionsSorter.SortPositions(design, approximate, result, current, x, y, ref perm);

                ReorderArray(perm, ref x);
                ReorderArray(perm, ref y);

                result.x[current] = x[0];
                result.y[current] = y[0];
                result.placed[current] = true;
                placed = true;
                helper.PlaceComponent(current, x[0], y[0]);
            }
            else
            {
                helper.PlaceComponent(current, result.x[current], result.y[current]);
                result.placed[current] = true;
                placed = false;
            }
        }

        /// <summary>
        /// Перепаковка компонент согласно перестановке
        /// </summary>
        /// <param name="perm"></param>
        /// <param name="unplacedComponents"></param>
        protected virtual void ReorderArray<T>(int[] perm, ref T[] unplacedComponents)
        {
            var reorderd = new T[perm.Length];
            for (int i = 0; i < perm.Length; i++)
            {
                var index = perm[i];
                reorderd[i] = unplacedComponents[index];
            }
            unplacedComponents = reorderd;
        }

        /// <summary>
        /// Фиксирование уже размещённых компонентов
        /// </summary>
        protected virtual void UpdatePlaced(Design design, PlacementGlobal approximate, PlacementDetail current, List<Component> notPalced, out Component[] unplacedComponents)
        {
            var unplaced = new List<Component>();
            foreach (Component component in design.components)
            {
                if (approximate.placed[component])
                {
                    current.placed[component] = true;
                }
                else
                {
                    if (!notPalced.Contains(component) && !current.placed[component])
                        unplaced.Add(component);
                }
            }
            unplacedComponents = unplaced.ToArray();
        }
    }

    public class DetailPlacerImpl : DetailPlacerBase
    {
        public DetailPlacerImpl()
            : base(
                new CompontsOrderer.Impl.CompontsOrderer(),
                new SpiralPositionSearcher(),
                new PositionsSorter(new NetsPositionComparer()))
        {
        }

        public DetailPlacerImpl(ICompontsOrderer componentsOrderer, IPositionSearcher positionSearcher, IPositionsSorter positionsSorter)
            : base(componentsOrderer, positionSearcher, positionsSorter)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(this.ComponentsOrderer.ToString());
            sb.AppendLine(this.PositionSearcher.ToString());
            sb.AppendLine(this.PositionsSorter.ToString());
            return sb.ToString();
        }
    }
}