using System.IO;
using System.Linq;
using System.Xml.Linq;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    class DocumentFileWriter
    {
        public void SaveDocument(Document document)
        {
            var documentElement = CreateDocumentElement(document);

            SaveDocument(document, documentElement);
        }

        private static XElement CreateDocumentElement(Document document)
        {
            var documentElement = new XElement("document");
            var phrases = from phrase in document.Phrases
                          orderby phrase.Name
                          select new
                          {
                              phrase.Name,
                              Texts = from translation in phrase.Translations
                                      select new
                                      {
                                          Culture = translation.Culture.Name,
                                          Value = translation.Text
                                      }
                          };

            foreach (var phrase in phrases)
            {
                var nameAttribute = new XAttribute("name", phrase.Name);
                var textElement = new XElement("text", nameAttribute);

                foreach (var text in phrase.Texts)
                {
                    var valueElement = new XElement("value");

                    if (text.Culture != string.Empty)
                    {
                        var cultureAttribute = new XAttribute("culture", text.Culture);

                        valueElement.Add(cultureAttribute);
                    }

                    if (text.Value != null)
                    {
                        valueElement.SetValue(text.Value);
                    }

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