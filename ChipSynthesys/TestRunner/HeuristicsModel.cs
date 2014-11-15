using System;

using DetailPlacer.Algorithm.CompontsOrderer;
using DetailPlacer.Algorithm.PositionSearcher;
using DetailPlacer.Algorithm.PositionSorter;
using DetailPlacer.Algorithm.PositionSorter.PositionComparer;

using PlaceModel;

namespace TestRunner
{
    [Serializable]
    public class HeuristicsModel : XmlModel
    {
        public HeuristicsModel()
        {
            Heuristics = new SerializableDictionary<string, string>();
        }

        public HeuristicsModel(
            ICompontsOrderer compontsOrderer, 
            IPositionSearcher positionSearcher, 
            IPositionComparer positionComparer, 
            IPositionsSorter positionsSorter)
        {
            Heuristics = new SerializableDictionary<string, string>
                                  {
                                      {
                                          "Сортировка компонент", 
                                          compontsOrderer.ToString()
                                      }, 
                                      {
                                          "Поиск позиций", 
                                          positionSearcher.ToString()
                                      }, 
                                      {
                                          "Сортировка позиций", 
                                          positionsSorter.ToString()
                                      }, 
                                      {
                                          "Сравнение позиций", 
                                          positionComparer.ToString()
                                      }
                                  };
        }

        public HeuristicsModel(IDetailPlacer placer)
        {
            Heuristics = new SerializableDictionary<string, string> { { "Размещатель", placer.ToString() } };
        }

        public SerializableDictionary<string, string> Heuristics { get; set; }
    }
}