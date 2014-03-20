using PlaceModel;
using System;

namespace ChipSynthesys
{
    public interface IGenerator
    {
        Design Generate();
    }

    /// <summary>
    /// Генератор тестовых примеров интегральных схем
    /// </summary>
    public class RandomGenerator : IGenerator
    {
        /// <summary>
        /// Случайная генерация (равномерный закон распределения)
        /// </summary>
        /// <param name="components">Число компонент</param>
        /// <param name="nets">Число сетей</param>
        /// <param name="percent">Процент заполненности (0,100)</param>
        /// <returns></returns>
        static public Design Random(int components, int nets, int percent)
        {
            int maxsizex = 9;   //максимальная ширина элемента
            int maxsizey = 3;   //максимальная высота элемента
            int maxnetsize = 7; //максимальное число компонент в цепи

            int fullness = 0;
            Random rnd = new Random();
            Component.Pool c = new Component.Pool();
            for (int i = 0; i < components; i++)
            {
                c.Add(rnd.Next(maxsizex) + 1, rnd.Next(maxsizey));
                fullness += c[i].sizex * c[i].sizey;
            }
            int cells = (int)Math.Ceiling(Math.Sqrt(fullness * (100.0 / percent)));
            Net.Pool n = new Net.Pool();
            for (int j = 0; j < nets; j++)
            {
                n.Add(new Component[rnd.Next(maxnetsize - 1) + 2]);
                for (int i = 0; i < n[j].items.Length; i++)
                {
                    int q = rnd.Next(components);
                    n[j].items[i] = c[q];
                }
            }
            return new Design(new Field(0, 0, cells, cells), c, n);
        }

        public Design Generate()
        {
            //todo аргументы через свойства класса.. возможны изменения
            return Random(1, 2, 3);
        }
    }
}