using PlaceModel;

namespace DetailPlacer.Algorithm
{
    public interface ICompontsOrderer
    {
        /// <summary>
        /// Получение перестановки для размещаемых компонентов
        /// </summary>
        /// <param name="design">Схема</param>
        /// <param name="approximate">Приближённое размещение</param>
        /// <param name="result">Текущее размещение</param>
        /// <param name="unplacedComponents">Неразмещенные компоненты</param>
        /// <param name="perm">Перестановка для неразмещённых компонентов</param>
        void SortComponents(Design design, PlacementGlobal approximate, PlacementDetail result, Component[] unplacedComponents, ref int[] perm);
    }
}