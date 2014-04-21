using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using TranslateMe.Model;
using TranslateMe.Properties;

namespace TranslateMe.FileHandling
{
    class ExcelFileReader
    {
        public bool LoadResources(Document document, string fileName)
        {
            var file = new FileInfo(fileName);
            if (file.Exists == false)
                return false;

            var package = OpenPackage(file);
            if (package == null)
                return false;

            if (package.Workbook.Worksheets.Count == 0)
            {
                ExclamationBox.Show("Excel file does not contain any worksheets.");
                return false;
            }

            var worksheet = package.Workbook.Worksheets.First();

            for (var
                columnIndex = worksheet.Dimension.Start.Column + 1;
                columnIndex <= worksheet.Dimension.End.Column;
                columnIndex++)
            {
                var cultureName = worksheet.Cells[1, columnIndex].Text;
                var culture = (cultureName == CultureInfo.InvariantCulture.GetName())
                                  ? CultureInfo.InvariantCulture
                                  : GetCulture(cultureName);

                if (culture == null)
                {
                    continue;
                }

                for (var
                    rowIndex = worksheet.Dimension.Start.Row + 1;
                    rowIndex <= worksheet.Dimension.End.Row;
                    rowIndex++)
                {
                    var name = worksheet.Cells[rowIndex, 1].Text;
                    var value = worksheet.Cells[rowIndex, columnIndex].Text;

                    document[name, culture] = value;
                }
            }

            return true;
        }

        private static ExcelPackage OpenPackage(FileInfo file)
        {
            try
            {
                return new ExcelPackage(file);
            }
            catch (IOException e)
            {
                ExclamationBox.Show(e.Message);
                return null;
            }
        }

        private static CultureInfo GetCulture(string cultureDisplayName)
        {
            return CultureInfo.
                GetCultures(CultureTypes.AllCultures).
                FirstOrDefault(c => c.DisplayName == cultureDisplayName);
        }
    }
}