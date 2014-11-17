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

                OfficeOpenXml.Drawing.Chart.ExcelChart ec = charts.Drawings.AddChart(
                    "Расстояния",
                    OfficeOpenXml.Drawing.Chart.eChartType.ColumnClustered);
                //common.a
                /*ec.Series.Add(
                    new ExcelNamedRange("ox", common, common, "A1:A20"),
                    new ExcelNamedRange("ox", common, common, "A1:A20"));*/

                ec.SetPosition(1, 0, 3, 0);
                ec.SetSize(500, 200);


                package.Save();
            }
        }

        private static DataTable GetCommonPage(string taskName, IStatisticResult result)
        {
            var table = new DataTable("common");

            /* table.Columns.Add("Характеристика");
            table.Columns.Add("До");
            table.Columns.Add("После");*/

            for (int i = 0; i < result.SquareDistance.Length; i++)
            {
                table.Columns.Add(string.Format("Column{0}", i));

            }

            table.Rows.Add("Задача", taskName);

            table.Rows.Add("Количество компонент", result.ComponentsAmount);
            table.Rows.Add("Количество цепей", result.NetsAmount);

            table.Rows.Add("Характеристика", "До", "После");
            table.Rows.Add("Размещено", result.PlacedAmount.Before, result.PlacedAmount.After);
            table.Rows.Add("Манхеттенская метрика", result.ManhattanMetrik.Before, result.ManhattanMetrik.After);
            table.Rows.Add(
                "Количество пересечений",
                result.InterserctionsAmount.Before,
                result.InterserctionsAmount.After);
            table.Rows.Add(
                "Суммарная площадь пересечений",
                result.AreaOfInterserctions.Before,
                result.AreaOfInterserctions.After);

            table.Rows.Add("Данные для диаграмм");

            table.Rows.Add(result.SquareDistance.Select(d => d.Abscissa).Cast<object>().ToArray());
            table.Rows.Add(result.SquareDistance.Select(d => d.Ordinate).Cast<object>().ToArray());

            return table;
        }
    }
}
