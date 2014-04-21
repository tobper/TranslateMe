using System.Collections.Generic;
using System.Globalization;

namespace TranslateMe.UI.Controls
{
    public interface IExportProvider
    {
        IEnumerable<CultureInfo> Cultures { get; }
        bool HasSelection { get; }
        string DocumentName { get; }
        IEnumerable<string[]> GetRows(CultureInfo[] cultures, bool selectionOnly);
    }
}