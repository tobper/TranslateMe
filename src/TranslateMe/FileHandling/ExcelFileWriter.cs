using System.Collections.Generic;
using System.IO;
using System.Windows;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace TranslateMe.FileHandling
{
    public class ExcelFileWriter
    {
        public void CreateFile(FileInfo file, IEnumerable<string[]> rows)
        {
            var package = new ExcelPackage(file);
            var worksheet = package.Workbook.Worksheets.Add("Translations");

            WriteContent(worksheet, rows);
            FormatHeaderRow(worksheet);
            AutoFitColumns(worksheet);
            AddConditionalFormatting(worksheet);
            FreezePanes(worksheet); 

            package.Save();
        }

        private static void FreezePanes(ExcelWorksheet worksheet)
        {
            worksheet.View.FreezePanes(2, 2);
        }

        private static void WriteContent(ExcelWorksheet worksheet, IEnumerable<string[]> rows)
        {
            worksheet.Cells["A1"].LoadFromArrays(rows);
        }

        private static void FormatHeaderRow(ExcelWorksheet worksheet)
        {
            var style = worksheet.Cells["1:1"].Style;
            var accentColor = Application.Current.GetResourceDrawingColor("TitleAccentColor");
            var backgroundColor = Application.Current.GetResourceDrawingColor("TitleBackgroundColor");
            
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(backgroundColor);
            style.Font.Color.SetColor(accentColor);
        }

        private static void AutoFitColumns(ExcelWorksheet worksheet)
        {
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private static void AddConditionalFormatting(ExcelWorksheet worksheet)
        {
            var address = new ExcelAddress(worksheet.Dimension.Address);
            var expression = worksheet.ConditionalFormatting.AddNotContainsText(address);

            expression.Style.Fill.BackgroundColor.Color = Application.Current.GetResourceDrawingColor("ErrorBackgroundColor");
            expression.Style.Fill.PatternType = ExcelFillStyle.Solid;
        }
    }
}