using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.CompontsOrderer.Impl;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSearcher.Impl;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.Impl;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer.Impl;

using PlaceModel;

namespace DetailPlacer.Algorithm
{
    /// <summary>
    /// Заглушка для размщения
    /// </summary>
    public class CombinePlacer : DetailPlacerImpl
    {
        public CombinePlacer()
            : base(new NetAreaCompontsOrderer(), new SpiralPositionSearcher(), new PositionsSorter(new MarkCrossingNetPositionComparer()))
        {
        }
    }
}
