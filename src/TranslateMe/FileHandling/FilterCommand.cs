using System.Collections.ObjectModel;
using System.Linq;
using TranslateMe.Model;

namespace TranslateMe.FileHandling
{
    public static class FilterCommand
    {
        public static ObservableCollection<Phrase> FilterDocument(Document document, string searchTerm)
        {
            return new ObservableCollection<Phrase>(document.Phrases.Where(f => f.Name.Contains(searchTerm)));
        }
    }
}
