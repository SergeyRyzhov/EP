﻿using PlaceModel;

namespace DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl
{
    internal class PositionComparer : IPositionComparer
    {
        public override string ToString()
        {
            return "Жадный выбор позиций со строго меньшими или большим координатами";
        }

        public bool Better(Design design, PlacementGlobal approximate, PlacementDetail placement, Component current, int firstX, int firstY, int secondX, int secondY)
        {
            if (firstX < secondX)
                if (firstY < secondY)
                    return true;

            if (firstX > secondX)
                if (firstY > secondY)
                    return true;
            return false;
        }
    }
}