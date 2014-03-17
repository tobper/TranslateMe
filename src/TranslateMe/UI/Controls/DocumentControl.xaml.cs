using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TranslateMe.Model;

namespace TranslateMe.UI.Controls
{
    public partial class DocumentControl : UserControl
    {
        public DocumentControl()
        {
            InitializeComponent();
        }

        private void Grid_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RemoveColumns(Grid.Columns);

            if (e.OldValue != null)
            {
                var document = (Document)e.OldValue;

                document.Cultures.CollectionChanged -= CulturesOnCollectionChanged;
            }

            if (e.NewValue != null)
            {
                var document = (Document)e.NewValue;

                foreach (var culture in document.Cultures)
                {
                    AddColumn(culture);
                }

                document.Cultures.CollectionChanged += CulturesOnCollectionChanged;
            }
        }

        private void CulturesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            foreach (CultureInfo culture in notifyCollectionChangedEventArgs.NewItems)
            {
                AddColumn(culture);
            }
        }

        private void AddColumn(CultureInfo culture)
        {
            var column = new DataGridTextColumn
            {
                Header = culture.DisplayName,
                Binding = new Binding("Translations[" + culture.Name + "].Text"),
                EditingElementStyle = (Style)FindResource("DataGridTextBoxStyle"),
                MinWidth = 100,
                Width = 175
            };

            if (culture.Equals(CultureInfo.InvariantCulture))
            {
                column.Header = "<Default>";
                Grid.Columns.Insert(1, column);
            }
            else
            {
                Grid.Columns.Add(column);
            }
        }

        private static void RemoveColumns(IList columns)
        {
            for (var i = columns.Count - 1; i >= 1; i--)
            {
                columns.RemoveAt(i);
            }
        }
    }
}
