using System.Globalization;
using System.Windows;

namespace TranslateMe.Model
{
    public class Translation : DependencyObject
    {
        private static readonly DependencyPropertyKey CulturePropertyKey = DependencyProperty.RegisterReadOnly(
            "Culture",
            typeof(CultureInfo),
            typeof(Translation),
            new PropertyMetadata(default(CultureInfo)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(Translation),
            new PropertyMetadata(""));

        public static readonly DependencyProperty CultureProperty = CulturePropertyKey.DependencyProperty;

        public Translation(CultureInfo culture)
        {
            Culture = culture;
        }

        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            private set { SetValue(CulturePropertyKey, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}