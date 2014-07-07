using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using TranslateMe.Model;
using TranslateMe.Properties;

namespace TranslateMe.FileHandling
{
    class ResXFileReader : IResourceFileReader
    {
        public string GetDocumentName(string fileName)
        {
            var name = Path.GetFileName(fileName);
            if (name != null)
            {
                var match = Regex.Match(name, "(?<name>.*?)(\\.([a-z]{2}(-[a-z]{2})?))?\\.resx$");
                if (match.Success)
                {
                    return match.Groups["name"].Value;
                }
            }

            return null;
        }

        public void LoadResources(Document document, string fileName)
        {
            var culture = GetCulture(fileName);

            try
            {
                using (var reader = new ResXResourceReader(fileName))
                {
                    foreach (DictionaryEntry text in reader)
                    {
                        var name = (string)text.Key;
                        var value = (string)text.Value;

                        document[name, culture] = value;
                    }
                }
            }
            catch (Exception e)
            {
                var xmlException = e.InnerException as XmlException;
                if (xmlException != null)
                {
                    var message = string.Format(
                        "{0}{1}{1}{2}",
                        Strings.ResXFileLoadFailed,
                        Environment.NewLine,
                        e.Message);

                    throw new FileLoadException(message, e);
                }

                throw;
            }
        }

        private static CultureInfo GetCulture(string fileName)
        {
            var match = Regex.Match(fileName, "(\\.(?<culture>[a-z]{2}(-[a-z]{2})?))\\.resx$");
            if (match.Success)
            {
                var cultureName = match.Groups["culture"].Value;

                return new CultureInfo(cultureName);
            }

            return CultureInfo.InvariantCulture;
        }
    }
}