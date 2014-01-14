using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace TranslateMe.Model
{
    public class Document
    {
        public Document(string directory, string name)
        {
            Directory = directory;
            Name = name;
            Cultures = new ObservableCollection<CultureInfo>();
            Phrases = new ObservableCollection<Phrase>();
        }

        public string Directory { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<CultureInfo> Cultures { get; private set; }
        public ObservableCollection<Phrase> Phrases { get; private set; }

        public string this[string name, CultureInfo culture]
        {
            get
            {
                var translation = Phrases.
                    Where(p => p.Name == name).
                    SelectMany(p => p.Translations).
                    FirstOrDefault(t => t.Culture.Equals(culture));

                return translation != null
                           ? translation.Text
                           : null;
            }
            set
            {
                if (!Cultures.Any(c => c.Equals(culture)))
                {
                    Cultures.Add(culture);
                }

                var phrase = Phrases.FirstOrDefault(p => p.Name == name);
                if (phrase == null)
                {
                    phrase = new Phrase(Cultures, name);
                    Phrases.Add(phrase);
                }

                phrase[culture].Text = value;
            }
        }
    }
}