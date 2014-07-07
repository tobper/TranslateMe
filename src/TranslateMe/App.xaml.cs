using System;
using System.Windows.Threading;
using TranslateMe.UI;

namespace TranslateMe
{
    public partial class App
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
