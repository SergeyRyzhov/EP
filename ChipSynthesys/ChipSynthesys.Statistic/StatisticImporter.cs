using System.Data;
using System.IO;
using System.Linq;
using ChipSynthesys.Statistic.Interfaces;
using OfficeOpenXml;

namespace ChipSynthesys.Statistic
{
    public class StatisticImporter
    {
        public static void SaveToFile(string fileName, IStatisticResult result)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var w = package.Workbook;
                var common = w.Worksheets.Add("Common");
                var commonPage = GetCommonPage("", result);

                common.Cells.LoadFromDataTable(commonPage, false);

                var charts = w.Worksheets.Add("Charts");

                var chartData = GetChartData(result);
                charts.Cells.LoadFromDataTable(chartData, false);

                OfficeOpenXml.Drawing.Chart.ExcelChart ec = charts.Drawings.AddChart(
                    "Расстояния",
                    OfficeOpenXml.Drawing.Chart.eChartType.ColumnClustered);
                ec.Series.Add(
                    new ExcelNamedRange("ox", charts, charts, "A2:W2"),
                    new ExcelNamedRange("oy", charts, charts, "A1:W1"));

                ec.SetPosition(2, 0, 2, 0);
                ec.SetSize(500, 200);

                package.Save();
            }
        }

        private static DataTable GetCommonPage(string taskName, IStatisticResult result)
        {
            var table = new DataTable("common");

            table.Columns.Add("Характеристика");
            table.Columns.Add("До");
            table.Columns.Add("После");

            table.Rows.Add("Задача", taskName);

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
            table.Rows.Add(result.DistanceChart.Select(d => (int)d.Ordinate).Cast<object>().ToArray());
            return table;
        }
    }
}
