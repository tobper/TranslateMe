using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    class ResourceFileWriter
    {
        public void SaveResources(Document document)
        {
            foreach (var culture in document.Cultures)
            {
                SaveResource(document, culture);
            }
        }

        private static void SaveResource(Document document, CultureInfo culture)
        {
            var fileName = GetResourceFileName(document, culture);
            var texts = from phrase in document.Phrases
                        let value = phrase.Translations[culture].Text
                        where !string.IsNullOrEmpty(value)
                        orderby phrase.Name
                        select new
                        {
                            phrase.Name,
                            Value = value
                        };

            using (var writer = new ResXResourceWriter(fileName))
            {
                foreach (var text in texts)
                {
                    writer.AddResource(text.Name, text.Value);
                }

                writer.Generate();
            }
        }

        private static string GetResourceFileName(Document document, CultureInfo culture)
        {
            var fileName = Path.Combine(document.Directory, document.Name);

            if (!culture.Equals(CultureInfo.InvariantCulture))
            {
                fileName += "." + culture.Name;
            }

            return fileName + ".resx";
        }
    }
}