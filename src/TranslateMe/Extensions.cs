using System;
using System.Collections.ObjectModel;
using System.Configuration;
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

        public static void Update<T>(this T settings, Action<T> action)
            where T : SettingsBase
        {
            action(settings);
            settings.Save();
        }
    }
}