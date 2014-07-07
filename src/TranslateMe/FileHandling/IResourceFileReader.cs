using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    internal interface IResourceFileReader
    {
        string GetDocumentName(string fileName);
        void LoadResources(Document document, string fileName);
    }
}