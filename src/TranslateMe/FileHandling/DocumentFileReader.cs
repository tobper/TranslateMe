using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    class DocumentFileReader
    {
        public Document LoadDocument(string fileName)
        {
            var workingDirectory = Path.GetDirectoryName(fileName);
            var documentName = Path.GetFileNameWithoutExtension(fileName);
            var document = new Document(workingDirectory, documentName);

            var texts = from textElement in XElement.Load(fileName).Elements("text")
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
                        let culture = new CultureInfo(cultureGroup.Key)
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
    }
}