using System;
using System.Globalization;
using System.Windows.Data;

namespace TranslateMe.Filtering
{
    public class FilterTextBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] converterValues, Type targetType, object parameter, CultureInfo culture)
        {
            var filterText = (string)converterValues[1];
            if (filterText.Length == 0)
                return false;

            var filterValue = converterValues[0];
            var matches = filterValue.Matches(filterText);

            return matches;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}