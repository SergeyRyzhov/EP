using System.Text;
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

            RandomStatisticTest();

            Design.Save(design, "test.xml");
        }

        private static void RandomStatisticTest()
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Случайный с размещением");

            var g = new RandomGeneratorWithPlacement();
            Design d;
            PlacementDetail p;
            g.NextDesignWithPlacement(2, 1, 2, 50, 3, 3, out d, out p);

            IStatistic<double, double> statistic = new CommonStatistic();
            IStatisticResult<double> designResult;
            IStatisticResult<double> placementResult;
            statistic.DesignStatistic(d, out designResult);
            statistic.PlacementStatistic(d, p, out placementResult);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(CommonStatistic.Name);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(designResult);
            Console.WriteLine(placementResult);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Размещение");

            Console.ForegroundColor = ConsoleColor.White;
            foreach (var c in d.components)
            {
                string s = string.Format("{0} [{3}x{4}] - ({1},{2})", c.id, p.x[c], p.y[c], c.sizex, c.sizey);
                Console.WriteLine(s);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Цепи");

            Console.ForegroundColor = ConsoleColor.White;
            foreach (var net in d.nets)
            {
                var s = new StringBuilder();
                s.Append(net.id);
                s.Append(": ");

                foreach (var component in net.items)
                {
                    s.Append(component.id);
                    s.Append(", ");
                }
                Console.WriteLine(s.ToString());
            }

        }

        private static void RandomTest()
        {
            var low = new TableRandom<int>();
            low.Add(2, 25);
            low.Add(1, 25);
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

            Console.WriteLine("Математическое ожидание: {0}.", low.MathematicalExpectation());
        }
    }
}