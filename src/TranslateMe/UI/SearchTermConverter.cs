using System;
using System.Globalization;
using System.Windows.Data;

namespace TranslateMe.UI
{
    public class SearchTermConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = GetStringValue(values);
            var searchTerm = GetSearchTerm(values);

            return !string.IsNullOrEmpty(searchTerm) &&
                   !string.IsNullOrEmpty(stringValue) &&
                   stringValue.ToLower().Contains(searchTerm.ToLower());
        }

        private static string GetStringValue(object[] values)
        {
            var value = values[0];
            if (value == null)
                return string.Empty;

            var text = value as string;
            if (text != null)
                return text;

            return value.ToString();
        }

        private static string GetSearchTerm(object[] values)
        {
            return (string)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}