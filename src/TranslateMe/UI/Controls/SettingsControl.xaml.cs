using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TranslateMe.Properties;

namespace TranslateMe.UI.Controls
{
    public partial class SettingsControl : UserControl
    {
        public static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent(
            "Close",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SettingsControl));

        public SettingsControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Close
        {
            add { AddHandler(CloseEvent, value); }
            remove { RemoveHandler(CloseEvent, value); }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void CloseCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseEvent));
        }

        private static void SaveSettings()
        {
            Settings.Default.Save();
        }
    }
}
