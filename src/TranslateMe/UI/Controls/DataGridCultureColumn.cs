using System.Globalization;
using System.Windows.Controls;

namespace TranslateMe.UI.Controls
{
    public class DataGridCultureColumn : DataGridTextColumn
    {
        public DataGridCultureColumn(CultureInfo culture)
        {
            Culture = culture;
            Header = culture.GetName();
        }

        public CultureInfo Culture { get; private set; }
    }
}