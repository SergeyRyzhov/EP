using System.Data;
using System.IO;
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

                var commonPage = GetCommonPage(result);
                common.Cells.LoadFromDataTable(commonPage,true);

                package.Save();
            }
        }

        private static DataTable GetCommonPage(IStatisticResult result)
        {
            var table = new DataTable("common");

            table.Columns.Add("Характеристика");
            table.Columns.Add("До");
            table.Columns.Add("После");

            table.Rows.Add(new object[]
            {
                "Размещено", result.PlacedAmount.Before,
                result.PlacedAmount.After
            });
            table.Rows.Add(new object[]
            {
                "Манхеттенская метрика", result.ManhattanMetrik.Before,
                result.ManhattanMetrik.After
            });
            table.Rows.Add(new object[]
            {
                "Количество пересечений", result.InterserctionsAmount.Before,
                result.InterserctionsAmount.After
            });
            table.Rows.Add(new object[]
            {
                "Суммарная площадь пересечений", result.AreaOfInterserctions.Before,
                result.AreaOfInterserctions.After
            });
            return table;
        }
    }
}
