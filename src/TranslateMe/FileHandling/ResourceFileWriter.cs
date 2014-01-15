using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Xml.Linq;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    class ResourceFileWriter
    {
        private static readonly XNamespace XmlNamespace = "http://www.w3.org/XML/1998/namespace";

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
                        orderby phrase.Name
                        select new
                        {
                            phrase.Name,
                            Value = phrase[culture].Text
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