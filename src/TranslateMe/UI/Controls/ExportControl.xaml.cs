using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using TranslateMe.FileHandling;
using TranslateMe.Model;

namespace TranslateMe.UI.Controls
{
    public partial class ExportControl : UserControl
    {
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document",
            typeof(Document),
            typeof(ExportControl),
            new UIPropertyMetadata(Document_PropertyChanged));

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

        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public IExportProvider ExportProvider
        {
            get { return (IExportProvider)GetValue(ExportProviderProperty); }
            set { SetValue(ExportProviderProperty, value); }
        }

        private void ResetDataContext()
        {
            var viewModel = (IDisposable)DataContext;
            if (viewModel != null)
                viewModel.Dispose();
        }

        private void BindDataContext(Document document)
        {
            var hasSelection = ExportProvider.HasSelection;
            var viewModel = new ExportViewModel(hasSelection, document.Cultures);

            DataContext = viewModel;
        }

        private void CloseControl()
        {
            Visibility = Visibility.Collapsed;
        }

        private static void Document_PropertyChanged(DependencyObject dependency, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (ExportControl)dependency;

            control.ResetDataContext();

            var newDocument = (Document)eventArgs.NewValue;
            if (newDocument != null)
            {
                control.BindDataContext(newDocument);
            }
        }

        private void CloseCommand_OnExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            CloseControl();
        }

        private void ExportCommand_CanExecute(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            var viewModel = (ExportViewModel)DataContext;

            eventArgs.CanExecute =
                viewModel != null &&
                viewModel.Languages.Any(l => l.IsSelected);
        }

        private void ExportCommand_OnExecuted(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var dialog = new SaveFileDialog
            {
                FileName = Document.Name + ".xlsx",
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
    }
}
