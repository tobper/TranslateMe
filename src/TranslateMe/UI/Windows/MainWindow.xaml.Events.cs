using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TranslateMe.Properties;

namespace TranslateMe.UI.Windows
{
    public partial class MainWindow
    {
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && !IsDocumentOpen)
            {
                DisplayFileOpenDialog();
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DisplayFileOpenDialog();
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

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchBox.Focus();
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void EscapeKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Overlay.Opacity == 1d)
            {
                var storyBoard = (Storyboard)FindResource("HideOverlay");
                BeginStoryboard(storyBoard);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseDocument())
            {
                _windowLocationSaveMethod.Dispose();
                _windowSizeSaveMethod.Dispose();
                _windowStateSaveMethod.Dispose();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.AutoCheckForUpdates)
            {
                CheckForUpdates();
            }
        }

        private void Window_OnLocationChanged(object sender, EventArgs e)
        {
            _windowLocationSaveMethod.CallDelayed();
        }

        private void Window_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _windowSizeSaveMethod.CallDelayed(e.NewSize);
        }

        private void Window_OnStateChanged(object sender, EventArgs e)
        {
            _windowStateSaveMethod.CallDelayed();
        }

        private void Window_OnDrag(object sender, DragEventArgs e)
        {
            var isDataValid = false;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                var file = files[0];
                var ext = Path.GetExtension(file);

                if (ext == ".resx" || ext == ".tmd")
                {
                    isDataValid = true;
                }
            }

            e.Effects = isDataValid ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        private void Window_OnDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, true);

            if (files != null && files.Length > 0)
            {
                OpenFile(files[0]);
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