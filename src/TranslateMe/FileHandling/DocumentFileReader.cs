using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TranslateMe.Model;
using TranslateMe.Properties;

namespace TranslateMe.FileHandling
{
    class DocumentFileReader
    {
        public Document LoadDocument(string fileName)
        {
            var workingDirectory = Path.GetDirectoryName(fileName);
            var documentName = Path.GetFileNameWithoutExtension(fileName);
            var document = new Document(workingDirectory, documentName);
            var xml = LoadXml(fileName);

            var texts = from textElement in xml.Elements("text")
                        from valueElement in textElement.Elements("value")
                        select new
                        {
                            CultureName = (string)valueElement.Attribute("culture"),
                            Name = (string)textElement.Attribute("name"),
                            Value = (string)valueElement
                        }
                        into text
                        group text by text.CultureName
                        into cultureGroup
                        let culture = new CultureInfo(cultureGroup.Key ?? string.Empty)
                        from text in cultureGroup
                        select new
                        {
                            Culture = culture,
                            text.Name,
                            text.Value
                        };

            foreach (var text in texts)
            {
                document[text.Name, text.Culture] = text.Value;
            }

            return document;
        }

        private static XElement LoadXml(string fileName)
        {
            try
            {
                return XElement.Load(fileName);
            }
            catch (XmlException e)
            {
                var message = string.Format(
                    "{0}{1}{1}{2}",
                    Strings.DocumentLoadFailed,
                    Environment.NewLine,
                    e.Message);

                throw new FileLoadException(message, e);
            }
        }
    }
}