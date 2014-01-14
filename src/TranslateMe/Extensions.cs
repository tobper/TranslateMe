using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TranslateMe
{
    public static class Extensions
    {
        public static void Remove<T>(this ObservableCollection<T> items, Func<T, bool> selector)
            where T : class
        {
            var item = items.FirstOrDefault(selector);
            if (item != null)
            {
                items.Remove(item);
            }
        }
    }
}