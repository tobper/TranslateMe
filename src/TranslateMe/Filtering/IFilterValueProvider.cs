using System.Collections.Generic;

namespace TranslateMe.Filtering
{
    public interface IFilterValueProvider
    {
        IEnumerable<string> GetFilterValues();
    }
}