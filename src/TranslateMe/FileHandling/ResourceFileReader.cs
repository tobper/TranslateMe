using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    class ResourceFileReader
    {
        public CultureInfo GetCulture(string fileName)
        {
            var match = Regex.Match(fileName, "(\\.(?<culture>[a-z]{2}(-[a-z]{2})?))\\.resx$");
            if (match.Success)
            {
                var cultureName = match.Groups["culture"].Value;

                return new CultureInfo(cultureName);
            }

            return CultureInfo.InvariantCulture;
        }

        public string GetName(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var match = Regex.Match(fileInfo.Name, "(?<name>.*?)(\\.([a-z]{2}(-[a-z]{2})?))?\\.resx$");

            return match.Success
                       ? match.Groups["name"].Value
                       : null;
        }

        public void LoadResource(Document document, string fileName)
        {
            var culture = GetCulture(fileName);
            var rootElement = XElement.Load(fileName);

            foreach (var dataElement in rootElement.Elements("data"))
            {
                var name = (string)dataElement.Attribute("name");
                var value = (string)dataElement.Element("value");

                document[name, culture] = value;
            }
        }
    }
}