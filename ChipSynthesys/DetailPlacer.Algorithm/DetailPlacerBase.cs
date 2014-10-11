﻿using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;
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
    public abstract class DetailPlacerBase : IDetailPlacer
    {
        private readonly ICompontsOrderer m_compontsOrderer;
        private readonly IPositionSearcher m_positionSearcher;
        private readonly IPositionsSorter m_positionsSorter;

        protected DetailPlacerBase(ICompontsOrderer compontsOrderer, IPositionSearcher positionSearcher, IPositionsSorter positionsSorter)
        {
            m_compontsOrderer = compontsOrderer;
            m_positionSearcher = positionSearcher;
            m_positionsSorter = positionsSorter;
        }

        public void Place(Design design, PlacementGlobal approximate, out PlacementDetail result)
        {
            result = new PlacementDetail(design);
            foreach (Component component in design.components)
            {
//                result.x[component] = (int)approximate.x[component];
//                result.y[component] = (int)approximate.y[component];
            }
            PositionHelper helper = new PositionHelper(design,result);
            helper.Build();
            var notPalced = new List<Component>();
            Component[] unplacedComponents;
            do
            {
                UpdatePlaced(design, approximate, result, notPalced, out unplacedComponents);
                if (unplacedComponents.Length == 0)
                    break;
                var perm = new int[unplacedComponents.Length];
                m_compontsOrderer.SortComponents(design, approximate, result, unplacedComponents, ref perm);

                ReorderArray(perm, ref unplacedComponents);

                var current = unplacedComponents.FirstOrDefault();

                bool placed;
                PlaceComponent(helper, design, approximate, current, result, out placed);

                if (!placed)
                    notPalced.Add(current);
            } while (unplacedComponents.Length > 0);

            foreach (Component component in notPalced)
            {
                result.x[component] = (int)approximate.x[component];
                result.y[component] = (int)approximate.y[component];
                result.placed[component] = false;
            }
        }

        protected virtual void PlaceComponent(PositionHelper helper, Design design, PlacementGlobal approximate, Component current, PlacementDetail result, out bool placed)
        {
            int[] x;
            int[] y;
            bool hasPosition;

            m_positionSearcher.AlvailablePositions(helper, design, approximate, result, current, out x, out y, out hasPosition);
            if (hasPosition)
            {
                var perm = new int[x.Length];
                m_positionsSorter.SortPositions(design, approximate, result, current, x, y, ref perm);

                ReorderArray(perm, ref x);
                ReorderArray(perm, ref y);

                result.x[current] = x[0];
                result.y[current] = y[0];
                result.placed[current] = true;
                placed = true;
                helper.MoveComponent(current, x[0], y[0]);
            }
            else
            {
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
            : base(new CompontsOrderer.Impl.CompontsOrderer(), new PositionSearcher.Impl.LinearPositionSearcher(), new PositionsSorter(new NetsPositionComparer()))
        {
        }

        public DetailPlacerImpl(ICompontsOrderer compontsOrderer, IPositionSearcher positionSearcher, IPositionsSorter positionsSorter)
            : base(compontsOrderer, positionSearcher, positionsSorter)
        {
        }
    }
}