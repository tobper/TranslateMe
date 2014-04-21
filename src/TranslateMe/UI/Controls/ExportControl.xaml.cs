using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using TranslateMe.FileHandling;

namespace TranslateMe.UI.Controls
{
    public partial class ExportControl : UserControl
    {
        public static readonly DependencyProperty ExportProviderProperty = DependencyProperty.Register(
            "ExportProvider",
            typeof(IExportProvider),
            typeof(ExportControl),
            new UIPropertyMetadata(null));

        private readonly ExcelFileWriter _excelFileWriter;

        public ExportControl()
        {
            _excelFileWriter = new ExcelFileWriter();

            InitializeComponent();
        }

        public IExportProvider ExportProvider
        {
            get { return (IExportProvider)GetValue(ExportProviderProperty); }
            set { SetValue(ExportProviderProperty, value); }
        }

        private void BindDataContext()
        {
            var hasSelection = ExportProvider.HasSelection;

            DataContext = new ExportViewModel
            {
                ScopeSelection = hasSelection,
                ScopeAll = !hasSelection,
                Languages = ExportProvider.Cultures.
                    Select(culture => new LanguageViewModel(culture)).
                    ToArray()
            };
        }

        private void CloseControl()
        {
            Visibility = Visibility.Collapsed;
        }

        private void ExportControl_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                BindDataContext();
        }

        private void CloseCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CloseControl();
        }

        private void ExportCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewModel = (ExportViewModel)DataContext;

            e.CanExecute =
                viewModel != null &&
                viewModel.Languages.Any(l => l.IsSelected);

        }

        private void ExportCommand_OnExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var file = new FileInfo(dialog.FileName);
                if (file.Exists)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception exception)
                    {
                        ExclamationBox.Show(exception.Message);
                        return;
                    }
                }

                var viewModel = (ExportViewModel)DataContext;
                var cultures = viewModel.Languages.
                    Where(l => l.IsSelected).
                    Select(l => l.Culture).
                    ToArray();

                var rows = ExportProvider.GetRows(cultures, viewModel.ScopeSelection);

                _excelFileWriter.CreateFile(file, rows);

                CloseControl();
            }
        }

        class ExportViewModel
        {
            public bool ScopeSelection { get; set; }
            public bool ScopeAll { get; set; }
            public LanguageViewModel[] Languages { get; set; }
        }

        class LanguageViewModel
        {
            public LanguageViewModel(CultureInfo culture)
            {
                var isInvariantCulture = culture.IsInvariantCulture();

                Culture = culture;
                Name = culture.GetName();
                IsSelected = isInvariantCulture;
                IsEnabled = !isInvariantCulture;
            }

            public CultureInfo Culture { get; private set; }
            public string Name { get; private set; }
            public bool IsSelected { get; set; }
            public bool IsEnabled { get; private set; }
        }
    }
}
