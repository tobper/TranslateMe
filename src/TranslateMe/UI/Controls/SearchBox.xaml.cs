using System.Windows;
using System.Windows.Controls;

namespace TranslateMe.UI.Controls
{
    public partial class SearchBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(SearchBox),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyPropertyKey FilterTextPropertyKey = DependencyProperty.RegisterReadOnly(
            "FilterText",
            typeof(string),
            typeof(SearchBox),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty FilterTextProperty = FilterTextPropertyKey.DependencyProperty;

        public SearchBox()
        {
            InitializeComponent();
            TextBox.IsKeyboardFocusedChanged += (s, e) => UpdateFilterText();
            TextBox.TextChanged += (s, e) => UpdateFilterText();
        }

        private void UpdateFilterText()
        {
            var value = (TextBox.IsKeyboardFocused) ? Text : string.Empty;

            SetValue(FilterTextPropertyKey, value);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
        }

        private void SearchBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
        }
    }
}
