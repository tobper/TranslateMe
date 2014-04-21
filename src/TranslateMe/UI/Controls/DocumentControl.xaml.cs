using System.Collections.Generic;
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
        private Document _document;

        public DocumentControl()
        {
            InitializeComponent();
        }

        private void DocumentControl_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetColumns(Grid.Columns);

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

            _document = (Document)e.NewValue;
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
            var column = new DataGridCultureColumn(culture)
            {
                Binding = new Binding("Translations[" + culture.Name + "].Text"),
                EditingElementStyle = (Style)FindResource("DataGridTextBoxStyle"),
                MinWidth = 100,
                Width = 175
            };

            if (culture.Equals(CultureInfo.InvariantCulture))
            {
                Grid.Columns.Insert(1, column);
            }
            else
            {
                Grid.Columns.Add(column);
            }
        }

        private static void ResetColumns(IList<DataGridColumn> columns)
        {
            for (var i = columns.Count - 1; i >= 1; i--)
            {
                columns.RemoveAt(i);
            }

            columns[0].Width = DataGridLength.Auto;
        }
    }
}
