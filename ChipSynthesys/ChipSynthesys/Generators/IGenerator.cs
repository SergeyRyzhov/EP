using ChipSynthesys.Common.Randoms;

using PlaceModel;

namespace ChipSynthesys.Common.Generators
{
    /// <summary>
    /// Интерфейс генератора Схемы и Размещения
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Случайная генерация (равномерный закон распределения)
        /// </summary>
        /// <param name="components">Число компонент</param>
        /// <param name="nets">Число сетей</param>
        /// <param name="maxNetSize">Максимальное число компонент в цепи</param>
        /// <param name="percent">Процент заполненности (0,100)</param>
        /// <param name="maxSizeX">Максимальная ширина элемента</param>
        /// <param name="maxSizeY">Максимальная высота элемента</param>
        /// <param name="design">Полученная схема</param>
        void NextDesign(int components, int nets, int maxNetSize, int percent, int maxSizeX, int maxSizeY, out Design design);

        /// <summary>
        /// Случайная генерация (заданный закон распределения)
        /// </summary>
        /// <param name="components">Число компонент</param>
        /// <param name="nets">Число сетей</param>
        /// <param name="maxNetSize">Генератор случаных длин цепей</param>
        /// <param name="percent">Процент заполненности (0,100)</param>
        /// <param name="maxSizeX">Генератор случаной максимальной ширины элемента</param>
        /// <param name="maxSizeY">Генератор случаной максимальной высоты элемента</param>
        /// <param name="design">Полученная схема</param>
        void NextDesign(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, out Design design);

        void NextDesignWithPlacement(int components, int nets, int maxNetSize, int percent, int maxSizeX,
            int maxSizeY, int width, int height, out Design design, out PlacementGlobal placement);

        void NextDesignWithPlacement(int components, int nets, IRandom<int> maxNetSize, int percent,
            IRandom<int> maxSizeX, IRandom<int> maxSizeY, int width, int height, out Design design, out PlacementGlobal placement);
    }
}