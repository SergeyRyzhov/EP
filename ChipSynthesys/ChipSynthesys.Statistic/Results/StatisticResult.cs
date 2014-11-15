using System.Diagnostics;
using ChipSynthesys.Statistic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChipSynthesys.Statistic.Results
{
    internal class StatisticResult : IStatisticResult<double>
    {
        internal StatisticResult()
        {
            Results = new Dictionary<string, double>();
        }

        internal void Add(ISatisticRow<double> row)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Add(row.Key, row.Compute());
            timer.Stop();
            //Console.WriteLine("Statistic {0} computed for {1}", row.Key, timer.Elapsed);
        }

        internal double this[string row]
        {
            get { return Results.ContainsKey(row) ? Results[row] : 0; }
            set { Add(row, value); }
        }

        protected void Add(string statisticRow, double value)
        {
            if (Results.ContainsKey(statisticRow))
            {
                Results[statisticRow] = value;
            }
            else
            {
                Results.Add(statisticRow, value);
            }
        }

        public Dictionary<string, double> Results { get; private set; }

        public override string ToString()
        {
            var output = new StringBuilder();
            foreach (var variable in Results)
            {
                var statisticLine = string.Format("{0}: {1};{2}", variable.Key, variable.Value,
                    Environment.NewLine);
                output.Append(statisticLine);
            }

            return output.ToString();
        }
    }
}