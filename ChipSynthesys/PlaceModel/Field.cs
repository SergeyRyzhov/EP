using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaceModel
{
    /// <summary>
    /// Параметры монтажного простанства (или подпространства) интегральной схемы
    /// </summary>
    [Serializable()]
    public class Field
    {
        /// <summary>
        /// Начальный ряд посадочных мест по горизонтали.
        /// </summary>
        public readonly int beginx;

        /// <summary>
        /// Начальный ряд посадочных мест по вертикали.
        /// </summary>
        public readonly int beginy;

        /// <summary>
        /// Число (допустимых для размещения) рядов посадочных мест по горизонтали.
        /// Нумерация рядов начинается с 0.
        /// </summary>
        public readonly int cellsx;

        /// <summary>
        /// Число (допустимых для размещения) рядов посадочных мест по вертикали.
        /// Нумерация рядов начинается с 0.
        /// </summary>
        public readonly int cellsy;

        public Field(int beginx, int beginy, int cellsx, int cellsy)
        {
            this.beginx = beginx;
            this.beginy = beginy;
            this.cellsx = cellsx;
            this.cellsy = cellsy;
        }
    }
}
