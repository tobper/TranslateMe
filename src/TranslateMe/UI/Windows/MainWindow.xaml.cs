﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Win32;
using TranslateMe.Commands;
using TranslateMe.FileHandling;
using TranslateMe.Model;
using TranslateMe.Properties;

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
        private readonly ResourceFileReader _resourceFileReader;
        private readonly ResourceFileWriter _resourceFileWriter;

        public MainWindow()
        {
            var windowThrottleInterval = TimeSpan.FromSeconds(1);

            _windowLocationSaveMethod = new MethodThrottle(SaveWindowLocation, windowThrottleInterval);
            _windowSizeSaveMethod = new MethodThrottle<Size>(SaveWindowSize, windowThrottleInterval);
            _windowStateSaveMethod = new MethodThrottle(SaveWindowState, windowThrottleInterval);

            _documentFileReader = new DocumentFileReader();
            _documentFileWriter = new DocumentFileWriter();
            _resourceFileReader = new ResourceFileReader();
            _resourceFileWriter = new ResourceFileWriter();

            InitializeComponent();
            SetupInitialWindow();
            LoadCommandLineFile();
            UpdateTitle();

            Window.SearchBox.OnTextChanged += SearchBox_OnTextChanged;
        }

        public Document VisualDocument
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

        public Document MainDocument;

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

            if (VisualDocument != null)
            {
                Title += " - " + VisualDocument.Name;

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
                Filter = "TranslateMe files (*.tmd, *.resx)|*.tmd;*.resx|Translation files (*.tmd)|*.tme|Resource files (*.resx)|*.resx|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                OpenFile(dialog.FileName);
            }
        }

        private void OpenFile(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);

            switch (fileExtension)
            {
                case ".resx":
                    OpenResourceFile(fileName);
                    break;

                case ".tmd":
                    OpenDocumentFile(fileName);
                    break;

                default:
                    DisplayFileFormatWarning();
                    break;
            }

            // create a Backup of phrases
            MainDocument = new Document(VisualDocument.Directory, VisualDocument.Name);
            foreach (var culture in VisualDocument.Cultures)
            {
                MainDocument.Cultures.Add(culture);
            }

            foreach (var phrase in VisualDocument.Phrases)
            {
                MainDocument.Phrases.Add(phrase);
            }

            VisualDocument.Phrases.CollectionChanged += PhrasesOnCollectionChanged;
        }       

        private void OpenResourceFile(string fileName)
        {
            var workingDirectory = Path.GetDirectoryName(fileName);
            var documentName = _resourceFileReader.GetName(fileName);

            if (IsDocumentOpen)
            {
                var resourceMatchesDocument = string.Equals(VisualDocument.Name, documentName, StringComparison.CurrentCultureIgnoreCase);
                if (resourceMatchesDocument)
                {
                    LoadResourcesFile(fileName);
                }
                else
                {
                    var appendConfirmation = MessageBox.Show(
                        "Resource file name does not match loaded document." + Environment.NewLine + "Close current document and load resource in a new document?",
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
                IsDocumentModified = false;
            }
        }

        private void OpenDocumentFile(string fileName)
        {
            if (IsDocumentOpen && !CloseDocument())
                return;

            VisualDocument = _documentFileReader.LoadDocument(fileName);
        }

        private void LoadResourcesFile(string fileName)
        {
            _resourceFileReader.LoadResource(VisualDocument, fileName);
            
            IsDocumentModified = true;
        }

        private void CreateNewDocument(string directory, string documentName)
        {
            VisualDocument = new Document(directory, documentName);

            var resourceFiles =
                from fileName in Directory.GetFiles(directory, VisualDocument.Name + "*.resx")
                let resourceName = _resourceFileReader.GetName(fileName)
                where string.Equals(resourceName, VisualDocument.Name, StringComparison.CurrentCultureIgnoreCase)
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
            _documentFileWriter.SaveDocument(VisualDocument);

            IsDocumentModified = false;

            if (Settings.Default.GenerateResourcesOnSave)
            {
                GenerateResources();
            }
        }

        private void SaveDocumentAs()
        {
            throw new NotImplementedException();
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

                VisualDocument = null;
            }

            return true;
        }

        private void GenerateResources()
        {
            _resourceFileWriter.SaveResources(MainDocument);
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
            if (args.Length > 1)
            {
                OpenFile(args[1]);
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

        private void SearchBox_OnTextChanged(object sender, string text)
        {
            if (text == string.Empty)
                VisualDocument.RestorePrases(MainDocument);
            else
                VisualDocument.UpdatePhrases(MainDocument, text);

            IsDocumentModified = true;
        }
    }
}
