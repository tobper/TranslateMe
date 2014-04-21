using System;
using System.Windows;
using System.Windows.Threading;

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
                ExclamationBox.Show("The requested feature has unfortunately not yet been implemented.");

                e.Handled = true;
            }
        }
    }
}
