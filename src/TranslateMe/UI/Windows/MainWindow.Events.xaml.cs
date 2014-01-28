using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using TranslateMe.Properties;

namespace TranslateMe.UI.Windows
{
    public partial class MainWindow
    {
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "TranslateMe files (*.tmd, *.resx)|*.tmd;*.resx|Translation files (*.tmd)|*.tme|Resource files (*.resx)|*.resx|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var fileExtension = Path.GetExtension(dialog.FileName);

                switch (fileExtension)
                {
                    case ".resx":
                        OpenResourceFile(dialog.FileName);
                        break;

                    case ".tmd":
                        OpenDocumentFile(dialog.FileName);
                        break;

                    default:
                        DisplayFileFormatWarning();
                        break;
                }
            }
        }

        private void DocumentCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsDocumentOpen;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDocument();
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDocumentAs();
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseDocument();
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Overlay.Opacity < 1d)
            {
                Application.Current.Shutdown();
            }
            else
            {
                var storyBoard = (Storyboard)FindResource("HideOverlay");
                BeginStoryboard(storyBoard);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CloseDocument();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.AutoCheckForUpdates)
            {
                CheckForUpdates();
            }
        }

        private void GenerateResources_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GenerateResources();
        }

        private void ReloadResources_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
    }
}