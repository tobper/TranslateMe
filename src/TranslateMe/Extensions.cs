using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TranslateMe.Properties;

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

        public static Color GetResourceColor(this Application app, string resourceName)
        {
            return (Color)app.Resources[resourceName];
        }

        public static System.Drawing.Color GetResourceDrawingColor(this Application app, string resourceName)
        {
            var color = GetResourceColor(app, resourceName);

            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static string GetName(this CultureInfo cultureInfo)
        {
            return cultureInfo.IsInvariantCulture()
                       ? Strings.InvariantLanguageName
                       : cultureInfo.DisplayName;
        }

        public static bool IsInvariantCulture(this CultureInfo cultureInfo)
        {
            return cultureInfo.Equals(CultureInfo.InvariantCulture);
        }
    }
}