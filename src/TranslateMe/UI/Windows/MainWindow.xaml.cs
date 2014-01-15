using System;
using System.IO;
using System.Linq;
using System.Windows;
using TranslateMe.FileHandling;
using TranslateMe.Model;
using TranslateMe.Properties;

namespace TranslateMe.UI.Windows
{
    public partial class MainWindow : Window
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

        private readonly DocumentFileReader _documentFileReader;
        private readonly DocumentFileWriter _documentFileWriter;
        private readonly ResourceFileReader _resourceFileReader;

        public MainWindow()
        {
            _documentFileReader = new DocumentFileReader();
            _documentFileWriter = new DocumentFileWriter();
            _resourceFileReader = new ResourceFileReader();

            InitializeComponent();
            UpdateTitle();
        }

        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set
            {
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
                if (value != IsDocumentModified)
                {
                    SetValue(IsDocumentModifiedPropertyKey, value);
                    UpdateTitle();
                }
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

        private void OpenResourceFile(string fileName)
        {
            var workingDirectory = Path.GetDirectoryName(fileName);
            var documentName = _resourceFileReader.GetName(fileName);

            if (IsDocumentOpen)
            {
                var resourceMatchesDocument = string.Equals(Document.Name, documentName, StringComparison.CurrentCultureIgnoreCase);
                if (resourceMatchesDocument)
                {
                    LoadResourcesFile(fileName);
                }
                else
                {
                    var appendConfirmation = MessageBox.Show(
                        "Resource file name does not match loaded document. Close current document and load resource in a new document?",
                        Strings.MessageBoxTitle,
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    switch (appendConfirmation)
                    {
                        case MessageBoxResult.Yes:
                            if (!CloseDocument())
                                return;

                            CreateNewDocument(workingDirectory, documentName);
                            return;

                        case MessageBoxResult.No:
                            LoadResourcesFile(fileName);
                            return;

                        default:
                            return;
                    }
                }
            }
            else
            {
                CreateNewDocument(workingDirectory, documentName);
            }
        }

        private void OpenDocumentFile(string fileName)
        {
            if (IsDocumentOpen && !CloseDocument())
                return;

            Document = _documentFileReader.LoadDocument(fileName);
        }

        private void LoadResourcesFile(string fileName)
        {
            _resourceFileReader.LoadResource(Document, fileName);
            IsDocumentModified = true;
        }

        private void CreateNewDocument(string directory, string documentName)
        {
            Document = new Document(directory, documentName);

            var resourceFiles =
                from fileName in Directory.GetFiles(directory, Document.Name + "*.resx")
                let resourceName = _resourceFileReader.GetName(fileName)
                where string.Equals(resourceName, Document.Name, StringComparison.CurrentCultureIgnoreCase)
                select fileName;

            foreach (var resourceFile in resourceFiles)
            {
                LoadResourcesFile(resourceFile);
            }
        }

        private static void DisplayFileFormatWarning()
        {
            MessageBox.Show(
                "Unknown file format.",
                Strings.MessageBoxTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }

        private void SaveDocument()
        {
            _documentFileWriter.Write(Document);

            IsDocumentModified = false;
        }

        private void SaveDocumentAs()
        {
            throw new NotImplementedException();

            SaveDocument();
        }

        /// <summary>
        /// Closed the current document.
        /// </summary>
        /// <returns>True if the document was closed, otherwise false.</returns>
        private bool CloseDocument()
        {
            if (IsDocumentOpen)
            {
                if (IsDocumentModified)
                {
                    var closeConfirmation = MessageBox.Show(
                        "Save changes to current document?",
                        Strings.MessageBoxTitle,
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

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
    }
}
