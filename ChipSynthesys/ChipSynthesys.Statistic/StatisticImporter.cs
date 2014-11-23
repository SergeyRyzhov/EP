using System;
using System.Data;
using System.IO;
using System.Linq;
using ChipSynthesys.Statistic.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace ChipSynthesys.Statistic
{
    public class StatisticImporter
    {
        public static void SaveToFile(
            string fileName,
            IStatisticResult result,
            string taskName = default(string),
            string methodName = default(string))
        {
            string oldSuffix = ".old";
            int oldCounter = 0;
            if (File.Exists(fileName))
            {
                var oldFile = fileName;
                while (File.Exists(oldFile))
                {
                    var lastOldIndex = oldFile.LastIndexOf(".old", StringComparison.InvariantCultureIgnoreCase);
                    var lastDotIndex = oldFile.LastIndexOf('.');
                    lastDotIndex = lastDotIndex < 0 ? oldFile.Length - 1 : lastDotIndex;
                    lastOldIndex = lastOldIndex < 0 ? lastDotIndex : lastOldIndex;

                    oldFile = oldFile.Substring(0, lastOldIndex) + oldSuffix + oldFile.Substring(lastDotIndex);
                    //string.Concat(fileName, oldSuffix);
                    oldSuffix = string.Format(".old{0}", ++oldCounter);
                }

                File.Move(fileName, oldFile);
            }

            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var w = package.Workbook;
                var common = w.Worksheets.Add("Common");
                var commonPage = GetCommonPage(taskName, methodName, result);

                common.Cells.LoadFromDataTable(commonPage, false);

                var charts = w.Worksheets.Add("Charts");

                var chartData = GetChartData(result);
                var r = charts.Cells.LoadFromDataTable(chartData, false);

                r.Style.Numberformat.Format = "#,##0.0";
                var index = r.End.Address.IndexOfAny(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                string letter = r.End.Address.Substring(0, index);

                ExcelChart ec = charts.Drawings.AddChart("Расстояния", eChartType.ColumnClustered);
                ec.Series.Add(string.Format("A2:{0}2", letter), string.Format("A1:{0}1", letter));

                ec.SetPosition(2, 0, 2, 0);
                ec.SetSize(500, 200);

                package.Save();
            }
        }

        private static DataTable GetCommonPage(string taskName, string methodName, IStatisticResult result)
        {
            var table = new DataTable("common");

            table.Columns.Add("Характеристика");
            table.Columns.Add("До");
            table.Columns.Add("После");

            table.Rows.Add("Задача", taskName);
            table.Rows.Add("Метод", methodName);

            table.Rows.Add("Количество компонент", result.ComponentsAmount);
            table.Rows.Add("Количество цепей", result.NetsAmount);

            table.Rows.Add("Характеристика", "До", "После");
            table.Rows.Add("Размещено", result.PlacedAmount.Before, result.PlacedAmount.After);
            table.Rows.Add("Манхеттенская метрика", result.ManhattanMetric.Before, result.ManhattanMetric.After);
            table.Rows.Add(
                "Количество пересечений",
                result.IntersectionsAmount.Before,
                result.IntersectionsAmount.After);
            table.Rows.Add(
                "Суммарная площадь пересечений",
                result.AreaOfIntersections.Before,
                result.AreaOfIntersections.After);

            return table;
        }

        private static DataTable GetChartData(IStatisticResult result)
        {
            var table = new DataTable("common");
            for (int i = 0; i < result.DistanceChart.Length; i++)
            {
                table.Columns.Add(string.Format("Column{0}", i));
            }

            table.Rows.Add(result.DistanceChart.Select(d => d.Abscissa).Cast<object>().ToArray());
            table.Rows.Add(result.DistanceChart.Select(d => d.Ordinate).Cast<object>().ToArray());
            return table;
        }
    }
}
