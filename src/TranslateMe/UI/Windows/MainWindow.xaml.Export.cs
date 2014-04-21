using System.Windows;
using System.Windows.Input;

namespace TranslateMe.UI.Windows
{
    public partial class MainWindow
    {
        private void OpenExportPanelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =
                IsDocumentOpen &&
                ExportPanel.Visibility != Visibility.Visible;
        }

        private void OpenExportPanelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExportPanel.Visibility = Visibility.Visible;
        }
    }
}
