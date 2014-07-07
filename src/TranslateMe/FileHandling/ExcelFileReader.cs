using System;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using TranslateMe.Model;
using TranslateMe.Properties;

namespace TranslateMe.FileHandling
{
    class ExcelFileReader : IResourceFileReader
    {
        public string GetDocumentName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        public void LoadResources(Document document, string fileName)
        {
            var package = OpenPackage(fileName);
            if (package.Workbook.Worksheets.Count == 0)
                throw new FileLoadException(Strings.ExcelFileMissingWorksheets);

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
        }

        private static ExcelPackage OpenPackage(string fileName)
        {
            var file = new FileInfo(fileName);

            try
            {
                return new ExcelPackage(file);
            }
            catch (IOException e)
            {
                var message = string.Format(
                    "{0}{1}{1}{2}",
                    Strings.ExcelFileLoadFailed,
                    Environment.NewLine,
                    e.Message);

                throw new FileLoadException(message, e);
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