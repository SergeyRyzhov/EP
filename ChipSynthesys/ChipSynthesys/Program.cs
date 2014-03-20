using ChipSynthesys.Statistic.Interfaces;
using ChipSynthesys.Statistic.Statistics;
using PlaceModel;
using System;

namespace ChipSynthesys
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Test.tests.run();

            Design design = RandomGenerator.Random(10, 10, 50);
            IStatistic<double, double> statistic = new CommonStatistic();
            IStatisticResult<double> designResult;
            IStatisticResult<double> placementResult;
            statistic.DesignStatistic(design, out designResult);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(CommonStatistic.Name);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(designResult);

            Design.Save(RandomGenerator.Random(10, 10, 50), "test.xml");
        }
    }
}