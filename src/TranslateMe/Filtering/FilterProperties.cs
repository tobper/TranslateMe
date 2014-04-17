using System.Windows;

namespace TranslateMe.Filtering
{
    public static class FilterProperties
    {
        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.RegisterAttached(
            "FilterText",
            typeof(string),
            typeof(FilterProperties),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        /* Using a DependencyProperty as the backing store for IsMatch.  This enables animation, styling, binding, etc...*/
        public static readonly DependencyProperty IsMatchProperty = DependencyProperty.RegisterAttached(
            "IsMatch",
            typeof(bool),
            typeof(FilterProperties),
            new UIPropertyMetadata(false));

        public static string GetFilterText(DependencyObject obj)
        {
            return (string)obj.GetValue(FilterTextProperty);
        }

        public static void SetFilterText(DependencyObject obj, string value)
        {
            obj.SetValue(FilterTextProperty, value);
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