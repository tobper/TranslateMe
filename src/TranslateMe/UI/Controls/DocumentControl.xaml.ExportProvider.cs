using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TranslateMe.Model;

namespace TranslateMe.UI.Controls
{
    public partial class DocumentControl : IExportProvider
    {
        public bool HasSelection
        {
            get { return Grid.SelectedItems.Count > 0; }
        }

        public IEnumerable<CultureInfo> Cultures
        {
            get
            {
                return Grid.Columns.
                    OfType<DataGridCultureColumn>().
                    Select(c => c.Culture);
            }
        }

        public IEnumerable<string[]> GetRows(CultureInfo[] cultures, bool selectionOnly)
        {
            var phrases = (selectionOnly)
                ? Grid.SelectedItems
                : ((Document)DataContext).Phrases;

            // Return header row
            var headerRow = new string[cultures.Length + 1];
            headerRow[0] = Grid.Columns[0].Header.ToString();

            for (var i = 0; i < cultures.Count(); i++)
            {
                headerRow[i + 1] = cultures[i].GetName();
            }

            yield return headerRow;

            // Return content rows
            foreach (Phrase phrase in phrases)
            {
                var row = new string[cultures.Length + 1];
                row[0] = phrase.Name;

                for (var i = 0; i < cultures.Length; i++)
                {
                    var culture = cultures[i];
                    var translation = phrase.Translations[culture];

                    row[i + 1] = translation.Text;
                }

                yield return row;
            }
        }
    }
}
