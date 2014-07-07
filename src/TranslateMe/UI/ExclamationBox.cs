using System.Windows;
using TranslateMe.Properties;

namespace TranslateMe.UI
{
    public static class ExclamationBox
    {
        public static void Show(string text)
        {
            MessageBox.Show(text, Strings.MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}