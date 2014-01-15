using System.IO;
using System.Xml.Linq;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    class DocumentFileWriter
    {
        public void Write(Document document)
        {
            var documentElement = CreateDocumentElement(document);

            SaveDocument(document, documentElement);
        }

        private static XElement CreateDocumentElement(Document document)
        {
            var documentElement = new XElement("document");

            foreach (var phrase in document.Phrases)
            {
                var nameAttribute = new XAttribute("name", phrase.Name);
                var textElement = new XElement("text", nameAttribute);

                foreach (var translation in phrase.Translations)
                {
                    var cultureAttribute = new XAttribute("culture", translation.Culture);
                    var valueElement = new XElement("value", cultureAttribute);

                    if (translation.Text != null)
                        valueElement.SetValue(translation.Text);

                    textElement.Add(valueElement);
                }

                documentElement.Add(textElement);
            }

            return documentElement;
        }

        private static void SaveDocument(Document document, XElement documentElement)
        {
            var fileName = Path.Combine(document.Directory, document.Name + ".tmd");

            documentElement.Save(fileName);
        }
    }
}