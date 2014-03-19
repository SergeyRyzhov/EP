﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChipSynthesys.Statistic
{
    class StatisticResult : IStatisticResult<double>
    {
        internal StatisticResult()
        {
            Results = new Dictionary<string, double>();
        }

        internal void Add(string statisticRow, double value)
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

        internal double this[string row]
        {
            get { return Results.ContainsKey(row) ? Results[row] : 0; }
            set { Add(row, value); }
        }

        public Dictionary<string, double> Results { get; private set; }

        public override string ToString()
        {
            var output = new StringBuilder();
            foreach (KeyValuePair<string, double> variable in Results)
            {
                var statisticLine = string.Format("{0}: {1};{2}", variable.Key, variable.Value,
                    Environment.NewLine);
                output.Append(statisticLine);
            }

            return output.ToString();
        }
    }
}