using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TranslateMe.UI.Controls
{
    public partial class SearchBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(SearchBox),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyPropertyKey SearchTermPropertyKey = DependencyProperty.RegisterReadOnly(
            "SearchTerm",
            typeof(string),
            typeof(SearchBox),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty SearchTermProperty = SearchTermPropertyKey.DependencyProperty;

        public SearchBox()
        {
            InitializeComponent();
            TextBox.IsKeyboardFocusedChanged += (s, e) => UpdateSearchTerm();
            TextBox.TextChanged += (s, e) => UpdateSearchTerm();
        }

        private void UpdateSearchTerm()
        {
            var value = (TextBox.IsKeyboardFocused) ? Text : string.Empty;

            SetValue(SearchTermPropertyKey, value);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string SearchTerm
        {
            get { return (string)GetValue(SearchTermProperty); }
        }

        private void SearchBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}
