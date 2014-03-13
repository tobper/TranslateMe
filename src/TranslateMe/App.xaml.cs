using System;
using System.Windows;
using System.Windows.Threading;
using TranslateMe.Properties;

namespace TranslateMe
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is NotImplementedException)
            {
                MessageBox.Show(
                    "The requested feature has unfortunately not yet been implemented.",
                    Strings.MessageBoxTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                e.Handled = true;
            }
        }
    }
}
