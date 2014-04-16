using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TranslateMe.Filtering
{
    public class FilterTextVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] converterValues, Type targetType, object parameter, CultureInfo culture)
        {
            var filterText = (string)converterValues[1];
            if (filterText.Length == 0)
                return Visibility.Visible;

            var filterValue = converterValues[0];
            var matches = filterValue.Matches(filterText);

            return matches ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}