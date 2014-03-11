using System.Windows;

namespace TranslateMe.UI
{
    public static class SearchProperties
    {
        public static readonly DependencyProperty SearchTermProperty = DependencyProperty.RegisterAttached(
            "SearchTerm",
            typeof(string),
            typeof(SearchProperties),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        /* Using a DependencyProperty as the backing store for IsMatch.  This enables animation, styling, binding, etc...*/
        public static readonly DependencyProperty IsMatchProperty = DependencyProperty.RegisterAttached(
            "IsMatch",
            typeof(bool),
            typeof(SearchProperties),
            new UIPropertyMetadata(false));

        public static string GetSearchTerm(DependencyObject obj)
        {
            return (string)obj.GetValue(SearchTermProperty);
        }

        public static void SetSearchTerm(DependencyObject obj, string value)
        {
            obj.SetValue(SearchTermProperty, value);
        }

        public static bool GetIsMatch(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMatchProperty);
        }

        public static void SetIsMatch(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMatchProperty, value);
        }
    }
}