using System.Windows;
using TranslateMe.Properties;

namespace TranslateMe.UI
{
    public static class QuestionBox
    {
        public static MessageBoxResult Show(string text, bool allowCancel = true)
        {
            var buttons = allowCancel
                ? MessageBoxButton.YesNoCancel
                : MessageBoxButton.YesNo;

            return MessageBox.Show(text, Strings.MessageBoxTitle, buttons, MessageBoxImage.Question);
        }
    }
}