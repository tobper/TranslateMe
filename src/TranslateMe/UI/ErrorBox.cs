using System.Windows;
using TranslateMe.Properties;

namespace TranslateMe.UI
{
    public static class ErrorBox
    {
        public static void Show(string text)
        {
            MessageBox.Show(text, Strings.MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}