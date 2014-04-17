using System.Collections.Generic;
using System.Linq;

namespace TranslateMe.Filtering
{
    public static class FilterExtensions
    {
        public static IEnumerable<string> GetFilterValues(this object source)
        {
            var valueProvider = source as IFilterValueProvider;
            if (valueProvider != null)
                return valueProvider.GetFilterValues();

            if (source == null)
                return new string[0];

            return new[]
            {
                source.ToString()
            };
        }

        public static bool Matches(this object source, string filterText)
        {
            return
                GetFilterValues(source).
                Any(filterValue => filterValue.Matches(filterText));
        }

        public static bool Matches(this string source, string filterText)
        {
            return
                !string.IsNullOrEmpty(source) &&
                !string.IsNullOrEmpty(filterText) &&
                source.ToLower().Contains(filterText.ToLower());
        }
    }
}
