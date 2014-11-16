﻿using System;
using System.Linq;
using ChipSynthesys.Common.Classes;
using ChipSynthesys.Statistic.Interfaces;
using ChipSynthesys.Statistic.Models;
using PlaceModel;

namespace ChipSynthesys.Statistic.Statistics
{
    public class CommonStatistic
    {
        public IStatisticResult Compute(Design design, PlacementGlobal global, PlacementDetail detail)
        {
            var statisticResult = new StatisticResult();
            statisticResult.PlacedAmount = new Result<int>(design.components.Count(c => global.placed[c]));
            statisticResult.ManhattanMetrik = new Result<double>(CriterionHelper.ComputeMetrik(design, global));
            //statisticResult.Interserctions = new Result<Interserction[]>();
//            sr

            return statisticResult;
        }

        public IStatisticResult Update(IStatisticResult current, Design design, PlacementGlobal global, PlacementDetail detail)
        {
            var statisticResult = current as StatisticResult;
            if (statisticResult == null)
            {
                throw new NotSupportedException();
            }

            if (statisticResult.PlacedAmount != null)
            {
                statisticResult.PlacedAmount.After = design.components.Count(c => detail.placed[c]);
            }

            if (statisticResult.ManhattanMetrik != null)
            {
                statisticResult.ManhattanMetrik.After = CriterionHelper.ComputeMetrik(design, detail);
            }

            return statisticResult;
        }
    }
}