using ChipSynthesys.Statistic.Interfaces;
using ChipSynthesys.Statistic.Statistics;
using PlaceModel;
using System;

namespace ChipSynthesys
{
    //todo вынести генераторы их этой сборки
    internal class Program
    {
        private static void Main(string[] args)
        {
            Test.Tests.run();
            IGenerator random = new RandomGenerator();
            Design design;
            random.NextDesign(10, 10, 7, 50, 9 ,3, out design);
            IStatistic<double, double> statistic = new CommonStatistic();
            IStatisticResult<double> designResult;
            IStatisticResult<double> placementResult;
            statistic.DesignStatistic(design, out designResult);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(CommonStatistic.Name);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(designResult);

            RandomTest();

            Design.Save(design, "test.xml");
        }

        private static void RandomTest()
        {
            var low = new TableRandom<int>();
            low.Add(2, 7);
            low.Add(1, 3);
            var counters = new ValuePair<int>();
            for (var i = 0; i < 10000; i++)
            {
                if (low.Next() == 1)
                {
                    counters.A++;
                }
                else
                {
                    counters.B++;
                }
            }
            Console.WriteLine("Результат теста генератора с табличным распределением (1 - 30%, 2 -70%), 10000 испытаний: {0}.", counters);
        }
    }
}