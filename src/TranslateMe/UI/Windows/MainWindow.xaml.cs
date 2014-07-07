using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using TranslateMe.Commands;
using TranslateMe.FileHandling;
using TranslateMe.Model;
using TranslateMe.Properties;
using FileLoadException = TranslateMe.FileHandling.FileLoadException;

namespace TranslateMe.UI.Windows
{
    public partial class MainWindow
    {
        private static readonly DependencyPropertyKey IsDocumentModifiedPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsDocumentModified",
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(false));

        private static readonly DependencyPropertyKey IsDocumentOpenPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsDocumentOpen",
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(false));

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            "Document",
            typeof(Document),
            typeof(MainWindow),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsDocumentModifiedProperty = IsDocumentModifiedPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsDocumentOpenProperty = IsDocumentOpenPropertyKey.DependencyProperty;

        private readonly IMethodThrottle _windowLocationSaveMethod;
        private readonly IMethodThrottle<Size> _windowSizeSaveMethod;
        private readonly IMethodThrottle _windowStateSaveMethod;
        private readonly DocumentFileReader _documentFileReader;
        private readonly DocumentFileWriter _documentFileWriter;
        private readonly ExcelFileReader _excelFileReader;
        private readonly ResXFileReader _resXFileReader;
        private readonly ResXFileWriter _resXFileWriter;

        public MainWindow()
        {
            var windowThrottleInterval = TimeSpan.FromSeconds(1);

            _windowLocationSaveMethod = new MethodThrottle(SaveWindowLocation, windowThrottleInterval);
            _windowSizeSaveMethod = new MethodThrottle<Size>(SaveWindowSize, windowThrottleInterval);
            _windowStateSaveMethod = new MethodThrottle(SaveWindowState, windowThrottleInterval);

            _documentFileReader = new DocumentFileReader();
            _documentFileWriter = new DocumentFileWriter();
            _excelFileReader = new ExcelFileReader();
            _resXFileReader = new ResXFileReader();
            _resXFileWriter = new ResXFileWriter();

            InitializeComponent();
            SetupInitialWindow();
            LoadCommandLineFile();
            UpdateTitle();
        }

        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set
            {
                if (value == Document)
                    return;

                SetValue(DocumentProperty, value);
                SetValue(IsDocumentModifiedPropertyKey, false);
                SetValue(IsDocumentOpenPropertyKey, value != null);
                UpdateTitle();
            }
        }

        public bool IsDocumentModified
        {
            get { return (bool)GetValue(IsDocumentModifiedProperty); }
            private set
            {
                if (value == IsDocumentModified)
                    return;

                SetValue(IsDocumentModifiedPropertyKey, value);
                UpdateTitle();
            }
        }

        public bool IsDocumentOpen
        {
            get { return (bool)GetValue(IsDocumentOpenProperty); }
        }

        private void UpdateTitle()
        {
            Title = Strings.ApplicationName;

            if (Document != null)
            {
                Title += " - " + Document.Name;

                if (IsDocumentModified)
                {
                    Title += " *";
                }
            }
        }

        private void DisplayFileOpenDialog()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter =
                    "TranslateMe files (*.tmd, *.resx, *.xlsx)|*.tmd;*.resx;*.xlsx|" +
                    "Translation files (*.tmd)|*.tme|" +
                    "Resource files (*.resx)|*.resx|" +
                    "Excel files|*.xlsx|" +
                    "All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                OpenFiles(dialog.FileNames);
            }
        }

        private void OpenFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                OpenFile(file);
            }
        }

        private void OpenFile(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);

            try
            {
                switch (fileExtension)
                {
                    case ".tmd":
                        OpenDocumentFile(fileName);
                        break;

                    case ".resx":
                        ImportResources(fileName, _resXFileReader);
                        break;

                    case ".xlsx":
                        ImportResources(fileName, _excelFileReader);
                        break;

                    default:
                        DisplayFileFormatWarning(fileName);
                        break;
                }
            }
            catch (FileLoadException e)
            {
                ErrorBox.Show(e.Message);
            }
        }

        private void OpenDocumentFile(string fileName)
        {
            if (IsDocumentOpen && !CloseDocument())
                return;

            Document = _documentFileReader.LoadDocument(fileName);
        }

        private void ImportResources(string fileName, IResourceFileReader resourceFileReader)
        {
            var workingDirectory = Path.GetDirectoryName(fileName);
            var documentName = resourceFileReader.GetDocumentName(fileName);

            if (IsDocumentOpen)
            {
                var fileMatchesDocument = string.Equals(Document.Name, documentName, StringComparison.CurrentCultureIgnoreCase);
                if (fileMatchesDocument == false)
                {
                    var closeConfirmation = QuestionBox.Show(Strings.ResourceFileLoad_CloseConfirmation);

                    switch (closeConfirmation)
                    {
                        case MessageBoxResult.Yes:
                            // Load resources in a new document
                            if (CloseDocument())
                                break;

                            // User chose not to close current document -> cancel
                            return;

                        case MessageBoxResult.No:
                            // Load resources in opened document
                            break;

                        case MessageBoxResult.Cancel:
                            // Do not load resources
                            return;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            var document = Document ?? CreateNewDocument(workingDirectory, documentName);

            resourceFileReader.LoadResources(document, fileName);

            Document = document;
            IsDocumentModified = true;
        }

        private static Document CreateNewDocument(string directory, string documentName)
        {
            return new Document(directory, documentName);
        }

        private static void DisplayFileFormatWarning(string fileName)
        {
            var message = string.Format(Strings.FileLoad_InvalidFileFormat, fileName);

            ExclamationBox.Show(message);
        }

        private void SaveDocument()
        {
            _documentFileWriter.SaveDocument(Document);

            IsDocumentModified = false;

            if (Settings.Default.GenerateResourcesOnSave)
            {
                GenerateResources();
            }
        }

        private void SaveDocumentAs()
        {
            throw new NotImplementedException();

            SaveDocument();
        }

        /// <summary>
        /// Closes the current document.
        /// </summary>
        /// <returns>True if the document was closed, otherwise false.</returns>
        public bool CloseDocument()
        {
            if (IsDocumentOpen)
            {
                if (IsDocumentModified)
                {
                    var closeConfirmation = QuestionBox.Show("Save changes to current document?");

                    switch (closeConfirmation)
                    {
                        case MessageBoxResult.Yes:
                            SaveDocument();
                            break;

                        case MessageBoxResult.Cancel:
                            return false;
                    }
                }

                Document = null;
            }

            return true;
        }

        private void GenerateResources()
        {
            _resXFileWriter.SaveResources(Document);
        }

        private void CheckForUpdates()
        {
            var command = new CheckForUpdatesCommand();
            if (command.CanExecute(this))
                command.Execute(this);
        }

        private void SetupInitialWindow()
        {
            if (Settings.Default.WindowLeft != null)
                Left = Settings.Default.WindowLeft.Value;

            if (Settings.Default.WindowTop != null)
                Top = Settings.Default.WindowTop.Value;

            Width = Settings.Default.WindowSize.Width;
            Height = Settings.Default.WindowSize.Height;
            WindowState = Settings.Default.WindowState;
        }

        private void LoadCommandLineFile()
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 1; i < args.Length; i++)
            {
                OpenFile(args[i]);
            }
        }

        private void SaveWindowLocation()
        {
            Settings.Default.Update(s =>
            {
                s.WindowLeft = Left;
                s.WindowTop = Top;
            });
        }

        private static void SaveWindowSize(Size newSize)
        {
            Settings.Default.Update(s =>
            {
                s.WindowSize = newSize;
            });
        }

        private void SaveWindowState()
        {
            var state = WindowState;
            if (state == WindowState.Minimized)
                state = WindowState.Normal;

            Settings.Default.Update(s =>
            {
                s.WindowState = state;
            });
        }
    }
}
