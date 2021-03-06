using System.Collections.Generic;
using System.Globalization;

namespace TranslateMe.UI.Controls
{
    public interface IExportProvider
    {
        bool HasSelection { get; }
        IEnumerable<string[]> GetRows(CultureInfo[] cultures, bool selectionOnly);
    }
}