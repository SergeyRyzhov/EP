using System.Collections.Generic;

namespace ChipSynthesys.Statistic.Interfaces
{
    public interface IStatisticResult<T>
    {
        Dictionary<string, T> Results { get; }

        string ToString();
    }


    /* public interface IStatisticResult
     {
         double ManhattanMetrik { get; }
         double AreaOfInterserctions { get; }
         double InterserctionsAmount { get; }
         double Interserctions { get; }

         Dictionary<string> Results { get; }

         string ToString();

         class Interserction
         {
             public Interserction(int firstComponentIndex, int secondComponentIndex)
             {
                FirstComponentIndex = firstComponentIndex;
                SecondComponentIndex= secondComponentIndex;
             }

             public int FirstComponentIndex { get; private set; }

             public int SecondComponentIndex { get; private set; }

             public double Area { get; set; }
         }
     }*/

}