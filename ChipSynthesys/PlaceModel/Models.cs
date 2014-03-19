﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaceModel
{
    /// <summary>
    /// Интерфейс алгоритмов глобального размещения компонентов интегральной схемы
    /// </summary>
    public interface IGlobalPlacer
    {
        /// <summary>
        /// Осуществляет глобальное размещение компонентов интегральной схемы
        /// </summary>
        /// <param name="design">Описание интегральной схемы</param>
        /// <param name="result">Результат - вектор глобального размещения (действительные координаты)</param>
        void Place(Design design, PlacementGlobal result); 
    }

    /// <summary>
    /// Интерфейс алгоритмов детального размещения компонентов интегральной схемы
    /// </summary>
    public interface IDetailPlacer
    {
        /// <summary>
        /// Осуществляет детальное размещение компонентов интегральной схемы
        /// </summary>
        /// <param name="design">Описание интегральной схемы</param>
        /// <param name="approximate">Некоторое начальное размещения (действительные координаты)</param>
        /// <param name="result">Результат - вектор детального размещения (целочисленные координаты)</param>
        void Place(Design design, PlacementGlobal approximate, PlacementDetail result);
    }
}
